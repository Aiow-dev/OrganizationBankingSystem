using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;
using OrganizationBankingSystem.Core.Helpers;
using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.Core.State.Views;
using OrganizationBankingSystem.Data;
using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.Services.EntityServices;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public List<FavoriteCourseItem> FavoriteCourseItems { get; set; }

        private FavoriteCourseDataService _favoriteCourseService;

        private dynamic _jsonData;
        private string _valueCourse;

        public DashboardView()
        {
            InitializeComponent();

            _favoriteCourseService = new(new BankSystemContextFactory());

            DataContext = this;

            LoadViewState();
        }

        private void LoadViewState()
        {
            if (!DashboardState.CachedFavoriteCourseItems.IsNullOrEmpty())
            {
                FavoriteCourses.ItemsSource = DashboardState.CachedFavoriteCourseItems;
            }
        }

        private async Task GetExchangeRateData(string fromCurrency, string toCurrency)
        {
            string QUERY_URL = $"{Properties.Settings.Default.serviceUri}function=CURRENCY_EXCHANGE_RATE&from_currency={fromCurrency}&to_currency={toCurrency}" +
    $"&apikey={Environment.GetEnvironmentVariable("ALPHAVANTAGE_API_KEY")}";
            Uri queryUri = new(QUERY_URL);

            HttpClient httpClient = new();

            _jsonData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(await Task.Run(() => httpClient.GetStreamAsync(queryUri)));
        }

        private async Task GetExchangeRate(string fromCurrency, string toCurrency)
        {
            try
            {
                await GetExchangeRateData(fromCurrency, toCurrency);

                JsonElement timeSeriesDaily = _jsonData["Realtime Currency Exchange Rate"];

                Dictionary<string, string> exchangeRate =
                    timeSeriesDaily.Deserialize<Dictionary<string, string>>();

                _valueCourse = exchangeRate["5. Exchange Rate"];
            }
            catch (WebException)
            {
                _valueCourse = string.Empty;
            }
            catch (KeyNotFoundException)
            {
                _valueCourse = string.Empty;
            }
        }

        private async Task GetFavoriteCourses(BankUser bankUser)
        {
            List<FavoriteCourse> favoriteCourses = await Task.Run(() => _favoriteCourseService.GetByBankUser(bankUser));

            FavoriteCourseItems = new();

            for (int i = 0; i < favoriteCourses.Count; i++)
            {
                await Task.Run(() => GetExchangeRate(favoriteCourses[i].FromCurrencyCode, favoriteCourses[i].ToCurrencyCode));
                FavoriteCourseItem favoriteCourseItem = new()
                {
                    NumberFavoriteCourse = i + 1,
                    FromCurrencyCode = favoriteCourses[i].FromCurrencyCode,
                    ToCurrencyCode = favoriteCourses[i].ToCurrencyCode,
                    ValueCourse = 0
                };

                if (_valueCourse != string.Empty)
                {
                    favoriteCourseItem.ValueCourse = Convert.ToDouble(_valueCourse.Replace(".", ","));
                }
                FavoriteCourseItems.Add(favoriteCourseItem);
            }

            DashboardState.CachedFavoriteCourseItems = FavoriteCourseItems;
            FavoriteCourses.ItemsSource = FavoriteCourseItems;
        }

        public async void GetFavoriteCoursesButton(object sender, RoutedEventArgs e)
        {
            NotificationManager.mainNotifier.ShowInformationPropertyMessage("Идет процесс получения избранных курсов валют...");

            ElementHelper.DisableElement(ButtonFavoriteCourses, 10000);

            BankUser bankUser = AuthenticatorState.authenticator.CurrentBankUser;

            if (bankUser != null && NetworkHelper.CheckInternetConnection())
            {
                await GetFavoriteCourses(bankUser);
            }
        }

        private void ExportCoursesToSpreadsheetDocument(object sender, RoutedEventArgs e)
        {
            ElementHelper.DisableElement(ButtonExportStatistics, 1500);

            if (FavoriteCourses.HasItems)
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new()
                {
                    InitialDirectory = Syroot.Windows.IO.KnownFolders.Downloads.Path,
                    Title = "Экспорт детальной статистики в Excel",
                    FileName = $"FavoriteCourses_report",
                    Filter = "|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

                string[] headers = { "Номер", "Код валюты (От)", "Код валюты (К)", "Значение" };

                List<List<DocumentItem>> documentItems = new();

                foreach (FavoriteCourseItem favoriteCourseItem in FavoriteCourses.ItemsSource)
                {
                    List<DocumentItem> documentRow = new()
                        {
                            new DocumentItem(Convert.ToString(favoriteCourseItem.NumberFavoriteCourse), CellValues.Number),
                            new DocumentItem(Convert.ToString(favoriteCourseItem.FromCurrencyCode)),
                            new DocumentItem(Convert.ToString(favoriteCourseItem.ToCurrencyCode)),
                            new DocumentItem(Convert.ToString(favoriteCourseItem.ValueCourse), CellValues.Number),
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
                NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Возможно, не обновлены избранные курсы валют");
            }
        }
    }
}