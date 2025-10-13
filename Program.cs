using System.Reflection;
using SleepApp;
using SleepApp.Models;
using SleepApp.Services;

class Program
{
    static void Main() // huvudmetod för att starta program
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + new string('=', 40) + " 💤 Welcome to SleepApp!💤 " + new string('=', 39));
            Console.ResetColor();

            Console.WriteLine("\nSleepApp helps to determine your sleep habits by answering 5 simple questions.\nYou answer by choosing the option that suits you the best and press enter for the next question.");
            Console.WriteLine("\nContinue to test by pressing enter or X to end program.\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(new string('=', 106));
            Console.ResetColor();

            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.X) return;
            if (key != ConsoleKey.Enter) continue;

            if (!File.Exists("Models/sleepModel.zip"))
            {
                TrainModel.Train(); // tränar om modell saknas
            }

            var data = new PersonData(); // objekt för användarens svar

            // frågor som ska besvaras
            data.SleepHours = Ask("\nHow many hours of sleep do you usually get?\n\n1) 1-2h\n2) 3-6h\n3) 7-8h");
            data.CaffeineHours = Ask("\nHow many hours before bed do you drink caffeine?\n\n1) 1-5h\n2) 6-7h\n3) 8-10h");
            data.StressLevel = Ask("\nHow high would you rate your stress levels?\n\n1) High\n2) Medium\n3) Low");
            data.ActivityLevel = Ask("\nHow active are you during the day?\n\n1) Low\n2) Medium\n3) High");
            data.SleepQuality = Ask("\nHow would you rate your sleep quality?\n\n1) Poor\n2) Average\n3) Good");

            Console.Clear();

            // gissar 
            var result = SleepPredictionService.Predict(data);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n" + new string('=', 40) + " 💤 SleepApp💤 " + new string('=', 39));
            Console.ResetColor();

            switch (result) // ändrar färg på resultattext
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nResult: " + result);
                    Console.ResetColor();
                    Console.WriteLine("Your sleep needs significant improvement\n");
                    break;

                case "2":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nResult: " + result);
                    Console.ResetColor();
                    Console.WriteLine("Your sleep could improve\n");
                    break;

                case "3":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nResult: " + result);
                    Console.ResetColor();
                    Console.WriteLine("Your sleep habits seem very good\n");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(new string('=', 94));
            Console.ResetColor();

            // stänger programmet på x
            Console.WriteLine("\nPress X to exit or enter to retake test");
            var exitKey = Console.ReadKey(true).Key;

            if (exitKey == ConsoleKey.X)
            {
                Console.WriteLine("\nTest has ended.\n");
            }
            else
            {
                Console.WriteLine("\nProgram ended");
            }
        }
    }

    static int Ask(string question)
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + new string('=', 40) + " 💤 SleepApp💤 " + new string('=', 39));
            Console.ResetColor();

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
}