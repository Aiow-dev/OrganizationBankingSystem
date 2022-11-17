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

        private async void RegisterBankUser(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                NotificationManager.notifier.ShowInformationPropertyMessage("Выполнение создания учетной записи...");
            });

            string lastName = LastName.Text;
            string firstName = FirstName.Text;
            string patronymic = Patronymic.Text;
            string phone = Phone.Text;
            string login = Login.Text;
            string password = Password.Password;
            string confirmPassword = PasswordConfirm.Password;

            IAuthenticator authenticator = new Authenticator(new AuthenticationService(new BankUserDataService(new Model.BankSystemContextFactory())));
            RegistrationResult registrationResult = await authenticator.Register(lastName, firstName, patronymic, phone, login, password, confirmPassword);

            if (registrationResult == RegistrationResult.Success)
            {
                Dispatcher.Invoke(() =>
                {
                    NotificationManager.notifier.ShowCompletedPropertyMessage("Учетная запись создана успешно!");
                });

                if (AutoLogin.IsChecked == true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        NotificationManager.notifier.ShowInformationPropertyMessage("Выполнение входа!");
                    });

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
    }
}
