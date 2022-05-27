using OrganizationBankingSystem.Core;
using OrganizationBankingSystem.Core.Helpers;
using System.Windows;
using System.Windows.Input;

namespace OrganizationBankingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!NetworkHelpers.CheckInternetConnection())
            {
                NotifierHelper.notifier.ShowWarningPropertyMessage("Отсутствует или является нестабильным подключение к сети Интернет. Это может повлиять на работу некоторых функций приложения");
            }
        }
    }
}