
%%configure -f
{"executorMemory": "1G", "numExecutors":8, "executorCores":1, "conf": {"spark.jars.packages": "com.microsoft.azure:azure-eventhubs-spark_2.11:2.1.6"}}

import org.apache.spark.sql._
import org.apache.spark.sql.types._
import org.apache.spark.sql.functions._
import org.apache.spark.sql.streaming
import java.sql.{Connection,DriverManager,ResultSet}
import spark.implicits._

val eventhubParameters = Map[String, String] (
     "eventhubs.policyname" -> "***",
     "eventhubs.policykey" -> "***",
     "eventhubs.namespace" -> "***",
     "eventhubs.name" -> "***",
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

val result = archivedata.select("data.*").withColumn("deviceTime", $"deviceTime".cast(TimestampType)).withWatermark("deviceTime", "5 minutes").where(col("eventType")==="RideStart").createOrReplaceTempView("ChallengeData")

val persistenceDF = spark.sql("Select rideId as RollerCoasterId, window.start as StartTime, count(*) as CountOfRides, sum(passengerCount) as SumOfRides from ChallengeData GROUP BY rideId, WINDOW(deviceTime, '5 minutes')")

val WriteToSQLQuery  = persistenceDF.writeStream.foreach(new ForeachWriter[Row] {
   var connection:java.sql.Connection = _
   var statement:java.sql.Statement = _

   val jdbcUsername = "***"
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
    val StartTime = value(1)
    val CountOfRides = value(2)
    val SumOfRides = value(3) 

    val valueStr = "'" + RollerCoasterId + "'," + "'" + StartTime + "'," + "'" + CountOfRides + "'," + "'" + SumOfRides + "'"
    statement.execute("INSERT INTO " + "dbo.team10" + " VALUES (" + valueStr + ")")   
    }

  def close(errorOrNull: Throwable): Unit = {
     connection.close
   }
  })

var streamingQuery = WriteToSQLQuery.start()


