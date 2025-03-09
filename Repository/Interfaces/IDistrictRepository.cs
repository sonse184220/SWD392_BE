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
        Task<District> GetByIdAsync(int id);
        Task<int> CreateAsync(District district);
        Task<int> UpdateAsync(District district);
        Task<bool> RemoveAsync(int id);
    }
}