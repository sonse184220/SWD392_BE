using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAccountRepository
    {
        
        Task<Account> GetByEmailAsync(string email);
        Task<Account> GetByUidAsync(string uid);
        Task<int> CreateAccountAsync(Account account);

    }
}
