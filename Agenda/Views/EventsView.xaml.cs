using Agenda.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Agenda.Views
{
    public partial class EventsView : UserControl
    {
        public ObservableCollection<CalendarEvent> Events { get; set; } = new();

        private DateTime _currentMonth;
        private List<SolidColorBrush> _eventColors;

        public EventsView()
        {
            InitializeComponent();
            DataContext = this;

            _currentMonth = DateTime.Now;
            InitializeEventColors();

            Loaded += EventsView_Loaded;
        }

        private void InitializeEventColors()
        {
            // Palette de couleurs pour les événements
            _eventColors = new List<SolidColorBrush>
            {
                new SolidColorBrush(Color.FromRgb(86, 204, 242)),   // Bleu clair
                new SolidColorBrush(Color.FromRgb(155, 89, 182)),   // Violet
                new SolidColorBrush(Color.FromRgb(52, 152, 219)),   // Bleu
                new SolidColorBrush(Color.FromRgb(46, 204, 113)),   // Vert
                new SolidColorBrush(Color.FromRgb(241, 196, 15)),   // Jaune
                new SolidColorBrush(Color.FromRgb(230, 126, 34)),   // Orange
                new SolidColorBrush(Color.FromRgb(231, 76, 60))     // Rouge
            };
        }

        private async void EventsView_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshCalendar();
        }

        private async System.Threading.Tasks.Task RefreshCalendar()
        {
            try
            {
                var service = new GoogleCalendarServices();
                var events = await service.GetPublicEventsAsync();

                // Assigner des couleurs aux événements
                var random = new Random();
                foreach (var ev in events)
                {
                    if (ev.Color == null)
                    {
                        ev.Color = _eventColors[random.Next(_eventColors.Count)];
                    }
                }

                Events.Clear();
                foreach (var ev in events)
                {
                    Events.Add(ev);
                }

                // Mettre à jour l'affichage
                UpdateCalendarDisplay();
                UpdateUpcomingEventsList();
            }
            catch (Exception ex)
            {
                // Logguer l'erreur silencieusement
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des événements: {ex.Message}");
            }
        }

        private void UpdateCalendarDisplay()
        {
            // Mettre à jour le texte du mois
            CurrentMonthText.Text = $" - {_currentMonth:MMMM yyyy}";

            // Générer la grille du calendrier
            GenerateCalendarGrid();
        }

        private void GenerateCalendarGrid()
        {
            // Réinitialiser la grille
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            // Calculer les dates de début et de fin pour le calendrier
            var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Ajuster pour inclure la semaine complète
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
            if (firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday) firstDayOfCalendar = firstDayOfCalendar.AddDays(-7);

            var lastDayOfCalendar = lastDayOfMonth;
            int daysToAdd = 7 - (int)lastDayOfCalendar.DayOfWeek;
            if (daysToAdd < 7) lastDayOfCalendar = lastDayOfCalendar.AddDays(daysToAdd);

            // Calculer le nombre de semaines à afficher
            int numberOfWeeks = (int)Math.Ceiling((lastDayOfCalendar - firstDayOfCalendar).TotalDays / 7.0);

            // Configurer les colonnes (jours de la semaine)
            for (int i = 0; i < 7; i++)
            {
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Configurer les lignes (semaines)
            for (int i = 0; i < numberOfWeeks; i++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            // Remplir la grille avec les jours
            DateTime currentDate = firstDayOfCalendar;
            for (int week = 0; week < numberOfWeeks; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    // Créer un conteneur pour le jour
                    var dayContainer = new Border
                    {
                        Style = (Style)Resources["CalendarDayStyle"],
                        Margin = new Thickness(2)
                    };

                    // Créer le contenu du jour
                    var dayContent = new Grid();
                    dayContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    dayContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    // Ajouter le numéro du jour ou l'indicateur du jour actuel
                    if (currentDate.Date == DateTime.Today)
                    {
                        // Créer un Grid pour contenir l'indicateur du jour actuel
                        var todayPanel = new Grid();

                        // Ajouter l'ellipse comme arrière-plan
                        var todayIndicator = new Ellipse
                        {
                            Width = 24,
                            Height = 24,
                            Fill = new SolidColorBrush(Color.FromRgb(66, 133, 244)),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Margin = new Thickness(2, 2, 0, 0)
                        };
                        todayPanel.Children.Add(todayIndicator);

                        // Ajouter le texte du jour
                        var todayText = new TextBlock
                        {
                            Text = currentDate.Day.ToString(),
                            Foreground = new SolidColorBrush(Colors.White),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 0)
                        };
                        todayPanel.Children.Add(todayText);

                        // Ajouter le panel au contenu du jour
                        dayContent.Children.Add(todayPanel);
                    }
                    else
                    {
                        // Ajouter simplement le numéro du jour
                        var dayNumber = new TextBlock
                        {
                            Text = currentDate.Day.ToString(),
                            Margin = new Thickness(5, 5, 0, 5),
                            FontWeight = currentDate.Month == _currentMonth.Month ? FontWeights.Normal : FontWeights.Light,
                            Foreground = currentDate.Month == _currentMonth.Month ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray)
                        };
                        dayContent.Children.Add(dayNumber);
                    }

                    // Conteneur pour les événements du jour
                    var eventsContainer = new StackPanel
                    {
                        Margin = new Thickness(2, 2, 2, 2)
                    };
                    Grid.SetRow(eventsContainer, 1);
                    dayContent.Children.Add(eventsContainer);

                    // Ajouter les événements pour ce jour
                    var dayEvents = Events.Where(e =>
                        (e.Start.Date <= currentDate.Date && e.End.Date >= currentDate.Date)).ToList();

                    foreach (var evt in dayEvents.Take(3))
                    {
                        var eventBorder = new Border
                        {
                            Style = (Style)Resources["CalendarEventStyle"],
                            Background = evt.Color,
                            Tag = evt
                        };

                        var eventContent = new TextBlock
                        {
                            Text = evt.IsAllDay ? evt.Title : $"{evt.Start.ToString("HH:mm")} {evt.Title}",
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            Foreground = new SolidColorBrush(Colors.White),
                            FontSize = 11
                        };

                        eventBorder.Child = eventContent;
                        eventsContainer.Children.Add(eventBorder);

                        // Ajouter un gestionnaire d'événements pour afficher les détails
                        eventBorder.MouseLeftButtonUp += EventBorder_MouseLeftButtonUp;
                    }

                    // Indiquer s'il y a plus d'événements
                    if (dayEvents.Count > 3)
                    {
                        eventsContainer.Children.Add(new TextBlock
                        {
                            Text = $"+ {dayEvents.Count - 3} autres",
                            Foreground = new SolidColorBrush(Colors.Gray),
                            FontSize = 10,
                            Margin = new Thickness(5, 2, 0, 0)
                        });
                    }

                    // Ajouter le conteneur du jour à la grille
                    dayContainer.Child = dayContent;
                    Grid.SetRow(dayContainer, week);
                    Grid.SetColumn(dayContainer, day);
                    CalendarGrid.Children.Add(dayContainer);

                    // Passer au jour suivant
                    currentDate = currentDate.AddDays(1);
                }
            }
        }

        private void UpdateUpcomingEventsList()
        {
            // Effacer les événements existants
            UpcomingEventsPanel.Children.Clear();
            UpcomingEventsPanel.Children.Add(NoEventsText);

            // Filtrer les événements à venir (à partir d'aujourd'hui)
            var upcomingEvents = Events
                .Where(e => e.End >= DateTime.Today)
                .OrderBy(e => e.Start)
                .Take(10)
                .ToList();

            if (upcomingEvents.Count == 0)
            {
                NoEventsText.Visibility = Visibility.Visible;
                return;
            }

            NoEventsText.Visibility = Visibility.Collapsed;

            // Ajouter chaque événement à la liste
            foreach (var evt in upcomingEvents)
            {
                var eventCard = CreateEventCard(evt);
                UpcomingEventsPanel.Children.Add(eventCard);
            }
        }

        private Border CreateEventCard(CalendarEvent evt)
        {
            var card = new Border
            {
                Style = (Style)Resources["EventCardStyle"]
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Indicateur de couleur
            var colorIndicator = new Border
            {
                Background = evt.Color,
                Height = 4,
                CornerRadius = new CornerRadius(2),
                Margin = new Thickness(0, 0, 0, 8),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = double.NaN
            };
            grid.Children.Add(colorIndicator);

            // Titre de l'événement
            var title = new TextBlock
            {
                Text = evt.Title,
                FontWeight = FontWeights.SemiBold,
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(title, 0);
            grid.Children.Add(title);

            // Date et heure
            var dateTime = new TextBlock
            {
                Text = evt.FormattedTime,
                Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(dateTime, 1);
            grid.Children.Add(dateTime);

            // Lieu (si disponible)
            if (!string.IsNullOrEmpty(evt.Location))
            {
                var location = new TextBlock
                {
                    Text = evt.Location,
                    Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153)),
                    FontSize = 12
                };
                Grid.SetRow(location, 2);
                grid.Children.Add(location);
            }

            card.Child = grid;
            return card;
        }

        private void EventBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is CalendarEvent calendarEvent)
            {
                ShowEventDetails(calendarEvent);
            }
        }

        private void ShowEventDetails(CalendarEvent calendarEvent)
        {
            // Ici, vous pourriez afficher une fenêtre de dialogue avec les détails de l'événement
            // Pour l'instant, nous allons simplement afficher un message
            MessageBox.Show(
                $"Titre: {calendarEvent.Title}\n" +
                $"Date: {calendarEvent.FormattedTime}\n" +
                $"Durée: {calendarEvent.Duration}\n" +
                $"Lieu: {(string.IsNullOrEmpty(calendarEvent.Location) ? "Non spécifié" : calendarEvent.Location)}\n" +
                $"Description: {(string.IsNullOrEmpty(calendarEvent.Description) ? "Aucune description" : calendarEvent.Description)}",
                "Détails de l'événement",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshCalendar();
        }

        private async void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            UpdateCalendarDisplay();
        }

        private async void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            UpdateCalendarDisplay();
        }

        private async void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = DateTime.Today;
            UpdateCalendarDisplay();
        }
    }
}
