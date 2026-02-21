using System.ComponentModel.DataAnnotations;

namespace ecomerce1.Models
{
    public class PasswordDto
    {
        [Required(ErrorMessage = "The current Password field is required"), MaxLength(100)]
        public string Currentpassword { get; set; } = "";

        [Required(ErrorMessage = "The New Password field is required"), MaxLength(100)]
        public string Newpassword { get; set; } = "";


        [Required(ErrorMessage = "The confirm Password field is required")]
        [Compare("Newpassword", ErrorMessage = "confirm Password and password do not Match")]
        public string confirmPassword { get; set; } = "";
    }
}
