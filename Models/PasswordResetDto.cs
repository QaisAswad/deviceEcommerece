using System.ComponentModel.DataAnnotations;

namespace ecomerce1.Models
{
    public class PasswordResetDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MaxLength(100)]
        public string Password { get; set; } = "";
        [Required(ErrorMessage = "the confim password field is requierd")]
        [Compare("Password", ErrorMessage = "confirm password and origenal password did not mach")]
        public string ConfirmPassword { get; set; } = "";
    }
}
