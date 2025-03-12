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

        public async Task<City> GetByIdAsync(string id)
            => await base.GetByIdAsync(id);

        public async Task<string> CreateAsync(City city)
        {
            await base.CreateAsync(city);
            return city.CityId;
        }

        public async Task UpdateAsync(City city)
            => await base.UpdateAsync(city);

        public async Task<bool> RemoveAsync(string id)
        {
            var city = await GetByIdAsync(id);
            if (city == null) return false;

            return await base.RemoveAsync(city);
        }
    }
}
