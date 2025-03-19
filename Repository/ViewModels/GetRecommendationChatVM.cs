using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
   
    public class GetRecommendationChatVM
    {
        public string Prompt { get; set; }
        public List<ApiRecommendationVM.DestinationDTO> Response { get; set; }
        public string CreatedAt { get; set; }
    }
}
