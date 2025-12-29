using BookRide.Models;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.ViewModels
{
    public partial class ConfirmRegistrationVM : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private Users user;

        // the property to bind image to UI
        [ObservableProperty]
        private ImageSource qrCode;

        public ConfirmRegistrationVM()
        {
            
        }

        [RelayCommand]
        private async Task BackToLogin()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

        private async Task<FileResult> PickImageAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Pick an image",
                    // Restrict file types to images only
                    FileTypes = FilePickerFileType.Images
                });

                if (result == null)
                    return null;

                // Check if the file is an image (optional, as FilePickerFileType.Images helps)
                if (!result.ContentType.StartsWith("image/"))
                {
                    await Shell.Current.DisplayAlert("Error", "Selected file is not an image.", "OK");
                    return null;
                }

                return result;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error picking file: {ex.Message}");
                return null;
            }
        }
        [RelayCommand]
        public async Task UploadImageAsync()
        {
            try
            {
                var photo = await PickImageAsync();
                if (photo == null)
                {
                    return;
                }
           
                // 1. Define the filename and path where the image will be stored temporarily
                string fileName = photo.FileName;
                string file = Path.Combine(FileSystem.CacheDirectory, fileName);

                // NOTE: Replace this with your actual image data (e.g., from a byte array, stream, or resource)
                // This example assumes you have an image source and save it to the file path.
                // For demonstration, we just write some placeholder data, you'd put your image bytes here.
                // For a real app, you would load your image into a byte array or stream and write it.
                // Example: await File.WriteAllBytesAsync(file, yourImageByteArray);
                // We'll simulate by creating a dummy file:
                if (!File.Exists(file))
                {
                    await File.WriteAllTextAsync(file, "This is a dummy image file content.");
                    // In a real app, ensure you save a valid image format (JPEG, PNG, etc.)
                }

                // 2. Use the .NET MAUI Share API to request sharing
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Share Image via WhatsApp",
                    File = new ShareFile(file)
                });
            }
            catch (Exception ex)
            {
                // Log or display the exception
                await Shell.Current.DisplayAlert("Error", $"Unable to share file: {ex.Message}", "OK");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            User = query["CurrentUser"] as Users;
            if (User != null && (User.VehicleType.Equals(eNum.eNumVehicleType.Car.ToString()) || User.VehicleType.Equals(eNum.eNumVehicleType.Tempo.ToString())))
            {
              
                QrCode = ImageSource.FromFile("car_tempo.png");
            }
            if (User != null && (User.VehicleType.Equals(eNum.eNumVehicleType.AutoRickshaw.ToString())))
            {
               
                QrCode = ImageSource.FromFile("for_motor_autorickshaw.png");
            }
            if (User != null && (User.VehicleType.Equals(eNum.eNumVehicleType.Bike.ToString()) || User.VehicleType.Equals(eNum.eNumVehicleType.Scooty.ToString())))
            {
                QrCode = ImageSource.FromFile("bike.png");
              
            }
            if (User != null && (User.VehicleType.Equals(eNum.eNumVehicleType.Bus.ToString()) || User.VehicleType.Equals(eNum.eNumVehicleType.Truck.ToString())))
            {
                QrCode = ImageSource.FromFile("bus_truck.png");
            }
        }


        //private async Task UploadImageAsync()
        //{
        //    var photo = await PickImageAsync();
        //    if (photo == null)
        //        return;

        //    // Read the file into a stream
        //    using Stream sourceStream = await photo.OpenReadAsync();
        //    // Convert the stream to a byte array for the request
        //    using MemoryStream memoryStream = new MemoryStream();
        //    await sourceStream.CopyToAsync(memoryStream);
        //    byte[] imageBytes = memoryStream.ToArray();

        //    var client = new HttpClient();
        //    var myServerUrl = "https://example.com/api/upload"; // Replace with your API endpoint

        //    // Create the HTTP content
        //    var content = new ByteArrayContent(imageBytes);
        //    // Set the correct MIME type
        //    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);

        //    // Send the POST request
        //    var response = await client.PostAsync(myServerUrl, content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Handle success
        //        await Shell.Current.DisplayAlert("Success", "Image uploaded successfully!", "OK");
        //    }
        //    else
        //    {
        //        // Handle error
        //        await Shell.Current.DisplayAlert("Error", $"Upload failed: {response.ReasonPhrase}", "OK");
        //    }
        //}


    }
}
