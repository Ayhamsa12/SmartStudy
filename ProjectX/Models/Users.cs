using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectX.Models
{
    public class Users : IdentityUser
    {

        public string? ProfilePicture { get; set; }
        public List<SavedQuestion> SavedQuestions { get; set; }
        public List<UserQuestion> AskedQuestions { get; set; }


    }
}
