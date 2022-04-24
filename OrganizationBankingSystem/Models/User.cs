using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Entity.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public ICollection<BankAccount> BankAccounts { get; set; }
    }
}