using System.Windows;

namespace OrganizationBankingSystem.Core.Helpers
{
    public static class ElementHelper
    {
        private static readonly System.Windows.Forms.Timer _timer = new();

        public static void DisableElement(FrameworkElement frameworkElement, int interval)
        {
            _timer.Interval = interval;
            _timer.Tick += (sender, e) => { TimerTick(frameworkElement); };
            _timer.Start();

            frameworkElement.IsEnabled = false;
        }

        private static void TimerTick(FrameworkElement frameworkElement)
        {
            frameworkElement.IsEnabled = true;
            _timer.Stop();
        }
    }
}
