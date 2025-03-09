using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityScout.Services
{
    public interface ISubCategoryService
    {
        Task<List<SubCategory>> GetAllAsync();
        Task<SubCategory> GetByIdAsync(int id);
        Task<int> CreateAsync(SubCategory subCategory);
        Task<int> UpdateAsync(SubCategory subCategory);
        Task<bool> RemoveAsync(int id);
    }
}