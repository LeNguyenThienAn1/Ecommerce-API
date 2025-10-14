using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _brandService.GetAllBrandsAsync());

        [HttpGet("filter")]
        public async Task<IActionResult> GetAllWithFilter([FromQuery] BrandFilterDto filter) =>
            Ok(await _brandService.GetAllBrandsAsync(filter));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _brandService.GetBrandByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(CreateOrUpdateBrandDto dto) =>
            Ok(await _brandService.CreateOrUpdateBrandAsync(dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id) =>
            Ok(await _brandService.DeleteBrandAsync(id));
    }
}
