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

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            WindowBorder.Width = SystemParameters.PrimaryScreenWidth;
            WindowBorder.Height = SystemParameters.PrimaryScreenHeight;
            WindowBorder.CornerRadius = new CornerRadius(0);

            borderContent.Width = SystemParameters.PrimaryScreenWidth - 160;
            borderContent.CornerRadius = new CornerRadius(0);

            int increaseWidthValue = 44;

            if (ToggleButtonHidePanel.IsChecked == false)
            {
                increaseWidthValue = 160;
            }

            DoubleAnimation borderContentAnimation = new(borderContent.Width,
                SystemParameters.PrimaryScreenWidth - increaseWidthValue,
                new Duration(TimeSpan.FromSeconds(1)));

            Storyboard.SetTargetName(borderContentAnimation, borderContent.Name);
            Storyboard.SetTargetProperty(borderContentAnimation, new PropertyPath(WidthProperty));

            Storyboard myStoryboard = new();
            myStoryboard.Children.Add(borderContentAnimation);

            myStoryboard.Begin(ToggleButtonHidePanel);
        }
    }
}