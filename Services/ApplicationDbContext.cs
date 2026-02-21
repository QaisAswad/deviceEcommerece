using ecomerce1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ecomerce1.Services
{ 
    public class ApplicationDbContext : IdentityDbContext<applicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base (options)
        {
            
        }
      
        public DbSet<product> products  { get; set; }
        public DbSet<Order>   Orders { get; set; }
    }
}
