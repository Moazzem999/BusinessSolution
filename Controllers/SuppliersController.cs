using BusinessSolution.Dtos.Supplier;
using BusinessSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController(ISuppliersService suppliersService) : ControllerBase
    {
        private readonly ISuppliersService suppliersService = suppliersService;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] SupplierSearchDto searchDto)
        {
            var result = await suppliersService.GetAll(searchDto);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await suppliersService.GetById(id);
            return Ok(result);
        }

        [HttpGet("GetByName/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await suppliersService.GetByName(name);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] SupplierRequestDto dto)
        {
            var result = await suppliersService.Create(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromForm] SupplierRequestDto dto)
        {
            var result = await suppliersService.Update(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await suppliersService.Delete(id);
            return Ok(result);
        }
    }
}
