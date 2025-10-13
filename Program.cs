using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using SleepApp;
using SleepApp.Models;
using SleepApp.Services;

class Program
{
    static void Main() // huvudmetod för att starta program
    {
        while (true) // loop för att starta om programmet
        {
            Console.Clear(); // rensa föregående test

            ShowHeader(" 💤 Welcome to SleepApp!💤 ");
            Console.WriteLine("\nSleepApp helps to determine your sleep habits by answering 5 simple questions.\nYou answer by choosing the option that suits you the best and press enter for the next question.\n\nContinue to test by pressing enter or X to end program.\n");
            EndHeader(106);

            while (true)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.X)
                {
                    TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
                    Environment.Exit(0); // avslutar programmet helt
                }
                else if (key == ConsoleKey.Enter)
                {
                    break; // fortsätter mainloop
                }
                else
                {
                    Console.WriteLine("\nInvalid choice. Press Enter to start or X to exit."); // loopar denna loop igen
                }
            }

            if (!File.Exists("Models/sleepModel.zip"))
            {
                TrainModel.Train(); // tränar om om modell saknas
            }

            var data = new PersonData(); // objekt för användarens svar

            // frågor som ska besvaras
            data.SleepHours = Ask("\nHow many hours of sleep do you usually get?\n\n1) 1-2h\n2) 3-6h\n3) 7-8h");
            data.CaffeineHours = Ask("\nHow many hours before bed do you drink caffeine?\n\n1) 1-5h\n2) 6-7h\n3) 8-10h");
            data.StressLevel = Ask("\nHow high would you rate your stress levels?\n\n1) High\n2) Medium\n3) Low");
            data.ActivityLevel = Ask("\nHow active are you during the day?\n\n1) Low\n2) Medium\n3) High");
            data.SleepQuality = Ask("\nHow would you rate your sleep quality?\n\n1) Poor\n2) Average\n3) Good");

            Console.Clear();

            // gissar baserat på datan
            var result = SleepPredictionService.Predict(data);

            ShowHeader(" 💤 SleepApp💤 ");

            string level = GetLevel(float.Parse(result));

            ConsoleColor levelColor = level switch // färglägger resultat
            {
                "Poor" => ConsoleColor.Red,
                "Average" => ConsoleColor.Yellow,
                "Good" => ConsoleColor.Green,
                _ => ConsoleColor.White
            };

            TextColor("\nSleep Level: " + level, levelColor);

            string description = level switch // tillför meddelande
            {
                "Poor" => "Your sleep habits need significant improvement. Try to get more rest and maintain a consistent sleep schedule.",
                "Average" => "Your sleep is okay but could be better. Consider improving your bedtime routine or reducing stress before bed.",
                "Good" => "Your sleep habits are very good! Keep up your healthy routines.",
                _ => ""
            };

            Console.WriteLine(description + "\n");

            Console.WriteLine("Result of answers:");
            Console.WriteLine("- Sleep Hours: " + (data.SleepHours switch { 1 => "1-2h", 2 => "3-6h", 3 => "7-8h", _ => "Unknown" }) + " " + GetLevel(data.SleepHours));

            if (data.SleepHours == 1)
            {
                TextColor("\n⚠️  Note: You are getting very little sleep hours. Try to rest more!\n", ConsoleColor.Red);
            }

            /*
            switch (result) // ändrar färg på resultattext
            {
                case "1":
                    TextColor("\nResult: " + result, ConsoleColor.Red);
                    Console.WriteLine("Your sleep habits need significant improvement. Try to get more rest and maintain a consistent sleep schedule.\n");
                    break;

                case "2":
                    TextColor("\nResult: " + result, ConsoleColor.Yellow);
                    Console.WriteLine("Your sleep is okay but could be better. Consider improving your bedtime routine or reducing stress before bed.\n");
                    break;

                case "3":
                    TextColor("\nResult: " + result, ConsoleColor.Green);
                    Console.WriteLine("Your sleep habits are very good! Keep up your healthy routines.\n");
                    break;
            }

            if (data.SleepHours == 1 && result == "3")
            {
                Console.WriteLine("⚠️  Even though your sleeping habits are good you need sleep more hours!\n");
            }*/

            EndHeader(94);

            // stänger programmet eller börjar om
            Console.WriteLine("\nPress X to exit or enter to retake test\n");
            var exitKey = Console.ReadKey(true).Key;

            if (exitKey == ConsoleKey.X)
            {
                TextColor("\n🛑 Test has ended.\n", ConsoleColor.Red);
                break;
            }
            else if (exitKey == ConsoleKey.Enter)
            {
                continue;
            }
        }
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
            Console.Write("\n\n");

            if (string.IsNullOrEmpty(input))
                continue;

            if (input.Trim().ToUpper() == "X")
                Environment.Exit(0);

            if (int.TryParse(input, out int value) && value >= 1 && value <= 3)
                return value;
        }
    }

    static void ShowHeader(string title) // metod för att "öppna header"
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n" + new string('=', 40) + title + new string('=', 39));
        Console.ResetColor();
    }
    static void EndHeader(int length) // metod för att "stänga headern"
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