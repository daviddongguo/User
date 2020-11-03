namespace user.Dtos
{
    public class UserGetDto
    {
        public string Id {get; set;}
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
