using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class BankAccountStatus
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName="money")]
        public decimal Balance { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
