using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class Credit
    {
        public int CreditId { get; set; }

        [Required]
        public int Closed { get; set; }

        [Required]
        [Column(TypeName="smallmoney")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime CloseTime { get; set; }

        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
