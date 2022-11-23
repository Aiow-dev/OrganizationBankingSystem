using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.Services.AuthenticationServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для RegistrationFormView.xaml
    /// </summary>
    public partial class RegistrationFormView : UserControl
    {
        private string _lastName;
        private string _firstName;
        private string _patronymic;
        private string _phone;
        private string _login;
        private string _password;
        private string _passwordConfirm;

        private RegistrationResult _registrationResult;

        public RegistrationFormView()
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

        private void PasswordConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordConfirm.Password.Length != 0)
            {
                PasswordConfirmPlaceholder.Visibility = Visibility.Hidden;
            }
            else
            {
                PasswordConfirmPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private async Task RegisterBankUser()
        {
            _registrationResult = await Task.Run(() => AuthenticatorState.authenticator.Register(_lastName, _firstName, _patronymic, _phone, _login, _password, _passwordConfirm));
        }

        private async void RegisterWindow(object sender, RoutedEventArgs e)
        {
            NotificationManager.notifier.ShowInformationPropertyMessage("Выполнение создания учетной записи...");

            _lastName = LastName.Text;
            _firstName = FirstName.Text;
            _patronymic = Patronymic.Text;
            _phone = Phone.Text;
            _login = Login.Text;
            _password = Password.Password;
            _passwordConfirm = PasswordConfirm.Password;

            await RegisterBankUser();

            if (_registrationResult == RegistrationResult.Success)
            {
                NotificationManager.notifier.ShowCompletedPropertyMessage("Учетная запись создана успешно!");

                if (AutoLogin.IsChecked == true)
                {
                    NotificationManager.notifier.ShowInformationPropertyMessage("Выполнение входа!");

                    bool success = await AuthenticatorState.authenticator.Login(_login, _password);

                    if (success)
                    {
                        Window registrationWindow = Window.GetWindow(this);
                        registrationWindow.Close();

                        Application.Current.MainWindow.Visibility = Visibility.Visible;
                    }
                }
            }
            else if (_registrationResult == RegistrationResult.PasswordDoNotMatch)
            {
                NotificationManager.notifier.ShowErrorPropertyMessage("Ошибка. Пароли не совпадают!");
            }
        }
    }
}
