using OrganizationBankingSystem.Core.Notifications;
using ToastNotifications.Core;

namespace OrganizationBankingSystem.Core
{
    /// <summary>
    /// Логика взаимодействия для InformationPropertyNotification.xaml
    /// </summary>
    public partial class InformationPropertyNotification : NotificationDisplayPart
    {
        public InformationPropertyNotification(NotificationInfo notificationInfo)
        {
            InitializeComponent();
            Bind(notificationInfo);
        }

        private void CloseInformationNotification(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Notification.Close();
        }
    }
}