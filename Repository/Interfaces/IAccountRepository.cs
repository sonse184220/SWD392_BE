using Repository.Models;
using Repository.RequestModels;
using Repository.ResponseModels;
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
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request, string? profilePicture);
        Task<ProfileResponse> GetProfileByIdAsync(string userId);
        Task<bool> SetAccountActiveStatusAsync(string userId, bool isActive);
        Task<List<Account>> GetAccountListAsync();
        Task<int> UpdateUserToAdmin(string userId, int roleId);


    }
}
