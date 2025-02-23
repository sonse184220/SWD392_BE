﻿using Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IDestinationService
    {
        Task<List<Destination>> GetAllDestinationsAsync();
        Task<Destination> GetDestinationByIdAsync(int id);
        Task<List<Destination>> SearchDestinationsAsync(string query);
        Task AddDestinationAsync(Destination destination);
        Task UpdateDestinationAsync(Destination destination);
        Task DeleteDestinationAsync(int id);
    }
}
