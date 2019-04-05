using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.ML.Data;

namespace MLNet.Sample
{
    public class SentimentSample
    {
        const string _dataPath = @"sentiment labelled sentences\imdb_labelled.txt";
        const string _testDataPath = @"sentiment labelled sentences\yelp_labelled.txt";

                IEnumerable<SentimentData> sentiments = new[]
         {
            new SentimentData
            {
                SentimentText = "Contoso's 11 is a wonderful experience",
                Sentiment = 0
            },
            new SentimentData
            {
                SentimentText = "The acting in this movie is very bad",
                Sentiment = 0
            },
            new SentimentData
            {
                SentimentText = "Joe versus the Volcano Coffee Company is a great film.",
                Sentiment = 0
            },
              new SentimentData
            {
                SentimentText = "this is very bad thing.",
                Sentiment = 0
            },
                 new SentimentData
            {
                SentimentText = "this is really cool thing.",
                Sentiment = 0
            }
        };

        public class SentimentData
        {
            [Column(ordinal: "0")]
            public string SentimentText;
            [Column(ordinal: "1", name: "Label")]
            public float Sentiment;
        }

        public class SentimentPrediction
        {
            [ColumnName("PredictedLabel")]
            public bool Sentiment;
        }

        public async Task Run()
        {

            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader<SentimentData>(_dataPath, useHeader: false, separator: "tab"));
            pipeline.Add(new TextFeaturizer("Features", "SentimentText"));
            pipeline.Add(new FastTreeBinaryClassifier() { NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2 });
            PredictionModel<SentimentData, SentimentPrediction> model =
                pipeline.Train<SentimentData, SentimentPrediction>();

            await model.WriteAsync("sentiment-model");

            Evaluate(model);

            Console.WriteLine();
            Console.WriteLine("Start Predicting...");
            Console.WriteLine("---------------------");

            var trainedModel = await PredictionModel<SentimentData, SentimentPrediction>.ReadAsync("sentiment-model");

            IEnumerable<SentimentPrediction> predictions = model.Predict(sentiments);

            var sentimentsAndPredictions = sentiments.Zip(predictions, (sentiment, prediction) => (sentiment, prediction));

            foreach (var item in sentimentsAndPredictions)
            {
                Console.WriteLine($"Sentiment: {item.sentiment.SentimentText} | Prediction: {(item.prediction.Sentiment ? "Positive" : "Negative")}");
            }

            Console.WriteLine();

        }

        public static void Evaluate(PredictionModel<SentimentData, SentimentPrediction> model)
        {
            var testData = new TextLoader<SentimentData>(_testDataPath, useHeader: false, separator: "tab");
            var evaluator = new BinaryClassificationEvaluator();
            BinaryClassificationMetrics metrics = evaluator.Evaluate(model, testData);

            Console.WriteLine();
            Console.WriteLine("PredictionModel quality metrics evaluation");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        }
    }
}
