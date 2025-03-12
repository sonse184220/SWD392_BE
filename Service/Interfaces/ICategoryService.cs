using Repository.Models;
using CityScout.DTOs;

namespace CityScout.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(string id);
        Task<string> CreateAsync(CategoryCreateDto dto);
        Task UpdateAsync(string id, CategoryCreateDto dto);
        Task<bool> RemoveAsync(string id);
    }
}
