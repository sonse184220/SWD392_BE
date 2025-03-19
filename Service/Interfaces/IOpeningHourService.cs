using CityScout.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IOpeningHourService
    {
        Task<List<OpeningHourDto>> GetAllAsync();
        Task<List<OpeningHourDto>> GetByDestinationIdAsync(string destinationId);
        Task<bool> CreateAsync(OpeningHourDto dto);
        Task<bool> UpdateAsync(OpeningHourDto dto);
        Task<bool> DeleteAsync(string destinationId, string dayOfWeek);
    }
}
