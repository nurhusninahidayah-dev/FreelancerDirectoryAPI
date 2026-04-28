using FreelancerDirectory.Domain.Entities;

namespace FreelancerDirectory.Application.Interfaces;

public interface IFreelancerRepository
{
    Task<IEnumerable<Freelancer>> GetAllAsync(bool includeArchived = false);
    Task<Freelancer?> GetByIdAsync(int id);
    Task<int> CreateAsync(Freelancer freelancer);
    Task<bool> UpdateAsync(Freelancer freelancer);
    Task<bool> ArchiveAsync(int id);
    Task<bool> UnarchiveAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Freelancer>> SearchAsync(string keyword);
}