using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
   public interface ICityRepository
    {
        Task<List<City>> GetAllAsync();
        Task<City> GetByIdAsync(string id);
        Task<string> CreateAsync(City city);
        Task UpdateAsync(City city);
        Task<bool> RemoveAsync(string id);
    }
}
