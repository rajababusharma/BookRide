using BookRide.Models;
using GoogleGson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bumptech.Glide.Load.Model;

#if ANDROID
using Android.Security.Keystore;
#endif

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

        //private string BuildUrl(string path)
        //=> $"{BaseUrl}{path}.json";

        private async Task<string> BuildUrl(string path)
        {
            var token = await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenKeyValue);
            return $"{BaseUrl}{path}.json?auth={token}";
        } 

        // CREATE / UPDATE
        public async Task<bool> SaveAsync<T>(string node, T data)
        {
           
           // var json = JsonSerializer.Serialize(data);
            var json = await Task.Run(() =>
            {
                return JsonSerializer.Serialize(data); // heavy work here
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(await BuildUrl(node), content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await Shell.Current.DisplayAlert("Alert", "Server is not responding at the moment, please try again after sometimes", "Ok");
                await Task.Run(async() =>
                {
                     await GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
                });
               
            }
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
           // var json = System.Text.Json.JsonSerializer.Serialize(data);
            var json = await Task.Run(() =>
            {
                return System.Text.Json.JsonSerializer.Serialize(data); // heavy work here
            });
            var response = await _httpClient.PostAsync(await BuildUrl(node),
                              new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await Shell.Current.DisplayAlert("Alert", "Server is not responding at the moment, please try again after sometimes", "Ok");
                await Task.Run(async () =>
                {
                    await GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
                });
            }


            var result = await response.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            return obj?["name"]; // Firebase generated key
        }


        // READ ALL
        //public async Task<Dictionary<string, T>> GetAllAsync<T>(string node)
        //{
        //    var json = await _httpClient.GetStringAsync(await BuildUrl(node));
        //    return JsonSerializer.Deserialize<Dictionary<string, T>>(json);
        //}

        public async Task<Dictionary<string, T>> GetAllAsync<T>(string node)
        {
            var result=new Dictionary<string, T>();
            string? json = null;
            try
            {
                 json = await _httpClient.GetStringAsync(await BuildUrl(node));
                //  return JsonSerializer.Deserialize<Dictionary<string, T>>(json);
            }
           
             catch (UnauthorizedAccessException excp)
            {
                await Shell.Current.DisplayAlert("Alert", "Server is not responding at the moment, please try again after sometimes", "Ok");
                await Task.Run(async () =>
                {
                    await GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
                });
                
            }
            finally
            {
                 result = await Task.Run(() =>
                {
                    return JsonSerializer.Deserialize<Dictionary<string, T>>(json); // heavy work here
                });

               
            }
            return result;


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
        //public async Task<T> GetAsync<T>(string node)
        //{

        //       var json = await _httpClient.GetStringAsync(await BuildUrl(node));
        //        return JsonSerializer.Deserialize<T>(json);

        //}
        public async Task<T> GetAsync<T>(string node)
        {
           
            string? json = null;
            try
            {
                json = await _httpClient.GetStringAsync(await BuildUrl(node));

               //  return JsonSerializer.Deserialize<T>(json);
            }
            catch (UnauthorizedAccessException excp)
            {
                await Shell.Current.DisplayAlert("Alert", "Server is not responding at the moment, please try again after sometimes", "Ok");
                await Task.Run(async () =>
                {
                    await GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
                });
                return JsonSerializer.Deserialize<T>(json);
            }
           var result= await Task.Run(() =>
            {
                return JsonSerializer.Deserialize<T>(json); // heavy work here
            });
          //  return JsonSerializer.Deserialize<T>(json);
          return result;

        }
        // public async Task<Dictionary<string, T>> GetAsync<T>(string node)
        // {
        //     var result = new Dictionary<string, T>();
        //     string? json = null;
        //     try
        //     {
        //         json = await _httpClient.GetStringAsync(await BuildUrl(node));

        //        // return JsonSerializer.Deserialize<T>(json);
        //     }
        //     catch (UnauthorizedAccessException excp)
        //     {
        //         await Shell.Current.DisplayAlert("Alert", "Server is not responding at the moment, please try again after sometimes", "Ok");
        //         await Task.Run(async () =>
        //         {
        //             await GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
        //         });

        //     }
        //     finally
        //     {
        ////         result = await Task.Run(() =>
        ////JsonSerializer.Deserialize<Dictionary<string, T>>(json));
        //result = JsonSerializer.Deserialize<Dictionary<string, T>>(json);
        //     }



        //     return result;

        // }


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
            var response = await _httpClient.DeleteAsync(await BuildUrl(node));
            //checking response code
            if(response.StatusCode==System.Net.HttpStatusCode.Unauthorized)
            {
                await Shell.Current.DisplayAlert("Alert", "Server is not responding at the moment, please try again after sometimes", "Ok");
                await Task.Run(async () =>
                {
                    await GetTokenAsync(Constants.Constants.Firebase_UserId, Constants.Constants.Firebase_Userpwd);
                });
            }
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

        // firebase authentication
        public async Task GetTokenAsync(string email, string password)
        {
            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Task.Run(() => 
            _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={Constants.Constants.Firebase_WebApi_key}",
                content)
            );

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            //  var tocken_result = JsonSerializer.Deserialize<FirebaseAuthResponse>(result);
            var tocken_result = await Task.Run(() =>
      JsonSerializer.Deserialize<FirebaseAuthResponse>(result));

            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenKeyValue, tocken_result.idToken);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_UIDKeyValue, tocken_result.localId);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_RefreshTokenKeyValue, tocken_result.refreshToken);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenTimeKeyValue, DateTime.UtcNow.ToString());

        }
    }
}
