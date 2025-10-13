using Microsoft.ML;
using SleepApp.Models;

namespace SleepApp
{
    public static class TrainModel
    {
        public static void Train()
        {
            // laddar data
            var mlContext = new MLContext();
            var data = mlContext.Data.LoadFromTextFile<PersonData>(path: "Data/sleepdata.csv", hasHeader: true, separatorChar: ',');

            // bygger pipeline
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(PersonData.SleepQuality))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(PersonData.SleepHours),
                                                             nameof(PersonData.CaffeineHours),
                                                             nameof(PersonData.StressLevel),
                                                             nameof(PersonData.ActivityLevel)))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // tränar modell
            var model = pipeline.Fit(data);

            // sparar modellen
            mlContext.Model.Save(model, data.Schema, "Models/sleepModel.zip");
            Console.WriteLine("Model is saved");
        }
    }
}