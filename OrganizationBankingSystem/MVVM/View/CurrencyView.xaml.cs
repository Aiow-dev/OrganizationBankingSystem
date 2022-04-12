using LiveCharts;
using OrganizationBankingSystem.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public ChartValues<double> Values1 { get; set; }

        public int RequiredValues { get; set; }

        public string ToCurrency
        {
            get => _toCurrency; set
            {
                if (value != null)
                    _toCurrency = value;
                else _toCurrency = " ";
            }
        }
        public string FromCurrency
        {
            get => _fromCurrency; set
            {
                if (value != null)
                    _fromCurrency = value;
                else _fromCurrency = " ";
            }
        }
        public string ValueExchangeRates
        {
            get => _valueExchangeRates; set
            {
                if (value != null)
                    _valueExchangeRates = value;
                else
                    _valueExchangeRates = " ";
            }
        }

        public double[] CurrencyValuesMas { get; set; }
        public string[] CurrencyDatesMas { get; set; }

        public CurrencyView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void GetExchangeRates()
        {
            string QUERY_URL = "https://www.alphavantage.co/query?function=FX_DAILY&from_symbol=" + FromCurrency + "&to_symbol=" + ToCurrency + "&apikey=64SWH72PQKF16IA1";
            Uri queryUri = new(QUERY_URL);

            //TODO: use HttpClient
            using HttpClient client = new();
            try
            {
                dynamic json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(new WebClient().DownloadString(queryUri));
                JsonElement timeSeriesDaily = json_data["Time Series FX (Daily)"];
                Dictionary<string, Dictionary<string, string>> daysCurrency =
                    timeSeriesDaily.Deserialize<Dictionary<string, Dictionary<string, string>>>();

                int requiredIndex = 0;
                CurrencyDatesMas = new string[RequiredValues];
                CurrencyValuesMas = new double[RequiredValues];

                foreach (string date in daysCurrency.Keys)
                {
                    if (requiredIndex == RequiredValues)
                    {
                        break;
                    }
                    CurrencyDatesMas[requiredIndex] = date;
                    foreach (string currency in daysCurrency[date].Values)
                    {

                        CurrencyValuesMas[requiredIndex] = Convert.ToDouble(currency.Replace(".", ","));
                    }

                    requiredIndex++;
                }
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
                RequiredValues = 30;

                await GetExchangeRatesAsync();

                Values1 = new ChartValues<double>();

                try
                {
                    StringBuilder datesCurrencyLine = new();

                    for (int i = CurrencyValuesMas.Length - 1; i >= 0; i--)
                    {
                        datesCurrencyLine.Append(CurrencyDatesMas[i] + " ");
                        if (i % 2 == 0)
                        {
                            datesCurrencyLine.Append('\n');
                        }
                        Values1.Add(CurrencyValuesMas[i]);
                    }

                    double firstCurrencyValue = CurrencyValuesMas[^1];
                    double lastCurrencyValue = CurrencyValuesMas[0];

                    SolidColorBrush fillColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA1CCA5");
                    SolidColorBrush fillColorOpacity = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA1CCA5");
                    string arrow = "↑";
                    string numberSign = "+";

                    if (firstCurrencyValue > lastCurrencyValue)
                    {
                        fillColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD21F3C");
                        fillColorOpacity = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD21F3C");
                        arrow = "↓";
                        numberSign = "";
                    }

                    fillColorOpacity.Opacity = 0.3;

                    graphSeries.Values = Values1;
                    graphSeries.Stroke = fillColor;
                    graphSeries.Fill = fillColorOpacity;

                    differencePercentValueText.Foreground = fillColor;
                    differencePercentValueText.Text = $"{numberSign}{lastCurrencyValue - firstCurrencyValue} ({lastCurrencyValue / firstCurrencyValue} %){arrow}";

                    listCurrencyDates.Text = datesCurrencyLine.ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MainWindow.notifier.ShowMessage("Неверное количество значений");
                }
            }
            else
            {
                textBlockValueExchangeRates.Text = "Ошибка. Возможно, отсутствует выбранное значение исходной или конечной валюты или выбрана валюта, не представленная в списках валют";
            }
        }
    }
}
