using ProjectX.Models;

namespace ProjectX.ViewModels
{
    public class UserQuestionViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Users User { get; set; }
        public List<UserAnswerViewModel> Answers { get; set; }
        public int Score { get; set; }
        public List<SavedQuestion> SavedQuestions { get; set; }
    }
}
