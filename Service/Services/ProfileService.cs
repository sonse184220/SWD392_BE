using Repository.Interfaces;
using Repository.Models;
using Repository.RequestModels;
using Repository.ResponseModels;
using Service.Interfaces;

namespace Service.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IAccountRepository _accountRepository;
        public ProfileService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request, string? profilePicture)
        {
            return await _accountRepository.UpdateProfileAsync(userId, request, profilePicture);
        }


        public async Task<ProfileResponse> GetProfileByIdAsync(string userId)
        {
            return await _accountRepository.GetProfileByIdAsync(userId);
        }


        public async Task<bool> SetAccountActiveStatusAsync(string userId, bool isActive)
        {
            return await _accountRepository.SetAccountActiveStatusAsync(userId, isActive);
        }

        public async Task<List<Account>> GetAccountListAsync()
        {
            return await _accountRepository.GetAccountListAsync();
        }

    }
}
