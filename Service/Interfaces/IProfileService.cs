using Repository.RequestModels;
namespace Service.Interfaces
{
    public interface IProfileService
    {
        Task<bool> UpdateProfileAsync(UpdateProfileRequest request);
        Task<object> GetProfileByIdAsync(int userId);
    }
}
