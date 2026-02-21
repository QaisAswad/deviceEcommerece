using ecomerce1.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ecomerce1.Services
{
    public class dataBaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<applicationUser>? userManager,
         RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }

            //check if we have the admin role or not 
            var exist = await roleManager.RoleExistsAsync("Admin");
            if(!exist)
            {
                Console.WriteLine("Admin role is not exist and will be created");
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            //check if we have the seller role or not 
             exist = await roleManager.RoleExistsAsync("seller");
            if (!exist)
            {
                Console.WriteLine("seller role is not exist and will be created");
                await roleManager.CreateAsync(new IdentityRole("seller"));
            }

            //check if we have the client role or not 
            exist = await roleManager.RoleExistsAsync("client");
            if (!exist)
            {
                Console.WriteLine("client role is not exist and will be created");
                await roleManager.CreateAsync(new IdentityRole("client"));
            }

            //check if we have at least one Admin user or not 
            var adminUser = await userManager.GetUsersInRoleAsync("admin");
            if (adminUser.Any())
            {
                // Admin user already exist 
                Console.WriteLine("Admin user already exists => exit");
                return;
            }

            //Create the admin user 
            var user = new applicationUser()
            {
                firstName = "Qais",
                secondName = "Admin",
                UserName = "qaisaswad@outlook.com",
                Email = "qaisaswad@outlook.com",
                createdAt = DateTime.Now,

            };

            string initialpassword = "admin123";

            var result = await userManager.CreateAsync(user , initialpassword);
            if (result.Succeeded)
            {
                //set the user role 
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin user created successfully! please update the initial password ");
                Console.WriteLine("Email:" + user.Email);
                Console.WriteLine("Initial password: " + initialpassword);
            }
        }
    }
}
