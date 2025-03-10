using CityScout.Repositories;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityScout.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync()
            => await _categoryRepository.GetAllAsync();

        public async Task<Category> GetByIdAsync(int id)
            => await _categoryRepository.GetByIdAsync(id);

        public async Task<int> CreateAsync(Category category)
            => await _categoryRepository.CreateAsync(category);

        public async Task<int> UpdateAsync(Category category)
            => await _categoryRepository.UpdateAsync(category);

        public async Task<bool> RemoveAsync(int id)
            => await _categoryRepository.RemoveAsync(id);
    }
}