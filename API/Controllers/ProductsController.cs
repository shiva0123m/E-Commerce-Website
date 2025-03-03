using Core.Entity;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository repo): ControllerBase
    {
       
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            return Ok(await repo.GetProductsAsync(brand , type , sort));
        }
        [HttpGet("{id:Guid}")]     // api/products
        public async Task<ActionResult<Product>> Getproduct(Guid id)
        {
            var product= await repo.GetProductByIdAsync(id);
            if(product == null) return  NotFound();
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.AddProduct(product);

            if(await repo.SaveChangesAsync())
            {
                return CreatedAtAction("GetProduct",new {id =product.Id},product);
            }
            return BadRequest("Problem creating product");
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateProduct(Guid id ,Product product)
        {
            if(product.Id != id || !ProductExists(id)) return BadRequest("Cannot update this product");

           repo.UpdateProduct(product);
           if(await repo.SaveChangesAsync())
           {
            return NoContent();
           }
            return BadRequest("Problem at updating the product");
        }
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteProduct(Guid id) 
        {
            var product = await repo.GetProductByIdAsync(id);
            if(product == null) return NotFound();
            repo.DeleteProduct(product);
            if(await repo.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem at updating the product");      
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repo.GetBrandsAsync());
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repo.GetTypesAsync());
        }
        private bool ProductExists(Guid id)
        {
            return repo.ProductExists(id);
        }
    }
}
