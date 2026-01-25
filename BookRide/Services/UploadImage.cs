using BookRide.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class UploadImage : IFirebaseUpload
    {
        public async Task<string> UploadAadharImagesToCloud(Stream filestream, string filename)
        {
            try
            {
                var appid = Constants.Constants.Firebase_project_id;

                //  string bucket = $"{appid}.appspot.com";
                // string fileName = $"{Guid.NewGuid()}.jpg";
                string fileName = $"{filename}_Aadhar_{DateTime.Now.Ticks}.jpg";

                var uploadUrl =
                    $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o" +
                    $"?uploadType=media&name={Constants.Constants.Firebase_AadharLocation}/{fileName}";

                using var httpClient = new HttpClient();
                using var content = new StreamContent(filestream);

                content.Headers.ContentType =
                    new MediaTypeHeaderValue("image/jpeg");

                Console.WriteLine("Uploading Aadhar image to Firebase Storage...");
                var response = await httpClient.PostAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();

                var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o/{Constants.Constants.Firebase_AadharLocation}%2F{fileName}?alt=media";
                await Shell.Current.DisplayAlert("Success", "File uploaded successfully.", "OK");
                Console.WriteLine("Aadhar image uploaded successfully.");
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
                var appid = Constants.Constants.Firebase_project_id;

                //  string bucket = $"{appid}.appspot.com";
                // string fileName = $"{Guid.NewGuid()}.jpg";
                string fileName = $"{filename}_Payments_{DateTime.Now.Ticks}.jpg";

                var uploadUrl =
                    $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o" +
                    $"?uploadType=media&name={Constants.Constants.Firebase_PaymentImageLocation}/{fileName}";

                using var httpClient = new HttpClient();
                using var content = new StreamContent(filestream);

                content.Headers.ContentType =
                    new MediaTypeHeaderValue("image/jpeg");

                Console.WriteLine("Uploading payment image to Firebase Storage...");
                var response = await httpClient.PostAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();

                var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o/{Constants.Constants.Firebase_PaymentImageLocation}%2F{fileName}?alt=media";
                await Shell.Current.DisplayAlert("Success", "File uploaded successfully.", "OK");
                Console.WriteLine("Payment image uploaded successfully.");
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
                var appid = Constants.Constants.Firebase_project_id;

                //  string bucket = $"{appid}.appspot.com";
                // string fileName = $"{Guid.NewGuid()}.jpg";
                string fileName = $"{filename}_Profile_{DateTime.Now.Ticks}.jpg";

                var uploadUrl =
                    $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o" +
                    $"?uploadType=media&name={Constants.Constants.Firebase_ProfileImageLocation}/{fileName}";

                using var httpClient = new HttpClient();
                using var content = new StreamContent(filestream);

                content.Headers.ContentType =
                    new MediaTypeHeaderValue("image/jpeg");

                Console.WriteLine("Uploading profile image to Firebase Storage...");
                var response = await httpClient.PostAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();

                var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{Constants.Constants.Firebase_Bucket}/o/{Constants.Constants.Firebase_ProfileImageLocation}%2F{fileName}?alt=media";
                await Shell.Current.DisplayAlert("Success", "Profile photo updated successfully.", "OK");
                Console.WriteLine("Profile image uploaded successfully.");
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
