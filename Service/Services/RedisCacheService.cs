using Service.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RedisCacheService: IRedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan expiration)
        {
            var jsonData = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, jsonData, expiration);
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            var jsonData = await _db.StringGetAsync(key);
            return jsonData.HasValue ? JsonSerializer.Deserialize<T>(jsonData!) : default;
        }
        public async Task ListLeftPushAsync(string key, string value)
        {
            await _db.ListLeftPushAsync(key, value);
        }

        public async Task ListTrimAsync(string key, long start, long stop)
        {
            await _db.ListTrimAsync(key, start, stop);
        }

        public async Task KeyExpireAsync(string key, TimeSpan expiry)
        {
            await _db.KeyExpireAsync(key, expiry);
        }

        public async Task<string[]> ListRangeAsync(string key, long start, long stop)
        {
            var values = await _db.ListRangeAsync(key, start, stop);
            return values.Select(v => v.ToString()).ToArray();
        }

    }
}
