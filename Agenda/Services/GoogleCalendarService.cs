using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Agenda.Services
{
    public class GoogleCalendarService
    {
        private readonly string serviceAccountKeyPath;
        private readonly string calendarId;
        private CalendarService calendarService;

        // Couleurs pour les événements
        private readonly List<SolidColorBrush> eventColors = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromRgb(86, 204, 242)),   // Bleu clair
            new SolidColorBrush(Color.FromRgb(155, 89, 182)),   // Violet
            new SolidColorBrush(Color.FromRgb(52, 152, 219)),   // Bleu
            new SolidColorBrush(Color.FromRgb(46, 204, 113)),   // Vert
            new SolidColorBrush(Color.FromRgb(241, 196, 15)),   // Jaune
            new SolidColorBrush(Color.FromRgb(230, 126, 34)),   // Orange
            new SolidColorBrush(Color.FromRgb(231, 76, 60))     // Rouge
        };

        public GoogleCalendarService(string jsonKeyPath = @"C:\path\to\your\agenda-456107-credentials.json", string calendarId = "primary")
        {
            this.serviceAccountKeyPath = jsonKeyPath;
            this.calendarId = calendarId;
        }

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream(serviceAccountKeyPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(CalendarService.Scope.Calendar); // Scope complet pour lecture/écriture
                }

                calendarService = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Agenda"
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<CalendarEvent>> GetEventsAsync(DateTime start, DateTime end)
        {
            if (calendarService == null)
                throw new InvalidOperationException("Vous devez vous authentifier avant d'appeler cette méthode.");

            var request = calendarService.Events.List(calendarId);
            request.TimeMin = start;
            request.TimeMax = end;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = await request.ExecuteAsync();
            var random = new Random();

            return events.Items.Select(e => new CalendarEvent
            {
                Id = e.Id,
                Title = e.Summary ?? "(Sans titre)",
                Description = e.Description ?? "",
                Start = e.Start.DateTime ?? DateTime.Parse(e.Start.Date),
                End = e.End.DateTime ?? DateTime.Parse(e.End.Date),
                IsAllDay = e.Start.DateTime == null,
                Location = e.Location ?? "",
                Color = eventColors[random.Next(eventColors.Count)]
            }).ToList();
        }

        public async Task<CalendarEvent> CreateEventAsync(CalendarEvent calendarEvent)
        {
            if (calendarService == null)
                throw new InvalidOperationException("Vous devez vous authentifier avant d'appeler cette méthode.");

            var newEvent = new Event
            {
                Summary = calendarEvent.Title,
                Description = calendarEvent.Description,
                Location = calendarEvent.Location,
                Start = calendarEvent.IsAllDay
                    ? new EventDateTime { Date = calendarEvent.Start.ToString("yyyy-MM-dd") }
                    : new EventDateTime { DateTime = calendarEvent.Start },
                End = calendarEvent.IsAllDay
                    ? new EventDateTime { Date = calendarEvent.End.ToString("yyyy-MM-dd") }
                    : new EventDateTime { DateTime = calendarEvent.End }
            };

            var request = calendarService.Events.Insert(newEvent, calendarId);
            var createdEvent = await request.ExecuteAsync();

            var random = new Random();
            return new CalendarEvent
            {
                Id = createdEvent.Id,
                Title = createdEvent.Summary ?? "(Sans titre)",
                Description = createdEvent.Description ?? "",
                Start = createdEvent.Start.DateTime ?? DateTime.Parse(createdEvent.Start.Date),
                End = createdEvent.End.DateTime ?? DateTime.Parse(createdEvent.End.Date),
                IsAllDay = createdEvent.Start.DateTime == null,
                Location = createdEvent.Location ?? "",
                Color = eventColors[random.Next(eventColors.Count)]
            };
        }

        public async Task<CalendarEvent> UpdateEventAsync(CalendarEvent calendarEvent)
        {
            if (calendarService == null)
                throw new InvalidOperationException("Vous devez vous authentifier avant d'appeler cette méthode.");

            // Récupérer l'événement existant
            var existingEvent = await calendarService.Events.Get(calendarId, calendarEvent.Id).ExecuteAsync();

            // Mettre à jour les propriétés
            existingEvent.Summary = calendarEvent.Title;
            existingEvent.Description = calendarEvent.Description;
            existingEvent.Location = calendarEvent.Location;
            existingEvent.Start = calendarEvent.IsAllDay
                ? new EventDateTime { Date = calendarEvent.Start.ToString("yyyy-MM-dd") }
                : new EventDateTime { DateTime = calendarEvent.Start };
            existingEvent.End = calendarEvent.IsAllDay
                ? new EventDateTime { Date = calendarEvent.End.ToString("yyyy-MM-dd") }
                : new EventDateTime { DateTime = calendarEvent.End };

            // Envoyer la mise à jour
            var updatedEvent = await calendarService.Events.Update(existingEvent, calendarId, calendarEvent.Id).ExecuteAsync();

            return new CalendarEvent
            {
                Id = updatedEvent.Id,
                Title = updatedEvent.Summary ?? "(Sans titre)",
                Description = updatedEvent.Description ?? "",
                Start = updatedEvent.Start.DateTime ?? DateTime.Parse(updatedEvent.Start.Date),
                End = updatedEvent.End.DateTime ?? DateTime.Parse(updatedEvent.End.Date),
                IsAllDay = updatedEvent.Start.DateTime == null,
                Location = updatedEvent.Location ?? "",
                Color = calendarEvent.Color // Conserver la couleur existante
            };
        }

        public async Task DeleteEventAsync(string eventId)
        {
            if (calendarService == null)
                throw new InvalidOperationException("Vous devez vous authentifier avant d'appeler cette méthode.");

            await calendarService.Events.Delete(calendarId, eventId).ExecuteAsync();
        }
    }
}

