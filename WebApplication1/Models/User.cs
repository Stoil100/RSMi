namespace WebApplication1.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }  // Added field
        public string? DisplayName { get; set; }  // Added field
        public bool IsAdmin { get; set; }  // Added field
    }
}
