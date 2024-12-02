using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

public class EventManager
{
    private List<Event> _events;
    private const string FilePath = "events.json"; // Filen där events lagras

    // Konstruktor
    public EventManager()
    {
        _events = new List<Event>();
        LoadEvents(); // Ladda events från JSON-fil vid start
    }

    // Skapa ett nytt event
    public void CreateEvent()
    {
        Console.WriteLine("\n--- Skapa ett nytt event ---");

        Console.Write("Ange eventnamn: ");
        string name = Console.ReadLine();

        DateTime dateTime;
        while (true)
        {
            Console.Write("Ange datum och tid för eventet (format: yyyy-MM-dd HH:mm): ");
            string dateTimeInput = Console.ReadLine();

            try
            {
                dateTime = DateTime.ParseExact(dateTimeInput, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                break;
            }
            catch (FormatException)
            {
                Console.WriteLine("Felaktigt format! Försök igen. Formatet ska vara: yyyy-MM-dd HH:mm.");
            }
        }

        Console.Write("Ange längd på eventet (i timmar och minuter, t.ex. 2:30): ");
        string[] durationParts = Console.ReadLine().Split(':');
        TimeSpan duration = new TimeSpan(int.Parse(durationParts[0]), int.Parse(durationParts[1]), 0);

        string timeZone;
        while (true)
        {
            Console.Write("Ange tidszon (t.ex. Eastern Standard Time eller UTC-offset som +05:30): ");
            timeZone = Console.ReadLine();

            if (IsValidTimeZone(timeZone))
            {
                break;
            }
            else
            {
                Console.WriteLine("Ogiltig tidszon! Försök igen.");
            }
        }

        Event newEvent = new Event(name, dateTime, duration, timeZone);
        _events.Add(newEvent);

        SaveEvents();
        Console.WriteLine("Eventet har skapats och sparats.");
    }

    // Kontrollera om tidszonen är giltig
    private bool IsValidTimeZone(string timeZoneId)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }


    // Visa alla events
    public void ShowEvents()
    {
        if (_events.Count == 0)
        {
            Console.WriteLine("\nInga events att visa.");
            return;
        }

        Console.WriteLine("\n--- Lista av events ---");
        foreach (var ev in _events)
        {
            Console.WriteLine($"Eventnamn: {ev.Name}");
            Console.WriteLine($"Datum och tid: {ev.DateTime} ({ev.TimeZoneId})");
            Console.WriteLine($"Längd: {ev.Duration}");
            Console.WriteLine("---------------------------");
        }
    }

    // Kontrollera event-konflikter
    public void CheckConflicts()
    {
        Console.WriteLine("\n--- Kontrollera konflikter ---");

        DateTime dateTime;
        while (true)
        {
            Console.Write("Ange datum och tid för det nya eventet (format: yyyy-MM-dd HH:mm): ");
            string dateTimeInput = Console.ReadLine();

            try
            {
                dateTime = DateTime.ParseExact(dateTimeInput, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                break;
            }
            catch (FormatException)
            {
                Console.WriteLine("Felaktigt format! Försök igen. Formatet ska vara: yyyy-MM-dd HH:mm.");
            }
        }

        Console.Write("Ange längd på det nya eventet (i timmar och minuter, t.ex. 2:30): ");
        string[] durationParts = Console.ReadLine().Split(':');
        TimeSpan newEventDuration = new TimeSpan(int.Parse(durationParts[0]), int.Parse(durationParts[1]), 0);

        bool conflictFound = false;

        foreach (var ev in _events)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(ev.TimeZoneId);
            DateTime eventStart = TimeZoneInfo.ConvertTime(ev.DateTime, timeZone);
            DateTime eventEnd = eventStart.Add(ev.Duration);

            DateTime newEventEnd = dateTime.Add(newEventDuration);

            if (dateTime < eventEnd && newEventEnd > eventStart)
            {
                conflictFound = true;
                Console.WriteLine($"Konflikt med event: {ev.Name} som börjar {eventStart} och slutar {eventEnd}.");
            }
        }

        if (!conflictFound)
        {
            Console.WriteLine("Inga konflikter hittades.");
        }
    }

    // Visa tid kvar till ett event
    public void TimeUntilEvent()
    {
        Console.WriteLine("\n--- Tid kvar till ett event ---");

        if (_events.Count == 0)
        {
            Console.WriteLine("Inga events att visa.");
            return;
        }

        for (int i = 0; i < _events.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {_events[i].Name} ({_events[i].DateTime})");
        }

        Console.Write("Välj ett event (ange siffra): ");
        int choice = int.Parse(Console.ReadLine()) - 1;

        if (choice >= 0 && choice < _events.Count)
        {
            Event selectedEvent = _events[choice];
            TimeSpan timeLeft = selectedEvent.DateTime - DateTime.Now;

            Console.WriteLine($"Tid kvar till {selectedEvent.Name}: {timeLeft.Days} dagar, {timeLeft.Hours} timmar, {timeLeft.Minutes} minuter och {timeLeft.Seconds} sekunder.");
        }
        else
        {
            Console.WriteLine("Ogiltigt val.");
        }
    }

    // Ladda events från JSON-fil
    private void LoadEvents()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            _events = JsonSerializer.Deserialize<List<Event>>(json) ?? new List<Event>();
        }
    }

    // Spara events till JSON-fil
    private void SaveEvents()
    {
        string json = JsonSerializer.Serialize(_events, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }
}
