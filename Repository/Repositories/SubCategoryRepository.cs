using PsyHealth.Repositories.Base;
using Repository.Models;

namespace CityScout.Repositories
{
    public class SubCategoryRepository : GenericRepository<SubCategory>, ISubCategoryRepository
    {
        public SubCategoryRepository(CityScoutContext context) : base(context) { }

        public async Task<List<SubCategory>> GetAllAsync()
            => await base.GetAllAsync();

        public async Task<SubCategory> GetByIdAsync(string id)
            => await base.GetByIdAsync(id);

        public async Task<string> CreateAsync(SubCategory subCategory)
        {
            await base.CreateAsync(subCategory);
            return subCategory.SubCategoryId;
        }

        public async Task UpdateAsync(SubCategory subCategory)
            => await base.UpdateAsync(subCategory);

        public async Task<bool> RemoveAsync(string id)
        {
            var subCategory = await GetByIdAsync(id);
            if (subCategory == null) return false;

            return await base.RemoveAsync(subCategory);
        }
    }
}
