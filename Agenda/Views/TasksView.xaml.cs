using Agenda.DAO;
using Agenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Agenda.Views
{
    public partial class TasksView : UserControl
    {
        private List<Todo> allTasks = new(); // Initialisé pour éviter les null
        private string currentSort = "Date d'échéance"; // Tri par défaut
        private bool isUiReady = false; // Protection au chargement

        public TasksView()
        {
            InitializeComponent();
            Loaded += TasksView_Loaded;
        }

        private void TasksView_Loaded(object sender, RoutedEventArgs e)
        {
            isUiReady = true;
            LoadTasks();
        }

        private void LoadTasks()
        {
            allTasks = TodoDAO.GetAll()?.Where(t => t != null).ToList() ?? new List<Todo>();
            if (isUiReady)
                RefreshLists();
        }

        private void RefreshLists()
        {
            if (!isUiReady || TasksTodoListView == null || TasksDoneListView == null)
                return;

            string keyword = SearchBox?.Text?.Trim().ToLower() ?? "";

            var filtered = allTasks
                .Where(t => string.IsNullOrEmpty(keyword) || (t.Title?.ToLower().Contains(keyword) ?? false))
                .ToList();

            // 🎯 Appliquer le tri choisi
            switch (currentSort)
            {
                case "Date d'ajout":
                    filtered = filtered.OrderBy(t => t.Id).ToList();
                    break;
                case "Nom (A-Z)":
                    filtered = filtered.OrderBy(t => t.Title).ToList();
                    break;
                case "Nom (Z-A)":
                    filtered = filtered.OrderByDescending(t => t.Title).ToList();
                    break;
                default: // "Date d'échéance"
                    filtered = filtered.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList();
                    break;
            }

            TasksTodoListView.ItemsSource = filtered
                .Where(t => t.IsCompleted == false || t.IsCompleted == null)
                .ToList();

            TasksDoneListView.ItemsSource = filtered
                .Where(t => t.IsCompleted == true)
                .ToList();
        }

        private void SortTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isUiReady || SortTabControl.SelectedItem is not TabItem selectedItem)
                return;

            currentSort = selectedItem.Header.ToString();
            RefreshLists();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var form = new TaskForm();
            if (form.ShowDialog() == true)
            {
                TodoDAO.Add(form.Task);
                LoadTasks();
            }
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Todo task)
            {
                var form = new TaskForm(task);
                if (form.ShowDialog() == true)
                {
                    TodoDAO.Update(form.Task);
                    LoadTasks();
                }
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Todo task)
            {
                if (MessageBox.Show($"Supprimer la tâche \"{task.Title}\" ?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    TodoDAO.Delete(task.Id);
                    LoadTasks();
                }
            }
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox)?.DataContext is Todo task)
            {
                TodoDAO.Update(task);
                RefreshLists();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrWhiteSpace(SearchBox.Text)
                ? Visibility.Visible : Visibility.Collapsed;

            ClearSearchButton.Visibility = string.IsNullOrWhiteSpace(SearchBox.Text)
                ? Visibility.Collapsed : Visibility.Visible;

            RefreshLists();
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
        }
    }
}
