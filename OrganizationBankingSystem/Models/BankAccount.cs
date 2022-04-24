using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Models
{
    public class BankAccount
    {
        [Column("BankId")]
        public Guid Id { get; set; }

        public DateTime DateCreateBankAccount { get; set; }

        public long Balance { get; set; }

        [ForeignKey(nameof(User))]
        public User Owner { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}