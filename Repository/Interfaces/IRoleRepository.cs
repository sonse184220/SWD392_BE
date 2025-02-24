using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IRoleRepository
    {
        Task SeedRoleAsync();
        Task<Role> GetRoleByNameAsync(string roleName);
    }
}
