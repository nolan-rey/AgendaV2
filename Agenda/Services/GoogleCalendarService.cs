using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media;

namespace Agenda.Services
{
    public class GoogleCalendarServices
    {
        private static readonly HttpClient client = new HttpClient();

        // ✅ Remplace par ta clé API et l'adresse du calendrier
        private const string apiKey = "CLE-API";
        private const string calendarId = "onn74700@gmail.com"; // ou l'ID du calendrier public

        public async Task<List<CalendarEvent>> GetPublicEventsAsync()
        {
            string url = $"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events?key={apiKey}&singleEvents=true&orderBy=startTime&timeMin={DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            var data = JsonDocument.Parse(json);
            var events = new List<CalendarEvent>();

            foreach (var item in data.RootElement.GetProperty("items").EnumerateArray())
            {
                try
                {
                    var ev = new CalendarEvent
                    {
                        Id = item.GetProperty("id").GetString(),
                        Title = item.GetProperty("summary").GetString() ?? "(Sans titre)",
                        Description = item.TryGetProperty("description", out var desc) ? desc.GetString() : "",
                        Location = item.TryGetProperty("location", out var loc) ? loc.GetString() : "",
                        Start = item.GetProperty("start").TryGetProperty("dateTime", out var dtStart)
                            ? DateTime.Parse(dtStart.GetString())
                            : DateTime.Parse(item.GetProperty("start").GetProperty("date").GetString()),
                        End = item.GetProperty("end").TryGetProperty("dateTime", out var dtEnd)
                            ? DateTime.Parse(dtEnd.GetString())
                            : DateTime.Parse(item.GetProperty("end").GetProperty("date").GetString()),
                        IsAllDay = !item.GetProperty("start").TryGetProperty("dateTime", out _),
                        Color = new SolidColorBrush(Color.FromRgb(66, 133, 244)) // Bleu Google
                    };

                    events.Add(ev);
                }
                catch
                {
                    // Ignorer les erreurs de parsing pour certains événements
                }
            }

            return events;
        }
    }
}
