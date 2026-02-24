using ecomerce1.Models;
using ecomerce1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
//this is my comment !!!
// this is another comment with Hamzahhhhhh
namespace ecomerce1.Controllers
{
    public class accountController : Controller
    {
        private readonly UserManager<applicationUser> userManager;
        private readonly SignInManager<applicationUser> signInManager;
        private readonly IConfiguration configuration;

        public accountController(UserManager<applicationUser> userManager,
            SignInManager<applicationUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }
        public IActionResult Register()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(registerDto registerDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }
            //Create a new account and authenticate the user
            var user = new applicationUser()
            {
                firstName = registerDto.firstName,
                secondName = registerDto.secondName,
                UserName = registerDto.email,
                Email = registerDto.email,
                PhoneNumber = registerDto.phoneNumber,
                address = registerDto.address,
                createdAt = DateTime.Now
            };

            var result = await userManager.CreateAsync(user, registerDto.password);

            if (result.Succeeded)
            {
                //successful user registration
                await userManager.AddToRoleAsync(user, "client");

                //sign in the new user
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");

            }

            //register failed => show registeration errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerDto);
        }

        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var reslut = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password
                , loginDto.RememberMe, false);

            if (reslut.Succeeded )
            { 
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErroMessage = "Invalid login attempt";
            }



            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var profileDto = new ProfileDto()
            {
                firstName = appUser.firstName,
                secondName = appUser.secondName,
                email = appUser.Email ?? "",
                phoneNumber = appUser.PhoneNumber,
                address = appUser.address,

            };
            return View(profileDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill all the required fields with valid values";
                return View(profileDto);
            }

            //Get the current user 
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }


            //Update the user profile 
            appUser.firstName = profileDto.firstName;
            appUser.secondName = profileDto.secondName;
            appUser.UserName = profileDto.email;
            appUser.Email = profileDto.email;
            appUser.PhoneNumber = profileDto.phoneNumber;
            appUser.address = profileDto.address;

            var result = await userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Profile updated successfully";
            }
            else
            {
                ViewBag.ErrorMessage = "unable to update the profile: " + result.Errors.First().Description;
            }


            return View(profileDto);
        }

        [Authorize]
        public IActionResult Password()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Password(PasswordDto passwordDto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //Get the current user 
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //update the password 
            var result = await userManager.ChangePasswordAsync(appUser,
                passwordDto.Currentpassword, passwordDto.Newpassword);

            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password updated Successfully";
            }
            else
            {
                ViewBag.ErrorMessage = "Error: " + result.Errors.First().Description;
            }

            return View(passwordDto);
        }




        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Email = email;
            if (!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email address!";
                return View();
            }
            var user = await userManager.FindByEmailAsync(email);
           
            if (user != null)
            {
                //Generate password rest token
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                string resetUrl = Url.ActionLink("ResetPassword", "Account", new { token }) ?? "URL Error";
                //Console.WriteLine("password rest token: " + resetUrl);
                //send Url by Email
                string senderName = configuration["BrevoSettings:SenderName"] ?? "";
                string senderEmail = configuration["BrevoSettings:SenderEmail"] ?? "";
                string username = user.firstName + " " + user.secondName;
                string subject = "Password Reset";
                string message = "Dear " + username + ", \n \n" +
                                 "You can Reset Your password using the following link: \n \n" +
                                 resetUrl + "\n \n" + "Best Regards";


                EmailSender.SendEmail(senderName, senderEmail, username, email, message, subject);

                ViewBag.SuccessMessage = "please check your email and click on the passwird Rest Link";
            }
            else
            {
                ViewBag.EmailError = "please enter a registerd email";
            }


            return View();

        }

        public IActionResult ResetPassword(string? token)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string? token, PasswordResetDto model)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid) 
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "This Email not Valid!" ;
                return View(model);
            }
            var result = await userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password Reset Successfully !"; 
            }
            else
            {
                foreach (var error  in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

                return View(model);
        }



    }
}