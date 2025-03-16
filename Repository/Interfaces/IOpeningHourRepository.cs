using Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IOpeningHourRepository
    {
        Task<List<OpeningHour>> GetAllAsync();
        Task<List<OpeningHour>> GetByDestinationIdAsync(string destinationId);
        Task<bool> CreateAsync(OpeningHour openingHour);
        Task<bool> UpdateAsync(OpeningHour openingHour);
        Task<bool> DeleteAsync(string destinationId, string dayOfWeek);
    }
}
