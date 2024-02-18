namespace ProjectX.Models
{
    public class Galaxsy
    {
        public int GalaxsyId { get; set; }
        public string Name { get; set; }
        //public int CategoryId { get; set; }
        public int SubjectId { get; set; }

        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsApproved { get; set; }
        //public virtual Category Category { get; set; }
        public virtual Users User { get; set; }
        //public List<Category> Categories { get; set; }
        public List<Subject> Subjects { get; set; }
        public string AttachmentFile { get; set; }
        public virtual Subject Subject { get; set; }

    }
}
