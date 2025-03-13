using CityScout.Repositories;
using Repository.Models;
using CityScout.DTOs;

namespace CityScout.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public SubCategoryService(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<List<SubCategory>> GetAllAsync()
            => await _subCategoryRepository.GetAllAsync();

        public async Task<SubCategory> GetByIdAsync(string id)
            => await _subCategoryRepository.GetByIdAsync(id);

        public async Task<string> CreateAsync(SubCategoryCreateDto dto)
        {
            var subCategory = new SubCategory
            {
                SubCategoryId = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId
            };
            return await _subCategoryRepository.CreateAsync(subCategory);
        }

        public async Task UpdateAsync(string id, SubCategoryCreateDto dto)
        {
            var subCategory = await _subCategoryRepository.GetByIdAsync(id);
            if (subCategory == null)
                throw new Exception("SubCategory not found");

            subCategory.Name = dto.Name;
            subCategory.Description = dto.Description;
            subCategory.CategoryId = dto.CategoryId;

            await _subCategoryRepository.UpdateAsync(subCategory);
        }

        public async Task<bool> RemoveAsync(string id)
            => await _subCategoryRepository.RemoveAsync(id);
    }
}
