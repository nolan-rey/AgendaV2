using System;
using System.Windows;
using Agenda.Models;

namespace Agenda.Views
{
    public partial class TaskForm : Window
    {
        public Todo Task { get; private set; }

        public TaskForm()
        {
            InitializeComponent();
            Task = new Todo();
        }

        public TaskForm(Todo existingTask) : this()
        {
            Task = existingTask;
            TitleBox.Text = Task.Title;
            DueDatePicker.SelectedDate = Task.DueDate;
            IsCompletedCheckBox.IsChecked = Task.IsCompleted;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Le titre est requis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Task.Title = TitleBox.Text.Trim();
            Task.DueDate = DueDatePicker.SelectedDate;
            Task.IsCompleted = IsCompletedCheckBox.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
