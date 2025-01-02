using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using LiveCharts;
using OrganizationBankingSystem.Assets;
using OrganizationBankingSystem.Assets.NotifierMessages;
using OrganizationBankingSystem.Core.Converters;
using OrganizationBankingSystem.Core.Helpers;
using OrganizationBankingSystem.Core.Helpers.BelarusBank;
using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.Data;
using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.MVVM.ViewModel;
using OrganizationBankingSystem.Services.AuthenticationServices;
using OrganizationBankingSystem.Services.EntityServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public ChartValues<double> GraphValues { get; set; }

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

        public double[] OpenCurrencyValuesMas { get; set; }

        public double[] MinCurrencyValuesMas { get; set; }

        public double[] MaxCurrencyValuesMas { get; set; }

        public double[] CloseCurrencyValuesMas { get; set; }

        public string[] CurrencyDatesMas { get; set; }

        public double MaxCurrencyValue { get; set; }

        public bool IsOnlineMode { get; set; }

        public bool IsSetMaxValueColor { get; set; }

        public List<DetailStatisticsItem> DetailStatisticsItems { get; set; }

        public List<ListCurrencyValuesItem> ListCurrencyValuesItems { get; set; }

        public CurrencyView()
        {
            InitializeComponent();

            GetListCurrencyValues();

            CheckSetOnlineMode();

            DataContext = this;
        }

        private void CheckSetOnlineMode()
        {
            if (NetworkHelper.CheckInternetConnection())
            {
                TextBoxConnectionMode.Text = "Онлайн-режим";
                TextBoxConnectionMode.Background = ColorBrush.Info;

                IsOnlineMode = true;
            }
            else
            {
                TextBoxConnectionMode.Text = "Офлайн-режим";
                TextBoxConnectionMode.Background = ColorBrush.Warning;

                IsOnlineMode = false;
            }
        }

        public void GetListCurrencyValues()
        {
            ListCurrencyValuesItems = new List<ListCurrencyValuesItem>();

            CsvConfiguration csvConfig = new(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false
            };

            try
            {
                using StreamReader streamReader = File.OpenText(Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, "Data\\list_currency_values.csv"));
                using CsvReader csvReader = new(streamReader, csvConfig);

                while (csvReader.Read())
                {
                    var currencyCode = csvReader.GetField(0);
                    var currencyDescription = csvReader.GetField(1);

                    ListCurrencyValuesItems.Add(new ListCurrencyValuesItem
                    {
                        CurrencyCode = currencyCode,
                        CurrencyDescription = $"{currencyDescription} ({currencyCode})"
                    });
                }
            }
            catch (FileNotFoundException)
            {
                MainViewModel.IsFileListCurrencyDamaged = true;
            }
            catch (CsvHelper.MissingFieldException)
            {
                MainViewModel.IsFileListCurrencyDamaged = true;
            }
        }

        private void GetExchangeRates()
        {
            string QUERY_URL = $"{Properties.Settings.Default.serviceUri}function=FX_DAILY&from_symbol={FromCurrency}&to_symbol={ToCurrency}" +
                $"&apikey={Environment.GetEnvironmentVariable("ALPHAVANTAGE_API_KEY")}";
            Uri queryUri = new(QUERY_URL);

            try
            {
                HttpClient httpClient = new();

                dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(httpClient.GetStreamAsync(queryUri).Result);

                JsonElement timeSeriesDaily = jsonData["Time Series FX (Daily)"];

                Dictionary<string, Dictionary<string, string>> daysCurrency =
                    timeSeriesDaily.Deserialize<Dictionary<string, Dictionary<string, string>>>();

                int requiredIndex = 0;

                int availableIndex = daysCurrency.Keys.Count;

                if (RequiredValues > availableIndex)
                {
                    RequiredValues = availableIndex;

                    Dispatcher.Invoke(() =>
                    {
                        NotificationManager.mainNotifier.ShowWarningPropertyMessage($"Указанное значение количества дней недоступно\nМаксимально доступное значение для данного курса выбранных валют: {availableIndex}");
                    });
                }

                OpenCurrencyValuesMas = new double[RequiredValues];
                MinCurrencyValuesMas = new double[RequiredValues];
                MaxCurrencyValuesMas = new double[RequiredValues];
                CloseCurrencyValuesMas = new double[RequiredValues];
                CurrencyDatesMas = new string[RequiredValues];

                foreach (string date in daysCurrency.Keys)
                {
                    if (requiredIndex == RequiredValues)
                    {
                        break;
                    }

                    OpenCurrencyValuesMas[requiredIndex] = Convert.ToDouble(daysCurrency[date]["1. open"].Replace(".", ","));
                    MaxCurrencyValuesMas[requiredIndex] = Convert.ToDouble(daysCurrency[date]["2. high"].Replace(".", ","));
                    MinCurrencyValuesMas[requiredIndex] = Convert.ToDouble(daysCurrency[date]["3. low"].Replace(".", ","));
                    CloseCurrencyValuesMas[requiredIndex] = Convert.ToDouble(daysCurrency[date]["4. close"].Replace(".", ","));

                    CurrencyDatesMas[requiredIndex] = date;

                    requiredIndex++;
                }
            }
            catch (WebException)
            {
                Dispatcher.Invoke(() =>
                {
                    NotificationManager.mainNotifier.ShowErrorPropertyMessage(ConnectionMessage.DISABLE_CONNECTION);
                });
            }
            catch (KeyNotFoundException)
            {
                Dispatcher.Invoke(() =>
                {
                    NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Возможно, на данный момент актуальный курс выбранных валют не доступен");
                });
            }
        }

        private double[] GetCurrencyValuesMasFromTypeGraph()
        {
            ComboBoxItem typeGraph = (ComboBoxItem)ComboBoxTypeGraph.SelectedItem;

            if (typeGraph != null)
            {
                string typeGraphContent = typeGraph.Content.ToString();

                return typeGraphContent switch
                {
                    "График открытых значений" => OpenCurrencyValuesMas,
                    "График минимальных значений" => MinCurrencyValuesMas,
                    "График максимальных значений" => MaxCurrencyValuesMas,
                    "График закрытых значений" => CloseCurrencyValuesMas,
                    _ => OpenCurrencyValuesMas,
                };
            }
            else
            {
                return OpenCurrencyValuesMas;
            }
        }

        private void SetConvertCurrencyValue(double unitCost = 1.0, bool includeSecondCurrencyValue = false)
        {
            double convertCurrencyValuesResult;

            if (Formaters.FormatNumbers(FirstCurrencyNumber.Text).Length > 0)
            {
                double firstFormatCurrencyValue = Formaters.FormatTextToDouble(FirstCurrencyNumber.Text);
                FirstCurrencyNumber.Text = firstFormatCurrencyValue.ToString();

                if (includeSecondCurrencyValue)
                {
                    double secondFormatCurrencyValue = Formaters.FormatTextToDouble(UnitCostCurrencyValue.Text);
                    UnitCostCurrencyValue.Text = secondFormatCurrencyValue.ToString();

                    convertCurrencyValuesResult = CurrencyValueConverter.ConvertCurrencyValues(firstFormatCurrencyValue, secondFormatCurrencyValue);
                }
                else
                {
                    convertCurrencyValuesResult = CurrencyValueConverter.ConvertCurrencyValues(firstFormatCurrencyValue, unitCost);
                }
            }
            else
            {
                convertCurrencyValuesResult = unitCost;
            }

            TextBlockValueExchangeRates.Text = convertCurrencyValuesResult.ToString();
        }

        private Tuple<double, double> SetValuesGraph()
        {
            double[] currencyValuesMas = GetCurrencyValuesMasFromTypeGraph();

            int currencyValuesMasLength = currencyValuesMas.Length;

            for (int i = currencyValuesMasLength - 1; i >= 0; i--)
            {
                DetailStatisticsItems.Add(new DetailStatisticsItem
                {
                    NumberOfDay = currencyValuesMasLength - i,
                    OpenValueCurrency = OpenCurrencyValuesMas[i],
                    MinValueCurrency = MinCurrencyValuesMas[i],
                    MaxValueCurrency = MaxCurrencyValuesMas[i],
                    CloseValueCurrency = CloseCurrencyValuesMas[i],
                    DateCurrency = CurrencyDatesMas[i]
                });

                GraphValues.Add(currencyValuesMas[i]);
            }

            MaxCurrencyValue = GraphValues.Max();

            if (Formaters.FormatNumbers(FirstCurrencyNumber.Text) != string.Empty)
            {
                if (Formaters.FormatNumbers(UnitCostCurrencyValue.Text) == string.Empty)
                {
                    SetConvertCurrencyValue(GraphValues[^1]);
                }
                else
                {
                    SetConvertCurrencyValue(includeSecondCurrencyValue: true);
                }
            }
            else
            {
                TextBlockValueExchangeRates.Text = Convert.ToString(GraphValues[^1]);
            }

            return new Tuple<double, double>(GraphValues[0], GraphValues[^1]);
        }

        private void RenderingGraph(double firstCurrencyValue, double lastCurrencyValue)
        {
            int requiredPointGeometrySize = ValidatorNumber.ValidateNumberTextInput(8, 15, GraphPointGeometrySize.Text);

            Brush fillColor = ButtonStrokeColorUp.Background.Clone();
            Brush fillColorOpacity = ButtonColorUp.Background.Clone();
            string arrow = "↑";
            string numberSign = "+";

            const double TOLERANCE = 0.01;

            if (firstCurrencyValue > lastCurrencyValue)
            {
                fillColor = ButtonStrokeColorDown.Background.Clone();
                fillColorOpacity = ButtonColorDown.Background.Clone();
                arrow = "↓";
                numberSign = string.Empty;
            }
            else if (Math.Abs(firstCurrencyValue - lastCurrencyValue) < TOLERANCE)
            {
                fillColor = ButtonStrokeColorEquals.Background.Clone();
                fillColorOpacity = ButtonColorEquals.Background.Clone();
                arrow = string.Empty;
                numberSign = string.Empty;
            }

            const int DEFAULT_OPACITY = 70;
            const int MAX_OPACITY = 101;

            fillColorOpacity.Opacity = 1.0 - (ValidatorNumber.ValidateNumberTextInput(DEFAULT_OPACITY, MAX_OPACITY, NumberPercentOpacityGraph.Text) / 100.0);

            GraphSeries.Values = GraphValues;
            GraphSeries.Stroke = fillColor;
            GraphSeries.Fill = fillColorOpacity;

            GraphSeries.PointGeometrySize = requiredPointGeometrySize;

            GraphAxisSectionMax.Value = MaxCurrencyValue;

            GraphAxisSectionMax.Stroke = IsSetMaxValueColor ? ButtonColorMaxValue.Background.Clone() : fillColor;

            bool showMaxValueCurrency = ToggleCheckBoxMaxValueCurrency.IsChecked.HasValue
                && ToggleCheckBoxMaxValueCurrency.IsChecked.Value
;

            GraphAxisSectionMax.Visibility = showMaxValueCurrency ? Visibility.Visible : Visibility.Collapsed;

            DifferencePercentValueText.Foreground = fillColor;
            DifferencePercentValueText.Text = $"{numberSign}{lastCurrencyValue - firstCurrencyValue} ({((lastCurrencyValue / firstCurrencyValue) - 1) * 100} %){arrow}";

            DetailStatistics.ItemsSource = DetailStatisticsItems;
        }

        private async Task GetCurrencyValueOnlineMode()
        {
            const int DEFAULT_REQUIRED_VALUES = 30;
            const int MAX_REQUIRED_VALUES = 360;

            RequiredValues = ValidatorNumber.ValidateNumberTextInput(DEFAULT_REQUIRED_VALUES, MAX_REQUIRED_VALUES, NumberValuesGraph.Text);

            NotificationManager.mainNotifier.ShowInformationPropertyMessage($"Идет процесс построения графика валют...\nИсходная валюта: {FromCurrency}\nКонечная валюта: {ToCurrency}");

            await Task.Run(GetExchangeRates);

            GraphValues = new ChartValues<double>();
            DetailStatisticsItems = new List<DetailStatisticsItem>();

            try
            {
                if (ValidatorObject.AllNotNull(OpenCurrencyValuesMas, MinCurrencyValuesMas, MaxCurrencyValuesMas, CloseCurrencyValuesMas))
                {
                    Tuple<double, double> tupleFirstLastCurrencyValues = SetValuesGraph();

                    RenderingGraph(tupleFirstLastCurrencyValues.Item1, tupleFirstLastCurrencyValues.Item2);
                }
                else
                {
                    NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Возможно, актуальный курс выбранных валют недоступен");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Неверное количество значений");
            }
        }

        private void GetCurrencyValueOfflineMode()
        {
            GraphSeries.Values = new ChartValues<double>();
            DifferencePercentValueText.Text = string.Empty;

            DetailStatistics.ItemsSource = new List<DetailStatisticsItem>();

            if (Formaters.FormatNumbers(UnitCostCurrencyValue.Text) != string.Empty)
            {
                SetConvertCurrencyValue(Formaters.FormatTextToDouble(UnitCostCurrencyValue.Text),
                    includeSecondCurrencyValue: true);
            }
            else
            {
                SetConvertCurrencyValue();
            }
        }

        private async void GetCurrencyValue(object sender, RoutedEventArgs e)
        {
            ElementHelper.DisableElement(ButtonGetCurrencyValue, 2000);

            ListCurrencyValuesItem selectedFromCurrency = (ListCurrencyValuesItem)ComboBoxFromCurrency.SelectedItem;
            ListCurrencyValuesItem selectedToCurrency = (ListCurrencyValuesItem)ComboBoxToCurrency.SelectedItem;

            if (ValidatorObject.AllNotNull(selectedFromCurrency, selectedToCurrency))
            {
                FromCurrency = selectedFromCurrency.CurrencyCode;
                ToCurrency = selectedToCurrency.CurrencyCode;

                CheckSetOnlineMode();

                if (IsOnlineMode)
                {
                    await GetCurrencyValueOnlineMode();
                }
                else if (Formaters.FormatNumbers(FirstCurrencyNumber.Text) != string.Empty ||
                    Formaters.FormatNumbers(UnitCostCurrencyValue.Text) != string.Empty)
                {
                    GetCurrencyValueOfflineMode();
                }
                else
                {
                    NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Не удается получить актуальный курс выбранных валют в офлайн-режиме. \nВозможно, не указано одно из следующих полей: количество, стоимость за единицу");

                    TextBlockValueExchangeRates.Text = string.Empty;
                }
            }
            else
            {
                NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Возможно, в списках валют не выбраны или выбраны валюты, не содержащиеся в них");
            }
        }

        private void SwapValuesComboBox(object sender, RoutedEventArgs e)
        {
            ComboBoxHelper.SwapValuesComboBox(ComboBoxFromCurrency, ComboBoxToCurrency,
                "Ошибка. Возможно, отсутствует выбранное значение исходной или конечной валюты. Или выбрана валюта, не представленная в списках валют");
        }

        private void ExportStatisticsToSpreadsheetDocument(object sender, RoutedEventArgs e)
        {
            ElementHelper.DisableElement(ButtonExportStatistics, 1500);

            if (DetailStatistics.HasItems)
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new()
                {
                    InitialDirectory = Syroot.Windows.IO.KnownFolders.Downloads.Path,
                    Title = "Экспорт детальной статистики в Excel",
                    FileName = $"{FromCurrency}_to_{ToCurrency}_detail_statistics_report",
                    Filter = "|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

                string[] headers = { "Номер дня", "Открытие", "Минимум", "Максимум", "Закрытие", "Дата" };

                List<List<DocumentItem>> documentItems = new();

                foreach (DetailStatisticsItem detailStatisticsItem in DetailStatistics.ItemsSource)
                {
                    List<DocumentItem> documentRow = new()
                        {
                            new DocumentItem(Convert.ToString(detailStatisticsItem.NumberOfDay), CellValues.Number),
                            new DocumentItem(Convert.ToString(detailStatisticsItem.OpenValueCurrency), CellValues.Number),
                            new DocumentItem(Convert.ToString(detailStatisticsItem.MinValueCurrency), CellValues.Number),
                            new DocumentItem(Convert.ToString(detailStatisticsItem.MaxValueCurrency), CellValues.Number),
                            new DocumentItem(Convert.ToString(detailStatisticsItem.CloseValueCurrency), CellValues.Number),
                            new DocumentItem(Convert.ToString(detailStatisticsItem.DateCurrency), CellValues.String)
                        };

                    documentItems.Add(documentRow);
                }

                try
                {
                    DocumentHelpers.Export(saveFileDialog.FileName, headers, documentItems);

                    NotificationManager.mainNotifier.ShowCompletedPropertyMessage("Операция выполнена успешно!");
                }
                catch (IOException)
                {
                    NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Возможно, данный файл занят другим процессом или уже открыт в Microsoft Excel");
                }
            }
            else
            {
                NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Детальная статистика и график курсов валют не построены");
            }
        }

        private void NumberValuesGraphPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = ValidatorNumber.IsNumberText(e.Text);
        }

        private void DoubleNumberValuesGraphPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = ValidatorNumber.IsDoubleNumberText(e.Text);
        }

        private void SortCurrencyDescriptionValuesComboBox(object sender, RoutedEventArgs e)
        {
            ComboBoxHelper.SortValuesComboBox(ComboBoxFromCurrency, "CurrencyDescription");
        }

        private void SortCurrencyCodeValuesComboBox(object sender, RoutedEventArgs e)
        {
            ComboBoxHelper.SortValuesComboBox(ComboBoxFromCurrency, "CurrencyCode");
        }

        private void ColorDialogSetBackground(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new();

            if (colorDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            System.Drawing.Color selectedColor = colorDialog.Color;

            Button button = (Button)sender;

            button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B));
        }

        private void ColorDialogSetMaxValueBackground(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new();

            if (colorDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            System.Drawing.Color selectedColor = colorDialog.Color;

            Button button = (Button)sender;

            button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B));
            button.BorderThickness = new Thickness(0);

            IsSetMaxValueColor = true;
        }

        private void ResetGraphParameters(object sender, RoutedEventArgs e)
        {
            ElementHelper.DisableElement(ButtonResetGraphParameters, 1000);

            NumberValuesGraph.Text = string.Empty;
            GraphPointGeometrySize.Text = string.Empty;

            ComboBoxTypeGraph.SelectedIndex = -1;

            ButtonColorUp.Background = ColorBrush.Green;
            ButtonColorDown.Background = ColorBrush.Red;
            ButtonColorEquals.Background = ColorBrush.Grey;

            ButtonStrokeColorUp.Background = ColorBrush.Green;
            ButtonStrokeColorDown.Background = ColorBrush.Red;
            ButtonStrokeColorEquals.Background = ColorBrush.Grey;

            ButtonColorMaxValue.Background = ColorBrush.Transparent;
            ButtonColorMaxValue.BorderThickness = new Thickness(1);

            ToggleCheckBoxMaxValueCurrency.IsChecked = false;
            NumberPercentOpacityGraph.Text = string.Empty;
        }

        private async Task GetCurrencyValueBelarusBankOnlineMode()
        {
            string currencyCode = "USD";

            ListCurrencyValuesItem selectedFromCurrency = (ListCurrencyValuesItem)ComboBoxFromCurrency.SelectedItem;
            ListCurrencyValuesItem selectedToCurrency = (ListCurrencyValuesItem)ComboBoxToCurrency.SelectedItem;

            if (selectedFromCurrency != null)
            {
                if (selectedToCurrency == null)
                {
                    currencyCode = selectedFromCurrency.CurrencyCode;
                }
                else
                {
                    currencyCode = selectedToCurrency.CurrencyCode;
                }
            }

            NotificationManager.mainNotifier.ShowInformationPropertyMessage($"Идет процесс получения курса валют Беларусбанка...\nВалюта: {currencyCode}\nОтделение: г. Брест, пр. Партизанский, отделение 100/1050");

            string responseValueExchangeRates = await Task.Run(() => BelarusBankHelper.GetExchangeRates(currencyCode));

            if (responseValueExchangeRates.Equals("Not defined"))
            {
                NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Возможно, отсутствует или является нестабильным подключение к сети Интернет.\nВозможно, на данный момент актуальный курс выбранных валют не доступен");
            }
            else
            {
                TextBlockValueExchangeRates.Text = responseValueExchangeRates;
            }
        }

        private async void GetCurrencyValueBelarusBank(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ElementHelper.DisableElement(BelarusBankBorder, 3000);

            CheckSetOnlineMode();

            if (IsOnlineMode)
            {
                await GetCurrencyValueBelarusBankOnlineMode();
            }
            else
            {
                NotificationManager.mainNotifier.ShowErrorPropertyMessage(ConnectionMessage.DISABLE_CONNECTION);
            }
        }

        private void ChangeBackgroundEnterBelarusBank(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BelarusBankBorderRectangle.Fill = ColorBrush.GreenLight;
        }

        private void ChangeBackgroundLeaveBelarusBank(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BelarusBankBorderRectangle.Fill = ColorBrush.Green;
        }

        private static async Task AddFavoriteCourse(string fromCurrencyCode, string toCurrencyCode)
        {
            FavoriteCourse favoriteCourse = new()
            {
                FromCurrencyCode = fromCurrencyCode,
                ToCurrencyCode = toCurrencyCode,
                BankUserId = AuthenticatorState.authenticator.CurrentBankUser.Id
            };

            FavoriteCourseDataService favoriteCourseService = new(new BankSystemContextFactory());
            await Task.Run(() => favoriteCourseService.Create(favoriteCourse));
        }

        private async void AddFavoriteCourseButton(object sender, RoutedEventArgs e)
        {
            ListCurrencyValuesItem fromCurrency = (ListCurrencyValuesItem)ComboBoxFromCurrency.SelectedItem;
            ListCurrencyValuesItem toCurrency = (ListCurrencyValuesItem)ComboBoxToCurrency.SelectedItem;

            if (fromCurrency != null && toCurrency != null)
            {
                await AddFavoriteCourse(fromCurrency.CurrencyCode, toCurrency.CurrencyCode);
                NotificationManager.mainNotifier.ShowCompletedPropertyMessage($"Пара валют {fromCurrency.CurrencyCode} - {toCurrency.CurrencyCode} добавлена в избранное");
            }
        }
    }
}