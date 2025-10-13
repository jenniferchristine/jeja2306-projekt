using System; 

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
        public string PredictedLevel { get; set; } = "";
        public float TotalScore { get; set; }
    }
}