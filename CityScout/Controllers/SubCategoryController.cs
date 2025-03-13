using Microsoft.AspNetCore.Mvc;
using CityScout.Services;
using CityScout.DTOs;

namespace CityScout.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;

        public SubCategoryController(ISubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var result = await _subCategoryService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategory(string id)
        {
            var subCategory = await _subCategoryService.GetByIdAsync(id);
            if (subCategory == null)
                return NotFound();

            return Ok(subCategory);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubCategory([FromBody] SubCategoryCreateDto dto)
        {
            var createdId = await _subCategoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetSubCategory), new { id = createdId }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubCategory(string id, [FromBody] SubCategoryCreateDto dto)
        {
            await _subCategoryService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(string id)
        {
            var success = await _subCategoryService.RemoveAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
