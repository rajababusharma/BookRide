using BookRide.Models;
using Java.Util.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        public async Task RefreshTokenAsync()
        {
           // var refreshToken = await SecureStorage.GetAsync("firebase_refresh_token");
            var refreshToken = await SecureStorage.GetAsync(Constants.Constants.Firebase_RefreshTokenKeyValue);
          

                var refreshData = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken }
                };

            var response = await _httpClient.PostAsync(
                $"https://securetoken.googleapis.com/v1/token?key={Constants.Constants.Firebase_WebApi_key}",
                new FormUrlEncodedContent(refreshData));

            var json = await response.Content.ReadAsStringAsync();
            var tokenResult = JsonSerializer.Deserialize<TokenRefreshResponse>(json);

            string newIdToken = tokenResult.id_token;
           // await SecureStorage.SetAsync("firebase_id_token", newIdToken);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenKeyValue, newIdToken);

        }
    }
}
