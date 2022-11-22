using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationBankingSystem.MVVM.Model
{
    [Table("FavoriteCourses")]
    public class FavoriteCourse
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName="char(3)")]
        public string FromCurrencyCode { get; set; }

        [Required]
        [Column(TypeName="char(3)")]
        public string ToCurrencyCode { get; set; }

        public int BankUserId { get; set; }
        public BankUser BankUser { get; set; }
    }
}
