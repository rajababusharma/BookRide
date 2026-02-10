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

        public async Task LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={Constants.Constants.Firebase_WebApi_key}";

           // var response = await _httpClient.PostAsync(url, content);
            var response = await _httpClient.PostAsync(url, content, ct);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Firebase sends useful error JSON
                throw new Exception($"Firebase login failed: {responseBody}");
            }

            var tokenResult = JsonSerializer.Deserialize<FirebaseAuthResponse>(responseBody);

            if (tokenResult == null || string.IsNullOrEmpty(tokenResult.idToken))
                throw new Exception("Invalid authentication response from Firebase.");

            // Save session info
            await SecureStorageService.SaveAsync<DateTime>(
                Constants.Constants.SessionStartTime, DateTime.UtcNow);

            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenKeyValue, tokenResult.idToken);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_UIDKeyValue, tokenResult.localId);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_RefreshTokenKeyValue, tokenResult.refreshToken);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenTimeKeyValue, DateTime.UtcNow.ToString("O"));
            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenExpiryKey,
    DateTime.UtcNow.AddSeconds(int.Parse(tokenResult.expiresIn)).ToString("O"));

        }

        //public async Task GetTokenAsync(string email, string password)
        //{
        //    var payload = new
        //    {
        //        email,
        //        password,
        //        returnSecureToken = true
        //    };

        //    var json = JsonSerializer.Serialize(payload);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await Task.Run(() =>
        //    _httpClient.PostAsync(
        //        $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={Constants.Constants.Firebase_WebApi_key}",
        //        content)
        //    ).ConfigureAwait(false);

        //    response.EnsureSuccessStatusCode();

        //    var result = await response.Content.ReadAsStringAsync();
        //    //  var tocken_result = JsonSerializer.Deserialize<FirebaseAuthResponse>(result);
        //    var tocken_result = await Task.Run(() =>
        //    JsonSerializer.Deserialize<FirebaseAuthResponse>(result));

        //    await SecureStorageService.SaveAsync<DateTime>(Constants.Constants.SessionStartTime, DateTime.Now);
        //    await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenKeyValue, tocken_result.idToken);
        //    await SecureStorage.SetAsync(Constants.Constants.Firebase_UIDKeyValue, tocken_result.localId);
        //    await SecureStorage.SetAsync(Constants.Constants.Firebase_RefreshTokenKeyValue, tocken_result.refreshToken);
        //    await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenTimeKeyValue, DateTime.UtcNow.ToString());

        //}

        public async Task RefreshTokenAsync()
        {
           // var refreshToken = await SecureStorage.GetAsync("firebase_refresh_token");
            var refreshToken = await SecureStorage.GetAsync(Constants.Constants.Firebase_RefreshTokenKeyValue);
            if (string.IsNullOrEmpty(refreshToken))
                throw new Exception("No refresh token found. User must log in again.");

            var refreshData = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken }
                };

            var response = await _httpClient.PostAsync(
                $"https://securetoken.googleapis.com/v1/token?key={Constants.Constants.Firebase_WebApi_key}",
                new FormUrlEncodedContent(refreshData));
          

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Token refresh failed: {json}");

          
            //  var tokenResult = JsonSerializer.Deserialize<TokenRefreshResponse>(json);
            var refreshResult = JsonSerializer.Deserialize<TokenRefreshResponse>(json);
            if (refreshResult == null || string.IsNullOrEmpty(refreshResult.id_token))
                throw new Exception("Invalid refresh response from Firebase.");
          //  string newIdToken = refreshResult.id_token;

            // 🔐 Save new tokens
            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenKeyValue, refreshResult.id_token);
            await SecureStorage.SetAsync(Constants.Constants.Firebase_RefreshTokenKeyValue, refreshResult.refresh_token);

            // ⏳ Save new expiry time
            var expiry = DateTime.UtcNow.AddSeconds(int.Parse(refreshResult.expires_in));
            await SecureStorage.SetAsync(Constants.Constants.Firebase_TokenExpiryKey, expiry.ToString("O"));

            await SecureStorageService.SaveAsync<DateTime>(Constants.Constants.SessionStartTime, DateTime.Now);
           

        }

        public async Task<bool> IsSessionExpiredAsunc()
        {
            var expiryString = await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenExpiryKey);

            if (string.IsNullOrEmpty(expiryString))
                return true;

            var expiry = DateTime.Parse(expiryString, null, System.Globalization.DateTimeStyles.RoundtripKind);

            // Refresh 5 minutes before actual expiry (safe buffer)
            return DateTime.UtcNow >= expiry.AddMinutes(-5);
        }

        public async Task<string> GetValidTokenAsync()
        {
            if (await IsSessionExpiredAsunc())
            {
                await RefreshTokenAsync();
            }

            return await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenKeyValue);
        }
    }
}
