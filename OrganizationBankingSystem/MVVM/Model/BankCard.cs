using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class BankCard
    {
        public int Id { get; set; }

        [Required]
        public DateTime OpenTime { get; set; }

        [Required]
        public DateTime ExpireTime { get; set; }

        [Required]
        [Column(TypeName="nchar(68)")]
        public string CvcCode { get; set; }

        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        public int TariffId { get; set; }
        public Tariff Tariff { get; set; }
    }
}
