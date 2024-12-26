using OrganizationBankingSystem.Core.Helpers;
using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
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
            _isAuthenticated = await Task.Run(() => AuthenticatorState.authenticator.Login(_login, _password));
        }

        private async void LoginWindow(object sender, RoutedEventArgs e)
        {
            _login = Login.Text;
            _password = Password.Password;

            if (!ValidatorText.NotEmpty(_login))
            {
                NotificationManager.signNotifier.ShowErrorPropertyMessage("Ошибка. Поле логина не заполнено");
                return;
            }
            if (!ValidatorText.NotEmpty(_password))
            {
                NotificationManager.signNotifier.ShowErrorPropertyMessage("Ошибка. Поле пароля не заполнено");
                return;
            }

            await LoginBankUser();

            if (_isAuthenticated)
            {
                Window loginWindow = Window.GetWindow(this);
                loginWindow.Close();

                Application.Current.MainWindow.Visibility = Visibility.Visible;
            } else
            {
                NotificationManager.signNotifier.ShowErrorPropertyMessage("Не удалось выполнить вход. Возможно, введен неверный логин или пароль");
            }
        }
    }
}
