using System.Text.Json;
using SleepApp.Models;

namespace SleepApp.Services
{
    public static class RecordService
    {
        private static readonly string RecordFile = "Data/sleepRecord.json"; // sparar filväg

        public static void SaveRecord(SleepRecord record) // sparar objekt i jsonfilen
        {

            var dir = Path.GetDirectoryName(RecordFile); // säkerställ att mappen finns
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            List<SleepRecord> records = new List<SleepRecord>(); // skapar lista för historik

            if (File.Exists(RecordFile)) // läser in tidigare poster om filen finns
            {
                string jsonExisting = File.ReadAllText(RecordFile);
                if (!string.IsNullOrWhiteSpace(jsonExisting))
                {
                    try
                    {
                        records = JsonSerializer.Deserialize<List<SleepRecord>>(jsonExisting) ?? new List<SleepRecord>();
                    }
                    catch
                    {

                        records = new List<SleepRecord>(); // om filen är korrupt, börjar om från tom lista
                    }
                }
            }

            records.Add(record); // lägger till ny post

            var options = new JsonSerializerOptions { WriteIndented = true }; // sparar listan igen som json
            string json = JsonSerializer.Serialize(records, options);

            File.WriteAllText(RecordFile, json); // skriver tillbaka hela listan
        }
    }
}
