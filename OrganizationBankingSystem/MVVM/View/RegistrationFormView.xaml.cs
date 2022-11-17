using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.Services.AuthenticationServices;
using OrganizationBankingSystem.Services.EntityServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private async Task Register()
        {

        }

        private async Task RegisterBankUser()
        {
            IAuthenticator authenticator = new Authenticator(new AuthenticationService(new BankUserDataService(new Model.BankSystemContextFactory())));
            _registrationResult = await Task.Run(() => authenticator.Register(_lastName, _firstName, _patronymic, _phone, _login, _password, _passwordConfirm));
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

                //if (AutoLogin.IsChecked == true)
                //{
                //    NotificationManager.notifier.ShowInformationPropertyMessage("Выполнение входа!");

                //    bool success = await authenticator.Login(login, password);

                //    if (success)
                //    {
                //        MainWindow mainWindow = new();
                //        mainWindow.Show();

                //        Window window = Window.GetWindow(this);
                //        window.Close();
                //    }
                //}
            }
        }
    }
}
