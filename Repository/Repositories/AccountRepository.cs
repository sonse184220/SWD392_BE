using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PsyHealth.Repositories.Base;
using Repository.Interfaces;
using Repository.Models;
using Repository.RequestModels;
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
        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request, string? profilePicture)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return false;

            if (request.Username is not null)
                user.UserName = request.Username;

            if (request.PhoneNumber is not null)
                user.PhoneNumber = request.PhoneNumber;

            if (request.Address is not null)
                user.Address = request.Address;

            if (!string.IsNullOrEmpty(profilePicture))
                user.ProfilePicture = profilePicture;

            await UpdateAsync(user);
            return true;
        }




        public async Task<object> GetProfileByIdAsync(string userId)
        {
            return await _context.Accounts
                .Where(u => u.UserId == userId.ToString()) 
                .Select(u => new
                {
                    u.UserId,
                    u.Email,
                    u.UserName,
                    u.PhoneNumber,
                    u.Address,
                    u.ProfilePicture
                })
                .FirstOrDefaultAsync();
        }



    }
}
