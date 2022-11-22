using System.Collections.Generic;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class TransferSender
    {
        public int Id { get; set; }

        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        public int TransferId { get; set; }
        public Transfer Transfer { get; set; }
    }
}
