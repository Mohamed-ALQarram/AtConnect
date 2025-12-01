using System.ComponentModel.DataAnnotations;

namespace AtConnect.DTOs
{
    public class LoginRequest
    {
        [Required, MinLength(3), MaxLength(50)]
        public string UserNameOrEmail { get; set; }= null!;
        [Required, MinLength(6), MaxLength(50)]
        public string Password { get; set; }= null!;
    }
}
