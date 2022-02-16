using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class Card
    { 
        [Column("CardId")]
        public Guid Id { get; set; }

        [ForeignKey(nameof(BankAccount))]
        public BankAccount bankAccount { get; set; }
    }
}
