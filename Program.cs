using System.Reflection;
using SleepApp;
using SleepApp.Models;
using SleepApp.Services;

class Program
{
    static void Main() // huvudmetod för att starta program
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n=== 💤 Welcome to SleepApp 💤 ===");
        Console.ResetColor();

        Console.WriteLine("\nSleepApp helps to determine your sleep quality by answering 5 simple questions.\nPlease select one of the following options and press enter for next question or X to exit the program.\n");
        Console.WriteLine(new string('=', 40));

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

        // gissar 
        // Console.WriteLine("\nPredicting...");
        var result = SleepPredictionService.Predict(data);

        switch (result) // ändrar färg på resultattext
        {
            case "1":
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nResult: " + result);
                Console.ResetColor();
                Console.WriteLine("Your sleep quality could deffintly improve");
                break;

            case "2":
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Based on your answers: " + result);
                break;

            case "3":
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Based on your answers: " + result);
                break;
        }

        // stänger programmet på x
        Console.WriteLine("\nPress X to exit");
        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.X)
        {
            Console.WriteLine("\nExiting program...");
        }
        else
        {
            Console.WriteLine("\nProgram ended");
        }
    }

    static int Ask(string question) // metod för att ställa fråga, ta emot svar och validera
    {

        while (true)
        {
            Console.WriteLine(question); // visar frågan
            Console.Write("\nChoose an option: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input.Trim().ToUpper() == "X")
                Environment.Exit(0); // avslutar programmet direkt

            if (int.TryParse(input, out int value) && value >= 1 && value <= 3)
                return value;
        }

    }
}