namespace user.Models
{
    public class User
    {
        public string Id {get; set;}
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
