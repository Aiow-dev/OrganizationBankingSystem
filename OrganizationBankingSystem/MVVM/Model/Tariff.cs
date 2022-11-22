using System;
using System.ComponentModel.DataAnnotations;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class Tariff
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public int TariffTypeId { get; set; }
        public TariffType TariffType { get; set; }

        public BankCard BankCard { get; set; }
    }
}
