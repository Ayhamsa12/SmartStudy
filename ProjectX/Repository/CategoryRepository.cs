using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;

namespace ProjectX.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            this._context = context;
        }
        public bool Add(Category category)
        {
            _context.Add(category);
            return Save();
        }

        public bool Delete(Category category)
        {
            _context.Remove(category);
            return Save();

        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }



        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(i => i.CategoryId == id);
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool Update(Category category)
        {
            _context.Update(category);
            return Save();
        }


    }
}

