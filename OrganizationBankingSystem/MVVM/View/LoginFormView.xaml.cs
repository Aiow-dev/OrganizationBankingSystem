using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.Services.AuthenticationServices;
using OrganizationBankingSystem.Services.EntityServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для LoginFormView.xaml
    /// </summary>
    public partial class LoginFormView : UserControl
    {
        private string _login;
        private string _password;
        private bool _isAuthenticated;

        public LoginFormView()
        {
            InitializeComponent();
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password.Password.Length != 0)
            {
                PasswordPlaceholder.Visibility = Visibility.Hidden;
            }
            else
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private async Task LoginBankUser()
        {
            IAuthenticator authenticator = new Authenticator(new AuthenticationService(new BankUserDataService(new Model.BankSystemContextFactory())));
            _isAuthenticated = await Task.Run(() => authenticator.Login(_login, _password));
        }

        private async void LoginWindow(object sender, RoutedEventArgs e)
        {
            //Dispatcher.Invoke(() => {
            NotificationManager.notifier.ShowInformationPropertyMessage("Выполнение входа...");
            //});

            _login = Login.Text;
            _password = Password.Password;

            await LoginBankUser();

            if (_isAuthenticated)
            {
                //MainWindow mainWindow = new();
                //Application.Current.MainWindow = mainWindow;
                //mainWindow.Show();

                //Window window = Window.GetWindow(this);
                //window.Close();

                Window loginWindow = Window.GetWindow(this);
                loginWindow.Close();

                Application.Current.MainWindow.Visibility = Visibility.Visible;
            }
        }
    }
}
