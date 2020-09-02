using Microsoft.ML.Data;

namespace Web.Schema
{
    // Input
    public class SentimentData
    {
        public string SentimentText;
        public bool Sentiment;
    }

    // Output
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}