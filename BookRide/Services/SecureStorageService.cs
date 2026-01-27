using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class SecureStorageService
    {
        public static async Task SaveAsync<T>(string key, T data)
        {
            var json = await Task.Run( ()=> JsonSerializer.Serialize(data));
            await SecureStorage.SetAsync(key, json);
        }

        public static async Task<T?> GetAsync<T>(string key)
        {
            var json = await SecureStorage.GetAsync(key);

            if (string.IsNullOrEmpty(json))
                return default;

           // return JsonSerializer.Deserialize<T>(json);
            return await Task.Run(() => JsonSerializer.Deserialize<T>(json));

        }
    }
}
