using CityScout.DTOs;
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
        Task<City> GetByIdAsync(string id);
        Task<string> CreateAsync(CityCreateDto dto);
        Task UpdateAsync(string id, CityCreateDto dto);
        Task<bool> RemoveAsync(string id);
    }
}
