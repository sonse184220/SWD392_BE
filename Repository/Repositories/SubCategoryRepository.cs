using PsyHealth.Repositories.Base;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityScout.Repositories
{
    public class SubCategoryRepository : GenericRepository<SubCategory>, ISubCategoryRepository
    {
        public SubCategoryRepository(CityScoutContext context) : base(context) { }

        public async Task<List<SubCategory>> GetAllAsync()
            => await base.GetAllAsync();

        public async Task<SubCategory> GetByIdAsync(int id)
            => await base.GetByIdAsync(id);

        public async Task<int> CreateAsync(SubCategory subCategory)
            => await base.CreateAsync(subCategory);

        public async Task<int> UpdateAsync(SubCategory subCategory)
            => await base.UpdateAsync(subCategory);

        public async Task<bool> RemoveAsync(int id)
        {
            var subCategory = await GetByIdAsync(id);
            if (subCategory == null) return false;

            return await base.RemoveAsync(subCategory);
        }
    }
}