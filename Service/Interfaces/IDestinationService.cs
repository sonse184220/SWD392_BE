using CityScout.DTOs;
using Repository.Models;

namespace CityScout.Services
{
    public interface IDestinationService
    {
        Task<List<Destination>> GetAllAsync();
        Task<Destination> GetByIdAsync(string id);
        Task<string> CreateAsync(DestinationCreateDto dto);
        Task UpdateAsync(string id, DestinationCreateDto dto);
        Task<bool> RemoveAsync(string id);
    }
}
