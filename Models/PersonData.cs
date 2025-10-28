/* Klass för användardata som matas in i programmet för att förutsäga sömnvane-nivå med ML */

using Microsoft.ML.Data;

namespace SleepApp.Models
{
    public class PersonData // klass för inputmodell
    {
        [LoadColumn(0)] // anger kolumn till egenskap
        public float SleepHours { get; set; }

        [LoadColumn(1)]
        public float CaffeineHours { get; set; }

        [LoadColumn(2)]
        public float StressLevel { get; set; }

        [LoadColumn(3)]
        public float ActivityLevel { get; set; }

        [LoadColumn(4)]
        public float SleepQuality { get; set; }

        [LoadColumn(5)]
        public float SleepHabits { get; set; } // label
    }
}