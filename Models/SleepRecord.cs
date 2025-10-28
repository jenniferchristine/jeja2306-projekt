/* Klass för att spara testresultat */

namespace SleepApp.Models
{
    public class SleepRecord // klass för att spara datum, svar, nivå och totalpoäng
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public float SleepHours { get; set; }
        public float CaffeineHours { get; set; }
        public float StressLevel { get; set; }
        public float ActivityLevel { get; set; }
        public float SleepQuality { get; set; }

        public float SleepHabits { get; set; } // nummervärde från modell
        public string PredictedLevel { get; set; } = ""; // textvärde för användaren
        public float TotalScore { get; set; }
    }
}