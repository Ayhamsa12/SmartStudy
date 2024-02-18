using System.ComponentModel.DataAnnotations;

namespace ProjectX.Models
{
    public class Reference
    {
        [Key]
        public int ReferenceId { get; set; }
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public string YouTubePlaylist { get; set; }
        public Subject Subject { get; set; }
    }
}
