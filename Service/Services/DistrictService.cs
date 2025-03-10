using CityScout.Repositories;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityScout.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRepository;

        public DistrictService(IDistrictRepository districtRepository)
        {
            _districtRepository = districtRepository;
        }

        public async Task<List<District>> GetAllAsync()
            => await _districtRepository.GetAllAsync();

        public async Task<District> GetByIdAsync(int id)
            => await _districtRepository.GetByIdAsync(id);

        public async Task<int> CreateAsync(District district)
            => await _districtRepository.CreateAsync(district);

        public async Task<int> UpdateAsync(District district)
            => await _districtRepository.UpdateAsync(district);

        public async Task<bool> RemoveAsync(int id)
            => await _districtRepository.RemoveAsync(id);
    }
}
