using Repository.Models;
using Repository.RequestModels;
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
        Task<int> CreateAccountAsync(Account account);
        Task<bool> UpdateProfileAsync(UpdateProfileRequest request);
        Task<object> GetProfileByIdAsync(string userId);

    }
}
