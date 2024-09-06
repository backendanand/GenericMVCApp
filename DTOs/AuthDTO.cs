using GenericMVCApp.Models;

namespace GenericMVCApp.DTOs
{
    public class AuthDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class RegisterDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
