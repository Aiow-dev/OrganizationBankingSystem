using OrganizationBankingSystem.Core.Helpers;
using System.Windows;
using System.Windows.Input;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginWindowMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ShutdownApplication(object sender, MouseButtonEventArgs e)
        {
            ApplicationHelper.SetShutdownApplication();
        }
    }
}
