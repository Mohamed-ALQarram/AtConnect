using System.ComponentModel.DataAnnotations;

namespace AtConnect.BLL.DTOs
{
    public class RegistrationRequest
    {
        [Required, MinLength(3), MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [Required, MinLength(3), MaxLength(50)]
        public string LastName { get; set; } = null!;
        [Required, MinLength(3), MaxLength(50)]
        public string UserName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

    }
}
