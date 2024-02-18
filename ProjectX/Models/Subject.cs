using System.ComponentModel.DataAnnotations;

namespace ProjectX.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SubjectName { get; set; }

        // Foreign Key relationships
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Reference> References { get; set; }
        

      
    }

}
