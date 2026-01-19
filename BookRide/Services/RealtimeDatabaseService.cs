using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class RealtimeDatabaseService
    {
      //  private readonly FirebaseClient _firebase;
        private readonly HttpClient _httpClient;

        private const string BaseUrl = Constants.Constants.Firebase_Realtime_Storage_url;
        public RealtimeDatabaseService()
        {
            //_firebase = new FirebaseClient(
            //    "https://bookride-f497d-default-rtdb.firebaseio.com/");

            //_firebase = new FirebaseClient(
            //   Constants.Constants.Firebase_Realtime_Storage_url);

            _httpClient = new HttpClient();
        }

        private string BuildUrl(string path)
        => $"{BaseUrl}{path}.json";

        // CREATE / UPDATE
        public async Task<bool> SaveAsync<T>(string node, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(BuildUrl(node), content);
            return response.IsSuccessStatusCode;
        }

        // Save or Update
        //public async Task SaveAsync<T>(string path, T data)
        //{

        //    // If the path already exists, this will overwrite the data
        //    await _firebase
        //        .Child(path)
        //        .PutAsync(data);
        //}

        // POST (Auto ID)
        public async Task<string> PushAsync<T>(string node, T data)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var response = await _httpClient.PostAsync(BuildUrl(node),
                              new StringContent(json, Encoding.UTF8, "application/json"));

            var result = await response.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            return obj?["name"]; // Firebase generated key
        }


        // READ ALL
        public async Task<Dictionary<string, T>> GetAllAsync<T>(string node)
        {
            var json = await _httpClient.GetStringAsync(BuildUrl(node));
            return JsonSerializer.Deserialize<Dictionary<string, T>>(json);
        }

        //public async Task<Dictionary<string, T>> GetAllAsync<T>(string node)
        //{
        //    var lists = new List<T>();
        //    var json = await _httpClient.GetStringAsync(BuildUrl(node));
        //    // return JsonSerializer.Deserialize<List<T>>(json);

        //  //  return lists;
        //    return JsonSerializer.Deserialize<Dictionary<string, T>>(json);
        //}


        // Get all records under a node
        //public async Task<List<T>> GetAllAsync<T>(string path)
        //{

        //    var items = await _firebase
        //        .Child(path)
        //        .OnceAsync<T>();

        //    return items.Select(i => i.Object).ToList();
        //}

        // READ SINGLE
        public async Task<T> GetAsync<T>(string node)
        {
            var json = await _httpClient.GetStringAsync(BuildUrl(node));
            return JsonSerializer.Deserialize<T>(json);
        }

        // Get a single record by key
        // 🔹 Retrieve Single Node
        //public async Task<T> GetAsync<T>(string path)
        //{
        //    return await _firebase
        //        .Child(path)
        //        .OnceSingleAsync<T>();
        //}  
        
        // DELETE
        public async Task<bool> DeleteAsync(string node)
        {
            var response = await _httpClient.DeleteAsync(BuildUrl(node));
            return response.IsSuccessStatusCode;
        }

        //// 🔹 Delete
        //public async Task DeleteAsync(string path)
        //{
        //    await _firebase
        //        .Child(path)
        //        .DeleteAsync();
        //}

        //public async Task DeleteAllAsync()
        //{
        //    await _firebase.Child("/").DeleteAsync();
        //}
    }
}
