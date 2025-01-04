using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace OrganizationBankingSystem.Core.Helpers.BelarusBankHelpers
{
    public enum TypeOperation
    {
        PURCHASE,
        SALE
    }

    public static class BelarusBankHelper
    {
        private static string ConvertTypeOperation(TypeOperation typeOperation)
        {
            return typeOperation == TypeOperation.PURCHASE ? "in" : "out";
        }

        public static string GetExchangeRates(string currencyCode = "USD", TypeOperation typeOperation = TypeOperation.PURCHASE)
        {
            string QUERY_URL = $"{Properties.Settings.Default.belarusBankServiceUri}?city=Брест";

            Uri queryUri = new(QUERY_URL);

            try
            {
                List<JsonElement> jsonData = JsonSerializer.Deserialize<List<JsonElement>>(new WebClient().DownloadString(queryUri));

                return jsonData[0].Deserialize<Dictionary<string, string>>()[$"{currencyCode}_{ConvertTypeOperation(typeOperation)}"];
            }
            catch (WebException)
            {
                return "Not defined";
            }
            catch (KeyNotFoundException)
            {
                return "Not defined";
            }
        }
    }
}
