using ProjectX.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectX.ViewModels
{
    public class RequestSubjectViewModel
    {
        public int SubjectId { get; set; }


        [Required(ErrorMessage = "Please enter a subject name")]
        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }
        public List<Galaxsy> Requests { get; set; }
    }
}
