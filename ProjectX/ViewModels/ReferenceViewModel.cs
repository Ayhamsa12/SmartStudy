using ProjectX.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectX.ViewModels
{
    public class ReferenceViewModel
    {
        [Required(ErrorMessage = "Please Enter a Reference Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Enter a Url")]
        public string YouTubePlaylist { get; set; }
        public List<Category> Categories { get; set; }
        public List<Subject> Subjects { get; set; }
        [Required(ErrorMessage = "Please Choose a Subject Name")]
        public int SubjectId { get; set; }
        [Required(ErrorMessage = "Please Choose a Department Name")]
        public int CategoryId { get; set; }
    }
}
