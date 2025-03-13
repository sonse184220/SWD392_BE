using CityScout.DTOs;
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

        public async Task<City> GetByIdAsync(string id)
            => await _cityRepository.GetByIdAsync(id);

        public async Task<string> CreateAsync(CityCreateDto dto)
        {
            var city = new City
            {
                CityId = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description
            };
            return await _cityRepository.CreateAsync(city);
        }

        public async Task UpdateAsync(string id, CityCreateDto dto)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
                throw new Exception("City not found");

            city.Name = dto.Name;
            city.Description = dto.Description;

            await _cityRepository.UpdateAsync(city);
        }

        public async Task<bool> RemoveAsync(string id)
            => await _cityRepository.RemoveAsync(id);
    }
}