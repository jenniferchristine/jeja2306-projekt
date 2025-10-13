using System.Runtime.CompilerServices;
using System.Text.Json;
using SleepApp.Models;

namespace SleepApp.Services
{
    public static class RecordService
    {
        private static readonly string RecordFile = "Data/sleepRecord.json"; // sparar filväg

        public static void SaveRecord(SleepRecord record) // sparar objekt i jsonfilen
        {
            List<SleepRecord> records = new List<SleepRecord>(); // skapar lista för historik

            if (File.Exists(RecordFile)) // läser in tidigare poster om filen finns
            {
                string jsonExisting = File.ReadAllText(RecordFile); 
                if (!string.IsNullOrEmpty(jsonExisting)) 
                {
                    records = JsonSerializer.Deserialize<List<SleepRecord>>(jsonExisting) ?? new List<SleepRecord>(); // skapar ny lista om fil är tom
                }
            }

            records.Add(record); // lägger till ny post

            var options = new JsonSerializerOptions { WriteIndented = true }; // sparar listan igen som json
            File.WriteAllText(RecordFile, JsonSerializer.Serialize(record, options)); // skriver listan till filen och uppdaterar
        }
    }
}