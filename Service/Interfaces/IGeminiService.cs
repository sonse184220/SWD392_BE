using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IGeminiService
    {
        Task<List<ApiRecommendationVM.DestinationDTO>> GetDestinationRecommendation(string message);
        Task<string> SendRequest(string message);
    }
}
