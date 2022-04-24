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
    }
}