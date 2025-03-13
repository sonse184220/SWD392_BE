using CityScout.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection service)
        {
            service.AddTransient<IAccountRepository, AccountRepository>();
            service.AddTransient<IRoleRepository, RoleRepository>();
            service.AddTransient<IDestinationRepository, DestinationRepository>();
            return service;
        }
    }
}
