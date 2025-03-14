using Repository.Models;

namespace CityScout.Repositories
{
    public interface IDestinationRepository
    {
        Task<List<Destination>> GetAllAsync();
        Task<Destination> GetByIdAsync(string id);
        Task<string> CreateAsync(Destination destination);
        Task UpdateAsync(Destination destination);
        Task<bool> RemoveAsync(string id);
        Task<List<Destination>> SearchDestinationsByNameAsync(string name);

    }
}
