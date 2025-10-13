using Microsoft.ML.Data;

namespace SleepApp.Models
{
    public class PersonData
    {
        [LoadColumn(0)]
        public float SleepHours { get; set; }

        [LoadColumn(0)]
        public float CaffeineHours { get; set; }

        [LoadColumn(0)]
        public float StressLevel { get; set; }

        [LoadColumn(0)]
        public float ActivityLevel { get; set; }

        [LoadColumn(0)]
        public float SleepQuality { get; set; }
    }
}