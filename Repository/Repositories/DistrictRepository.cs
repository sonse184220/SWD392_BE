﻿using PsyHealth.Repositories.Base;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityScout.Repositories
{
    public class DistrictRepository : GenericRepository<District>, IDistrictRepository
    {
        public DistrictRepository(CityScoutContext context) : base(context) { }

        public async Task<List<District>> GetAllAsync()
            => await base.GetAllAsync();

        public async Task<District> GetByIdAsync(string id)
            => await base.GetByIdAsync(id);

        public async Task<string> CreateAsync(District district)
        {
            await base.CreateAsync(district);
            return district.DistrictId;
        }

        public async Task UpdateAsync(District district)
            => await base.UpdateAsync(district);

        public async Task<bool> RemoveAsync(string id)
        {
            var district = await GetByIdAsync(id);
            if (district == null) return false;

            return await base.RemoveAsync(district);
        }
    }
}