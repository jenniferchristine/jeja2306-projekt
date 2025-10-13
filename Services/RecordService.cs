using System.Runtime.CompilerServices;
using System.Text.Json;
using SleepApp.Models;

namespace SleepApp.Services
{
    public static class RecordService
    {
        private static readonly string RecordFile = "Data/sleepRecord.json";

        public static void SaveRecord(SleepRecord record)
        {
            List<SleepRecord> records = new List<SleepRecord>();

            if (File.Exists(RecordFile))
            {
                string jsonExisting = File.ReadAllText(RecordFile);
                if (!string.IsNullOrEmpty(jsonExisting))
                {
                    records = JsonSerializer.Deserialize<List<SleepRecord>>(jsonExisting) ?? new List<SleepRecord>();
                }
            }
        }
    }
}