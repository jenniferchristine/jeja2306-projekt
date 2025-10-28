/* Klass för att träna och spara ML-modell */

using Microsoft.ML;
using SleepApp.Models;

namespace SleepApp
{
    public static class TrainModel
    {
        public static void Train()
        {
            // laddar träningsdata från csv med PersonData
            var mlContext = new MLContext();
            var data = mlContext.Data.LoadFromTextFile<PersonData>(path: "Data/sleepdata.csv", hasHeader: true, separatorChar: ',');

            // bygger pipeline och kombinerar alla inputs till features
            var pipeline = mlContext.Transforms.Concatenate("Features", nameof(PersonData.SleepHours),
                                                                     nameof(PersonData.CaffeineHours),
                                                                     nameof(PersonData.StressLevel),
                                                                     nameof(PersonData.ActivityLevel),
                                                                     nameof(PersonData.SleepQuality))
                            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(PersonData.SleepHabits), maximumNumberOfIterations: 100));

            // tränar modell med data
            var model = pipeline.Fit(data);

            mlContext.Model.Save(model, data.Schema, "Models/sleepModel.zip"); // sparar modellen som zip
            Console.WriteLine("Model is saved");
        }
    }
}