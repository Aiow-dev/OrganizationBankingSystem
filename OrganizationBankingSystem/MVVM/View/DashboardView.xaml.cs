using OrganizationBankingSystem.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //DashboardViewModel dashboardViewModel = new DashboardViewModel();
            //Console.WriteLine(dashboardViewModel.exchangeRates);
            //dynamic dynamic = dashboardViewModel.exchangeRates["Realtime Currency Exchange Rate"];
            //currencyExchangeRatesText.Text = dynamic.GetProperty("5. Exchange Rate").ToString();
            //JsonSerializer.Deserialize<Dictionary<dynamic, dynamic>>(dashboardViewModel.exchangeRates["Realtime Currency Exchange Rate"])
            //currencyExchangeRatesText.Text = dashboardViewModel.exchangeRates["8. Bid Price"];
            //currencyExchangeRatesText.Text = dashboardViewModel.exchangeRates;
        }
    }
}
