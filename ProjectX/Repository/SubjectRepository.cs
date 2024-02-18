using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;

namespace ProjectX.Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _context;
        public SubjectRepository(ApplicationDbContext context)
        {
            this._context = context;
        }
        public bool Add(Subject subject)
        {
            _context.Add(subject);
            return Save();
        }

        public bool Delete(Subject subject)
        {
            _context.Remove(subject);
            return Save();

        }

        public async Task<IEnumerable<Subject>> GetAll()
        {
            return await _context.Subjects.ToListAsync();
        }



        public async Task<Subject> GetByIdAsync(int id)
        {
            return await _context.Subjects.FirstOrDefaultAsync(i => i.SubjectId == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges() ;
            return saved > 0;
            
        }

        public bool Update(Subject subject)
        {
            _context.Update(subject);
            return Save();
        }
        public IEnumerable<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
        public async Task<IEnumerable<Subject>> GetSubjectsByCategoryAsync(int categoryId)
        {
            return await _context.Subjects
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
