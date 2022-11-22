using OrganizationBankingSystem.Core.Helpers;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.Data;
using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.Services.EntityServices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public List<FavoriteCourseItem> FavoriteCourseItems { get; set; }

        private dynamic _jsonData;
        private string _valueCourse;

        public DashboardView()
        {
            InitializeComponent();

            DataContext = this;
        }

        private async Task GetExchangeRateData(string fromCurrency, string toCurrency)
        {
            string QUERY_URL = $"{Properties.Settings.Default.serviceUri}function=CURRENCY_EXCHANGE_RATE&from_currency={fromCurrency}&to_currency={toCurrency}" +
    $"&apikey={ServiceManager.ServiceManager.GetServiceKey()}";
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
            FavoriteCourseDataService favoriteCourseService = new(new BankSystemContextFactory());
            List<FavoriteCourse> favoriteCourses = await Task.Run(() => favoriteCourseService.GetByBankUser(bankUser));

            FavoriteCourseItems = new();

            for (int i = 0; i < favoriteCourses.Count; i++)
            {
                await Task.Run(() => GetExchangeRate(favoriteCourses[i].FromCurrencyCode, favoriteCourses[i].ToCurrencyCode));

                if (_valueCourse != string.Empty)
                {
                    FavoriteCourseItem favoriteCourseItem = new()
                    {
                        NumberFavoriteCourse = i + 1,
                        FromCurrencyCode = favoriteCourses[i].FromCurrencyCode,
                        ToCurrencyCode = favoriteCourses[i].ToCurrencyCode,
                        ValueCourse = Convert.ToDouble(_valueCourse.Replace(".", ","))
                    };

                    FavoriteCourseItems.Add(favoriteCourseItem);
                }
            }

            DetailStatistics.ItemsSource = FavoriteCourseItems;
        }

        private async void GetFavoriteCoursesButton(object sender, RoutedEventArgs e)
        {
            BankUser bankUser = AuthenticatorState.authenticator.CurrentBankUser;

            if (bankUser != null && NetworkHelper.CheckInternetConnection())
            {
                await GetFavoriteCourses(bankUser);
            }
        }
    }
}