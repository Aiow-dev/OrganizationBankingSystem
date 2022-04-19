using ToastNotifications;

namespace OrganizationBankingSystem.Core
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
    }
}
