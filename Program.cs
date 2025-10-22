using System.Text.Json;
using SleepApp;
using SleepApp.Models;
using SleepApp.Services;

class Program
{
    private const string RecordPath = "Data/sleepRecord.json";

    static void Main()
    {
        while (true) // Huvudloop
        {
            ShowStartPage();
        }
    }

    public static void ShowStartPage()
    {
        Console.Clear();
        ShowHeader(" 💤 Welcome to SleepApp!💤 ");

        Console.WriteLine("\nSleepApp helps to determine your sleeping habits by answering 5 simple questions.");
        Console.WriteLine("You answer by choosing the option that suits you the best and press enter for the next question.");
        ShowLastRegisteredDate();

        Console.WriteLine("\nPress Enter -| Continue to test");
        Console.WriteLine("Press Y -----| Show record");
        Console.WriteLine("Press X -----| End program\n");
        ShowFooter(106);

        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.X)
        {
            Console.WriteLine("\nEnding program...");
            Thread.Sleep(1500);
            TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
            Environment.Exit(0);
        }
        else if (key == ConsoleKey.Y)
        {
            ShowRecord();
            return; // återgå till huvudloopen
        }
        else if (key == ConsoleKey.Enter)
        {
            if (!File.Exists("Models/sleepModel.zip"))
                TrainModel.Train();

            if (GetLatestTestDate())
            {
                Console.WriteLine("\n⚠️  A test for today is already registered.");
                Console.WriteLine("-- Press Y to continue with a new test or N to cancel.");
                var confirm = Console.ReadKey(true).Key;

                if (confirm != ConsoleKey.Y)
                {
                    Console.WriteLine("\n🛑 Test cancelled\nReturning to start menu...");
                    Thread.Sleep(1500);
                    return;
                }
            }

            var data = StartTest();
            var record = SaveAndCreateResult(data);
            ShowResult(record);

            Console.WriteLine("\nPress Enter to return to menu or X to exit");
            var nextKey = Console.ReadKey(true).Key;
            if (nextKey == ConsoleKey.X)
            {
                Console.WriteLine("\nEnding program...");
                Thread.Sleep(1500);
                TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
                Environment.Exit(0);
            }

            return; // tillbaka till huvudloopen
        }
        else
        {
            Console.WriteLine("\n❗ Invalid choice. Press Enter, Y or X.");
            Thread.Sleep(1000);
        }
    }

    public static void ShowRecord()
    {
        while (true)
        {
            Console.Clear();
            ShowHeader(" 💤 SleepApp Record💤 ");

            var records = GetRecordData();

            if (records.Count == 0)
            {
                Console.WriteLine("\nNo records found.");
                Console.WriteLine("\nPress Enter to return to menu");
                Console.ReadLine();
                return; // tillbaka till huvudmeny
            }

            ShowAllRecordsWithIndex(records);
            ShowFooter(101);

            Console.WriteLine("\nPress D -----| Delete a result");
            Console.WriteLine("Press Enter -| Return to menu");
            Console.WriteLine("Press X -----| End program");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.D)
            {
                Console.Write("\nPress the number of the record to delete: ");
                if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= records.Count)
                {
                    var recordToDelete = records[index - 1];
                    Console.WriteLine($"\n⚠️  Are you sure you want to delete the record from {recordToDelete.Date:yyyy-MM-dd}? (Y/N)");
                    var confirm = Console.ReadKey(true).Key;

                    if (confirm == ConsoleKey.Y)
                    {
                        records.RemoveAt(index - 1);
                        File.WriteAllText(RecordPath, JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true }));
                        Console.WriteLine("\n✅ Record deleted successfully!");
                        Thread.Sleep(1500);
                    }
                }
                continue; // ladda om listan
            }
            else if (key == ConsoleKey.Enter)
            {
                return; // tillbaka till huvudloopen
            }
            else if (key == ConsoleKey.X)
            {
                Console.WriteLine("\nEnding program...");
                Thread.Sleep(1500);
                TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("\n❗ Invalid choice. Press D, Enter or X.");
                Thread.Sleep(1000);
            }
        }
    }

    static bool GetLatestTestDate()
    {
        if (!File.Exists(RecordPath)) return false;

        var json = File.ReadAllText(RecordPath);
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json);
        return records != null && records.Any(r => r.Date.Date == DateTime.Now.Date);
    }

    static void ShowLastRegisteredDate()
    {
        if (!File.Exists(RecordPath)) return;

        var json = File.ReadAllText(RecordPath);
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json);
        if (records == null || records.Count == 0) return;

        var lastRecord = records.Last();
        Console.WriteLine("\n📅 Test last registered: " + lastRecord.Date.ToString("yyyy-MM-dd"));
    }

    public static List<SleepRecord> GetRecordData()
    {
        if (!File.Exists(RecordPath)) return new List<SleepRecord>();

        var json = File.ReadAllText(RecordPath);
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json);
        return records ?? new List<SleepRecord>();
    }

    private static void ShowAllRecordsWithIndex(List<SleepRecord> records)
    {
        for (int i = 0; i < records.Count; i++)
        {
            var r = records[i];
            Console.WriteLine($"\n[{i + 1}] {r.Date:yyyy-MM-dd}:");
            Console.WriteLine($"- Sleep: {r.SleepHours switch { 1 => "1–2h", 2 => "3–6h", 3 => "7–8h", _ => "Unknown" }}");
            Console.WriteLine($"- Caffeine: {r.CaffeineHours switch { 1 => "1–5h before bed", 2 => "6–7h before bed", 3 => "8–10h before bed", _ => "Unknown" }}");
            Console.WriteLine($"- Stress: {r.StressLevel switch { 1 => "High", 2 => "Medium", 3 => "Low", _ => "Unknown" }}");
            Console.WriteLine($"- Activity: {r.ActivityLevel switch { 1 => "Low", 2 => "Medium", 3 => "High", _ => "Unknown" }}");
            Console.WriteLine($"- Sleep Quality: {r.SleepQuality switch { 1 => "Poor", 2 => "Average", 3 => "Good", _ => "Unknown" }}");
            Console.WriteLine($"- Level: {r.PredictedLevel}");
            Console.WriteLine($"- Score: {r.TotalScore}");
        }
    }

    static PersonData StartTest()
    {
        var data = new PersonData();

        data.SleepHours = Ask("\nHow many hours of sleep do you usually get?\n1) 1-2h\n2) 3-6h\n3) 7-8h");
        data.CaffeineHours = Ask("\nHow many hours before bed do you drink caffeine?\n1) 1-5h\n2) 6-7h\n3) 8-10h");
        data.StressLevel = Ask("\nHow high would you rate your stress levels?\n1) High\n2) Medium\n3) Low");
        data.ActivityLevel = Ask("\nHow active are you during the day?\n1) Low\n2) Medium\n3) High");
        data.SleepQuality = Ask("\nHow would you rate your sleep quality?\n1) Poor\n2) Average\n3) Good");

        Console.Clear();
        return data;
    }

    static int Ask(string question)
    {
        while (true)
        {
            Console.Clear();
            ShowHeader(" 💤 SleepApp💤 ");
            Console.WriteLine(question);
            Console.Write("\nChoose an option: ");

            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) continue;

            if (input.Trim().ToUpper() == "X")
            {
                Console.Write("\n⚠️  Are you sure you want to exit? Your progress will not be saved.\nY/N: ");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\nEnding program...");
                    Thread.Sleep(1500);
                    TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
                    Environment.Exit(0);
                }
                continue;
            }

            if (int.TryParse(input, out int value) && value >= 1 && value <= 3)
            {
                return value;
            }

            Console.WriteLine("\n❗ Invalid input. Choose 1, 2 or 3.");
            Thread.Sleep(1000);
        }
    }

    static string GetLevel(float result) => (int)result switch
    {
        1 => "Poor",
        2 => "Average",
        3 => "Good",
        _ => "Unknown"
    };

    static SleepRecord SaveAndCreateResult(PersonData data)
    {
        var result = SleepPredictionService.Predict(data);
        string level = GetLevel(float.Parse(result));

        float totalScore = data.SleepHours + data.CaffeineHours + data.StressLevel + data.ActivityLevel + data.SleepQuality;

        var record = new SleepRecord
        {
            SleepHours = data.SleepHours,
            CaffeineHours = data.CaffeineHours,
            StressLevel = data.StressLevel,
            ActivityLevel = data.ActivityLevel,
            SleepQuality = data.SleepQuality,
            PredictedLevel = level,
            TotalScore = totalScore
        };

        RecordService.SaveRecord(record);
        return record;
    }

    static void ShowResult(SleepRecord record)
    {
        ShowHeader(" 💤 SleepApp💤 ");
        ConsoleColor levelColor = record.PredictedLevel switch
        {
            "Poor" => ConsoleColor.Red,
            "Average" => ConsoleColor.Yellow,
            "Good" => ConsoleColor.Green,
            _ => ConsoleColor.White
        };
        TextColor("\nSleep Habit Level: " + record.PredictedLevel, levelColor);

        string description = record.PredictedLevel switch
        {
            "Poor" => "Your sleeping habits need significant improvement.\nTry to get more rest and maintain a consistent sleep schedule.",
            "Average" => "Your sleeping habits are okay but could be better.\nConsider improving your bedtime routine or reducing stress before bed.",
            "Good" => "Your sleeping habits are very good!\nKeep up your healthy routines.",
            _ => ""
        };
        Console.WriteLine(description + "\n");

        Console.WriteLine("\n📈 Result of your answers:\n");
        Console.WriteLine("- Sleep Hours: " + (record.SleepHours switch { 1 => "1-2h", 2 => "3-6h", 3 => "7-8h", _ => "Unknown" }) + " | " + GetLevel(record.SleepHours));
        Console.WriteLine("- Caffeine Hours: " + (record.CaffeineHours switch { 1 => "1-5h before bed", 2 => "3-6h before bed", 3 => "7-8h before bed", _ => "Unknown" }) + " | " + GetLevel(record.CaffeineHours));
        Console.WriteLine("- Stress Level: " + (record.StressLevel switch { 1 => "High", 2 => "Medium", 3 => "Low", _ => "Unknown" }) + " | " + GetLevel(record.StressLevel));
        Console.WriteLine("- Activity Level: " + (record.ActivityLevel switch { 1 => "Low", 2 => "Medium", 3 => "High", _ => "Unknown" }) + " | " + GetLevel(record.ActivityLevel));
        Console.WriteLine("- Sleep Quality: " + (record.SleepQuality switch { 1 => "Poor", 2 => "Average", 3 => "Good", _ => "Unknown" }) + " | " + GetLevel(record.SleepQuality));

        Console.WriteLine("\nTotal Score: " + record.TotalScore + " / 15\n5–7 → Poor\n8–11 → Average\n12–15 → Good\n");

        if (record.SleepHours == 1)
        {
            TextColor("⚠️  Note: You are getting very little sleep hours. Try to rest more!\n", ConsoleColor.Red);
        }

        ShowFooter(94);
    }

    static void ShowHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n" + new string('=', 40) + title + new string('=', 39));
        Console.ResetColor();
    }

    static void ShowFooter(int length)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(new string('=', length));
        Console.ResetColor();
    }

    static void TextColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
