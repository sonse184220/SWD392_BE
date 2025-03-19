using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class GetChatVM
    {
        public string Prompt { get; set; }
        public string Response { get; set; }
        public string CreatedAt { get; set; }
    }
}
