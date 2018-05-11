using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MLNet.Sample
{
    class Program
    {

        static void Main(string[] args)
        {
            TaxiFareSample taxiSample = new TaxiFareSample();
            taxiSample.Run().Wait();

            SentimentSample sample = new SentimentSample();
            sample.Run().Wait();        
        }

    

        public class SentimentData
        {
            [Column(ordinal: "0", name: "Label")]
            public float Sentiment;
            [Column(ordinal: "1")]
            public string SentimentText;
        }

        public class SentimentPrediction
        {
            [ColumnName("PredictedLabel")]
            public bool Sentiment;
        }

    }
}
