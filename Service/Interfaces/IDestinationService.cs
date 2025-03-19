using CityScout.DTOs;
using Repository.Models;

namespace CityScout.Services
{
    public interface IDestinationService
    {
        Task<List<Destination>> GetAllAsync();
        Task<Destination> GetByIdAsync(string id);
        Task<string> CreateAsync(DestinationCreateDto dto,string imageUrl);
        Task UpdateAsync(string id, DestinationCreateDto dto,string imageUrl);
        Task<bool> RemoveAsync(string id);

        Task<List<Destination>> SearchDestinationsByNameAsync(string name);

    }
}
