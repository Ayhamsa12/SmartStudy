namespace ProjectX.Models
{
    public class SavedQuestion
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int UserQuestionId { get; set; }

        // Navigation properties
        public Users User { get; set; }
        public UserQuestion UserQuestion { get; set; }
    }
}
