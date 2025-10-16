using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using SleepApp;
using SleepApp.Models;
using SleepApp.Services;

/*
Att göra: 
[] Vid nytt test - Varna för överskrivning
[] Skriv över json med ny data vid fortsättning
*/

class Program
{
    static void Main() // huvudmetod för att starta program
    {
        RunProgram();
    }
    static void RunProgram()
    {
        ShowStartPage();

        if (!File.Exists("Models/sleepModel.zip"))
        {
            TrainModel.Train(); // tränar om om modell saknas
        }

        var data = StartTest();

        var record = SaveAndCreateResult(data);
        ShowResult(record);

        Console.WriteLine("\nPress X to exit or enter to retake test"); // stänger programmet eller börjar om
        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.Enter)
        {
            Console.Clear();
            RunProgram(); // kör testet igen
        }
        else
        {
            TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
        }
    }

    public static void ShowStartPage()
    {
        Console.Clear(); // rensa föregående test
        ShowHeader(" 💤 Welcome to SleepApp!💤 ");

        ShowLastRegisteredDate();

        Console.WriteLine("\nSleepApp helps to determine your sleep habits by answering 5 simple questions.\nYou answer by choosing the option that suits you the best and press enter for the next question.\n\nPress Enter -| Continue to test\nPress Y -----| Show record\nPress X -----| End program");
        ShowFooter(106);

        while (true)
        {
            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.X)
            {
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
                break; // fortsätter mainloop
            }
            else
            {
                Console.WriteLine("\n❗ Invalid choice. Press Enter to start test or X to exit."); // loopar denna loop igen
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
            "Poor" => "Your sleep habits need significant improvement.\nTry to get more rest and maintain a consistent sleep schedule.",
            "Average" => "Your sleep is okay but could be better.\nConsider improving your bedtime routine or reducing stress before bed.",
            "Good" => "Your sleep habits are very good!\nKeep up your healthy routines.",
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


        Console.WriteLine("\nTotal Score: " + record.TotalScore + " / 15\n\n" + "5–7 → Poor\n8–11 → Average\n12–15 → Good"); // skriver ut totalpoäng och vilken nivå poängen hör till

        if (record.SleepHours == 1) // varnar vid få sömntimmar
        {
            TextColor("⚠️  Note: You are getting very little sleep hours. Try to rest more!", ConsoleColor.Red);
        }

        ShowFooter(94);
    }

    static string GetLevel(float result) // metod för resultatparameter, konverterar denna till string och jämför med värden
    {
        return result.ToString() switch
        {
            "1" => "Poor",
            "2" => "Average",
            "3" => "Good",
            _ => "Unknown"
        };
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

    static void ShowHeader(string title) // metod för att "öppna header"
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n" + new string('=', 40) + title + new string('=', 39));
        Console.ResetColor();
    }
    static void ShowFooter(int length) // metod för att "stänga headern"
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n" + new string('=', length));
        Console.ResetColor();
    }

    static void TextColor(string text, ConsoleColor color) // metod för att färgge text
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void GetRecordData() // visar tidigare testresultat
    {
        if (!File.Exists("Data/sleepRecord.json")) // kontroll om fil ej finns
        {
            Console.WriteLine("\nNo record of earlier test.");
            return;
        }

        var json = File.ReadAllText("Data/sleepRecord.json"); // läser in innehåll som textsträng om fil finns
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json); // konverterar jsontext till lista av objekt

        if (records == null || records.Count == 0) // dubbelkollar om filen är tom eller felaktig
        {
            Console.WriteLine("\nNo record of earlier test.");
            return;
        }

        foreach (var r in records) // varje post loopas igenom och skrivs ut i text
        {
            Console.WriteLine($"{r.Date}: Sleep {r.SleepHours}, Caffeine {r.CaffeineHours}, Stress {r.StressLevel}, Activity {r.ActivityLevel}, Sleep Quality {r.SleepQuality}, Level {r.PredictedLevel}, Score {r.TotalScore}");
        }
    }

    public static void ShowRecord()
    {
        Console.Clear();

        ShowHeader(" 💤 SleepApp Record💤 ");
        GetRecordData(); // visar historik från jsonfil
        ShowFooter(101);
        Console.WriteLine("\nPress Enter -| Continue to test\nPress X -----| End program");
    }

    static void ShowLastRegisteredDate() // hitta senaste registrerade test-datum
    {
        string path = "Data/sleepRecord.json";
        if (!File.Exists(path)) return; // gör inget om filen ej finns

        var json = File.ReadAllText(path);
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json);

        if (records == null || records.Count == 0) return; // gör inget om listan är tom

        var lastRecord = records.Last(); // tar senaste post
        string formattedDate = lastRecord.Date.ToString("yyyy-MM-dd"); // tar endast datumet

        Console.WriteLine("\nTest last registered: " + formattedDate);
    }
}