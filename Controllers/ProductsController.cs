using BusinessSolution.Dtos.Product;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductsService productsService) : ControllerBase
    {
        private readonly IProductsService productsService = productsService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] ProductSearchDto searchDto)
        {
            var result = await productsService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await productsService.GetById(id);
            return Ok(result);
        }

        [HttpGet("GetByName/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await productsService.GetByName(name);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ProductRequestDto dto)
        {
            var result = await productsService.Create(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromForm] ProductRequestDto dto)
        {
            var result = await productsService.Update(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await productsService.Delete(id);
            return Ok(result);
        }
    }
}
