using System.ComponentModel.DataAnnotations;

namespace ProjectX.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name="Email Address")]

        [Required(ErrorMessage = "Email address is requaired ")]
       
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }
}
