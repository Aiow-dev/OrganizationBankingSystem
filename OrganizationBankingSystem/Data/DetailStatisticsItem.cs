namespace OrganizationBankingSystem.Data
{
    public class DetailStatisticsItem
    {
        public int NumberOfDay { get; set; }

        public double OpenValueCurrency { get; set; }

        public double MinValueCurrency { get; set; }

        public double MaxValueCurrency { get; set; }

        public double CloseValueCurrency { get; set; }

        public string DateCurrency { get; set; }
    }
}