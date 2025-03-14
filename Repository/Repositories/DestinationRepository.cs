using PsyHealth.Repositories.Base;
using Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace CityScout.Repositories
{
    public class DestinationRepository : GenericRepository<Destination>, IDestinationRepository
    {
        public DestinationRepository(CityScoutContext context) : base(context) { }

        public async Task<List<Destination>> GetAllAsync()
            => await base.GetAllAsync();

        public async Task<Destination> GetByIdAsync(string id)
            => await base.GetByIdAsync(id);

        public async Task<string> CreateAsync(Destination destination)
        {
            await base.CreateAsync(destination);
            return destination.DestinationId;
        }

        public async Task UpdateAsync(Destination destination)
            => await base.UpdateAsync(destination);

        public async Task<bool> RemoveAsync(string id)
        {
            var destination = await GetByIdAsync(id);
            if (destination == null) return false;

            return await base.RemoveAsync(destination);
        }

        public async Task<List<Destination>> SearchDestinationsByNameAsync(string name)
        {
            return await _context.Destinations
                                 .Where(d => d.DestinationName.Contains(name))
                                 .ToListAsync();
        }

    }
}
