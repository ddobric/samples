using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MLNet.Sample
{
    public class TaxiFareSample
    {
        const string m_DataPath = @"taxi fare data\taxi-fare-train.csv";
        const string m_TestDataPath = @"taxi fare data\taxi-fare-test.csv";

        public async Task Run()
        {
            var pipeline = new LearningPipeline();

            pipeline.Add(new TextLoader<TaxiTrip>(m_DataPath, useHeader: true, separator: ","));

            pipeline.Add(new ColumnCopier(("fare_amount", "Label")));

            pipeline.Add(new CategoricalOneHotVectorizer("vendor_id"));
            //"rate_code","payment_type"));

            //pipeline.Add(new ColumnConcatenator("Features",
            //                        "vendor_id",
            //                        "rate_code",
            //                        "passenger_count",
            //                        "trip_distance",
            //                        "payment_type"));

            pipeline.Add(new ColumnConcatenator(
                                "Features",
                                 "vendor_id",
                                 "passenger_count",
                                 "trip_distance"));

            pipeline.Add(new FastTreeRegressor());

            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = pipeline.Train<TaxiTrip, TaxiTripFarePrediction>();

            string modelName = "taxifair-model.zip";
            await model.WriteAsync(modelName);

            var testData = new TextLoader<TaxiTrip>(m_TestDataPath, useHeader: true, separator: ",");

            var evaluator = new RegressionEvaluator();

            RegressionMetrics metrics = evaluator.Evaluate(model, testData);

            Console.WriteLine();
            Console.WriteLine("PredictionModel quality metrics evaluation");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Abs. loss: {metrics.L1:P2}");
            Console.WriteLine($"Squared Loss: {metrics.L2:P2}");
            Console.WriteLine($"Rms: {metrics.Rms:F2}");

            var trips = new TaxiTrip[]{
                new TaxiTrip
                {
                    vendor_id = "VTS",
                    //rate_code = "1",
                    passenger_count = 1,
                    trip_distance = 10.33f,
                    //payment_type = "CSH",
                    fare_amount = 0 // predict it. actual = 29.5
                },
                 new TaxiTrip
                {
                    //VTS,1,1,1020,4.46,CSH,16.5
                    vendor_id = "VTS",
                    //rate_code = "1",
                    passenger_count = 1,
                    trip_distance = 4.46f,
                    //payment_type = "CRD",
                    fare_amount = 0 // predict it. actual = 16.5
                },

                 //CMT,1,1,183,0.6,CRD,4.5
                   new TaxiTrip
                {
                    //CMT,1,1,183,0.4,CRD,4.0
                    vendor_id = "CMT",
                    //rate_code = "1",
                    passenger_count = 1,
                    trip_distance = 0.4f,
                    //payment_type = "CRD",
                    fare_amount = 0 // predict it. actual = 4.0
                },
            };

            var trainedModel = await PredictionModel<TaxiTrip, TaxiTripFarePrediction>.ReadAsync(modelName);

            var prediction = model.Predict(trips);

            var enumerator = prediction.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine($"Fare amount: {enumerator.Current.fare_amount}");
            }
        }
    }

    public class TaxiTrip
    {
        [Column(ordinal: "0")]
        public string vendor_id;
        //[Column(ordinal: "1")]
        //public string rate_code;
        [Column(ordinal: "2")]
        public float passenger_count;
        //[Column(ordinal: "3")]
        //public float trip_time_in_secs;
        [Column(ordinal: "4")]
        public float trip_distance;
        //[Column(ordinal: "5")]
        //public string payment_type;
        [Column(ordinal: "6")]
        public float fare_amount;
    }

    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float fare_amount;
    }
}
