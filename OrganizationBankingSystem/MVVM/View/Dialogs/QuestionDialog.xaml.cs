using System;
using System.Windows;
using System.Windows.Input;

namespace OrganizationBankingSystem.MVVM.View.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для QuestionDialog.xaml
    /// </summary>
    public partial class QuestionDialog : Window
    {
        public event Action<bool> OnClickButton;
        public QuestionDialog(string title, string questionText)
        {
            InitializeComponent();
            this.Title = title;
            QuestionText.Text = questionText;
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ClickConfirm(object sender, System.Windows.RoutedEventArgs e)
        {
            OnClickButton?.Invoke(true);
            this.Close();
        }

        private void ClickCancel(object sender, System.Windows.RoutedEventArgs e)
        {
            OnClickButton?.Invoke(false);
            this.Close();
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            OnClickButton?.Invoke(false);
            this.Close();
        }
    }
}
