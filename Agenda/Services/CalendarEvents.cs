using System;
using System.Windows.Media;

namespace Agenda.Services
{
    public class CalendarEvent
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Location { get; set; }
        public SolidColorBrush Color { get; set; }

        // Propriété calculée pour l'affichage de la durée
        public string Duration
        {
            get
            {
                if (IsAllDay)
                    return "Toute la journée";

                TimeSpan duration = End - Start;
                if (duration.TotalHours < 1)
                    return $"{duration.Minutes} min";
                else if (duration.TotalDays < 1)
                    return $"{Math.Floor(duration.TotalHours)}h{duration.Minutes:D2}";
                else
                    return $"{Math.Floor(duration.TotalDays)} jour(s) {duration.Hours}h";
            }
        }

        // Propriété calculée pour l'affichage de la date et l'heure
        public string FormattedTime
        {
            get
            {
                if (IsAllDay)
                    return Start.ToString("dd MMMM yyyy");

                if (Start.Date == End.Date)
                    return $"{Start.ToString("dd MMMM yyyy")} {Start.ToString("HH:mm")} - {End.ToString("HH:mm")}";
                else
                    return $"{Start.ToString("dd MMMM yyyy HH:mm")} - {End.ToString("dd MMMM yyyy HH:mm")}";
            }
        }
    }
}

