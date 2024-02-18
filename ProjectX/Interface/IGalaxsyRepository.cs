using ProjectX.Models;

namespace ProjectX.Interface
{
    public interface IGalaxsyRepository
    {
        Task<IEnumerable<Galaxsy>> GetAll();
        Task<Galaxsy> GetByIdAsync(int id);
        bool Add(Galaxsy galaxsy);
        bool Update(Galaxsy galaxsy);
        bool Delete(Galaxsy galaxsy);
        bool Save();
        IEnumerable<Galaxsy> GetSummariesBySubject(int subjectId);
    }
}
