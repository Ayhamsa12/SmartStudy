using ProjectX.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectX.ViewModels
{
    public class GalaxsyViewModel
    {
        public int GalaxsyId { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Choase a Department Name")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please Choase a Subject Name")]
        public int SubjectId { get; set; }

        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsApproved { get; set; }
        public virtual Category Category { get; set; }
        public virtual Users User { get; set; }
        public List<Category> Categories { get; set; }
        public List<Subject> Subjects { get; set; }
        public IFormFile AttachmentFile { get; set; }
    }
}
