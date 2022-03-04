using OrganizationBankingSystem.Core;

namespace OrganizationBankingSystem.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand DashboardViewCommand { get; set; }
        public RelayCommand BankAccountViewCommand { get; set; }
        public DashboardViewModel DashboardVM;
        public BankAccountViewModel BankAccountVM;
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
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

            CurrentView = DashboardVM;

            DashboardViewCommand = new RelayCommand(o =>
            {
                CurrentView = DashboardVM;
            });

            BankAccountViewCommand = new RelayCommand(o =>
            {
                CurrentView = BankAccountVM;
            });
        }
    }
}
