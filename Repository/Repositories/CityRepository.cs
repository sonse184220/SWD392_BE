using PsyHealth.Repositories.Base;
using Repository.Interfaces;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
  public class CityRepository :GenericRepository<City>, ICityRepository
    {
        public CityRepository(CityScoutContext context) : base(context) { }

        public async Task<List<City>> GetAllAsync()
            => await base.GetAllAsync();

        public async Task<City> GetByIdAsync(int id)
            => await base.GetByIdAsync(id);

        public async Task<int> CreateAsync(City city)
            => await base.CreateAsync(city);

        public async Task<int> UpdateAsync(City city)
            => await base.UpdateAsync(city);

        public async Task<bool> RemoveAsync(int id)
        {
            var city = await GetByIdAsync(id);
            if (city == null) return false;

            return await base.RemoveAsync(city);
        }
        }
}
