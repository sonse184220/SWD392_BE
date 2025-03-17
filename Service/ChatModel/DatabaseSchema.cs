using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ChatModel
{
    public class DatabaseSchema
    {
        public List<TableSchema> SchemaStructured { get; set; }
        public List<string> SchemaRaw { get; set; }

        //enhance
        public List<string> WardNames { get; set; }      // Added for wards
        public List<string> DistrictNames { get; set; }  // Added for districts
        public List<string> CityNames { get; set; }      // Added for cities
        public List<string> CategoryNames { get; set; }  // Added for categories
    }
}
