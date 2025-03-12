using CityScout.Repositories;
using Repository.Models;
using CityScout.DTOs;

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

        public async Task<District> GetByIdAsync(string id)
            => await _districtRepository.GetByIdAsync(id);

        public async Task<string> CreateAsync(DistrictCreateDto dto)
        {
            var district = new District
            {
                DistrictId = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                CityId = dto.CityId
            };
            return await _districtRepository.CreateAsync(district);
        }

        public async Task UpdateAsync(string id, DistrictCreateDto dto)
        {
            var district = await _districtRepository.GetByIdAsync(id);
            if (district == null)
                throw new Exception("District not found");

            district.Name = dto.Name;
            district.Description = dto.Description;
            district.CityId = dto.CityId;

            await _districtRepository.UpdateAsync(district);
        }

        public async Task<bool> RemoveAsync(string id)
            => await _districtRepository.RemoveAsync(id);
    }
}
