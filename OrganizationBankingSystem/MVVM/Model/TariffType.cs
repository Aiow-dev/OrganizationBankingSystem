using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class TariffType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(35)]
        public string TariffName { get; set; }

        [Required]
        [Column(TypeName="smallmoney")]
        public decimal TariffPrice { get; set; }

        [Required]
        public int MonthPeriod { get; set; }

        public List<Tariff> Tariffs { get; set; }
    }
}
