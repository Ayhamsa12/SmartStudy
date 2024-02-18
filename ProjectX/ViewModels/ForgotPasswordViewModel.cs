using System.ComponentModel.DataAnnotations;

namespace ProjectX.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
