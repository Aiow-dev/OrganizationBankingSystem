using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class Transfer
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName="smallmoney")]
        public decimal Amount { get; set; }

        public TransferSender TransferSender { get; set; }
        public TransferReceiver TransferReceiver { get; set; }
    }
}
