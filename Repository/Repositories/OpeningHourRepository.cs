using Microsoft.EntityFrameworkCore;
using PsyHealth.Repositories.Base;
using Repository.Interfaces;
using Repository.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class OpeningHourRepository : GenericRepository<OpeningHour>, IOpeningHourRepository
    {
        public async Task<List<OpeningHour>> GetAllAsync()
        {
            return await _context.OpeningHours.ToListAsync();
        }

        public async Task<List<OpeningHour>> GetByDestinationIdAsync(string destinationId)
        {
            return await _context.OpeningHours
                .Where(oh => oh.DestinationId == destinationId)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(OpeningHour openingHour)
        {
            await _context.OpeningHours.AddAsync(openingHour);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(OpeningHour openingHour)
        {
            var existing = await _context.OpeningHours
                .FirstOrDefaultAsync(oh => oh.DestinationId == openingHour.DestinationId && oh.DayOfWeek == openingHour.DayOfWeek);

            if (existing == null) return false;

            existing.OpenTime = openingHour.OpenTime;
            existing.CloseTime = openingHour.CloseTime;
            existing.IsClosed = openingHour.IsClosed;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(string destinationId, string dayOfWeek)
        {
            var openingHour = await _context.OpeningHours
                .FirstOrDefaultAsync(oh => oh.DestinationId == destinationId && oh.DayOfWeek == dayOfWeek);

            if (openingHour == null) return false;

            _context.OpeningHours.Remove(openingHour);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
