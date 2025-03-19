using Repository.Models;
using Repository.RequestModels;
using Repository.ResponseModels;

namespace Service.Interfaces
{
    public interface IProfileService
    {
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request, string? profilePicture);
        Task<ProfileResponse> GetProfileByIdAsync(string userId);
        Task<bool> SetAccountActiveStatusAsync(string userId, bool isActive);
        Task<List<Account>> GetAccountListAsync();

    }
}
