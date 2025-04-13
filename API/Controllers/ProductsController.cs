using Core.Entity;
using Core.Interfaces;
using Core.Specification;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IGenericRepository<Product> repo): ControllerBase
    {
       
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            var spec=new ProductSpecification(brand,type,sort);

            var products = await repo.ListAsync(spec);
            return Ok(products);
        }
        [HttpGet("{id:Guid}")]     // api/products
        public async Task<ActionResult<Product>> Getproduct(Guid id)
        {
            var product= await repo.GetByIdAsync(id);
            if(product == null) return  NotFound();
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.Add(product);

            if(await repo.SaveAllAsync())
            {
                return CreatedAtAction("GetProduct",new {id =product.Id},product);
            }
            return BadRequest("Problem creating product");
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateProduct(Guid id ,Product product)
        {
            if(product.Id != id || !ProductExists(id)) return BadRequest("Cannot update this product");

           repo.Update(product);
           if(await repo.SaveAllAsync())
           {
            return NoContent();
           }
            return BadRequest("Problem at updating the product");
        }
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteProduct(Guid id) 
        {
            var product = await repo.GetByIdAsync(id);
            if(product == null) return NotFound();
            repo.Remove(product);
            if(await repo.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem at updating the product");      
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec=new BrandListSpecification();
            return Ok(await repo.ListAsync(spec));
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec=new TypeListSpecification();
            return Ok(await repo.ListAsync(spec));
        }
        private bool ProductExists(Guid id)
        {
            return repo.Exists(id);
        }
    }

}
