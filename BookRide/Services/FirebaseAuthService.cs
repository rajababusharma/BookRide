using BookRide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class FirebaseAuthService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = Constants.Constants.Firebase_WebApi_key;

        public FirebaseAuthService()
        {
            _httpClient = new HttpClient();
        }
        //public async Task<FirebaseRegisterResponse> RegisterAsync(string email, string password)
        //{
        //    var request = new FirebaseRegisterRequest
        //    {
        //        Email = email,
        //        Password = password
        //    };

        //    var json = JsonSerializer.Serialize(request);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

          

        //    var response = await _httpClient.PostAsync(
        //        $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ApiKey}",
        //        content);


        //    var responseJson = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //        throw new Exception(responseJson);

        //    return JsonSerializer.Deserialize<FirebaseRegisterResponse>(responseJson);
        //}

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

            await SecureStorageService.SaveAsync<DateTime>(Constants.Constants.SessionStartTime, DateTime.Now);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenKeyValue, tocken_result.idToken);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_UIDKeyValue, tocken_result.localId);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_RefreshTokenKeyValue, tocken_result.refreshToken);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenTimeKeyValue, DateTime.UtcNow.ToString());

        }
    }
}
