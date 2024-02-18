using System.ComponentModel.DataAnnotations;

namespace ProjectX.Models
{
    public class UserQuestion
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please provide a title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please provide content")]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Score { get; set; }
        public Users User { get; set; }
        public List<UserAnswer> Answers { get; set; }
        public List<SavedQuestion> SavedQuestions { get; set; }
    }
}
