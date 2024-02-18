using System.ComponentModel.DataAnnotations;

namespace ProjectX.ViewModels
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is required")]
        public string Email { get; set; }
        //public string PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
