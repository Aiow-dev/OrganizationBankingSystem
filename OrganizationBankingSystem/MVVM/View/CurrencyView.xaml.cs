using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для Currency.xaml
    /// </summary>
    public partial class CurrencyView : UserControl
    {
        private string _fromCurrency;
        private string _toCurrency;
        private string _valueExchangeRates;
        public CurrencyView()
        {
            InitializeComponent();
        }

        public string ToCurrency { get => _toCurrency; set => _toCurrency = value; }
        public string FromCurrency { get => _fromCurrency; set => _fromCurrency = value; }
        public string ValueExchangeRates { get => _valueExchangeRates; set => _valueExchangeRates = value; }

        public void GetExchangeRates()
        {
            string QUERY_URL = "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=" + FromCurrency + "&to_currency=" + ToCurrency + "&apikey=64SWH72PQKF16IA1";
            Uri queryUri = new(QUERY_URL);

            using WebClient client = new();
            try
            {
                dynamic json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(client.DownloadString(queryUri));
                dynamic dynamic = json_data["Realtime Currency Exchange Rate"];
                ValueExchangeRates = dynamic.GetProperty("5. Exchange Rate").ToString();
            }
            catch (WebException)
            {
                ValueExchangeRates = "Ошибка. Возможно, отсутствует или является нестабильным подключение к сети Интернет";
            }
            catch (KeyNotFoundException)
            {
                ValueExchangeRates = "Ошибка. Возможно, на данный момент актуальный курс выбранных валют не доступен";
            }
        }

        public async Task GetExchangeRatesAsync()
        {
            await Task.Run(() => GetExchangeRates());
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedFromCurrency = (ComboBoxItem)comboBoxFromCurrency.SelectedItem;
            ComboBoxItem selectedToCurrency = (ComboBoxItem)comboBoxToCurrency.SelectedItem;
            if (selectedFromCurrency != null && selectedToCurrency != null)
            {
                FromCurrency = selectedFromCurrency.Content.ToString();
                ToCurrency = selectedToCurrency.Content.ToString();
                await GetExchangeRatesAsync();
                textBlockValueExchangeRates.Text = ValueExchangeRates;
            }
            else
            {
                textBlockValueExchangeRates.Text = "Ошибка. Возможно, отсутствует выбранное значение исходной или конечной валюты или выбрана валюта, не представленная в списках валют";
            }
        }
    }
}
