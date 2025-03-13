using Repository.Models;
using CityScout.DTOs;

namespace CityScout.Services
{
    public interface ISubCategoryService
    {
        Task<List<SubCategory>> GetAllAsync();
        Task<SubCategory> GetByIdAsync(string id);
        Task<string> CreateAsync(SubCategoryCreateDto dto);
        Task UpdateAsync(string id, SubCategoryCreateDto dto);
        Task<bool> RemoveAsync(string id);
    }
}
