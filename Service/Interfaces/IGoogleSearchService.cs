﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IGoogleSearchService
    {
        Task<List<string>> SearchImagesAsync(string query, int expectedResult);

    }
}
