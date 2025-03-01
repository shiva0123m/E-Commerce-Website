using System;
using Core.Entity;
using Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
// here inheriting the dbcontext class and using constructor to form the connection 
public class StoreContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products{ get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // using the data configurred in product configuration class 
        modelBuilder.ApplyConfigurationsFromAssembly( typeof(ProductConfiguration).Assembly );
    }
}
