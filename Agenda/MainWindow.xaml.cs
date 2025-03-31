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
        private Button _activeNavButton = null;


        private void SetActiveNavButton(Button clickedButton)
        {
            // Si on clique sur autre chose que le Button directement (ex: TextBlock), on remonte au parent Button
            if (clickedButton == null)
                return;

            if (_activeNavButton != null)
            {
                _activeNavButton.ClearValue(Button.BackgroundProperty);
            }

            clickedButton.Background = (Brush)FindResource("ActiveNavBrush");
            _activeNavButton = clickedButton;
        }



        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DashboardView();
            SetActiveNavButton(BtnDashboard);
        }

        private void Events_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EventsView();
            SetActiveNavButton(BtnEvents);

        }

        private void Contacts_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ContactsView();
            SetActiveNavButton(BtnContacts);

        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TasksView();
            SetActiveNavButton(BtnTasks);

        }

        private void Messages_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MessagesView();
            SetActiveNavButton(BtnMessages);

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Pour fermer l'app
        }

    }
}