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

        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            return await _accountRepository.UpdateProfileAsync(request);
        }

        public async Task<object> GetProfileByIdAsync(int userId)
        {
            return await _accountRepository.GetProfileByIdAsync(userId.ToString());
        }
    }
}
