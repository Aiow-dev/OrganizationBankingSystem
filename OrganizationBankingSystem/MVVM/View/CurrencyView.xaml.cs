using LiveCharts;
using OrganizationBankingSystem.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

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
        public SeriesCollection Series { get; set; }
        public ChartValues<double> Values1 { get; set; }

        public CurrencyView()
        {
            InitializeComponent();
            CurrencyValues = new List<double>();
            DataContext = this;
        }

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

        public List<double> CurrencyValues { get; set; }

        public void GetExchangeRates()
        {
            string QUERY_URL = "https://www.alphavantage.co/query?function=FX_DAILY&from_symbol=" + FromCurrency + "&to_symbol=" + ToCurrency + "&apikey=64SWH72PQKF16IA1";
            Uri queryUri = new(QUERY_URL);

            //TODO: use HttpClient
            using HttpClient client = new();
            try
            {
                /*var stock = client.GetFromJsonAsync<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(QUERY_URL);
                stock.ContinueWith<Dictionary<string, Dictionary<string, string>>>(daysCurrensy => 
                ValueExchangeRates = stock.ToString();*/
                dynamic json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(new WebClient().DownloadString(queryUri));
                JsonElement timeSeriesDaily = json_data["Time Series FX (Daily)"];
                Dictionary<string, Dictionary<string, string>> daysCurrency =
                    timeSeriesDaily.Deserialize<Dictionary<string, Dictionary<string, string>>>();
                foreach (Dictionary<string, string> dayCurrency in daysCurrency.Values)
                {
                    foreach (string currency in dayCurrency.Values)
                    {
                        CurrencyValues.Add(Convert.ToDouble(currency.Replace(".", ",")));
                    }
                }
                CurrencyValues.Reverse();
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
            CurrencyValues.Clear();

            ComboBoxItem selectedFromCurrency = (ComboBoxItem)comboBoxFromCurrency.SelectedItem;
            ComboBoxItem selectedToCurrency = (ComboBoxItem)comboBoxToCurrency.SelectedItem;

            if (selectedFromCurrency != null && selectedToCurrency != null)
            {
                FromCurrency = selectedFromCurrency.Content.ToString();
                ToCurrency = selectedToCurrency.Content.ToString();
                await GetExchangeRatesAsync();
                Values1 = new ChartValues<double>();
                int requiredNumberGraphValues = 30;

                try
                {
                    for (int i = 0; i < requiredNumberGraphValues; i++)
                    {
                        Values1.Add(CurrencyValues[i]);
                    }

                    SolidColorBrush fillColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA1CCA5");
                    string arrow = "↑";
                    string numberSign = "+";

                    double firstCurrencyValue = CurrencyValues[0];
                    double lastCurrencyValue = CurrencyValues[requiredNumberGraphValues - 1];

                    textBlockValueExchangeRates.Text = firstCurrencyValue + " " + lastCurrencyValue;

                    if (firstCurrencyValue > lastCurrencyValue)
                    {
                        fillColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD21F3C");
                        arrow = "↓";
                        numberSign = "";
                    }

                    graphSeries.Stroke = fillColor;
                    fillColor.Opacity = 0.7;
                    graphSeries.Fill = fillColor;
                    //graphSeries.PointGeometry = null;
                    graphSeries.Values = Values1;

                    differencePercentValueText.Foreground = fillColor;
                    differencePercentValueText.Text = $"{numberSign}{lastCurrencyValue - firstCurrencyValue} ({lastCurrencyValue / firstCurrencyValue} %){arrow}";
                }
                catch (ArgumentOutOfRangeException)
                {
                    notifier.ShowMessage("Неверное количество значений");
                }
            }
            else
            {
                textBlockValueExchangeRates.Text = "Ошибка. Возможно, отсутствует выбранное значение исходной или конечной валюты или выбрана валюта, не представленная в списках валют";
            }
        }

        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });
    }
}
