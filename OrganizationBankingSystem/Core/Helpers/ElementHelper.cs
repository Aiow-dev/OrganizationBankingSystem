using System;
using System.Windows;

namespace OrganizationBankingSystem.Core.Helpers
{
    public class ElementHelper
    {
        private static readonly System.Windows.Forms.Timer _timer = new();

        public static void DisableElement(FrameworkElement frameworkElement, int interval)
        {
            _timer.Interval = interval;
            _timer.Tick += (sender, e) => { TimerTick(sender, e, frameworkElement); };
            _timer.Start();

            frameworkElement.IsEnabled = false;
        }

        private static void TimerTick(object sender, EventArgs e, FrameworkElement frameworkElement)
        {
            frameworkElement.IsEnabled = true;
            _timer.Stop();
        }
    }
}
