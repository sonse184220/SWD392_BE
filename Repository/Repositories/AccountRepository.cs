using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PsyHealth.Repositories.Base;
using Repository.Interfaces;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
       
        public async Task<Account> GetByEmailAsync(string email)
        {
            return await _context.Accounts.Include(a=>a.Role).FirstOrDefaultAsync(x => x.Email == email);
        }

        

        public async Task<int> CreateAccountAsync(Account account)
        {
            return await CreateAsync(account);
        }
        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == request.UserId);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(request.Username))
                user.UserName = request.Username;
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrEmpty(request.Address))
                user.Address = request.Address;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetProfileByIdAsync(int userId)
        {
            return await _context.Accounts
                .Where(u => u.UserId == userId.ToString()) 
                .Select(u => new
                {
                    u.UserId,
                    u.UserName,
                    u.PhoneNumber,
                    u.Address
                })
                .FirstOrDefaultAsync();
        }



    }
}
