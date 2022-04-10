using ToastNotifications;

namespace OrganizationBankingSystem.Core
{
    internal static class MessageExtension
    {
        public static void ShowMessage(this Notifier notifier, string message)
        {
            notifier.Notify<NotificationWarn>(() => new NotificationWarn(message));
        }
    }
}
