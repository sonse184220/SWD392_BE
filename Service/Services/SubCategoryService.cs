using CityScout.Repositories;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<SubCategory> GetByIdAsync(int id)
            => await _subCategoryRepository.GetByIdAsync(id);

        public async Task<int> CreateAsync(SubCategory subCategory)
            => await _subCategoryRepository.CreateAsync(subCategory);

        public async Task<int> UpdateAsync(SubCategory subCategory)
            => await _subCategoryRepository.UpdateAsync(subCategory);

        public async Task<bool> RemoveAsync(int id)
            => await _subCategoryRepository.RemoveAsync(id);
    }
}