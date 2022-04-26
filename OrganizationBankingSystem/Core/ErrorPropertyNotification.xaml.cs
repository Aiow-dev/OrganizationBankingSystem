using ToastNotifications.Core;

namespace OrganizationBankingSystem.Core
{
    /// <summary>
    /// Логика взаимодействия для ErrorPropertyNotification.xaml
    /// </summary>
    public partial class ErrorPropertyNotification : NotificationDisplayPart
    {
        public ErrorPropertyNotification(NotificationErr notificationErr)
        {
            InitializeComponent();
            Bind(notificationErr);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Notification.Close();
        }
    }
}