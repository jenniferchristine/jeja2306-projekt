using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.VisualBasic;
using SleepApp;
using SleepApp.Models;
using SleepApp.Services;

class Program
{
    private const string RecordPath = "Data/sleepRecord.json";

    static void Main() // huvudmetod för att starta program
    {
        RunProgram();
    }
    static void RunProgram() // kör programmet
    {
        ShowStartPage(); // anropar metod där startsida skapas

        if (!File.Exists("Models/sleepModel.zip"))
        {
            TrainModel.Train(); // tränar om om modell saknas
        }

        var data = StartTest();

        var record = SaveAndCreateResult(data); // skickar besvarade frågor till resultatet
        ShowResult(record); // visar resultat

        Console.WriteLine("\nPress X to exit or Enter to return to menu"); // stänger programmet eller börjar om
        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.Enter)
        {
            Console.WriteLine("\nReturning to start menu...");
            Thread.Sleep(1500);
            Console.Clear();
            RunProgram(); // kör testet igen
        }
        else
        {
            Console.WriteLine("\nEnding program...");
            Thread.Sleep(1500);
            TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
        }
    }
    public static void ShowStartPage()
    {
        Console.Clear(); // rensa föregående test
        ShowHeader(" 💤 Welcome to SleepApp!💤 ");

        Console.WriteLine("\nSleepApp helps to determine your sleeping habits by answering 5 simple questions.\nYou answer by choosing the option that suits you the best and press enter for the next question.");
        ShowLastRegisteredDate();
        Console.WriteLine("\nPress Enter -| Continue to test\nPress Y -----| Show record\nPress X -----| End program\n");

        ShowFooter(106);

        while (true)
        {
            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.X)
            {
                Console.WriteLine("\nEnding program...");
                Thread.Sleep(1500);
                TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
                Environment.Exit(0); // avslutar programmet helt
            }
            else if (key == ConsoleKey.Y)
            {
                ShowRecord();
                continue; // går tillbaka till början av mainloopen
            }
            else if (key == ConsoleKey.Enter)
            {
                if (GetLatestTestDate())
                {
                    Console.WriteLine("\n⚠️  A test for today is already registered. Starting a new test will overwrite it.\n-- Press Y to continue with a new test or N to cancel.");
                    var confirm = Console.ReadKey(true).Key;

                    if (confirm != ConsoleKey.Y)
                    {
                        Console.WriteLine("\n🛑 Test cancelled\n\nReturning to start menu...");
                        Thread.Sleep(1500);
                        ShowStartPage();
                        return;
                    }
                }
                break;
            }
            else
            {
                Console.WriteLine("\n❗ Invalid choice. Press Enter to start test or X to exit."); // loopar denna loop igen
            }
        }
    }
    static bool GetLatestTestDate()
    {
        if (!File.Exists(RecordPath)) return false;

        var json = File.ReadAllText(RecordPath);
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json);

        if (records == null || records.Count == 0) return false;

        return records.Any(r => r.Date.Date == DateTime.Now.Date); // kollar om test med samma datum finns
    }
    static void ShowLastRegisteredDate() // hitta senaste registrerade test-datum
    {
        if (!File.Exists(RecordPath)) return; // gör inget om filen ej finns

        var json = File.ReadAllText(RecordPath);
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json);

        if (records == null || records.Count == 0) return; // gör inget om listan är tom

        var lastRecord = records.Last(); // tar senaste post
        string formattedDate = lastRecord.Date.ToString("yyyy-MM-dd"); // tar endast datumet

        Console.WriteLine("\n📅 Test last registered: " + formattedDate);
    }
    public static List<SleepRecord> GetRecordData() // visar tidigare testresultat
    {
        if (!File.Exists(RecordPath)) // kontroll om fil ej finns
        {
            Console.WriteLine("\nNo record of earlier test.");
            return new List<SleepRecord>();
        }

        var json = File.ReadAllText(RecordPath); // läser in innehåll som textsträng om fil finns
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json); // konverterar jsontext till lista av objekt

        if (records == null || records.Count == 0) // dubbelkollar om filen är tom eller felaktig
        {
            Console.WriteLine("\nNo record of earlier test.");
            return new List<SleepRecord>();
        }

        return records;
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
    private static void GetAndShowRecordText(SleepRecord r)
    {
        string sleepText = r.SleepHours switch
        {
            1 => "1–2h",
            2 => "3–6h",
            3 => "7–8h",
            _ => "Unknown"
        };

        string caffeineText = r.CaffeineHours switch
        {
            1 => "1–5h before bed",
            2 => "6–7h before bed",
            3 => "8–10h before bed",
            _ => "Unknown"
        };

        string stressText = r.StressLevel switch
        {
            1 => "High",
            2 => "Medium",
            3 => "Low",
            _ => "Unknown"
        };

        string activityText = r.ActivityLevel switch
        {
            1 => "Low",
            2 => "Medium",
            3 => "High",
            _ => "Unknown"
        };

        string sleepQualityText = r.SleepQuality switch
        {
            1 => "Poor",
            2 => "Average",
            3 => "Good",
            _ => "Unknown"
        };

        Console.WriteLine($"{r.Date:yyyy-MM-dd}:");
        Console.WriteLine($"- Sleep: {sleepText}");
        Console.WriteLine($"- Caffeine: {caffeineText}");
        Console.WriteLine($"- Stress: {stressText}");
        Console.WriteLine($"- Activity: {activityText}");
        Console.WriteLine($"- Sleep Quality: {sleepQualityText}");
        Console.WriteLine($"- Level: {r.PredictedLevel}");
        Console.WriteLine($"- Score: {r.TotalScore}");
        Console.WriteLine();
    }
    public static void ShowRecord()
    {
        Console.Clear();
        ShowHeader(" 💤 SleepApp Record💤 ");

        var records = GetRecordData();

        if (records.Count == 0)
        {
            Console.WriteLine("\nNo records found.");
            Console.WriteLine("\nPress Enter to return to menu");
            Console.ReadLine();
            RunProgram();
            return;
        }

        ShowAllRecordsWithIndex(records);
        ShowFooter(101);
        Console.WriteLine("\nPress D -----| Delete a result\nPress Enter -| Return to menu\nPress X -----| End program");

        while (true)
        {
            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.D)
            {
                Console.Write("\nPress the number of the record to delete and then Enter: ");
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
                    }
                    else
                    {
                        Console.WriteLine("\nDeletion cancelled.");
                    }

                    Thread.Sleep(1500);
                    Console.Clear();
                    ShowAllRecordsWithIndex(records);
                    return;
                }
                else
                {
                    Console.WriteLine("\n❗ Invalid number.");
                }
            }
            else if (key == ConsoleKey.Enter)
            {
                Console.WriteLine("\nReturning to start menu...");
                Thread.Sleep(1500);
                Console.Clear();
                ShowStartPage();
                return;
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
            }
        }
    }
    static PersonData StartTest()
    {
        var data = new PersonData(); // objekt för användarens svar

        // frågor som ska besvaras
        data.SleepHours = Ask("\nHow many hours of sleep do you usually get?\n\n1) 1-2h\n2) 3-6h\n3) 7-8h");
        data.CaffeineHours = Ask("\nHow many hours before bed do you drink caffeine?\n\n1) 1-5h\n2) 6-7h\n3) 8-10h");
        data.StressLevel = Ask("\nHow high would you rate your stress levels?\n\n1) High\n2) Medium\n3) Low");
        data.ActivityLevel = Ask("\nHow active are you during the day?\n\n1) Low\n2) Medium\n3) High");
        data.SleepQuality = Ask("\nHow would you rate your sleep quality?\n\n1) Poor\n2) Average\n3) Good");

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

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("\n❗ Please choose an option.\n-- Press any key to try again --");
                Console.ReadKey(true);
                continue;
            }

            if (input.Trim().ToUpper() == "X")
            {
                Console.Write("\n⚠️  Are you sure you want to exit? Your progress will not be saved.\n-- Press Y to exit or N to continue.");
                var confirm = Console.ReadKey(true).Key;

                if (confirm == ConsoleKey.Y)
                {
                    Console.WriteLine("\nEnding program...");
                    Thread.Sleep(1500);
                    TextColor("\n\n🛑 Test has ended.\n", ConsoleColor.Red);
                    Environment.Exit(0);
                }
                else
                {
                    continue;
                }
            }

            if (int.TryParse(input, out int value))
            {
                if (value >= 1 && value <= 3)
                    return value;

                Console.WriteLine("\n❗ Invalid input. Choose one of the options above (1, 2 or 3).");
            }
            else
            {
                Console.WriteLine("\n❗ Invalid input: Choose one of the options above (1, 2 or 3).");
            }

            Console.WriteLine("-- Press any key to try again --");
            Console.ReadKey(true);
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

        var result = SleepPredictionService.Predict(data); // gissar baserat på datan
        string level = GetLevel(float.Parse(result));

        float totalScore = data.SleepHours + data.CaffeineHours + data.StressLevel + data.ActivityLevel + data.SleepQuality; // beräkna totalpoäng genom summan av alla svar

        var record = new SleepRecord // skapar historikpost
        {
            SleepHours = data.SleepHours,
            CaffeineHours = data.CaffeineHours,
            StressLevel = data.StressLevel,
            ActivityLevel = data.ActivityLevel,
            SleepQuality = data.SleepQuality,
            PredictedLevel = level,
            TotalScore = totalScore
        };

        RecordService.SaveRecord(record); // sparar post i jsonfil
        return record;
    }
    static void ShowResult(SleepRecord record)
    {
        ShowHeader(" 💤 SleepApp💤 ");

        ConsoleColor levelColor = record.PredictedLevel switch // färglägger resultat
        {
            "Poor" => ConsoleColor.Red,
            "Average" => ConsoleColor.Yellow,
            "Good" => ConsoleColor.Green,
            _ => ConsoleColor.White
        };

        TextColor("\nSleep Habit Level: " + record.PredictedLevel, levelColor);

        string description = record.PredictedLevel switch // tillför meddelande
        {
            "Poor" => "Your sleeping habits need significant improvement.\nTry to get more rest and maintain a consistent sleep schedule.",
            "Average" => "Your sleeping habits is okay but could be better.\nConsider improving your bedtime routine or reducing stress before bed.",
            "Good" => "Your sleeping habits are very good!\nKeep up your healthy routines.",
            _ => ""
        };

        Console.WriteLine(description + "\n");

        // skriver ut varje svar med text och nivå via switch
        Console.WriteLine("\n📈 Result of your answers:\n"); // skriver ut användarens egna svar
        Console.WriteLine("- Sleep Hours: " + (record.SleepHours switch { 1 => "1-2h", 2 => "3-6h", 3 => "7-8h", _ => "Unknown" }) + " " + "| " + GetLevel(record.SleepHours));
        Console.WriteLine("- Caffeine Hours: " + (record.CaffeineHours switch { 1 => "1-5h before bed", 2 => "3-6h before bed", 3 => "7-8h before bed", _ => "Unknown" }) + " " + "| " + GetLevel(record.CaffeineHours));
        Console.WriteLine("- Stress Level: " + (record.StressLevel switch { 1 => "High", 2 => "Medium", 3 => "Low", _ => "Unknown" }) + " " + "| " + GetLevel(record.StressLevel));
        Console.WriteLine("- Activity Level: " + (record.ActivityLevel switch { 1 => "Low", 2 => "Medium", 3 => "High", _ => "Unknown" }) + " " + "| " + GetLevel(record.ActivityLevel));
        Console.WriteLine("- Sleep Quality: " + (record.SleepQuality switch { 1 => "Poor", 2 => "Average", 3 => "Good", _ => "Unknown" }) + " " + "| " + GetLevel(record.SleepQuality));


        Console.WriteLine("\nTotal Score: " + record.TotalScore + " / 15\n\n" + "5–7 → Poor\n8–11 → Average\n12–15 → Good\n"); // skriver ut totalpoäng och vilken nivå poängen hör till

        if (record.SleepHours == 1) // varnar vid få sömntimmar
        {
            TextColor("⚠️  Note: You are getting very little sleep hours. Try to rest more!\n", ConsoleColor.Red);
        }

        ShowFooter(94);
    }
    static void ShowHeader(string title) // metod för att "öppna header"
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n" + new string('=', 40) + title + new string('=', 39));
        Console.ResetColor();
    }
    static void ShowFooter(int length) // metod för att "stänga headern"
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(new string('=', length));
        Console.ResetColor();
    }
    static void TextColor(string text, ConsoleColor color) // metod för att färgge text
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}