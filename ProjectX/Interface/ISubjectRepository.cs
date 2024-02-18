using ProjectX.Models;

namespace ProjectX.Interface
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAll();
        Task<Subject> GetByIdAsync(int id);
        bool Add(Subject subject);
        bool Update(Subject subject);
        bool Delete(Subject subject);
        bool Save();
        IEnumerable<Category> GetCategories();
        Task<IEnumerable<Subject>> GetSubjectsByCategoryAsync(int categoryId);
    }
}

