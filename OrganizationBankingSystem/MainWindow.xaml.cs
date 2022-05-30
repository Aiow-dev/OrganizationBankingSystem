using OrganizationBankingSystem.Core;
using OrganizationBankingSystem.Core.Helpers;
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
                NotifierHelper.notifier.ShowWarningPropertyMessage("Отсутствует или является нестабильным подключение к сети Интернет. Это может повлиять на работу некоторых функций приложения");
            }
        }

        private void MaximizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            WindowBorder.Width = SystemParameters.PrimaryScreenWidth;
            WindowBorder.Height = SystemParameters.PrimaryScreenHeight;
            WindowBorder.CornerRadius = new CornerRadius(0);

            borderContent.Width = SystemParameters.PrimaryScreenWidth - 160;
            borderContent.CornerRadius = new CornerRadius(0);

            int decreaseWidthValue = 44;

            if (ToggleButtonHidePanel.IsChecked == false)
            {
                decreaseWidthValue = 160;
            }

            DoubleAnimation borderContentAnimation = new(borderContent.Width,
                SystemParameters.PrimaryScreenWidth - decreaseWidthValue,
                new Duration(TimeSpan.FromSeconds(1)));

            Storyboard.SetTargetName(borderContentAnimation, borderContent.Name);
            Storyboard.SetTargetProperty(borderContentAnimation, new PropertyPath(WidthProperty));

            Storyboard myStoryboard = new();
            myStoryboard.Children.Add(borderContentAnimation);

            myStoryboard.Begin(ToggleButtonHidePanel);
        }

        private void NormalizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;

            WindowBorder.Width = 1000;
            WindowBorder.Height = 600;
            WindowBorder.CornerRadius = new CornerRadius(10);

            borderContent.Width = 840;
            borderContent.CornerRadius = new CornerRadius(10);

            int borderContentWidth = 956;

            if (ToggleButtonHidePanel.IsChecked == false)
            {
                borderContentWidth = 840;
            }

            DoubleAnimation borderContentAnimation = new(borderContent.Width,
                borderContentWidth,
                new Duration(TimeSpan.FromSeconds(1)));

            Storyboard.SetTargetName(borderContentAnimation, borderContent.Name);
            Storyboard.SetTargetProperty(borderContentAnimation, new PropertyPath(WidthProperty));

            Storyboard myStoryboard = new();
            myStoryboard.Children.Add(borderContentAnimation);

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
        }

        private void MenuNormalizeWindow(object sender, RoutedEventArgs e)
        {
            NormalizeWindow();
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