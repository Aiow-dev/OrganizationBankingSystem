namespace OrganizationBankingSystem.Core.Converters
{
    public static class CurrencyValueConverter
    {
        public static double ConvertCurrencyValues(double numberUnit, double unitCost)
        {
            return numberUnit * unitCost;
        }
    }
}
