using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;

namespace ProjectX.Repository
{
    public class ReferenceRepository : IReferenceRepository
    {
        private readonly ApplicationDbContext _context;

        public ReferenceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Reference reference)
        {
            _context.Add(reference);
            return Save();
        }

        public bool Delete(Reference reference)
        {
            _context.Remove(reference);
            return Save();

        }

        public async Task<IEnumerable<Reference>> GetAll()
        {

            return await _context.References.ToListAsync();

        }

        public async Task<Reference> GetByIdAsync(int id)
        {
            return await _context.References.FindAsync(id);
        }


        public IEnumerable<Reference> GetReferencesBySubject(int subjectId)
        {
            return _context.References.Where(r => r.SubjectId == subjectId).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;

        }

        public bool Update(Reference reference)
        {
            throw new NotImplementedException();
        }
    }
}
