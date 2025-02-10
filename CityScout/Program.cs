using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Repository;
using Repository.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<CityScoutContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericInterface<>), typeof(GenericRepo<>));
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CityScout API",
        Version = "v1",
        Description = "API for managing CityScout resources",
        Contact = new OpenApiContact { Name = "Your Name", Email = "your.email@example.com" }
    });

    // Make sure this path exists and is correctly referenced
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Enable Swagger in all environments (remove the IsDevelopment check)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CityScout API v1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at the root of "/swagger"
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();