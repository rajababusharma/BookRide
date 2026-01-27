using BookRide.Models;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookRide.Interfaces;

namespace BookRide.ViewModels
{
    public partial class RechargeCreditVM : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private Drivers user;
        private readonly IFirebaseUpload _firebaseUpload;

        // the property to bind image to UI
        [ObservableProperty]
        private ImageSource qrCode;
       private readonly IWhatsAppConnect _whatsAppConnect;
        public RechargeCreditVM(IWhatsAppConnect whatsApp, IFirebaseUpload firebaseUpload)
        {
            _whatsAppConnect = whatsApp;
            _firebaseUpload = firebaseUpload;
        }

        [RelayCommand]
        private async Task BackToLogin()
        {
           // await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            await Shell.Current.GoToAsync("..//..");
          //  await Shell.Current.GoToAsync("..");
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
                // Browse and pick an image
                var photo = await PickImageAsync();
                if (photo == null)
                {
                    return;
                }
                // Upload the image to Firebase and get the download URL
                var imageStream = await photo.OpenReadAsync();
                var imageUrl = await _firebaseUpload.UploadPaymentImagesToCloud(imageStream, User.UserId);
                // _whatsAppConnect.WhatsappConnect("918693849475", $"Hello, my name is {User.FirstName}"+" and I have done the payment and sharing the payment screenshot with you. "+ $"You can call me on {User.Mobile} if any query");


            }
            catch (Exception ex)
            {
                // Log or display the exception
                await Shell.Current.DisplayAlert("Error", $"Unable to share file: {ex.Message}", "OK");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                User = (Drivers)query["CurrentUser"];
                if(User == null)
                    {
                    return;
                }
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ApplyQueryAttributes: {ex.Message}");
            }
               
        }

    }
}
