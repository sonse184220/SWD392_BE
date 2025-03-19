using CityScout.DTOs;
using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class OpeningHourService : IOpeningHourService
    {
        private readonly IOpeningHourRepository _openingHourRepository;

        public OpeningHourService(IOpeningHourRepository openingHourRepository)
        {
            _openingHourRepository = openingHourRepository;
        }

        public async Task<List<OpeningHourDto>> GetAllAsync()
        {
            var data = await _openingHourRepository.GetAllAsync();
            return data.Select(oh => new OpeningHourDto
            {
                DestinationId = oh.DestinationId,
                DayOfWeek = oh.DayOfWeek,
                OpenTime = oh.OpenTime.HasValue ? oh.OpenTime.Value.ToString(@"hh\:mm\:ss") : null,
                CloseTime = oh.CloseTime.HasValue ? oh.CloseTime.Value.ToString(@"hh\:mm\:ss") : null,
                IsClosed = oh.IsClosed ?? false
            }).ToList();
        }

        public async Task<List<OpeningHourDto>> GetByDestinationIdAsync(string destinationId)
        {
            var data = await _openingHourRepository.GetByDestinationIdAsync(destinationId);
            return data.Select(oh => new OpeningHourDto
            {
                DestinationId = oh.DestinationId,
                DayOfWeek = oh.DayOfWeek,
                OpenTime = oh.OpenTime.HasValue ? oh.OpenTime.Value.ToString(@"hh\:mm\:ss") : null,
                CloseTime = oh.CloseTime.HasValue ? oh.CloseTime.Value.ToString(@"hh\:mm\:ss") : null,
                IsClosed = oh.IsClosed ?? false 
            }).ToList();
        }


        public async Task<bool> CreateAsync(OpeningHourDto dto)
        {
            var openingHour = new OpeningHour
            {
                DestinationId = dto.DestinationId,
                DayOfWeek = dto.DayOfWeek,
                OpenTime = TimeSpan.Parse(dto.OpenTime), // Convert String -> TimeSpan
                CloseTime = TimeSpan.Parse(dto.CloseTime), // Convert String -> TimeSpan
                IsClosed = dto.IsClosed
            };
            return await _openingHourRepository.CreateAsync(openingHour);
        }

        public async Task<bool> UpdateAsync(OpeningHourDto dto)
        {
            var openingHour = new OpeningHour
            {
                DestinationId = dto.DestinationId,
                DayOfWeek = dto.DayOfWeek,
                OpenTime = TimeSpan.Parse(dto.OpenTime), // Convert String -> TimeSpan
                CloseTime = TimeSpan.Parse(dto.CloseTime), // Convert String -> TimeSpan
                IsClosed = dto.IsClosed
            };
            return await _openingHourRepository.UpdateAsync(openingHour);
        }

        public async Task<bool> DeleteAsync(string destinationId, string dayOfWeek)
        {
            return await _openingHourRepository.DeleteAsync(destinationId, dayOfWeek);
        }
    }
}
