using System.Windows.Media;

namespace OrganizationBankingSystem.Assets
{
    public static class ColorBrush
    {
        public static readonly SolidColorBrush Green = new(Color.FromRgb(161, 204, 165));
        public static readonly SolidColorBrush GreenLight = new(Color.FromRgb(191, 222, 193));

        public static readonly SolidColorBrush Red = new(Color.FromRgb(210, 31, 60));

        public static readonly SolidColorBrush Grey = new(Color.FromRgb(125, 132, 145));

        public static readonly SolidColorBrush Transparent = new(Colors.Transparent);

        public static readonly SolidColorBrush Info = new(Color.FromRgb(188, 219, 255));

        public static readonly SolidColorBrush Warning = new(Color.FromRgb(255, 241, 208));
    }
}
