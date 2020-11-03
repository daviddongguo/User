namespace user.Dtos
{
    public class GetUserDto
    {
        public string Id {get; set;}
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
