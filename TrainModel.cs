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
            var pipeline = mlContext.Transforms.Concatenate("Features", nameof(PersonData.SleepHours),
                                                                     nameof(PersonData.CaffeineHours),
                                                                     nameof(PersonData.StressLevel),
                                                                     nameof(PersonData.ActivityLevel))
                            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(PersonData.SleepQuality), maximumNumberOfIterations: 100));

            // tränar modell
            var model = pipeline.Fit(data);

            // sparar modellen
            mlContext.Model.Save(model, data.Schema, "Models/sleepModel.zip");
            Console.WriteLine("Model is saved");
        }
    }
}