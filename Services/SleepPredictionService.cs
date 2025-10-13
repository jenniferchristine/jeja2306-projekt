using System.Data;
using Microsoft.ML;
using Microsoft.ML.Data;
using SleepApp.Models;

namespace SleepApp.Services
{
    public static class SleepPredictionService
    {
        private static readonly string ModelPath = "Models/sleepModel.zip";
        private static PredictionEngine<PersonData, SleepPrediction>? predEngine;

        static SleepPredictionService()
        {
            var mlContext = new MLContext();
            DataViewSchema modelSchema;

            var model = mlContext.Model.Load(ModelPath, out modelSchema);
            predEngine = mlContext.Model.CreatePredictionEngine<PersonData, SleepPrediction>(model);
        }

        public static string Predict(PersonData input)
        {
            var prediction = predEngine?.Predict(input);
            return prediction?.PredictedLabel ?? "Unknown";
        }
    }

    public class SleepPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; } = string.Empty;
    }
}