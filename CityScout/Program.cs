using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CityScoutContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericInterface<>), typeof(GenericRepo<>));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
