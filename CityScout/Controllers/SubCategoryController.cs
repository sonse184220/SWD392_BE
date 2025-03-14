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
            try
            {
                var result = await _subCategoryService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategory(string id)
        {
            try
            {
                var subCategory = await _subCategoryService.GetByIdAsync(id);
                if (subCategory == null) return NotFound();
                return Ok(subCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateSubCategory([FromBody] SubCategoryCreateDto dto)
        {
            try
            {
                var createdId = await _subCategoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetSubCategory), new { id = createdId }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubCategory(string id, [FromBody] SubCategoryCreateDto dto)
        {
            try
            {
                await _subCategoryService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(string id)
        {
            try
            {
                var success = await _subCategoryService.RemoveAsync(id);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
