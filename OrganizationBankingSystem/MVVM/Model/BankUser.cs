using System.ComponentModel.DataAnnotations;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class BankUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public User Profile { get; set; }
    }
}
