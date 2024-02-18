using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ProjectX.Models;

namespace ProjectX.Data
{

    public class ApplicationDbContext : IdentityDbContext<Users>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        {
            
        }

       
        public DbSet<Category> Categories { get; set; }        
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Reference> References { get; set; }
        
        public DbSet<UserQuestion> UserQuestions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<SavedQuestion> SavedQuestions { get; set; }
        public DbSet<Galaxsy> galaxsies { get; set; }
        public DbSet<UserQuestionVote> userQuestionVotes { get; set; }
    }
}
