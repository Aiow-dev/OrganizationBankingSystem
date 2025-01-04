using System;
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
    /// Логика взаимодействия для ChangePhoneNumberWindow.xaml
    /// </summary>
    public partial class ChangePhoneNumberDialog : Window
    {
        private DbResult _result;
        private BankUserDataService _bankUserDataService;
        private int _userId;
        private Notifier _notifier;
        public event Action<string> OnChangePhoneNumber;

        public ChangePhoneNumberDialog(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _bankUserDataService = new(new BankSystemContextFactory());
            _notifier = NotificationManager.CreateDialogNotifier(this);
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async Task ChangePhoneResult(string phone)
        {
            _result = await Task.Run(() => _bankUserDataService.ChangeUserPhone(_userId, phone));
        }

        private async void ChangePhone(object sender, System.Windows.RoutedEventArgs e)
        {
            string phoneText = NewPhoneNumber.Text;
            if (!ValidatorText.NotEmpty(phoneText))
            {
                _notifier.ShowErrorPropertyMessage("Ошибка. Поле номера телефона не заполнено");
                return;
            }
            await ChangePhoneResult(phoneText);

            if (_result == DbResult.NotFound)
            {
                _notifier.ShowErrorPropertyMessage("Ошибка. Пользователь не найден");
                return;
            }
            if (_result == DbResult.MatchingValue)
            {
                _notifier.ShowErrorPropertyMessage("Ошибка. Новый номер телефона совпадает с текущим");
                return;
            }

            AuthenticatorState.authenticator.UpdateUserPhone(phoneText);
            OnChangePhoneNumber?.Invoke(phoneText);
            this.Close();
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
