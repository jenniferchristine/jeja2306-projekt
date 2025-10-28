/* Ett program för att förutse sömnvane-nivå skapat av Jennifer Jakobsson */

using System.Text.Json;
using SleepApp;
using SleepApp.Models;
using SleepApp.Services;

class Program
{
    private const string RecordPath = "Data/sleepRecord.json"; // sökväg till json-data som nås i hela programmet

    static void Main() // startpunkt
    {
        while (true) // huvudloop som körs tills programmet avslutas
        {
            ShowStartPage();
        }
    }

    public static void ShowStartPage() // startsida
    {
        Console.Clear(); // rensar konsol
        ShowHeader(" 💤 Welcome to SleepApp!💤 "); // skriver ut header med titel

        Console.WriteLine("\nSleepApp helps to determine your sleeping habits by answering 5 simple questions.");
        Console.WriteLine("You answer by choosing the option that suits you the best and press enter for the next question.");
        ShowLastRegisteredDate(); // om datum för tidigare resultat finns så visas det

        Console.WriteLine("\nPress Enter -| Continue to test");
        Console.WriteLine("Press R -----| Show record");
        Console.WriteLine("Press X -----| End program\n");
        ShowFooter(106); // skriver ut footer

        var key = Console.ReadKey(true).Key; // väntar på knapptryckt val

        if (key == ConsoleKey.X) // avslutar programmet
        {
            Console.WriteLine("\nEnding program...");
            Thread.Sleep(1500);
            TextColor("\n🛑 Program has ended.\n", ConsoleColor.Red);
            Environment.Exit(0);
        }
        else if (key == ConsoleKey.R) // visar resultat-historik
        {
            ShowRecord();
            return; // avslutar metod och återgår till main()
        }
        else if (key == ConsoleKey.Enter) // startar nytt test
        {
            if (!File.Exists("Models/sleepModel.zip")) 
                TrainModel.Train(); // tränar maskininlärningsmodell om ingen hittats

            if (GetLatestTestDate()) // kontrollerar om test för idag registrerats
            {
                Console.WriteLine("\n⚠️  A test for today is already registered. A new test will overwrite the existing one.");
                Console.WriteLine("-- Press Y to continue with a new test or N to cancel.");
                var confirm = Console.ReadKey(true).Key;

                if (confirm != ConsoleKey.Y)
                {
                    Console.WriteLine("\n🛑 Test cancelled\n\nReturning to start menu...");
                    Thread.Sleep(1500);
                    return; // tillbaka till huvudloopen
                }
            }

            var data = StartTest(); // samlar in användarens svar
            var record = SaveAndCreateResult(data); // skapar resultat och sparar
            ShowResult(record); // visar resultat

            Console.WriteLine("\nPress Enter to return to menu or X to exit");
            var nextKey = Console.ReadKey(true).Key;
            if (nextKey == ConsoleKey.X)
            {
                Console.WriteLine("\nEnding program...");
                Thread.Sleep(1500);
                TextColor("\n🛑 Program has ended.\n", ConsoleColor.Red);
                Environment.Exit(0);
            }

            return; // tillbaka till huvudloopen
        }
        else
        {
            Console.WriteLine("\n❗ Invalid choice. Press Enter, R or X.");
            Thread.Sleep(1500);
        }
    }

    static bool GetLatestTestDate() // kontroll för dagens test
    {
        if (!File.Exists(RecordPath)) return false; // om inte testresultatsfil finns så finns inget resultat

        var json = File.ReadAllText(RecordPath); // läser in json som text
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json); // konverterar json till SleepRecord-lista
        return records != null && records.Any(r => r.Date.Date == DateTime.Now.Date); // returnerar true om lista finns med dagens datum
    }

    static void ShowLastRegisteredDate() // visar datum för senast registerade test
    {
        if (!File.Exists(RecordPath)) return;

        var json = File.ReadAllText(RecordPath);
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json);
        if (records == null || records.Count == 0) return;


        var lastRecord = records.OrderByDescending(r => r.Date).First(); // hitta den senaste posten baserat på date
        Console.WriteLine("\n📅 Test last registered: " + lastRecord.Date.ToString("yyyy-MM-dd")); // skriver ut datum för senaste registrerade test
    }

    public static List<SleepRecord> GetRecordData() // läser in sparade resultat från json-filen
    {
        if (!File.Exists(RecordPath)) return new List<SleepRecord>();

        var json = File.ReadAllText(RecordPath);
        var records = JsonSerializer.Deserialize<List<SleepRecord>>(json);
        return records ?? new List<SleepRecord>(); // returnerar listan (eller tom lista) med säkerhetsåtgärd om konvertering misslyckas
    }

    public static void ShowRecord() // visar resultat
    {
        while (true) // loop som snurrar så länge man är på historik-sidan
        {
            Console.Clear();
            ShowHeader(" 💤 SleepApp Record💤 ");

            var records = GetRecordData(); // läs filen på nytt varje gång

            if (records.Count == 0) // om inga resultat finns
            {
                Console.WriteLine("\nNo records found.\n");
                ShowFooter(101);
                Console.WriteLine("\nPress Enter to return to menu");
                Console.ReadLine();
                return; // tillbaka till huvudmeny
            }

            ShowAllRecordsWithIndex(records); // visa poster med rätt index
            ShowFooter(101);

            Console.WriteLine("\nPress D -----| Delete a result");
            Console.WriteLine("Press Enter -| Return to menu");
            Console.WriteLine("Press X -----| End program");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.D) // ta bort post
            {
                Console.Write("\nPress the number of the record to delete and then Enter: ");
                if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= records.Count)
                {
                    var recordToDelete = records[index - 1]; // hämtar resultat för inmatat index
                    Console.WriteLine($"\n⚠️  Are you sure you want to delete the record from {recordToDelete.Date:yyyy-MM-dd}?");
                    Console.WriteLine("-- Press Y to delete result or N to cancel.");
                    var confirm = Console.ReadKey(true).Key;

                    if (confirm == ConsoleKey.Y) // bekräfta val
                    {
                        records.RemoveAt(index - 1); // ta bort posten
                        File.WriteAllText(RecordPath, JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true })); // skriv tillbaka filen
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
                TextColor("\n🛑 Program has ended.\n", ConsoleColor.Red);
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("\n❗ Invalid choice. Press D, Enter or X.");
                Thread.Sleep(1000);
            }
        }
    }

    private static void ShowAllRecordsWithIndex(List<SleepRecord> records) // visar resultat med index
    {
        for (int i = 0; i < records.Count; i++) // loopar igenom alla poster
        {
            var r = records[i];
            Console.WriteLine($"\n[{i + 1}] {r.Date:yyyy-MM-dd}:"); // alla console.writeline nedan visar resultat i läsbart format
            Console.WriteLine($"- Sleep: {r.SleepHours switch { 1 => "1–2h", 2 => "3–6h", 3 => "7–8h", _ => "Unknown" }}");
            Console.WriteLine($"- Caffeine: {r.CaffeineHours switch { 1 => "1–5h before bed", 2 => "6–7h before bed", 3 => "8–10h before bed", _ => "Unknown" }}");
            Console.WriteLine($"- Stress: {r.StressLevel switch { 1 => "High", 2 => "Medium", 3 => "Low", _ => "Unknown" }}");
            Console.WriteLine($"- Activity: {r.ActivityLevel switch { 1 => "Low", 2 => "Medium", 3 => "High", _ => "Unknown" }}");
            Console.WriteLine($"- Sleep Quality: {r.SleepQuality switch { 1 => "Poor", 2 => "Average", 3 => "Good", _ => "Unknown" }}");
            Console.WriteLine($"- Predicted Level: {r.PredictedLevel}");
            Console.WriteLine($"- Score: {r.TotalScore}");
            Console.WriteLine();
        }
    }

    static PersonData StartTest() // startar test
    {
        var data = new PersonData(); // skapar objekt för att lagra svar

        data.SleepHours = Ask("\nHow many hours of sleep do you usually get?\n1) 1-2h\n2) 3-6h\n3) 7-8h"); // frågar och sparar svar
        data.CaffeineHours = Ask("\nHow many hours before bed do you drink caffeine?\n1) 1-5h\n2) 6-7h\n3) 8-10h");
        data.StressLevel = Ask("\nHow high would you rate your stress levels?\n1) High\n2) Medium\n3) Low");
        data.ActivityLevel = Ask("\nHow active are you during the day?\n1) Low\n2) Medium\n3) High");
        data.SleepQuality = Ask("\nHow would you rate your sleep quality?\n1) Poor\n2) Average\n3) Good");

        Console.Clear();
        return data; // returnerar objekt med svar
    }

    static int Ask(string question) // frågar frågor
    {
        while (true) // loopar tills giltiga svar angivits
        {
            Console.Clear();
            ShowHeader(" 💤 SleepApp💤 ");
            Console.WriteLine(question);
            Console.Write("\nChoose an option: ");

            string? input = Console.ReadLine(); // läser in inmatning

            if (string.IsNullOrEmpty(input)) continue; // fortsätter loop tills inmatning getts

            if (input.Trim().ToUpper() == "X") // avbryter test
            {
                Console.Write("\n⚠️  Are you sure you want to exit? Your progress will not be saved.");
                Console.WriteLine("\n-- Press Y to end test or N to continue test.");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\nEnding test...");
                    Thread.Sleep(1500);
                    TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
                    Console.WriteLine("Returning to menu...");
                    Thread.Sleep(1500);
                    Main();
                }
                continue;
            }

            if (int.TryParse(input, out int value) && value >= 1 && value <= 3) // om giltigt inmatning getts...
            {
                return value; //... returneras svaret
            }

            Console.WriteLine("\n❗ Invalid input. Choose 1, 2 or 3."); // felhantering
            Thread.Sleep(1500);
        }
    }

    static string GetLevel(float result) => (int)result switch // tar float-värde av resultat och returnerar textbeskrivning
    {
        1 => "Poor",
        2 => "Average",
        3 => "Good",
        _ => "Unknown"
    };

    static SleepRecord SaveAndCreateResult(PersonData data) // tar användarens svar (PersonData) och skapar fullständigt resultat (SleepRecord)
    {
        var result = SleepPredictionService.Predict(data); // anropar ML för att förutse resultat baserat på inmatning
        float predictedValue = float.Parse(result); // konverterar resultat från string till float
        string level = GetLevel(float.Parse(result)); // översätter nummer till textnivå

        float totalScore = data.SleepHours + data.CaffeineHours + data.StressLevel + data.ActivityLevel + data.SleepQuality; // beräknar totalpoäng

        var record = new SleepRecord // skapar objekt som innehåller användardata och förutsägning
        {
            SleepHours = data.SleepHours,
            CaffeineHours = data.CaffeineHours,
            StressLevel = data.StressLevel,
            ActivityLevel = data.ActivityLevel,
            SleepQuality = data.SleepQuality,
            SleepHabits = predictedValue,
            PredictedLevel = level,
            TotalScore = totalScore
        };

        RecordService.SaveRecord(record); // sparar fil
        return record; // returnerar sparar objekt
    }

    static void ShowResult(SleepRecord record) // skriver ut resultat
    {
        ShowHeader(" 💤 SleepApp💤 ");
        ConsoleColor levelColor = record.PredictedLevel switch
        {
            "Poor" => ConsoleColor.Red,
            "Average" => ConsoleColor.Yellow,
            "Good" => ConsoleColor.Green,
            _ => ConsoleColor.White
        };
        TextColor("\nSleep Habit Level: " + record.PredictedLevel, levelColor); // färglägger text

        string description = record.PredictedLevel switch // feedback utefter förutsett resultat
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

        Console.WriteLine("\nTotal Score: " + record.TotalScore + " / 15\n\n5–7 → Poor\n8–11 → Average\n12–15 → Good\n");

        if (record.SleepHours == 1)
        {
            TextColor("⚠️  Note: You are getting very little sleep hours. Try to rest more!\n", ConsoleColor.Red);
        }

        ShowFooter(94);
    }

    static void ShowHeader(string title) // skriver ut head
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n" + new string('=', 40) + title + new string('=', 39));
        Console.ResetColor();
    }

    static void ShowFooter(int length) // skriver ut footer
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(new string('=', length));
        Console.ResetColor();
    }

    static void TextColor(string text, ConsoleColor color) // färglägger text
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}