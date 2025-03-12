using PsyHealth.Repositories.Base;
using Repository.Models;

namespace CityScout.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(CityScoutContext context) : base(context) { }

        public async Task<List<Category>> GetAllAsync()
            => await base.GetAllAsync();

        public async Task<Category> GetByIdAsync(string id)
            => await base.GetByIdAsync(id);

        public async Task<string> CreateAsync(Category category)
        {
            await base.CreateAsync(category);
            return category.CategoryId;
        }

        public async Task UpdateAsync(Category category)
            => await base.UpdateAsync(category);

        public async Task<bool> RemoveAsync(string id)
        {
            var category = await GetByIdAsync(id);
            if (category == null) return false;

            return await base.RemoveAsync(category);
        }
    }
}
