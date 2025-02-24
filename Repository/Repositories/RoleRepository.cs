using Microsoft.EntityFrameworkCore;
using PsyHealth.Repositories.Base;
using Repository.Enums;
using Repository.Interfaces;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == roleName);
        }

        public async Task SeedRoleAsync()
        {
            var existingRoles = _context.Roles.Select(r => r.RoleName).ToList();

            var rolesToAdd = new List<Role>();

            if (!existingRoles.Contains(UserRole.Admin.ToString()))
            {
                rolesToAdd.Add(new Role { RoleName = UserRole.Admin.ToString() });
            }

            if (!existingRoles.Contains(UserRole.User.ToString()))
            {
                rolesToAdd.Add(new Role { RoleName = UserRole.User.ToString() });
            }

            if (rolesToAdd.Count > 0)
            {
                _context.Roles.AddRange(rolesToAdd);
                await _context.SaveChangesAsync();
            }
        }
    }
}
