using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для Currency.xaml
    /// </summary>
    public partial class CurrencyView : UserControl
    {
        public CurrencyView()
        {
            InitializeComponent();
            //string QUERY_URL = "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=USD&to_currency=EUR&apikey=64SWH72PQKF16IA1";
            //Uri queryUri = new Uri(QUERY_URL);

            //using (WebClient client = new WebClient())
            //{
            //    dynamic json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(client.DownloadString(queryUri));
            //    //dynamic dynamic = json_data["Realtime Currency Exchange Rate"];
            //    dynamic dynamic = JsonSerializer.Deserialize<Dictionary<dynamic, dynamic>>(json_data["Realtime Currency Exchange Rate"]);

            //    //currencyExchangeRatesText.Text = dashboardViewModel.exchangeRates["8. Bid Price"];
            //    //currencyExchangeRatesText.Text = dashboardViewModel.exchangeRates;
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedFromCurrency = (ComboBoxItem) comboBoxFromCurrency.SelectedItem;
            ComboBoxItem selectedToCurrency = (ComboBoxItem) comboBoxToCurrency.SelectedItem;
            string QUERY_URL = "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=" + selectedFromCurrency.Content.ToString() + "&to_currency=" + selectedToCurrency.Content.ToString() + "&apikey=64SWH72PQKF16IA1";
            Uri queryUri = new Uri(QUERY_URL);

            using (WebClient client = new WebClient())
            {
                dynamic json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(client.DownloadString(queryUri));
                //textBlockValueExchangeRates.Text = QUERY_URL;
                dynamic dynamic = json_data["Realtime Currency Exchange Rate"];
                //dynamic dynamic = JsonSerializer.Deserialize<Dictionary<dynamic, dynamic>>(json_data["Realtime Currency Exchange Rate"]);
                textBlockValueExchangeRates.Text = dynamic.GetProperty("5. Exchange Rate").ToString();
                //currencyExchangeRatesText.Text = dashboardViewModel.exchangeRates["8. Bid Price"];
                //currencyExchangeRatesText.Text = dashboardViewModel.exchangeRates;
            }
        }
    }
}
