using Repository.RequestModels;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAuthService
    {
        Task<AccountViewModel> AuthenticateWithFirebaseAsync(FirebaseTokenRequest request);
        Task<int> PromoteToAdmin(string userId);

    }
}
