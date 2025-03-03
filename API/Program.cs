using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Adding the service to implement the repository pattern 
// this scoped service will be active till the request is there ,Singleton service also would have been used instead of this but it will alive till the application is running 
builder.Services.AddScoped<IProductRepository, ProductRepository>();
var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
// we are using this service outside of the dependecy injection thats why i need to add scope so that once the application is stopped it will be stopped automatically it 
// its inside dedpendency injection then automatically framework would have disposed it noow we have to do it manullay but due to this every time we start the application it will integrate the pending migration and create db if required
try
{
    using var scope=app.Services.CreateScope();
    var services=scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch(Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
// / Tried to use mostly bare minimum services here for understanding the use of services and deployment serve