using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.ViewModels
{
    public partial class DriverProfileVM : ObservableObject, IQueryAttributable
    {

        [ObservableProperty]
        public Drivers user;

        [ObservableProperty]
        public bool isVisible;

        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        private string? profileImageUrl;

        [ObservableProperty]
        private string isActive;

        private readonly IFirebaseUpload _firebaseUpload;

        private readonly RealtimeDatabaseService _db;
        public DriverProfileVM(IFirebaseUpload firebaseUpload)
        {
            _firebaseUpload = firebaseUpload;
            _db = new RealtimeDatabaseService();
        }

        [RelayCommand]
        public async Task AddCreditAsync()
        {
            // Navigate to RechargeCreditPage with User as parameter if credit points are less than 1
            if (User.CreditPoint < 1)
            {
                //await Shell.Current.DisplayAlert(
                //    "Insufficient Credit Points",
                //    "Your credit points are insufficient. Please recharge to continue using our services.",
                //    "OK");
                var navigationParameter = new Dictionary<string, object>
                    {
                        { "CurrentUser", User }
                    };
                await Shell.Current.GoToAsync(nameof(Views.RechargeCreditPage), navigationParameter);
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Alert",
                    $"You can add credit point when it reaches to zero",
                    "OK");
            }

        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            User = query["CurrentUser"] as Drivers;

            Task.Run(async () => {
                ProfileImageUrl = User.ProfileImageUrl;

                // check if profile image is null or empty
                if (string.IsNullOrEmpty(ProfileImageUrl))
                {
                    ProfileImageUrl = "person.png";
                }

                // check if user is active
                if (User.IsActive && User.CreditPoint > 0)
                {
                    IsActive = "Active";
                    IsVisible = false;
                }
                else if (User.CreditPoint == 0)
                {
                    // update IsActive info in the users profile
                    User.IsActive = false;
                    IsVisible = true;
                    await _db.SaveAsync<Drivers>($"Drivers/{User.UserId}", User);
                    IsActive = "Deactivated";
                    await Shell.Current.DisplayAlert(
                        "Info",
                         $"Your current credit points are {User.CreditPoint}. Please recharge to add credit points to keep your account active.",
                        "OK");
                }
                else if (!User.IsActive)
                {
                    IsActive = "Deactivated";
                    await Shell.Current.DisplayAlert(
                        "Info",
                         $"Your account has been deactivated due to some complaince reason. Please contact to our support system.",
                        "OK");
                }
                else
                {
                    IsActive = "Deactivated";
                    await Shell.Current.DisplayAlert(
                        "Info",
                         $"Your account has been deactivated due to some complaince reason. Please contact to our support system.",
                        "OK");
                }
            });

           

        }

        // add profile photo command
        [RelayCommand]
        public async Task AddProfilePhotoAsync()
        {
            IsBusy = true;
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null)
                return;
            // Save the file into firebase storage and get the URL
            var imageStream = await photo.OpenReadAsync();
            var imageUrl = await _firebaseUpload.UploadProfieImagesToCloud(imageStream, User.UserId);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                User.ProfileImageUrl = imageUrl;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ProfileImageUrl = imageUrl;
                });
               
                await _db.SaveAsync<Drivers>($"Drivers/{User.UserId}", User);
              //  await Shell.Current.DisplayAlert("Success", "Profile photo updated successfully.", "OK");

               IsBusy = false;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to upload profile photo.", "OK");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task UpdateProfileAsync()
        {

            var navigationParameter = new Dictionary<string, object>
                    {
                        { "DRIVERS", User }
                    };
            await Shell.Current.GoToAsync(nameof(DriverRegistration), navigationParameter);

        }
    }
}

