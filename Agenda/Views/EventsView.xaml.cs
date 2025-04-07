using Agenda.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

        private GoogleCalendarService _calendarService;
        private DateTime _currentMonth;
        private CalendarEvent _selectedEvent;
        private bool _isEditMode = false;

        public EventsView()
        {
            InitializeComponent();
            DataContext = this;

            _currentMonth = DateTime.Now;

            // Initialiser les ComboBox d'heure
            InitializeTimeComboBoxes();

            Loaded += EventsView_Loaded;
        }

        private void InitializeTimeComboBoxes()
        {
            // Remplir les ComboBox avec des heures par intervalles de 30 minutes
            for (int hour = 0; hour < 24; hour++)
            {
                StartTimePicker.Items.Add($"{hour:D2}:00");
                StartTimePicker.Items.Add($"{hour:D2}:30");
                EndTimePicker.Items.Add($"{hour:D2}:00");
                EndTimePicker.Items.Add($"{hour:D2}:30");
            }

            // Sélectionner une heure par défaut
            StartTimePicker.SelectedIndex = 18; // 09:00
            EndTimePicker.SelectedIndex = 20;   // 10:00
        }

        private async void EventsView_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialiser le service Google Calendar
            _calendarService = new GoogleCalendarService();

            var authSuccess = await _calendarService.AuthenticateAsync();
            if (!authSuccess)
            {
                MessageBox.Show("Échec de l'authentification avec Google Calendar.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Charger le calendrier pour le mois en cours
            await LoadCalendarMonth();
        }

        private async System.Threading.Tasks.Task LoadCalendarMonth()
        {
            // Mettre à jour le texte du mois
            CurrentMonthText.Text = $" - {_currentMonth:MMMM yyyy}";

            // Calculer les dates de début et de fin pour récupérer les événements
            var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Ajuster pour inclure la semaine complète
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
            if (firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday) firstDayOfCalendar = firstDayOfCalendar.AddDays(-7);

            var lastDayOfCalendar = lastDayOfMonth;
            int daysToAdd = 7 - (int)lastDayOfCalendar.DayOfWeek;
            if (daysToAdd < 7) lastDayOfCalendar = lastDayOfCalendar.AddDays(daysToAdd);

            // Récupérer les événements
            var calendarEvents = await _calendarService.GetEventsAsync(firstDayOfCalendar, lastDayOfCalendar);
            Events.Clear();
            foreach (var ev in calendarEvents)
            {
                Events.Add(ev);
            }

            // Générer la grille du calendrier
            GenerateCalendarGrid(firstDayOfCalendar, lastDayOfCalendar);
        }

        private void GenerateCalendarGrid(DateTime startDate, DateTime endDate)
        {
            // Réinitialiser la grille
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            // Calculer le nombre de semaines à afficher
            int numberOfWeeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7.0);

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
            DateTime currentDate = startDate;
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

                    // Ajouter un gestionnaire d'événements pour créer un nouvel événement
                    dayContainer.Tag = currentDate;
                    dayContainer.MouseLeftButtonUp += DayContainer_MouseLeftButtonUp;

                    // Passer au jour suivant
                    currentDate = currentDate.AddDays(1);
                }
            }
        }

        private void EventBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is CalendarEvent calendarEvent)
            {
                ShowEventDetails(calendarEvent);
                e.Handled = true;
            }
        }

        private void DayContainer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Handled) return;

            if (sender is Border border && border.Tag is DateTime selectedDate)
            {
                ShowNewEventForm(selectedDate);
            }
        }

        private void ShowEventDetails(CalendarEvent calendarEvent)
        {
            _selectedEvent = calendarEvent;

            // Remplir les détails de l'événement
            EventTitleText.Text = calendarEvent.Title;
            EventTimeText.Text = calendarEvent.FormattedTime;
            EventDurationText.Text = calendarEvent.Duration;
            EventLocationText.Text = string.IsNullOrEmpty(calendarEvent.Location) ? "Aucun lieu spécifié" : calendarEvent.Location;
            EventDescriptionText.Text = string.IsNullOrEmpty(calendarEvent.Description) ? "Aucune description" : calendarEvent.Description;

            // Afficher le panneau de détails
            SidebarTitle.Text = "Détails de l'événement";
            EventDetailsPanel.Visibility = Visibility.Visible;
            EventFormPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowNewEventForm(DateTime selectedDate)
        {
            _selectedEvent = null;
            _isEditMode = false;

            // Initialiser le formulaire
            EventTitleInput.Text = "";
            StartDatePicker.SelectedDate = selectedDate;
            EndDatePicker.SelectedDate = selectedDate;
            StartTimePicker.SelectedIndex = 18; // 09:00
            EndTimePicker.SelectedIndex = 20;   // 10:00
            AllDayCheckbox.IsChecked = false;
            LocationInput.Text = "";
            DescriptionInput.Text = "";

            // Afficher le formulaire
            SidebarTitle.Text = "Nouvel événement";
            EventDetailsPanel.Visibility = Visibility.Collapsed;
            EventFormPanel.Visibility = Visibility.Visible;
        }

        private void ShowEditEventForm()
        {
            if (_selectedEvent == null) return;

            _isEditMode = true;

            // Remplir le formulaire avec les détails de l'événement
            EventTitleInput.Text = _selectedEvent.Title;
            StartDatePicker.SelectedDate = _selectedEvent.Start.Date;
            EndDatePicker.SelectedDate = _selectedEvent.End.Date;

            if (_selectedEvent.IsAllDay)
            {
                AllDayCheckbox.IsChecked = true;
            }
            else
            {
                AllDayCheckbox.IsChecked = false;

                // Trouver l'index correspondant à l'heure de début
                string startTimeStr = _selectedEvent.Start.ToString("HH:mm");
                for (int i = 0; i < StartTimePicker.Items.Count; i++)
                {
                    if (StartTimePicker.Items[i].ToString() == startTimeStr)
                    {
                        StartTimePicker.SelectedIndex = i;
                        break;
                    }
                }

                // Trouver l'index correspondant à l'heure de fin
                string endTimeStr = _selectedEvent.End.ToString("HH:mm");
                for (int i = 0; i < EndTimePicker.Items.Count; i++)
                {
                    if (EndTimePicker.Items[i].ToString() == endTimeStr)
                    {
                        EndTimePicker.SelectedIndex = i;
                        break;
                    }
                }
            }

            LocationInput.Text = _selectedEvent.Location ?? "";
            DescriptionInput.Text = _selectedEvent.Description ?? "";

            // Afficher le formulaire
            SidebarTitle.Text = "Modifier l'événement";
            EventDetailsPanel.Visibility = Visibility.Collapsed;
            EventFormPanel.Visibility = Visibility.Visible;
        }

        private async void SaveEvent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EventTitleInput.Text))
            {
                MessageBox.Show("Veuillez saisir un titre pour l'événement.", "Champ requis", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Veuillez sélectionner des dates valides.", "Dates invalides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Créer l'objet événement
                var newEvent = new CalendarEvent
                {
                    Title = EventTitleInput.Text,
                    Description = DescriptionInput.Text,
                    Location = LocationInput.Text,
                    IsAllDay = AllDayCheckbox.IsChecked ?? false
                };

                // Définir les dates et heures
                if (newEvent.IsAllDay)
                {
                    newEvent.Start = StartDatePicker.SelectedDate.Value;
                    newEvent.End = EndDatePicker.SelectedDate.Value.AddDays(1); // Pour les événements sur toute la journée, Google utilise une date de fin exclusive
                }
                else
                {
                    // Extraire les heures et minutes
                    string[] startTimeParts = StartTimePicker.SelectedItem.ToString().Split(':');
                    string[] endTimeParts = EndTimePicker.SelectedItem.ToString().Split(':');

                    int startHour = int.Parse(startTimeParts[0]);
                    int startMinute = int.Parse(startTimeParts[1]);
                    int endHour = int.Parse(endTimeParts[0]);
                    int endMinute = int.Parse(endTimeParts[1]);

                    newEvent.Start = StartDatePicker.SelectedDate.Value.AddHours(startHour).AddMinutes(startMinute);
                    newEvent.End = EndDatePicker.SelectedDate.Value.AddHours(endHour).AddMinutes(endMinute);
                }

                // Vérifier que la date de fin est après la date de début
                if (newEvent.End <= newEvent.Start)
                {
                    MessageBox.Show("La date et l'heure de fin doivent être postérieures à la date et l'heure de début.", "Dates invalides", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Créer ou mettre à jour l'événement
                if (_isEditMode && _selectedEvent != null)
                {
                    newEvent.Id = _selectedEvent.Id;
                    newEvent.Color = _selectedEvent.Color;
                    await _calendarService.UpdateEventAsync(newEvent);
                }
                else
                {
                    await _calendarService.CreateEventAsync(newEvent);
                }

                // Recharger le calendrier
                await LoadCalendarMonth();

                // Revenir à l'affichage du calendrier
                EventDetailsPanel.Visibility = Visibility.Collapsed;
                EventFormPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement de l'événement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent != null)
            {
                ShowEventDetails(_selectedEvent);
            }
            else
            {
                EventDetailsPanel.Visibility = Visibility.Collapsed;
                EventFormPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            ShowEditEventForm();
        }

        private async void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null) return;

            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet événement ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _calendarService.DeleteEventAsync(_selectedEvent.Id);

                    // Recharger le calendrier
                    await LoadCalendarMonth();

                    // Revenir à l'affichage du calendrier
                    EventDetailsPanel.Visibility = Visibility.Collapsed;
                    EventFormPanel.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression de l'événement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AllDayCheckbox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool isAllDay = AllDayCheckbox.IsChecked ?? false;

            StartTimePicker.IsEnabled = !isAllDay;
            EndTimePicker.IsEnabled = !isAllDay;
        }

        private void NewEventButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNewEventForm(DateTime.Today);
        }

        private async void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            await LoadCalendarMonth();
        }

        private async void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            await LoadCalendarMonth();
        }

        private async void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = DateTime.Today;
            await LoadCalendarMonth();
        }
    }
}

