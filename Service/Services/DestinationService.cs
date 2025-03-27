using CityScout.Repositories;
using Repository.Models;
using Repository.ViewModels;

namespace CityScout.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly IDestinationRepository _destinationRepository;

        public DestinationService(IDestinationRepository destinationRepository)
        {
            _destinationRepository = destinationRepository;
        }

        public async Task<List<Destination>> GetAllAsync()
            => await _destinationRepository.GetAllAsync();

        public async Task<Destination> GetByIdAsync(string id)
            => await _destinationRepository.GetByIdAsync(id);

        public async Task<string> CreateAsync(DestinationCreateDto dto,string imageUrl)
        {
            var destination = new Destination
            {
                DestinationId = Guid.NewGuid().ToString(),
                DestinationName = dto.DestinationName,
                Address = dto.Address,
                Description = dto.Description,
                Rate = dto.Rate,
                CategoryId = dto.CategoryId,
                Ward = dto.Ward,
                Status = dto.Status,
                DistrictId = dto.DistrictId,
                ImageUrl = imageUrl
            };
            return await _destinationRepository.CreateAsync(destination);
        }

        public async Task UpdateAsync(string id, DestinationCreateDto dto,string imageUrl)
        {
            var destination = await _destinationRepository.GetByIdAsync(id);
            if (destination == null)
                throw new Exception("Destination not found");

            destination.DestinationName = dto.DestinationName;
            destination.Address = dto.Address;
            destination.Description = dto.Description;
            destination.Rate = dto.Rate;
            destination.CategoryId = dto.CategoryId;
            destination.Ward = dto.Ward;
            destination.Status = dto.Status;
            destination.DistrictId = dto.DistrictId;
            destination.ImageUrl = imageUrl;

            await _destinationRepository.UpdateAsync(destination);
        }

        public async Task<bool> RemoveAsync(string id)
            => await _destinationRepository.RemoveAsync(id);

        public async Task<List<Destination>> SearchDestinationsByNameAsync(string name)
        {
            return await _destinationRepository.SearchDestinationsByNameAsync(name);
        }

    }
}
