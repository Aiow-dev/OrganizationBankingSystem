using OrganizationBankingSystem.Core;
using System.Windows.Forms;

namespace OrganizationBankingSystem.MVVM.ViewModel
{
    internal class LoginViewModel : ObservableObject
    {
        public RelayCommand LoginFormViewCommand { get; set; }

        public RelayCommand RegistrationFormViewCommand { get; set; }

        public LoginFormViewModel LoginFormVM { get; set; }
        public RegistrationFormViewModel RegistrationFormVM { get; set; }

        private object _currentView;

        public object CurrentFormView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public LoginViewModel()
        {
            LoginFormVM = new LoginFormViewModel();
            RegistrationFormVM = new RegistrationFormViewModel();

            CurrentFormView = LoginFormVM;

            LoginFormViewCommand = new RelayCommand(o =>
            {
                CurrentFormView = LoginFormVM;
            });

            RegistrationFormViewCommand = new RelayCommand(o =>
            {
                CurrentFormView = RegistrationFormVM;
            });
        }
    }
}
