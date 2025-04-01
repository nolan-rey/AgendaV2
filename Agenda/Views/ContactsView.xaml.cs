using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Agenda.DAO;
using Agenda.Models;

namespace Agenda.Views
{
    public partial class ContactsView : UserControl
    {
        private List<Contact> allContacts;

        public ContactsView()
        {
            InitializeComponent();
            Loaded += ContactsView_Loaded;
        }

        private void ContactsView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadContacts();
        }

        private void LoadContacts()
        {
            allContacts = ContactDAO.GetAll();
            ContactsListView.ItemsSource = allContacts;
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var form = new ContactForm();
            if (form.ShowDialog() == true)
            {
                ContactDAO.Add(form.Contact);
                LoadContacts();
            }
        }

        private void EditContact_Click(object sender, RoutedEventArgs e)
        {
            var contact = (sender as FrameworkElement)?.DataContext as Contact;
            var form = new ContactForm(contact);
            if (form.ShowDialog() == true)
            {
                ContactDAO.Update(form.Contact);
                LoadContacts();
            }
        }

        private void DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            var contact = (sender as FrameworkElement)?.DataContext as Contact;
            if (MessageBox.Show($"Supprimer {contact.FirstName} {contact.LastName} ?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ContactDAO.Delete(contact.Id);
                LoadContacts();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (allContacts == null)
                return;

            string keyword = SearchBox.Text.Trim().ToLower();

            SearchPlaceholder.Visibility = string.IsNullOrWhiteSpace(keyword) ? Visibility.Visible : Visibility.Collapsed;
            ClearSearchButton.Visibility = string.IsNullOrWhiteSpace(keyword) ? Visibility.Collapsed : Visibility.Visible;

            var filtered = allContacts.Where(c =>
                (!string.IsNullOrEmpty(c.FirstName) && c.FirstName.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(c.LastName) && c.LastName.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(keyword))
            ).ToList();

            ContactsListView.ItemsSource = filtered;
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            foreach (var item in ContactsListView.Items)
            {
                var container = ContactsListView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                if (container != null)
                {
                    var expander = FindVisualChild<Expander>(container);
                    if (expander != null && expander != sender)
                    {
                        expander.IsExpanded = false;
                    }
                }
            }
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T correctlyTyped)
                    return correctlyTyped;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
