using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Models
{
    public class Operation
    {
        [Column("OperationId")]
        public Guid Id { get; set; }

        public DateTime DateOperation { get; set; }

        public string Description { get; set; }

        public long SumOperation { get; set; }
    }
}