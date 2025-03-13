using Repository.Models;

namespace CityScout.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(string id);
        Task<string> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task<bool> RemoveAsync(string id);
    }
}
