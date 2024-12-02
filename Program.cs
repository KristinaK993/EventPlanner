using System;

class Program
{
    static void Main(string[] args)
    {
        EventManager eventManager = new EventManager();

        while (true)
        {
            Console.WriteLine("\n--- Eventplanerare ---");
            Console.WriteLine("1. Skapa ett nytt event");
            Console.WriteLine("2. Visa alla events");
            Console.WriteLine("3. Kontrollera konflikter");
            Console.WriteLine("4. Visa tid kvar till ett event");
            Console.WriteLine("5. Avsluta");
            Console.Write("Välj ett alternativ: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    eventManager.CreateEvent();
                    break;
                case "2":
                    eventManager.ShowEvents();
                    break;
                case "3":
                    eventManager.CheckConflicts();
                    break;
                case "4":
                    eventManager.TimeUntilEvent();
                    break;
                case "5":
                    Console.WriteLine("Programmet avslutas. Tack för att du använde eventplaneraren!");
                    return;
                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    break;
            }
        }
    }
}
