using OrganizationBankingSystem.Core;

namespace OrganizationBankingSystem.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public DashboardViewModel DashboardVM;
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
            CurrentView = DashboardVM;
        }
    }
}
