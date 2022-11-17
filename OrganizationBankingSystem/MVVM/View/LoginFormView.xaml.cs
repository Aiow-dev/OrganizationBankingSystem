using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.Services.AuthenticationServices;
using OrganizationBankingSystem.Services.EntityServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для LoginFormView.xaml
    /// </summary>
    public partial class LoginFormView : UserControl
    {
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

        private async void LoginBankUser(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                NotificationManager.notifier.ShowInformationPropertyMessage("Выполнение входа...");
            });

            string login = Login.Text;
            string password = Password.Password;

            IAuthenticator authenticator = new Authenticator(new AuthenticationService(new BankUserDataService(new Model.BankSystemContextFactory())));
            bool success = await authenticator.Login(login, password);

            if (success)
            {
                MainWindow mainWindow = new();
                mainWindow.Show();

                Window window = Window.GetWindow(this);
                window.Close();
            }
        }
    }
}
