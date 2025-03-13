using CityScout;
using CityScout.Repositories;
using CityScout.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PsyHealth.Repositories.Base;
using Repository;
using Repository.Interfaces;
using Repository.Models;
using Repository.Repositories;
using Service;
using Service.Interfaces;
using Service.Services;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
if (FirebaseApp.DefaultInstance == null)
{
    var firebaseApp = FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile("firebase-adminsdk.json")
    });
}
builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // Add this line after builder is created
builder.Services.AddHttpClient();

// Configure CityScoutContext with the connection string
builder.Services.AddDbContext<CityScoutContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddHttpClient<IFcmService, FcmService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
//redis

//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    var redisConnectionString = builder.Configuration["RedisFreeStyle:ConnectionString"];
//    if (string.IsNullOrEmpty(redisConnectionString))
//    {
//        throw new InvalidOperationException("Redis:ConnectionString is not configured in appsettings.json or Application Settings.");
//    }
//    options.Configuration = redisConnectionString;
//    options.InstanceName = "CityScout_";
//});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = builder.Configuration["RedisFreeStyle:ConnectionString"];
    if (string.IsNullOrEmpty(redisConnectionString))
    {
        throw new InvalidOperationException("Redis:ConnectionString is not configured in appsettings.json or Application Settings.");
    }
    return ConnectionMultiplexer.Connect(redisConnectionString);
});



//jwt
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddControllers().AddControllersAsServices();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CityScout API",
        Version = "v1",
        Description = "API for managing CityScout resources",
        Contact = new OpenApiContact { Name = "Your Name", Email = "your.email@example.com" }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.OperationFilter<FileUploadOperationFilter>();
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleRepository = services.GetRequiredService<IRoleRepository>();
        await roleRepository.SeedRoleAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CityScout API v1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at the root of "/swagger"
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();