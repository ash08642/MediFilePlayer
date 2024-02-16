using Microsoft.VisualBasic.FileIO;
using System;
using System.Linq.Expressions;

namespace MediFilePlayer
{
    internal class Program
    {
        static void printOptions(string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine($"Enter [{i}] To Play: {array[i]}");
            }
            Console.WriteLine();
            for (int i = 0;i < array.Length;i++)
            {
                Console.WriteLine($"Drücken Sie [{i}] um {array[i]} abzuspielen");
            }
            Console.WriteLine("\nEnter Song Number: ");
        }

        public static int getMusicOption(int length)
        {
            string option;
            while (true)
            {
                option = Console.ReadLine();
                try
                {
                    if (Int32.Parse(option) < 0 || Int32.Parse(option) >= length)
                    {
                        Console.WriteLine("Option does not exist. Please input a valid Number!");
                        Console.WriteLine("Die Option existiert nicht. Bitte geben Sie eine gültige Nummer ein!");
                        continue;
                    }
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Only Numbers are allowed. Please input a valid Number!");
                    Console.WriteLine("Nur Zahlen sind erlaubt. Bitte geben Sie eine gültige Nummer ein!");
                    continue;
                }
                return Int32.Parse(option);
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string[] fileArray = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.mid");
            Console.WriteLine("Choose Music. Wahlen ein Lied");
            for (int i = 0; i < fileArray.Length; i++)
            {
                fileArray[i] = fileArray[i].Substring(fileArray[i].LastIndexOf('\\') + 1);
            }
            printOptions(fileArray);

            MediPlayer mediPlayer;

            mediPlayer = new MediPlayer(fileArray[getMusicOption(fileArray.Length)]);
            mediPlayer.startPlaying();

            bool running = true;
            Console.WriteLine("\n[E/e] Exit Application\t[P/p] Pause Song\t\t[C/c] Continue Song\n"
                + "[B/b] Change Song\t[+] Increase System Volume\t[-] Decrease System Volume");
            Console.WriteLine("\n[E/e] Anwendung beenden\t[P/p] Song anhalten\t[C/c] Song fortsetzen\n"
                + "[B/b] Song ändern\t[+] Systemlautstärke erhöhen\t[-] Systemlautstärke verringern");
            while (running == true) 
            {
                Console.WriteLine("Enter Option:");
                string input = Console.ReadLine();

                switch (input) 
                {
                    case "B":
                    case "b":
                        mediPlayer.stopPlaying();
                        printOptions(fileArray);
                        Console.WriteLine("Choose new Song");
                        mediPlayer.changeMusic(fileArray[getMusicOption(fileArray.Length)]);
                        break;
                    case "P":
                    case "p":
                        mediPlayer.pause();
                        break;
                    case "C":
                    case "c":
                        mediPlayer.play();
                        break;
                    case "+":
                        mediPlayer.increaseVolume();
                        break;
                    case "-":
                        mediPlayer.decreaseVolume();
                        break;
                    case "E":
                    case "e":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid Command. Try Again!");
                        break;
                }
            }
        }
    }
}
