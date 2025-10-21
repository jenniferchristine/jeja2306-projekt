using Microsoft.ML;
using Microsoft.ML.Data;
using SleepApp.Models;

namespace SleepApp.Services
{
    public static class SleepPredictionService // klass för att göra gissningar
    {
        private static readonly string ModelPath = "Models/sleepModel.zip"; // där modellen är sparad
        private static PredictionEngine<PersonData, SleepPrediction>? predEngine; // objekt för att göra gissning

        static SleepPredictionService() // körs när klassen används för första gången
        {
            var mlContext = new MLContext(); // ingång till ml.net
            DataViewSchema modelSchema;

            var model = mlContext.Model.Load(ModelPath, out modelSchema); // laddar tränad modell med dess kolumner
            predEngine = mlContext.Model.CreatePredictionEngine<PersonData, SleepPrediction>(model); // skapar objekt som kan ta in persondata och returnera gissning
        }

        public static string Predict(PersonData input)
        {
            if (predEngine == null)
                return "Model not trained";

            var prediction = predEngine.Predict(input);
            return prediction.Score.ToString("0"); // avrunda till närmaste heltal 1–3
        }


    }

    public class SleepPrediction // output från gissning
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}