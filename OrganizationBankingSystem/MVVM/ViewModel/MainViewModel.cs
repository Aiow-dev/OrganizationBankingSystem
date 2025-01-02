using OrganizationBankingSystem.Core;
using OrganizationBankingSystem.Core.Notifications;

namespace OrganizationBankingSystem.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand DashboardViewCommand { get; set; }

        public RelayCommand BankAccountViewCommand { get; set; }

        public RelayCommand CurrencyViewCommand { get; set; }

        public RelayCommand ProfileViewCommand { get; set; }

        public static bool IsFileListCurrencyDamaged { get; set; }

        public DashboardViewModel DashboardVM;
        public BankAccountViewModel BankAccountVM;
        public CurrencyViewModel CurrencyVM;
        public ProfileViewModel ProfileVM;

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            DashboardVM = new DashboardViewModel();
            BankAccountVM = new BankAccountViewModel();
            CurrencyVM = new CurrencyViewModel();
            ProfileVM = new ProfileViewModel();

            CurrentView = DashboardVM;

            DashboardViewCommand = new RelayCommand(o =>
            {
                CurrentView = DashboardVM;
            });

            BankAccountViewCommand = new RelayCommand(o =>
            {
                CurrentView = BankAccountVM;
            });

            ProfileViewCommand = new RelayCommand(o =>
            {
                CurrentView = ProfileVM;
            });

            CurrencyViewCommand = new RelayCommand(o =>
            {
                CurrentView = CurrencyVM;

                if (IsFileListCurrencyDamaged)
                {
                    NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Возможно, отсутствует или поврежден файл списков валют (файл был удален или неккоректно изменен)");
                }
            }
            );
        }
    }
}