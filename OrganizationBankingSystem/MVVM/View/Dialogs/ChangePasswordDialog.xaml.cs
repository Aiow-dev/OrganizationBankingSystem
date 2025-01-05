using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OrganizationBankingSystem.Core.Helpers;
using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.Services.EntityServices;
using ToastNotifications;

namespace OrganizationBankingSystem.MVVM.View.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ChangePasswordDialog.xaml
    /// </summary>
    public partial class ChangePasswordDialog : Window
    {
        private DbResult _result;
        private readonly BankUserDataService _bankUserDataService;
        private readonly int _bankUserId;
        private readonly Notifier _notifier;
        public ChangePasswordDialog(int userId)
        {
            InitializeComponent();
            _bankUserId = userId;
            _bankUserDataService = new(new BankSystemContextFactory());
            _notifier = NotificationManager.CreateDialogNotifier(this);
        }
        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CurrentPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentPassword.Password.Length != 0)
            {
                CurrentPasswordPlaceholder.Visibility = Visibility.Hidden;
            }
            else
            {
                CurrentPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (NewPassword.Password.Length != 0)
            {
                NewPasswordPlaceholder.Visibility = Visibility.Hidden;
            }
            else
            {
                NewPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private async Task ChangePasswordResult(string currentPassword, string newPassword)
        {
            _result = await Task.Run(() => _bankUserDataService.ChangePassword(_bankUserId, currentPassword, newPassword));
        }

        private async void ChangePassword(object sender, System.Windows.RoutedEventArgs e)
        {
            string currentPasswordText = CurrentPassword.Password;
            string newPasswordText = NewPassword.Password;

            if (!ValidatorText.NotEmpty(currentPasswordText))
            {
                _notifier.ShowErrorPropertyMessage("Ошибка. Поле текущего пароля не заполнено");
                return;
            }
            if (!ValidatorText.NotEmpty(newPasswordText))
            {
                _notifier.ShowErrorPropertyMessage("Ошибка. Поле нового пароля не заполнено");
                return;
            }
            if (currentPasswordText == newPasswordText)
            {
                _notifier.ShowErrorPropertyMessage("Ошибка. Новый пароль совпадает с текущим");
                return;
            }
            await ChangePasswordResult(currentPasswordText, newPasswordText);

            if (_result == DbResult.NotFound)
            {
                _notifier.ShowErrorPropertyMessage("Ошибка. Пользователь не найден");
                return;
            }
            if (_result == DbResult.PasswordDoNotMatch)
            {
                _notifier.ShowErrorPropertyMessage("Ошибка. Текущий пароль не совпадает с паролем учетной записи");
                return;
            }

            NotificationManager.mainNotifier.ShowCompletedPropertyMessage("Пароль учетной записи изменен");
            this.Close();
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
