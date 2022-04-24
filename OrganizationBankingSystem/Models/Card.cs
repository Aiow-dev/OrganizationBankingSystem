using System;
using System.ComponentModel.DataAnnotations.Schema;

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