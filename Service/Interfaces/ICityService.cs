using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
  public  interface ICityService
    {
        Task<List<City>> GetAllAsync();
        Task<City> GetByIdAsync(int id);
        Task<int> CreateAsync(City city);
        Task<int> UpdateAsync(City city);
        Task<bool> RemoveAsync(int id);
    }
}
