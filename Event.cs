using System;
using System.Text.Json.Serialization;

public class Event
{
    public string Name { get; set; }
    public DateTime DateTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string TimeZoneId { get; set; }

    [JsonIgnore]
    public TimeZoneInfo TimeZone => TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

    public Event(string name, DateTime dateTime, TimeSpan duration, string timeZoneId)
    {
        Name = name;
        DateTime = dateTime;
        Duration = duration;
        TimeZoneId = timeZoneId;
    }
}
