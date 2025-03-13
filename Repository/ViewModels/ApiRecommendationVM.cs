using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ApiRecommendationVM
    {
        public class DestinationDTO
        {
            public string DestinationId { get; set; }
            public string DestinationName { get; set; }
            public string Address { get; set; }
            public string Description { get; set; }
            public double? Rate { get; set; }
            public string? CategoryId { get; set; }
            public string Ward { get; set; }
            public string Status { get; set; }
            public string? DistrictId { get; set; }
            public DistrictDTO District { get; set; }
        }

        public class DistrictDTO
        {
            public string DistrictId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string? CityId { get; set; }
            public CityDTO City { get; set; }
        }

        public class CityDTO
        {
            public string CityId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

    }
}
