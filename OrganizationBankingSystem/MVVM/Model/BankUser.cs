using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class BankUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        [Required]
        [Column(TypeName="nchar(68)")]
        public string Password { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<FavoriteCourse> FavoriteCourses { get; set; }

        public List<BankAccount> BankAccounts { get; set; }
    }
}
