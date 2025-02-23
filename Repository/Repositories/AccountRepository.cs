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
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<Account> GetByUidAsync(string uid)
        {
            return await GetByIdAsync(uid);
        }

        public async Task<int> CreateAccountAsync(Account account)
        {
            return await CreateAsync(account);
        }

        
    }
}
