using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectX.ViewModels
{
    public class RegisterViewModel
    {
        public string UserName { get; set; }
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is required")]
        public string Email { get; set; }
        //public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]+$", ErrorMessage = "The password must have at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
