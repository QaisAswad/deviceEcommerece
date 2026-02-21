using Microsoft.AspNetCore.Identity;

namespace ecomerce1.Models
{
    public class applicationUser : IdentityUser
    {
        public string firstName   { get; set; } ="";
        public string secondName  { get; set; } ="";
        public string address { get; set; } = "";
        public DateTime createdAt { get; set; }
    }
}
