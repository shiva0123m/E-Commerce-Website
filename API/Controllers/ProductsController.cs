using Core.Entity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext Context;
        public ProductsController(StoreContext context)
        {
            this.Context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await Context.Products.ToListAsync();
        }
        [HttpGet("{id:Guid}")]     // api/products
        public async Task<ActionResult<Product>> Getproduct(Guid id)
        {
            var product= await Context.Products.FindAsync(id);
            if(product == null) return  NotFound();
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            Context.Products.Add(product);

            await Context.SaveChangesAsync();   

            return product;
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateProduct(Guid id ,Product product)
        {
            if(product.Id != id || !ProductExists(id)) return BadRequest("Cannot update this product");

            Context.Entry(product).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteProduct(Guid id) 
        {
            var product = await Context.Products.FindAsync(id);
            if(product == null) return NotFound();
            Context.Products.Remove(product);
            await Context.SaveChangesAsync();
            return NoContent();            
        }
        private bool ProductExists(Guid id)
        {
            return Context.Products.Any(p => p.Id == id);
        }
    }
}
