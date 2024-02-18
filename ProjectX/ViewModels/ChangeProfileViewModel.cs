using System.ComponentModel.DataAnnotations;

namespace ProjectX.ViewModels
{
    public class ChangeProfileViewModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public IFormFile? ProfilePicture { get; set; }
    }
}
