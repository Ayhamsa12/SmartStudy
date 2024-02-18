using ProjectX.Models;

namespace ProjectX.ViewModels
{
    public class SubjectDetailViewModel
    {
        public Subject Subject { get; set; }
        public IEnumerable<Galaxsy> Summaries { get; set; }
        public IEnumerable<Reference> References { get; set; }
        
    }
}
