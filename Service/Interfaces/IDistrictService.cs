using Repository.Models;
using Repository.ViewModels;

namespace CityScout.Services
{
    public interface IDistrictService
    {
        Task<List<District>> GetAllAsync();
        Task<District> GetByIdAsync(string id);
        Task<string> CreateAsync(DistrictCreateDto dto);
        Task UpdateAsync(string id, DistrictCreateDto dto);
        Task<bool> RemoveAsync(string id);
    }
}
