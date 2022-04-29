using OrganizationBankingSystem.Core;
using OrganizationBankingSystem.Core.Helpers;
using System;
using System.Windows;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

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
                notifier.ShowWarningPropertyMessage("Отсутствует или является нестабильным подключение к сети Интернет. Это может повлиять на работу некоторых функций приложения");
            }
        }

        public static readonly Notifier notifier = new(configureAction: cfg =>
              {
                  cfg.PositionProvider = new WindowPositionProvider(
                      parentWindow: Application.Current.MainWindow,
                      corner: Corner.BottomRight,
                      offsetX: 10,
                      offsetY: 10);

                  cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                      notificationLifetime: TimeSpan.FromSeconds(5),
                      maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                  cfg.Dispatcher = Application.Current.Dispatcher;
              });
    }
}