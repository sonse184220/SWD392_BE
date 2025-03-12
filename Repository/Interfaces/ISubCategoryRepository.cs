using Repository.Models;

namespace CityScout.Repositories
{
    public interface ISubCategoryRepository
    {
        Task<List<SubCategory>> GetAllAsync();
        Task<SubCategory> GetByIdAsync(string id);
        Task<string> CreateAsync(SubCategory subCategory);
        Task UpdateAsync(SubCategory subCategory);
        Task<bool> RemoveAsync(string id);
    }
}
