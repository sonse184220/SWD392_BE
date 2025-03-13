using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityScout.Repositories
{
    public interface IDistrictRepository
    {
        Task<List<District>> GetAllAsync();
        Task<District> GetByIdAsync(string id);
        Task<string> CreateAsync(District district);
        Task UpdateAsync(District district);
        Task<bool> RemoveAsync(string id);
    }
}