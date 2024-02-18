using ProjectX.Models;

namespace ProjectX.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetByIdAsync(int id);
        bool Add(Category category);
        bool Update(Category category);
        bool Delete(Category category);
        bool Save();
    }
}
