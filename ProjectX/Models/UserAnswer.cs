using System.ComponentModel.DataAnnotations;

namespace ProjectX.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int UserQuestionId { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please provide a reply")]
        public string ReplyContent { get; set; }
        public DateTime CreatedAt { get; set; }

        public Users User { get; set; }
        public UserQuestion UserQuestion { get; set; }

    }
}
