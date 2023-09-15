using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : CustomBaseController
    {
        private readonly IServiceGeneric<Product, ProductDto> _serviceGeneric;

        public ProductsController(IServiceGeneric<Product, ProductDto> serviceGeneric)
        {
            _serviceGeneric = serviceGeneric;
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return ActionResultInstance(await _serviceGeneric.GetAllAsync());
        }


        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductDto productDto)
        {
            return ActionResultInstance(await _serviceGeneric.Add(productDto));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto)
        {
            return ActionResultInstance(await _serviceGeneric.Update(productDto,productDto.Id));
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return ActionResultInstance(await _serviceGeneric.Remove(id));
        }


    }
}
