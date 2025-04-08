using Agenda.Services;
using System.Windows;

namespace Agenda.Views
{
    public partial class EventDetailsWindow : Window
    {
        public EventDetailsWindow(CalendarEvent calendarEvent)
        {
            InitializeComponent();

            // Remplir les détails de l'événement
            TitreText.Text = $"Titre: {calendarEvent.Title}";
            DateText.Text = $"Date: {calendarEvent.FormattedTime}";
            DureeText.Text = $"Durée: {calendarEvent.Duration}";
            LieuText.Text = $"Lieu: {(string.IsNullOrEmpty(calendarEvent.Location) ? "Non spécifié" : calendarEvent.Location)}";
            DescriptionText.Text = $"Description: {(string.IsNullOrEmpty(calendarEvent.Description) ? "Aucune description" : calendarEvent.Description)}";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}