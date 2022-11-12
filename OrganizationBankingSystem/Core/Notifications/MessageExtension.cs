using ToastNotifications;

namespace OrganizationBankingSystem.Core.Notifications
{
    internal static class MessageExtension
    {
        public static void ShowWarningPropertyMessage(this Notifier notifier, string message)
        {
            notifier.Notify(() => new NotificationWarn(message));
        }

        public static void ShowInformationPropertyMessage(this Notifier notifier, string message)
        {
            notifier.Notify(() => new NotificationInfo(message));
        }

        public static void ShowErrorPropertyMessage(this Notifier notifier, string message)
        {
            notifier.Notify(() => new NotificationErr(message));
        }

        public static void ShowCompletedPropertyMessage(this Notifier notifier, string message)
        {
            notifier.Notify(() => new NotificationComp(message));
        }
    }
}