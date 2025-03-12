using CityScout.Repositories;
using Repository.Models;
using CityScout.DTOs;

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

        public async Task<Category> GetByIdAsync(string id)
            => await _categoryRepository.GetByIdAsync(id);

        public async Task<string> CreateAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                CategoryId = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description
            };
            return await _categoryRepository.CreateAsync(category);
        }

        public async Task UpdateAsync(string id, CategoryCreateDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _categoryRepository.UpdateAsync(category);
        }

        public async Task<bool> RemoveAsync(string id)
            => await _categoryRepository.RemoveAsync(id);
    }
}
