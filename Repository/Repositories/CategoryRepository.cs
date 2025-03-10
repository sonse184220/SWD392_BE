using PsyHealth.Repositories.Base;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityScout.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(CityScoutContext context) : base(context) { }

        public async Task<List<Category>> GetAllAsync()
            => await base.GetAllAsync();

        public async Task<Category> GetByIdAsync(int id)
            => await base.GetByIdAsync(id);

        public async Task<int> CreateAsync(Category category)
            => await base.CreateAsync(category);

        public async Task<int> UpdateAsync(Category category)
            => await base.UpdateAsync(category);

        public async Task<bool> RemoveAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null) return false;

            return await base.RemoveAsync(category);
        }
    }
}