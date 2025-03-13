using Repository.Interfaces;
using Repository.RequestModels;  
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


        public async Task<object> GetProfileByIdAsync(string userId)
        {
            return await _accountRepository.GetProfileByIdAsync(userId.ToString());
        }
    }
}
