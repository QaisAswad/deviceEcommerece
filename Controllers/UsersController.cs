using ecomerce1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ecomerce1.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("/Admin/[controller]/{action=Index}/{Id?}")]
    public class UsersController : Controller
    {
        private readonly UserManager<applicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly int pageSize = 5;

        public UsersController(UserManager<applicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index(int? pageIndex)
        {
            IQueryable<applicationUser> query;
            query = userManager.Users.OrderByDescending(u => u.createdAt);
            //Pagination functionality 
            if (pageIndex == null || pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip(((int)pageIndex - 1) * pageSize).Take(pageSize);

            var users = query.ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.PageIndex = pageIndex;

            return View(users);
        }

        public async Task<IActionResult> Details(string? Id)
        {

            if (Id == null)
            {
                return RedirectToAction("Index", "Users");
            }
            var appUser = await userManager.FindByIdAsync(Id);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Users");
            }
            ViewBag.Roles = await userManager.GetRolesAsync(appUser);

            //get available roles :
            var  avaliableRoles = roleManager.Roles.ToList();
            var items = new List<SelectListItem>();
            Console.WriteLine("//////////" + appUser + "/////end");
            foreach (var role in avaliableRoles)
            {
                items.Add
                    (new SelectListItem
                    {
                        Text = role.NormalizedName,
                        Value = role.Name,
                        Selected = await userManager.IsInRoleAsync(appUser, role.Name!),
                    });         
            }
            ViewBag.SelectItems = items;
            return View(appUser);
        }

        public async Task<ActionResult> EditRole(string? id, string? newRole)
        {
            if (id == null || newRole == null)
            {
                return RedirectToAction("Index", "Users");
            }
            var roleExisits = await roleManager.RoleExistsAsync(newRole);
            var appUser = await userManager.FindByIdAsync(id);
            if (appUser == null || !roleExisits)
            {
                  return RedirectToAction("Index", "Users");
            }

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You can't update your own Role!";
                return RedirectToAction("Details", "Users", new { id });
            }
            //update user role 
            var userRoles = await userManager.GetRolesAsync(appUser);
            await userManager.RemoveFromRolesAsync(appUser, userRoles);
            await userManager.AddToRoleAsync(appUser, newRole);

            TempData["SuccessMessage"] = "User Role Updated Successfully";
            return RedirectToAction("Details", "Users", new { id });
           
        }




        public async Task<IActionResult> DeleteAccount (string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var appUser = await userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser!.Id == appUser.Id)
            {

                TempData["ErrorMessage"] = "You can't Delete your own Account!";
                return RedirectToAction("Details", "Users", new { id });
            }

            //Delete User Account 
            var result = await userManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Users");
            }

            TempData["ErrorMessage"] = "unable to delete this Account:" + result.Errors.First().Description;

            return RedirectToAction("Details", "Users", new { id });

        }


    }
}
