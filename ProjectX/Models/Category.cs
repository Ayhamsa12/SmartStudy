using System.ComponentModel.DataAnnotations;

namespace ProjectX.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }

        public ICollection<Subject> Subjects { get; set; }
    }
}
