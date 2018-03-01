
%%configure -f
{"executorMemory": "1G", "numExecutors":8, "executorCores":1, "conf": {"spark.jars.packages": "com.microsoft.azure:azure-eventhubs-spark_2.11:2.1.6"}}

import org.apache.spark.sql._
import org.apache.spark.sql.types._
import org.apache.spark.sql.functions._
import org.apache.spark.sql.streaming
import java.sql.{Connection,DriverManager,ResultSet}
import spark.implicits._

val eventhubParameters = Map[String, String] (
     "eventhubs.policyname" -> "iothubowner",
     "eventhubs.policykey" -> "nI7ONdIaQPXruvX64BPYX56RN3WlWpDOZPk9YFM/C+4=",
     "eventhubs.namespace" -> "iothub-ns-t10iothub-367150-299f8fb8eb",
     "eventhubs.name" -> "t10iothub",
     "eventhubs.partition.count" -> "4",
     "eventhubs.consumergroup" -> "$Default",
     "eventhubs.progressTrackingDir" -> "/eventhubs/progress",
     "eventhubs.sql.containsProperties" -> "true"
     )

val inputStream = spark.readStream.
format("eventhubs").
options(eventhubParameters).
load()

val schema = StructType(StructField("deviceTime", TimestampType, false)::
                        StructField("rideId", StringType, false)::
                        StructField("trainId", StringType, false)::
                        StructField("correlationId", StringType, false)::
                        StructField("passengerCount", StringType, false)::
                        StructField("eventType", StringType, false)::Nil)

val archivedata = inputStream.selectExpr("CAST(body as STRING)", "enqueuedTime").select(from_json(col("body"), schema).alias("data"))

val result4DF = archivedata.select("data.*").withColumn("deviceTime", $"deviceTime".cast(TimestampType)).withWatermark("deviceTime", "5 minutes").where(col("eventType")==="RideStart" || col("eventType")==="RideEnd" || col("eventType")==="PhotoTriggered").createOrReplaceTempView("Challenge4Data")

val challenge4DF = spark.sql("SELECT MIN(rideId) AS RollerCoasterId, MIN(trainId) AS RideVehicleId, correlationId AS CorrelationId, MAX(deviceTime) AS EndOfRideTime, SUM(BitWise) AS PhotoTriggered FROM (SELECT *, CASE eventType WHEN 'RideStart' THEN 1 WHEN 'PhotoTriggered' THEN 2 WHEN 'RideEnd' THEN 4 ELSE 0 END AS BitWise FROM Challenge4Data) as Tab1 GROUP BY correlationId HAVING SUM(BitWise) >= 5")

val WriteToSQLQuery  = challenge4DF.writeStream.outputMode("complete").foreach(new ForeachWriter[Row] {
   var connection:java.sql.Connection = _
   var statement:java.sql.Statement = _

   val jdbcUsername = ***"
   val jdbcPassword = "***"
   val jdbcHostname = "***.database.windows.net" //typically, this is in the form or servername.database.windows.net
   val jdbcPort = 1433
   val jdbcDatabase ="sql-db"
   val driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver"
   val jdbc_url = s"jdbc:sqlserver://${jdbcHostname}:${jdbcPort};database=${jdbcDatabase};encrypt=true;trustServerCertificate=false;hostNameInCertificate=*.database.windows.net;loginTimeout=30;"

  def open(partitionId: Long, version: Long):Boolean = {
    Class.forName(driver)
    connection = DriverManager.getConnection(jdbc_url, jdbcUsername, jdbcPassword)
    statement = connection.createStatement
    true
  }

  def process(value: Row): Unit = {
    val RollerCoasterId  = value(0)
    val RideVehicleId = value(1)
    val CorrelationId = value(2)
    val EndOfRideTime = value(3)
    val PhotoTriggered = value(4) 

    if (PhotoTriggered == 5)
    {
        val valueStr = "'" + RollerCoasterId + "','" + RideVehicleId + "','" + CorrelationId + "','" + EndOfRideTime + "','" + PhotoTriggered + "'"
        statement.execute("INSERT INTO dbo.ridesWithoutPhoto (RollerCoasterId, RideVehicleId, CorrelationId, EndOfRideTime, PhotoTriggered) VALUES (" + valueStr + ")")   
    }
    else
    {
      statement.execute("DELETE FROM dbo.ridesWithoutPhoto WHERE CorrelationID = '" + CorrelationId + "'")
    }
   }

  def close(errorOrNull: Throwable): Unit = {
     connection.close
   }
  })

var streamingQuery = WriteToSQLQuery.start()


