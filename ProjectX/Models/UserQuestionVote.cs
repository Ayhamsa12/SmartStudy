namespace ProjectX.Models
{
    public class UserQuestionVote
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuestionId { get; set; }
        public bool IsUpvote { get; set; }
    }
}
