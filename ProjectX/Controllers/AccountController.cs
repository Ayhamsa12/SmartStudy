using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Attributes;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;
using ProjectX.Services;
using ProjectX.ViewModels;
using System.Reflection.Metadata.Ecma335;

namespace ProjectX.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManeger;
        private readonly SignInManager<Users> _signInManeger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ICloudService _cloudService;
        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager, ApplicationDbContext context, IEmailSender emailSender, ICloudService cloudService)
        {
            this._userManeger = userManager;
            this._signInManeger = signInManager;
            this._context = context;
            this._emailSender = emailSender;
            this._cloudService = cloudService;
        }
        [RedirectToHomeIfLoggedIn]
        public IActionResult Login()
        {
            ViewBag.RegisterViewModel = new RegisterViewModel();
            ViewBag.LoginViewModel = new LoginViewModel();
            return View();
        }
        [RedirectToHomeIfLoggedIn]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManeger.FindByEmailAsync(model.Email);
            if (user != null && await _userManeger.IsEmailConfirmedAsync(user))
            {
                var passwordCheck = await _userManeger.CheckPasswordAsync(user, model.Password);
                if (passwordCheck)
                {
                    var result = await _signInManeger.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                TempData["Error"] = "wrong email or password try again ";
                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "You must confirm your email address before logging in.");
            }
           
            return View(model);
        }

        [RedirectToHomeIfLoggedIn]
        public IActionResult Register()
        {
            ViewBag.RegisterViewModel = new RegisterViewModel();
            ViewBag.LoginViewModel = new LoginViewModel();
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [RedirectToHomeIfLoggedIn]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return View(registerViewModel);

            var user = await _userManeger.FindByEmailAsync(registerViewModel.Email);
            var username = await _userManeger.FindByNameAsync(registerViewModel.UserName);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use.";
                return View(registerViewModel);
            }
            if (username != null)
            {
                TempData["Error"] = "This UserName is already in use.";
                return View(registerViewModel);
            }
            var newUser = new Users()
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email,
            };

            var newUserResponse = await _userManeger.CreateAsync(newUser, registerViewModel.Password);
            
            if (newUserResponse.Succeeded)
            {
                var addToRole = await _userManeger.AddToRoleAsync(newUser, UserRoles.User);
                // Generate an email confirmation token
                var emailConfirmationToken = await _userManeger.GenerateEmailConfirmationTokenAsync(newUser);

                // Construct the confirmation link
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = newUser.Id, token = emailConfirmationToken }, Request.Scheme);

                // Send confirmation email using IEmailSender
                await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email", $"Please confirm your account by clicking this link:  <a href = '{confirmationLink}' > Confirm Email</a>");
               
                TempData["RegisterSuccess"] = "Registration successful. Please check your email for confirmation.";

                return RedirectToAction("Login", "Account");
            }

            // If registration fails, show errors
            foreach (var error in newUserResponse.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerViewModel);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                // Handle invalid token or user ID
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManeger.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManeger.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View("EmailConfirmed");
            }
            else
            {
                // Handle errors, e.g., invalid token
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManeger.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Saved()
        {
            var user = await _userManeger.GetUserAsync(User);

            var savedQuestions = _context.SavedQuestions
                .Where(sq => sq.UserId == user.Id)
                .Include(sq => sq.UserQuestion)
                .ToList();

            return View(savedQuestions);
        }
        public async Task<IActionResult> ProfileUser()
        {
            var user = await _userManeger.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManeger.GetUserId(User)}'.");
            }
            var userProfileViewModel = new UserProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture
            };
            return View("ProfileUser", userProfileViewModel);
        }
        //Change password
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userManeger.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManeger.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userManeger.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManeger.RefreshSignInAsync(user);

            var userProfileViewModel = new UserProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email

            };

            return View("ProfileUser", userProfileViewModel);
        }

        //Add Admin

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddAdmin()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAdmin(AddAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newUser = new Users
            {
                UserName = model.UserName,
                Email = model.Email,

            };

            var result = await _userManeger.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                newUser.EmailConfirmed = true;
                var addToRoleResult = await _userManeger.AddToRoleAsync(newUser, UserRoles.Admin);

                if (addToRoleResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in addToRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                // Handle user creation failure
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManeger.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangeProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email
            };

            // Pass the old photo URL to the view
            ViewData["OldPhotoUrl"] = user.ProfilePicture;

            return View(model);
        }
        //==========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ChangeProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManeger.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Store the old profile picture URL
            var oldPhotoUrl = user.ProfilePicture;

            // Handle profile picture upload
            if (model.ProfilePicture != null)
            {
                // Upload the new profile picture to Cloudinary
                var uploadResult = await _cloudService.AddPhotoAsync(model.ProfilePicture);

                if (uploadResult.Error == null)
                {
                    // Save the new profile picture URL
                    user.ProfilePicture = uploadResult.Url.ToString();

                    // Delete the old photo from Cloudinary
                    if (!string.IsNullOrEmpty(oldPhotoUrl))
                    {
                        var publicId = ExtractPublicIdFromUrl(oldPhotoUrl);
                        await _cloudService.RemoveImageAsync(publicId);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Error uploading profile picture: {uploadResult.Error.Message}");
                    return View(model);
                }
            }

            // Update other user properties (e.g., username, email)
            user.UserName = model.UserName;
            // Do not update the email if you want to keep it the same
            // user.Email = model.Email;

            // Update the user
            var result = await _userManeger.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction("ProfileUser");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            // Extract the public ID from the Cloudinary image URL
            var publicIdIndex = imageUrl.LastIndexOf('/') + 1;
            var publicId = imageUrl.Substring(publicIdIndex);
            return publicId;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManeger.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManeger.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var resetToken = await _userManeger.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "Account", new { code = resetToken, email = user.Email }, protocol: HttpContext.Request.Scheme);

            // Send the password reset link to the user's email
            await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>Reset password link</a>");

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null, string email = null)
        {
            if (code == null || email == null)
            {
                return View("Error");
            }

            var model = new ResetPasswordViewModel
            {
                Code = code,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManeger.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManeger.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}