using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IRedisCacheService
    {
        Task SetCacheAsync<T>(string key, T value, TimeSpan expiration);
        Task<T?> GetCacheAsync<T>(string key);
        Task ListLeftPushAsync(string key, string value);
        Task ListTrimAsync(string key, long start, long stop);
        Task KeyExpireAsync(string key, TimeSpan expiry);
        Task<string[]> ListRangeAsync(string key, long start, long stop);

    }
}
