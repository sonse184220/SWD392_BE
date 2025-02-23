using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly IDestinationRepository _repository;

        public DestinationService(IDestinationRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Destination>> GetAllDestinationsAsync() => _repository.GetAllDestinationsAsync();

        public Task<Destination> GetDestinationByIdAsync(int id) => _repository.GetDestinationByIdAsync(id);

        public Task<List<Destination>> SearchDestinationsAsync(string query) => _repository.SearchDestinationsAsync(query);

        public Task AddDestinationAsync(Destination destination) => _repository.AddDestinationAsync(destination);

        public Task UpdateDestinationAsync(Destination destination) => _repository.UpdateDestinationAsync(destination);

        public Task DeleteDestinationAsync(int id) => _repository.DeleteDestinationAsync(id);
    }
}
