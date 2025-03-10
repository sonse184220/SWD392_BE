using Microsoft.AspNetCore.Mvc;
using CityScout.Services;
using Repository.Models;

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
            try
            {
                var subCategories = await _subCategoryService.GetAllAsync();
                return Ok(subCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            try
            {
                var subCategory = await _subCategoryService.GetByIdAsync(id);
                if (subCategory == null)
                    return NotFound();

                return Ok(subCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubCategory([FromBody] SubCategory subCategory)
        {
            try
            {
                var createdId = await _subCategoryService.CreateAsync(subCategory);
                subCategory.SubCategoryId = createdId;
                return CreatedAtAction(nameof(GetSubCategory), new { id = subCategory.SubCategoryId }, subCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] SubCategory subCategory)
        {
            try
            {
                if (id != subCategory.SubCategoryId)
                    return BadRequest("SubCategory ID mismatch.");

                await _subCategoryService.UpdateAsync(subCategory);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            try
            {
                var result = await _subCategoryService.RemoveAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
