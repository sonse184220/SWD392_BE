using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class DestinationRepository : IDestinationRepository
    {
        private readonly CityScoutContext _context;

        public DestinationRepository(CityScoutContext context)
        {
            _context = context;
        }

        public async Task<List<Destination>> GetAllDestinationsAsync()
        {
            return await _context.Destinations.AsNoTracking().ToListAsync();
        }

        public async Task<Destination> GetDestinationByIdAsync(string id)
        {
            return await _context.Destinations.AsNoTracking().FirstOrDefaultAsync(d => d.DestinationId == id);
        }

        public async Task<List<Destination>> SearchDestinationsAsync(string query)
        {
            return await _context.Destinations.AsNoTracking()
                .Where(d => d.DestinationName.Contains(query) || d.Address.Contains(query))
                .ToListAsync();
        }

        public async Task AddDestinationAsync(Destination destination)
        {
            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDestinationAsync(Destination destination)
        {
            _context.Destinations.Update(destination);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDestinationAsync(string id)
        {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination != null)
            {
                _context.Destinations.Remove(destination);
                await _context.SaveChangesAsync();
            }
        }
    }
}
