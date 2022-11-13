namespace BankSystemModel
{
    public class BankUser
    {
        public int BankUserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public User User { get; set; }
    }
}
