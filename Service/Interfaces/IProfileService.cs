using Repository.RequestModels;

namespace Service.Interfaces
{
    public interface IProfileService
    {
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request, string? profilePicture);
        Task<object> GetProfileByIdAsync(string userId);
    }
}
