using CityScout.Services;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection service)
        {
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IDestinationService, DestinationService>();
            service.AddScoped<IGeminiService, GeminiService>();
            service.AddScoped<AIService>();
            service.AddScoped<IDatabaseService, SqlServerDatabaseService>();
            service.AddScoped<IRedisCacheService, RedisCacheService>();
            service.AddScoped<IGoogleSearchService, GoogleSearchService>();
            service.AddScoped<ICloudinaryService, CloudinaryService>();

            return service;
        }
    }
}
