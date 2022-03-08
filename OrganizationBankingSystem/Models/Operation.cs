using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
