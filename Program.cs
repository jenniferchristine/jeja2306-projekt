using System.Reflection;
using SleepApp;
using SleepApp.Models;
using SleepApp.Services;

class Program
{
    static void Main() // huvudmetod för att starta program
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Welcome to SleepApp ===");
        Console.ResetColor();

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
        Console.WriteLine("\nPredicting...");
        var result = SleepPredictionService.Predict(data);

        // visar resultat
        Console.WriteLine("Based on your answers: " + result);

        // stänger programmet på x
        Console.WriteLine("Press X to exit");
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
        Console.WriteLine(question); // visar frågan
        Console.Write("Answer 1-3: ");

        int value;
        while (!int.TryParse(Console.ReadLine(), out value) || value < 1 || value > 3)
            Console.Write("Please enter 1, 2 or 3 as your answer");
        return value;
        
    }
}