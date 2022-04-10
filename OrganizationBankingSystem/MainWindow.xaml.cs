using OrganizationBankingSystem.Core;
using System;
using System.Net.NetworkInformation;
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

        private bool CheckInternetConnection()
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    return false;
                }

                Ping ping = new Ping();
                PingOptions pingOptions = new PingOptions();
                PingReply pingReply = ping.Send("google.com", 1000, new byte[32], pingOptions);
                return (pingReply.Status == IPStatus.Success);
            }
            catch (PingException)
            {
                return false;
            }
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CheckInternetConnection())
            {
                Notifier notifier = new Notifier(cfg =>
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

                notifier.ShowMessage("Отсутствует подключение к сети Интернет. Это может повлиять на работу некоторых функций приложения");
            }
        }
    }
}
