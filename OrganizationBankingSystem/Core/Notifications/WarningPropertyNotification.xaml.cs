using OrganizationBankingSystem.Core.Notifications;
using System;
using System.Runtime.Versioning;
using ToastNotifications.Core;

namespace OrganizationBankingSystem.Core
{
    /// <summary>
    /// Логика взаимодействия для DisplayPartUI.xaml
    /// </summary>

    public partial class DisplayPartUI : NotificationDisplayPart
    {
        [SupportedOSPlatform("windows")]
        public DisplayPartUI(NotificationWarn notificationWarn)
        {
            InitializeComponent();

            if (OperatingSystem.IsWindows())
            {
                Bind(notificationWarn);
            }
        }

        private void CloseWarningNotification(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Notification.Close();
        }
    }
}