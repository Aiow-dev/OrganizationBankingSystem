using System.ComponentModel.DataAnnotations;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(25)]
        public string Patronymic { get; set; }

        [Required]
        public string Phone { get; set; }

        public int BankUserId { get; set; }
        public BankUser BankUser { get; set; }
    }
}
