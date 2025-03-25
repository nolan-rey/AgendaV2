using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Agenda.Views;

namespace Agenda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Page par défaut
            MainContent.Content = new DashboardView();
        }
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DashboardView();
        }

        private void Events_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EventsView();
        }

        private void Contacts_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ContactsView();
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TasksView();
        }

        private void Messages_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MessagesView();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Pour fermer l'app
        }
    }
}