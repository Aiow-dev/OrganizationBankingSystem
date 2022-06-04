using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using LiveCharts;
using OrganizationBankingSystem.Core;
using OrganizationBankingSystem.Core.Helpers;
using OrganizationBankingSystem.Data;
using OrganizationBankingSystem.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
            if (NetworkHelpers.CheckInternetConnection())
            {
                TextBoxConnectionMode.Text = "Онлайн режим";
                TextBoxConnectionMode.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(188, 219, 255));

                IsOnlineMode = true;
            }
            else
            {
                TextBoxConnectionMode.Text = "Офлайн режим";
                TextBoxConnectionMode.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 241, 208));

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
            string QUERY_URL = $"{Properties.Settings.Default.serviceUri}&from_symbol={FromCurrency}&to_symbol={ToCurrency}" +
                $"&apikey={ServiceManager.ServiceManager.GetServiceKey()}";
            Uri queryUri = new(QUERY_URL);

            //TODO: use HttpClient
            /*using HttpClient client = new();*/

            try
            {
                dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(new WebClient().DownloadString(queryUri));

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
                        NotifierHelper.notifier.ShowWarningPropertyMessage($"Указанное значение количества дней недоступно\nМаксимально доступное значение для данного курса выбранных валют: {availableIndex}");

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
                    NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, отсутствует или является нестабильным подключение к сети Интернет");
                });
            }
            catch (KeyNotFoundException)
            {
                Dispatcher.Invoke(() =>
                {
                    NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, на данный момент актуальный курс выбранных валют не доступен");
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

        private static double ConvertCurrencyValues(double numberUnit, double unitCost)
        {
            return numberUnit * unitCost;
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

                    convertCurrencyValuesResult = ConvertCurrencyValues(firstFormatCurrencyValue, secondFormatCurrencyValue);
                }
                else
                {
                    convertCurrencyValuesResult = ConvertCurrencyValues(firstFormatCurrencyValue, unitCost);
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
                FirstCurrencyNumber.Text = string.Empty;
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

            NotifierHelper.notifier.ShowInformationPropertyMessage($"Идет процесс построения графика валют...\nИсходная валюта: {FromCurrency}\nКонечная валюта: {ToCurrency}");

            await Task.Run(GetExchangeRates);

            GraphValues = new ChartValues<double>();
            DetailStatisticsItems = new List<DetailStatisticsItem>();

            try
            {
                if (ValidatorObject.AllNotNull(OpenCurrencyValuesMas, MinCurrencyValuesMas, MaxCurrencyValuesMas, CloseCurrencyValuesMas))
                {
                    Tuple<double, double> tupleFirstLastCurrencyValues = SetValuesGraph();

                    RenderingGraph(tupleFirstLastCurrencyValues.Item1, tupleFirstLastCurrencyValues.Item2);

                    TextBlockValueExchangeRates.Text = OpenCurrencyValuesMas[0].ToString();
                }
                else
                {
                    NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, актуальный курс выбранных валют недоступен");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Неверное количество значений");
            }
        }

        private void GetCurrencyValueOfflineMode()
        {
            GraphSeries.Values = new ChartValues<double>();
            DifferencePercentValueText.Text = string.Empty;

            DetailStatistics.ItemsSource = new List<DetailStatisticsItem>();

            if (Formaters.FormatNumbers(UnitCostCurrencyValue.Text).Length > 0)
            {
                SetConvertCurrencyValue(includeSecondCurrencyValue: true);
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
                else
                {
                    GetCurrencyValueOfflineMode();
                }
            }
            else
            {
                NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, в списках валют не выбраны или выбраны валюты, не содержащиеся в них");
            }
        }

        private void SwapValuesComboBox(object sender, RoutedEventArgs e)
        {
            ListCurrencyValuesItem fromCurrency = (ListCurrencyValuesItem)ComboBoxFromCurrency.SelectedItem;
            ListCurrencyValuesItem toCurrency = (ListCurrencyValuesItem)ComboBoxToCurrency.SelectedItem;

            if (fromCurrency != null && toCurrency != null)
            {
                ComboBoxFromCurrency.SelectedItem = toCurrency;
                ComboBoxToCurrency.SelectedItem = fromCurrency;
            }
            else
            {
                NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, отсутствует выбранное значение исходной или конечной валюты. Или выбрана валюта, не представленная в списках валют");
            }
        }

        private void ExportStatisticsToSpreadsheetDocument(object sender, RoutedEventArgs e)
        {
            ElementHelper.DisableElement(ButtonExportStatistics, 1500);

            if (DetailStatistics.HasItems)
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new();
                saveFileDialog.InitialDirectory = Syroot.Windows.IO.KnownFolders.Downloads.Path;
                saveFileDialog.Title = "Экспорт детальной статистики в Excel";
                saveFileDialog.FileName = $"{FromCurrency}_to_{ToCurrency}_detail_statistics_report";
                saveFileDialog.Filter = "|*.xlsx";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

                try
                {
                    using SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Create(saveFileDialog.FileName.ToString(), SpreadsheetDocumentType.Workbook);

                    WorkbookPart workBookPart = spreadSheetDocument.AddWorkbookPart();
                    workBookPart.Workbook = new Workbook();

                    WorksheetPart workSheetPart = workBookPart.AddNewPart<WorksheetPart>();
                    workSheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = spreadSheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new()
                    {
                        Id = spreadSheetDocument.WorkbookPart.GetIdOfPart(workSheetPart),
                        SheetId = 1,
                        Name = saveFileDialog.FileName.ToString()
                    };

                    sheets.Append(sheet);

                    Worksheet workSheet = workSheetPart.Worksheet;
                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();

                    Row rowHeader = new();

                    DocumentHelpers.AppendCellsToRow(rowHeader,
                        new string[6] {"Номер дня", "Открытие", "Минимум",
                            "Максимум", "Закрытие", "Дата"}, CellValues.String);

                    sheetData.AppendChild(rowHeader);

                    foreach (DetailStatisticsItem detailStatisticsItem in DetailStatistics.ItemsSource)
                    {
                        Row row = new();

                        DocumentHelpers.AppendCellToRow(row, Convert.ToString(detailStatisticsItem.NumberOfDay), CellValues.Number);
                        DocumentHelpers.AppendCellToRow(row, Convert.ToString(detailStatisticsItem.OpenValueCurrency), CellValues.Number);
                        DocumentHelpers.AppendCellToRow(row, Convert.ToString(detailStatisticsItem.MinValueCurrency), CellValues.Number);
                        DocumentHelpers.AppendCellToRow(row, Convert.ToString(detailStatisticsItem.MaxValueCurrency), CellValues.Number);
                        DocumentHelpers.AppendCellToRow(row, Convert.ToString(detailStatisticsItem.CloseValueCurrency), CellValues.Number);
                        DocumentHelpers.AppendCellToRow(row, Convert.ToString(detailStatisticsItem.DateCurrency), CellValues.String);

                        sheetData.AppendChild(row);
                    }

                    workSheetPart.Worksheet.Save();
                    spreadSheetDocument.Save();

                    spreadSheetDocument.Close();

                    NotifierHelper.notifier.ShowCompletedPropertyMessage("Операция выполнена успешно!");
                }
                catch (IOException)
                {
                    NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, данный файл занят другим процессом или уже открыт в Microsoft Excel");
                }
            }
            else
            {
                NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Детальная статистика и график курсов валют не построены");
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
            ComboBoxFromCurrency.Items.SortDescriptions.Clear();

            ComboBoxFromCurrency.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("CurrencyDescription",
                System.ComponentModel.ListSortDirection.Ascending));
        }

        private void SortCurrencyCodeValuesComboBox(object sender, RoutedEventArgs e)
        {
            ComboBoxFromCurrency.Items.SortDescriptions.Clear();

            ComboBoxFromCurrency.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("CurrencyCode",
                System.ComponentModel.ListSortDirection.Ascending));
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

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });

            e.Handled = true;
        }

        private void ResetGraphParameters(object sender, RoutedEventArgs e)
        {
            ElementHelper.DisableElement(ButtonResetGraphParameters, 1000);

            NumberValuesGraph.Text = string.Empty;
            GraphPointGeometrySize.Text = string.Empty;

            ComboBoxTypeGraph.SelectedIndex = -1;

            ButtonColorUp.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(161, 204, 165));
            ButtonColorDown.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(210, 31, 60));
            ButtonColorEquals.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(125, 132, 145));

            ButtonStrokeColorUp.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(161, 204, 165));
            ButtonStrokeColorDown.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(210, 31, 60));
            ButtonStrokeColorEquals.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(125, 132, 145));

            ButtonColorMaxValue.Background = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
            ButtonColorMaxValue.BorderThickness = new Thickness(1);

            ToggleCheckBoxMaxValueCurrency.IsChecked = false;
            NumberPercentOpacityGraph.Text = string.Empty;
        }

        private void GetExchangeRatesBelarusbank()
        {
            string QUERY_URL = $"{Properties.Settings.Default.belarusBankServiceUri}?city=Брест";
            Uri queryUri = new(QUERY_URL);

            try
            {
                List<JsonElement> jsonData = JsonSerializer.Deserialize<List<JsonElement>>(new WebClient().DownloadString(queryUri));

                ValueExchangeRates = jsonData[0].Deserialize<Dictionary<string, string>>()["USD_in"];
            }
            catch (WebException)
            {
                Dispatcher.Invoke(() =>
                {
                    NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, отсутствует или является нестабильным подключение к сети Интернет");
                });
            }
            catch (KeyNotFoundException)
            {
                Dispatcher.Invoke(() =>
                {
                    NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, на данный момент актуальный курс выбранной валюты не доступен");
                });
            }
        }

        private async void GetCurrencyValueBelarusBank(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await Task.Run(GetExchangeRatesBelarusbank);

            TextBlockValueExchangeRates.Text = ValueExchangeRates;
        }

        private void ChangeBackgroundEnterBelarusBank(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BelarusBankBorderRectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(3, 166, 75));
        }

        private void ChangeBackgroundLeaveBelarusBank(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BelarusBankBorderRectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 148, 66));
        }
    }
}