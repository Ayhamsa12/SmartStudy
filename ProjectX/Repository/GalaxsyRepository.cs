using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;

namespace ProjectX.Repository
{
    public class GalaxsyRepository : IGalaxsyRepository
    {
        private readonly ApplicationDbContext _context;

        public GalaxsyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Galaxsy>> GetAll()
        {
            return await _context.galaxsies.ToListAsync();
        }

        public async Task<Galaxsy> GetByIdAsync(int id)
        {
            return await _context.galaxsies.FindAsync(id);
        }

        public bool Add(Galaxsy galaxsy)
        {
            _context.galaxsies.Add(galaxsy);
            return Save();
        }

        public bool Update(Galaxsy galaxsy)
        {
            _context.galaxsies.Update(galaxsy);
            return Save();
        }

        public bool Delete(Galaxsy galaxsy)
        {
            _context.galaxsies.Remove(galaxsy);
            return Save();
        }

        public bool Save()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                // Handle exceptions accordingly
                return false;
            }
        }

        public IEnumerable<Galaxsy> GetSummariesBySubject(int subjectId)
        {
            return _context.galaxsies.Where(g => g.SubjectId == subjectId).ToList();
        }
    }
}
