using System;
using System.Windows;

namespace OrganizationBankingSystem.Core.Helpers
{
    public class ElementHelper
    {
        private static readonly System.Windows.Forms.Timer timer = new();

        public static void DisableElement(FrameworkElement frameworkElement, int interval)
        {
            timer.Interval = interval;
            timer.Tick += (sender, e) => { TimerTick(sender, e, frameworkElement); };
            timer.Start();

            frameworkElement.IsEnabled = false;
        }

        private static void TimerTick(object sender, EventArgs e, FrameworkElement frameworkElement)
        {
            frameworkElement.IsEnabled = true;
            timer.Stop();
        }
    }
}
