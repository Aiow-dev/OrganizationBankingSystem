using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace OrganizationBankingSystem.Core.Helpers
{
    public static class BelarusBankHelper
    {
        public static string GetExchangeRates()
        {
            string QUERY_URL = $"{Properties.Settings.Default.belarusBankServiceUri}?city=Брест";

            Uri queryUri = new(QUERY_URL);

            try
            {
                List<JsonElement> jsonData = JsonSerializer.Deserialize<List<JsonElement>>(new WebClient().DownloadString(queryUri));

                return jsonData[0].Deserialize<Dictionary<string, string>>()["USD_in"];
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