using System.Windows;

namespace OrganizationBankingSystem.Core.Helpers
{
    public class ApplicationHelper
    {
        public static void SetShutdownApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
