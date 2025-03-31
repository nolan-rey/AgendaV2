using Agenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Agenda.Views
{
    /// <summary>
    /// Logique d'interaction pour ContactForm.xaml
    /// </summary>
    public partial class ContactForm : Window
    {
        public Contact Contact { get; private set; }
        public ContactForm(Contact contact = null)
        {
            InitializeComponent();

            if (contact != null)
            {
                Title = "Modifier Contact";
                Contact = contact;
                FirstNameBox.Text = contact.FirstName;
                LastNameBox.Text = contact.LastName;
                EmailBox.Text = contact.Email;
                PhoneBox.Text = contact.PhoneNumber;
                AddressBox.Text = contact.Address;

                // Conversion DateOnly? vers DateTime? pour le DatePicker
                if (contact.BirthDate.HasValue)
                    BirthDatePicker.SelectedDate = contact.BirthDate.Value.ToDateTime(TimeOnly.MinValue);
                else
                    BirthDatePicker.SelectedDate = null;
            }
            else
            {
                Title = "Nouveau Contact";
                Contact = new Contact();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Contact.FirstName = FirstNameBox.Text;
            Contact.LastName = LastNameBox.Text;
            Contact.Email = EmailBox.Text;
            Contact.PhoneNumber = PhoneBox.Text;
            Contact.Address = AddressBox.Text;

            // Conversion DateTime? vers DateOnly?
            if (BirthDatePicker.SelectedDate.HasValue)
                Contact.BirthDate = DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value);
            else
                Contact.BirthDate = null;

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
