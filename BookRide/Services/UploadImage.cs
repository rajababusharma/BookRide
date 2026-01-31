using BookRide.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class UploadImage : IFirebaseUpload
    {
       
        public async Task<ImageSource> DownloadImageStream(string? downloadUrl)
        {
            var idToken = await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenKeyValue);
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", idToken);

            var imageStream = await httpClient.GetStreamAsync(downloadUrl);
            // return stream;
            if (imageStream == null)
                return null;

            // Important: copy stream because MAUI may reuse it
            var memoryStream = new MemoryStream();
            imageStream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            // ProfileImage = ImageSource.FromStream(() => memoryStream);
            return ImageSource.FromStream(() => memoryStream);
            //await MainThread.InvokeOnMainThreadAsync(() =>
            //{
            //    ProfileImage.Source = ImageSource.FromStream(() => stream);
            //});
        }

        public async Task<string> UploadAadharImagesToCloud(Stream filestream, string filename)
        {
            try
            {
                var token = await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenKeyValue);
                var appid = Constants.Constants.Firebase_project_id;

                //  string bucket = $"{appid}.appspot.com";
                // string fileName = $"{Guid.NewGuid()}.jpg";
                string fileName = $"{filename}_Aadhar_{DateTime.Now.Ticks}.jpg";

                var uploadUrl =
                    $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o" +
                    $"?uploadType=media&name={Constants.Constants.Firebase_AadharLocation}/{fileName}";

                using var httpClient = new HttpClient();
                using var content = new StreamContent(filestream);
                System.Diagnostics.Debug.WriteLine($"TOKEN: {token}");
                httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                content.Headers.ContentType =
                    new MediaTypeHeaderValue("image/jpeg");

                Console.WriteLine("Uploading Aadhar image to Firebase Storage...");
                var response = await httpClient.PostAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();

                var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o/{Constants.Constants.Firebase_AadharLocation}%2F{fileName}?alt=media";
                await Shell.Current.DisplayAlert("Success", "File uploaded successfully.", "OK");
              //  Console.WriteLine("Aadhar image uploaded successfully.");
                return downloadUrl;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
                return null;
            }
        }

     

        public async Task<string> UploadPaymentImagesToCloud(Stream filestream, string filename)
        {
            try
            {
                var token = await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenKeyValue);
                var appid = Constants.Constants.Firebase_project_id;

                //  string bucket = $"{appid}.appspot.com";
                // string fileName = $"{Guid.NewGuid()}.jpg";
                string fileName = $"{filename}_Payments_{DateTime.Now.Ticks}.jpg";

                var uploadUrl =
                    $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o" +
                    $"?uploadType=media&name={Constants.Constants.Firebase_PaymentImageLocation}/{fileName}";

                using var httpClient = new HttpClient();
                using var content = new StreamContent(filestream);
                System.Diagnostics.Debug.WriteLine($"TOKEN: {token}");
                httpClient.DefaultRequestHeaders.Authorization =
   new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                content.Headers.ContentType =
                    new MediaTypeHeaderValue("image/jpeg");

                Console.WriteLine("Uploading payment image to Firebase Storage...");
                var response = await httpClient.PostAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();

               // var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o/{Constants.Constants.Firebase_PaymentImageLocation}%2F{fileName}?alt=media";
                var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o/{Constants.Constants.Firebase_PaymentImageLocation}%2F{fileName}?alt=media";
                await Shell.Current.DisplayAlert("Success", "File uploaded successfully.", "OK");
              //  Console.WriteLine("Payment image uploaded successfully.");
                return downloadUrl;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
                return null;
            }
        }

        public async Task<string> UploadProfieImagesToCloud(Stream filestream, string filename)
        {
            try
            {
                var token = await SecureStorage.GetAsync(Constants.Constants.Firebase_TokenKeyValue);
                var appid = Constants.Constants.Firebase_project_id;

                //  string bucket = $"{appid}.appspot.com";
                // string fileName = $"{Guid.NewGuid()}.jpg";
                string fileName = $"{filename}_Profile_{DateTime.Now.Ticks}.jpg";

                var uploadUrl =
                    $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o" +
                    $"?uploadType=media&name={Constants.Constants.Firebase_ProfileImageLocation}/{fileName}";

                using var httpClient = new HttpClient();
                using var content = new StreamContent(filestream);
                System.Diagnostics.Debug.WriteLine($"TOKEN: {token}");

                httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


                content.Headers.ContentType =
                    new MediaTypeHeaderValue("image/jpeg");

                Console.WriteLine("Uploading profile image to Firebase Storage...");

               // var url = $"https://firebasestorage.googleapis.com/v0/b/YOUR_BUCKET_NAME/o?uploadType=media&name={fileName}";

                var response = await httpClient.PostAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();

              //  var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o/{Constants.Constants.Firebase_ProfileImageLocation}%2F{fileName}?alt=media";
                  var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o/{Constants.Constants.Firebase_ProfileImageLocation}%2F{fileName}?alt=media";
            
                await Shell.Current.DisplayAlert("Success", "Profile photo updated successfully.", "OK");
               // Console.WriteLine("Profile image uploaded successfully.");
                return downloadUrl;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
                return null;
            }
        }

     
    }
}
