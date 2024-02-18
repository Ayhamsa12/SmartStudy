using ProjectX.Models;

namespace ProjectX.Interface
{
    public interface IReferenceRepository
    {
        Task<IEnumerable<Reference>> GetAll();
        Task<Reference> GetByIdAsync(int id);
        bool Add(Reference reference);
        bool Update(Reference reference);
        bool Delete(Reference reference);
        bool Save();
        IEnumerable<Reference> GetReferencesBySubject(int subjectId);
    }
}
