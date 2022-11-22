using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class BankAccount
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName="char(15)")]
        public string Iban { get; set; }

        [Column(TypeName="money")]
        public decimal Balance { get; set; }

        [Required]
        public DateTime OpenTime { get; set; }

        public int BankUserId { get; set; }
        public BankUser BankUser { get; set; }

        public List<BankAccountStatus> BankAccountStatuses { get; set; }

        public List<BankCard> BankCards { get; set; }

        public List<Credit> Credits { get; set; }

        public List<TransferSender> TransferSenders { get; set; }
        public List<TransferReceiver> TransferReceivers { get; set; }
    }
}
