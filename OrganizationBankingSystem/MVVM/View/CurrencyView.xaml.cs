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
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
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
                textBoxConnectionMode.Text = "Онлайн режим";
                textBoxConnectionMode.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(188, 219, 255));

                IsOnlineMode = true;
            }
            else
            {
                textBoxConnectionMode.Text = "Офлайн режим";
                textBoxConnectionMode.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 241, 208));

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

        public void GetExchangeRates()
        {
            string QUERY_URL = $"{Properties.Settings.Default.serviceUri}&from_symbol={FromCurrency}&to_symbol={ToCurrency}" +
                $"&apikey={ServiceManager.ServiceManager.GetServiceKey()}";
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

                ValueExchangeRates = Convert.ToString(RequiredValues);

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
            ComboBoxItem typeGraph = (ComboBoxItem)comboBoxTypeGraph.SelectedItem;

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

            if (Formaters.FormatNumbers(firstCurrencyNumber.Text).Length > 0)
            {
                double firstFormatCurrencyValue = Formaters.FormatTextToDouble(firstCurrencyNumber.Text);
                firstCurrencyNumber.Text = firstFormatCurrencyValue.ToString();

                if (includeSecondCurrencyValue)
                {
                    double secondFormatCurrencyValue = Formaters.FormatTextToDouble(unitCostCurrencyValue.Text);
                    unitCostCurrencyValue.Text = secondFormatCurrencyValue.ToString();

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

            textBlockValueExchangeRates.Text = convertCurrencyValuesResult.ToString();
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

            if (Formaters.FormatNumbers(firstCurrencyNumber.Text) != string.Empty)
            {
                if (Formaters.FormatNumbers(unitCostCurrencyValue.Text) == string.Empty)
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
                firstCurrencyNumber.Text = string.Empty;
            }

            return new Tuple<double, double>(GraphValues[0], GraphValues[^1]);
        }

        private void RenderingGraph(double firstCurrencyValue, double lastCurrencyValue)
        {
            int requiredPointGeometrySize = ValidatorNumber.ValidateNumberTextInput(8, 15, graphPointGeometrySize.Text);

            Brush fillColor = buttonStrokeColorUp.Background.Clone();
            Brush fillColorOpacity = buttonColorUp.Background.Clone();
            string arrow = "↑";
            string numberSign = "+";

            if (firstCurrencyValue > lastCurrencyValue)
            {
                fillColor = buttonStrokeColorDown.Background.Clone();
                fillColorOpacity = buttonColorDown.Background.Clone();
                arrow = "↓";
                numberSign = string.Empty;
            }
            else if (firstCurrencyValue == lastCurrencyValue)
            {
                fillColor = buttonStrokeColorEquals.Background.Clone();
                fillColorOpacity = buttonColorEquals.Background.Clone();
                arrow = string.Empty;
                numberSign = string.Empty;
            }

            fillColorOpacity.Opacity = 1.0 - (ValidatorNumber.ValidateNumberTextInput(70, 101, numberPercentOpacityGraph.Text) / 100.0);

            graphSeries.Values = GraphValues;
            graphSeries.Stroke = fillColor;
            graphSeries.Fill = fillColorOpacity;

            graphSeries.PointGeometrySize = requiredPointGeometrySize;

            graphAxisSectionMax.Value = MaxCurrencyValue;

            if (IsSetMaxValueColor)
            {
                graphAxisSectionMax.Stroke = buttonColorMaxValue.Background.Clone();
            }
            else
            {
                graphAxisSectionMax.Stroke = fillColor;
            }

            if ((bool)toggleCheckBoxMaxValueCurrency.IsChecked)
            {
                graphAxisSectionMax.Visibility = Visibility.Visible;
            }
            else
            {
                graphAxisSectionMax.Visibility = Visibility.Collapsed;
            }

            differencePercentValueText.Foreground = fillColor;
            differencePercentValueText.Text = $"{numberSign}{lastCurrencyValue - firstCurrencyValue} ({((lastCurrencyValue / firstCurrencyValue) - 1) * 100} %){arrow}";

            detailStatistics.ItemsSource = DetailStatisticsItems;
        }

        private async void GetCurrencyValue(object sender, RoutedEventArgs e)
        {
            ListCurrencyValuesItem selectedFromCurrency = (ListCurrencyValuesItem)comboBoxFromCurrency.SelectedItem;
            ListCurrencyValuesItem selectedToCurrency = (ListCurrencyValuesItem)comboBoxToCurrency.SelectedItem;

            if (ValidatorObject.AllNotNull(selectedFromCurrency, selectedToCurrency))
            {
                FromCurrency = selectedFromCurrency.CurrencyCode;
                ToCurrency = selectedToCurrency.CurrencyCode;

                CheckSetOnlineMode();

                if (IsOnlineMode)
                {
                    RequiredValues = ValidatorNumber.ValidateNumberTextInput(30, 1000, numberValuesGraph.Text);

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
                else
                {
                    graphSeries.Values = new ChartValues<double>();
                    differencePercentValueText.Text = string.Empty;

                    detailStatistics.ItemsSource = new List<DetailStatisticsItem>();

                    NotifierHelper.notifier.ShowInformationPropertyMessage($"1: {firstCurrencyNumber.Text}\n2: {unitCostCurrencyValue.Text}");

                    if (Formaters.FormatNumbers(unitCostCurrencyValue.Text).Length > 0)
                    {
                        SetConvertCurrencyValue(includeSecondCurrencyValue: true);
                    }
                    else
                    {
                        SetConvertCurrencyValue();
                    }
                }
            }
            else
            {
                NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, в списках валют не выбраны или выбраны валюты, не содержащиеся в них");
            }
        }

        private void SwapValuesComboBox(object sender, RoutedEventArgs e)
        {
            ListCurrencyValuesItem fromCurrency = (ListCurrencyValuesItem)comboBoxFromCurrency.SelectedItem;
            ListCurrencyValuesItem toCurrency = (ListCurrencyValuesItem)comboBoxToCurrency.SelectedItem;

            if (fromCurrency != null && toCurrency != null)
            {
                comboBoxFromCurrency.SelectedItem = toCurrency;
                comboBoxToCurrency.SelectedItem = fromCurrency;
            }
            else
            {
                NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, отсутствует выбранное значение исходной или конечной валюты. Или выбрана валюта, не представленная в списках валют");
            }
        }

        private void ExportStatisticsToSpreadsheetDocument(object sender, RoutedEventArgs e)
        {
            if (detailStatistics.HasItems)
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new();
                saveFileDialog.InitialDirectory = Syroot.Windows.IO.KnownFolders.Downloads.Path;
                saveFileDialog.Title = "Экспорт детальной статистики в Excel";
                saveFileDialog.FileName = $"{FromCurrency}_to_{ToCurrency}_detail_statistics_report";
                saveFileDialog.Filter = "|*.xlsx";

                if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
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

                        foreach (DetailStatisticsItem detailStatisticsItem in detailStatistics.ItemsSource)
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
            }
            else
            {
                NotifierHelper.notifier.ShowErrorPropertyMessage("Ошибка. Детальная статистика и график курсов валют не построены");
            }
        }

        private static readonly Regex _regexNumber = new(@"[^0-9]+");

        private void NumberValuesGraphPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = _regexNumber.IsMatch(e.Text);
        }

        private static readonly Regex _regexDouble = new(@"[^0-9.,]+");

        private void DoubleNumberValuesGraphPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = _regexDouble.IsMatch(e.Text);
        }

        private void SortCurrencyDescriptionValuesComboBox(object sender, RoutedEventArgs e)
        {
            comboBoxFromCurrency.Items.SortDescriptions.Clear();

            comboBoxFromCurrency.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("CurrencyDescription",
                System.ComponentModel.ListSortDirection.Ascending));
        }

        private void SortCurrencyCodeValuesComboBox(object sender, RoutedEventArgs e)
        {
            comboBoxFromCurrency.Items.SortDescriptions.Clear();

            comboBoxFromCurrency.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("CurrencyCode",
                System.ComponentModel.ListSortDirection.Ascending));
        }

        private void ColorDialogSetBackground(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color selectedColor = colorDialog.Color;

                Button button = (Button)sender;

                button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B));
            }
        }

        private void ColorDialogSetMaxValueBackground(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color selectedColor = colorDialog.Color;

                Button button = (Button)sender;

                button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B));
                button.BorderThickness = new Thickness(0);

                IsSetMaxValueColor = true;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });

            e.Handled = true;
        }

        private void ResetParametersGraph(object sender, RoutedEventArgs e)
        {
            numberValuesGraph.Text = string.Empty;
            graphPointGeometrySize.Text = string.Empty;

            comboBoxTypeGraph.SelectedIndex = -1;

            buttonColorUp.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(161, 204, 165));
            buttonColorDown.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(210, 31, 60));
            buttonColorEquals.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(125, 132, 145));

            buttonStrokeColorUp.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(161, 204, 165));
            buttonStrokeColorDown.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(210, 31, 60));
            buttonStrokeColorEquals.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(125, 132, 145));

            buttonColorMaxValue.Background = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
            buttonColorMaxValue.BorderThickness = new Thickness(1);

            toggleCheckBoxMaxValueCurrency.IsChecked = false;
            numberPercentOpacityGraph.Text = string.Empty;
        }
    }
}