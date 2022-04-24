using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using LiveCharts;
using OrganizationBankingSystem.Core;
using OrganizationBankingSystem.Data;
using OrganizationBankingSystem.MVVM.ViewModel;
using System;
using System.Collections.Generic;
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

        public List<DetailStatisticsItem> DetailStatisticsItems { get; set; }

        public List<ListCurrencyValuesItem> ListCurrencyValuesItems { get; set; }

        public CurrencyView()
        {
            InitializeComponent();

            GetListCurrencyValues();
            DataContext = this;
        }

        public static bool AllNotNull(params object[] objects)
        {
            return objects.All(s => s != null);
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
            string QUERY_URL = "https://www.alphavantage.co/query?function=FX_DAILY&from_symbol="
                + FromCurrency + "&to_symbol=" + ToCurrency + "&apikey=64SWH72PQKF16IA1";
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
                    MainWindow.notifier.ShowWarningPropertyMessage($"Указанное значение количества дней недоступно\nМаксимально доступное значение для данного курса выбранных валют: {availableIndex}");
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
                MainWindow.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, отсутствует или является нестабильным подключение к сети Интернет");
            }
            catch (KeyNotFoundException)
            {
                MainWindow.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, на данный момент актуальный курс выбранных валют не доступен");
            }
        }

        public async Task GetExchangeRatesAsync()
        {
            await Task.Run(() => GetExchangeRates());
        }

        private static int ValidateNumberTextInput(int defaultTextInputValue, int maxTextInputValue, string textFromInput, string warningMessage, string warningMessageExtend)
        {
            try
            {
                int textFromInputValue = Convert.ToInt32(textFromInput);

                if (textFromInputValue < maxTextInputValue)
                {
                    return textFromInputValue;
                }
                else
                {
                    return defaultTextInputValue;
                }
            }
            catch (FormatException)
            {
                MainWindow.notifier.ShowWarningPropertyMessage(warningMessage);
                return defaultTextInputValue;
            }
            catch (OverflowException)
            {
                MainWindow.notifier.ShowWarningPropertyMessage(warningMessageExtend);
                return defaultTextInputValue;
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

        private Tuple<double, double> SetValuesGraph()
        {
            int currencyValuesMasLength = OpenCurrencyValuesMas.Length;

            double[] currencyValuesMas = GetCurrencyValuesMasFromTypeGraph();

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

            return new Tuple<double, double>(GraphValues[0], GraphValues[^1]);
        }

        private void RenderingGraph(double firstCurrencyValue, double lastCurrencyValue)
        {
            int requiredPointGeometrySize = ValidateNumberTextInput(8, 15, graphPointGeometrySize.Text, "1", "2");

            Brush fillColor = buttonColorUp.Background.Clone();
            Brush fillColorOpacity = buttonColorUp.Background.Clone();
            string arrow = "↑";
            string numberSign = "+";

            if (firstCurrencyValue > lastCurrencyValue)
            {
                fillColor = buttonColorDown.Background.Clone();
                fillColorOpacity = buttonColorDown.Background.Clone();
                arrow = "↓";
                numberSign = "";
            }
            else if (firstCurrencyValue == lastCurrencyValue)
            {
                fillColor = buttonColorEquals.Background.Clone();
                fillColorOpacity = buttonColorEquals.Background.Clone();
                arrow = "";
                numberSign = "";
            }

            fillColorOpacity.Opacity = 0.3;

            graphSeries.Values = GraphValues;
            graphSeries.Stroke = fillColor;
            graphSeries.Fill = fillColorOpacity;

            graphSeries.PointGeometrySize = requiredPointGeometrySize;

            differencePercentValueText.Foreground = fillColor;
            differencePercentValueText.Text = $"{numberSign}{lastCurrencyValue - firstCurrencyValue} ({((lastCurrencyValue / firstCurrencyValue) - 1) * 100} %){arrow}";

            detailStatistics.ItemsSource = DetailStatisticsItems;

            textBlockValueExchangeRates.Text = ValueExchangeRates;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ListCurrencyValuesItem selectedFromCurrency = (ListCurrencyValuesItem)comboBoxFromCurrency.SelectedItem;
            ListCurrencyValuesItem selectedToCurrency = (ListCurrencyValuesItem)comboBoxToCurrency.SelectedItem;

            if (AllNotNull(selectedFromCurrency, selectedToCurrency))
            {
                FromCurrency = selectedFromCurrency.CurrencyCode;
                ToCurrency = selectedToCurrency.CurrencyCode;

                RequiredValues = ValidateNumberTextInput(30, 1000, numberValuesGraph.Text, "1", "2");

                MainWindow.notifier.ShowInformationPropertyMessage($"Идет процесс построения графика валют...\nИсходная валюта: {FromCurrency}\nКонечная валюта: {ToCurrency}");

                await GetExchangeRatesAsync();

                GraphValues = new ChartValues<double>();
                DetailStatisticsItems = new List<DetailStatisticsItem>();

                try
                {
                    if (AllNotNull(OpenCurrencyValuesMas, MinCurrencyValuesMas, MaxCurrencyValuesMas, CloseCurrencyValuesMas))
                    {
                        Tuple<double, double> tupleFirstLastCurrencyValues = SetValuesGraph();

                        RenderingGraph(tupleFirstLastCurrencyValues.Item1, tupleFirstLastCurrencyValues.Item2);
                    }
                    else
                    {
                        MainWindow.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, актуальный курс выбранных валют недоступен");
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    MainWindow.notifier.ShowErrorPropertyMessage("Ошибка. Неверное количество значений");
                }
            }
            else
            {
                MainWindow.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, в списках валют не выбраны или выбраны валюты, не содержащиеся в них");
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
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
                MainWindow.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, отсутствует выбранное значение исходной или конечной валюты. Или выбрана валюта, не представленная в списках валют");
            }
        }

        public static void AppendCellToRow(Row row, string cellItem, CellValues dataType)
        {
            if (dataType == CellValues.Number)
            {
                row.Append(new Cell()
                {
                    CellValue = new CellValue(Convert.ToDouble(cellItem)),
                    DataType = dataType
                });
            }
            else
            {
                row.Append(new Cell()
                {
                    CellValue = new CellValue(cellItem),
                    DataType = dataType
                });
            }
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
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

                        AppendCellToRow(rowHeader, "Номер дня", CellValues.String);
                        AppendCellToRow(rowHeader, "Открытие", CellValues.String);
                        AppendCellToRow(rowHeader, "Минимум", CellValues.String);
                        AppendCellToRow(rowHeader, "Максимум", CellValues.String);
                        AppendCellToRow(rowHeader, "Закрытие", CellValues.String);
                        AppendCellToRow(rowHeader, "Дата", CellValues.String);

                        sheetData.AppendChild(rowHeader);

                        foreach (DetailStatisticsItem detailStatisticsItem in detailStatistics.ItemsSource)
                        {
                            Row row = new();

                            AppendCellToRow(row, Convert.ToString(detailStatisticsItem.NumberOfDay), CellValues.Number);
                            AppendCellToRow(row, Convert.ToString(detailStatisticsItem.OpenValueCurrency), CellValues.Number);
                            AppendCellToRow(row, Convert.ToString(detailStatisticsItem.MinValueCurrency), CellValues.Number);
                            AppendCellToRow(row, Convert.ToString(detailStatisticsItem.MaxValueCurrency), CellValues.Number);
                            AppendCellToRow(row, Convert.ToString(detailStatisticsItem.CloseValueCurrency), CellValues.Number);
                            AppendCellToRow(row, Convert.ToString(detailStatisticsItem.DateCurrency), CellValues.String);

                            sheetData.AppendChild(row);
                        }

                        workSheetPart.Worksheet.Save();
                        spreadSheetDocument.Save();

                        spreadSheetDocument.Close();

                        MainWindow.notifier.ShowCompletedPropertyMessage("Операция выполнена успешно!");
                    }
                    catch (IOException)
                    {
                        MainWindow.notifier.ShowErrorPropertyMessage("Ошибка. Возможно, данный файл занят другим процессом или уже открыт в Microsoft Excel");
                    }
                }
            }
            else
            {
                MainWindow.notifier.ShowErrorPropertyMessage("Ошибка. Детальная статистика и график курсов валют не построены");
            }
        }

        private static readonly Regex _regex = new(@"[^0-9]+");

        private void NumberValuesGraphPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private static readonly Regex _regexDouble = new(@"[^0-9].,+");

        private void DoubleNumberValuesGraphPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = _regexDouble.IsMatch(e.Text);
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            comboBoxFromCurrency.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("CurrencyDescription",
                System.ComponentModel.ListSortDirection.Ascending));
        }

        private void RadioButton_Click_3(object sender, RoutedEventArgs e)
        {
            comboBoxToCurrency.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("CurrencyDescription",
                System.ComponentModel.ListSortDirection.Ascending));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color selectedColor = colorDialog.Color;

                Button button = (Button)sender;
                button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B));
            }
        }
    }
}