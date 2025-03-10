using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityScout.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task<int> CreateAsync(Category category);
        Task<int> UpdateAsync(Category category);
        Task<bool> RemoveAsync(int id);
    }
}