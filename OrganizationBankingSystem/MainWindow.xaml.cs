using OrganizationBankingSystem.Core.Helpers;
using OrganizationBankingSystem.Core.Notifications;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace OrganizationBankingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!NetworkHelpers.CheckInternetConnection())
            {
                NotificationManager.notifier.ShowWarningPropertyMessage("Отсутствует или является нестабильным подключение к сети Интернет. Это может повлиять на работу некоторых функций приложения");
            }
        }

        private void MaximizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            WindowBorder.Width = SystemParameters.PrimaryScreenWidth;
            WindowBorder.Height = SystemParameters.PrimaryScreenHeight;
            WindowBorder.CornerRadius = new CornerRadius(0);

            BorderContent.Width = SystemParameters.PrimaryScreenWidth - 160;
            BorderContent.CornerRadius = new CornerRadius(0);

            int decreaseWidthValue = 44;

            if (ToggleButtonHidePanel.IsChecked == false)
            {
                decreaseWidthValue = 160;
            }

            DoubleAnimation BorderContentAnimation = new(BorderContent.Width,
                SystemParameters.PrimaryScreenWidth - decreaseWidthValue,
                new Duration(TimeSpan.FromSeconds(1)));

            Storyboard.SetTargetName(BorderContentAnimation, BorderContent.Name);
            Storyboard.SetTargetProperty(BorderContentAnimation, new PropertyPath(WidthProperty));

            Storyboard myStoryboard = new();
            myStoryboard.Children.Add(BorderContentAnimation);

            myStoryboard.Begin(ToggleButtonHidePanel);
        }

        private void NormalizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;

            WindowBorder.Width = 1000;
            WindowBorder.Height = 600;
            WindowBorder.CornerRadius = new CornerRadius(10);

            BorderContent.Width = 840;
            BorderContent.CornerRadius = new CornerRadius(10);

            int BorderContentWidth = 956;

            if (ToggleButtonHidePanel.IsChecked == false)
            {
                BorderContentWidth = 840;
            }

            DoubleAnimation BorderContentAnimation = new(BorderContent.Width,
                BorderContentWidth,
                new Duration(TimeSpan.FromSeconds(1)));

            Storyboard.SetTargetName(BorderContentAnimation, BorderContent.Name);
            Storyboard.SetTargetProperty(BorderContentAnimation, new PropertyPath(WidthProperty));

            Storyboard myStoryboard = new();
            myStoryboard.Children.Add(BorderContentAnimation);

            myStoryboard.Begin(ToggleButtonHidePanel);
        }

        private static void SetMinimizedWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private static void SetShutdownApplication()
        {
            Application.Current.Shutdown();
        }

        private void ResizeWindow(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                MaximizeWindow();
            }
            else
            {
                NormalizeWindow();
            }
        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            SetMinimizedWindow();
        }

        private void ShutdownApplication(object sender, MouseButtonEventArgs e)
        {
            SetShutdownApplication();
        }

        private void MenuMaximizeWindow(object sender, RoutedEventArgs e)
        {
            MaximizeWindow();

            ButtonResizeWindow.IsChecked = true;
        }

        private void MenuNormalizeWindow(object sender, RoutedEventArgs e)
        {
            NormalizeWindow();

            ButtonResizeWindow.IsChecked = false;
        }

        private void MenuMinimizeWindow(object sender, RoutedEventArgs e)
        {
            SetMinimizedWindow();
        }

        private void MenuRestartApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            System.Windows.Forms.Application.Restart();
        }

        private void MenuShutdownApplication(object sender, RoutedEventArgs e)
        {
            SetShutdownApplication();
        }
    }
}