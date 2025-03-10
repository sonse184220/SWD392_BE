using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<List<City>> GetAllAsync()
            => await _cityRepository.GetAllAsync();

        public async Task<City> GetByIdAsync(int id)
            => await _cityRepository.GetByIdAsync(id);

        public async Task<int> CreateAsync(City city)
            => await _cityRepository.CreateAsync(city);

        public async Task<int> UpdateAsync(City city)
            => await _cityRepository.UpdateAsync(city);

        public async Task<bool> RemoveAsync(int id)
            => await _cityRepository.RemoveAsync(id);
    }
}
