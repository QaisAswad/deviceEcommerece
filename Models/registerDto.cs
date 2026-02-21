using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ecomerce1.Models
{
    public class registerDto
    {
        [Required(ErrorMessage ="The First Name field is required"),MaxLength(100)]
        public string firstName {  get; set; } = "";
        [Required(ErrorMessage = "The last Name field is required"), MaxLength(100)]
        public string secondName { get; set; } = "";
        [Required, EmailAddress, MaxLength(100)]
        public string email { get; set; } = "";
        [Phone(ErrorMessage ="The format of the phone number is not valid"),MaxLength(20)]
        public string? phoneNumber { get; set; } = "";
        [Required,MaxLength(200)]
        public string address { get; set; } = "";
        [Required,MaxLength(100)]
        public string password { get; set; } = "";
        [Required(ErrorMessage = "The confirm Password field is required")]
        [Compare("password", ErrorMessage="confirm Password and password do not Match")]
        public string confirmPassword { get; set; } = "";
    }
}
