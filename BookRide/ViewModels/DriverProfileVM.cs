using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using BookRide.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public DriverProfileVM(IFirebaseUpload firebaseUpload, RealtimeDatabaseService databaseService)
        {
            _firebaseUpload = firebaseUpload;
            _db = databaseService;
           // _db = new RealtimeDatabaseService();
        }

        [RelayCommand]
        public async Task AddCreditAsync()
        {
            // Navigate to RechargeCreditPage with User as parameter if credit points are less than 1
            if (User.CreditPoint < 1)
            {
                //await Shell.Current.DisplayAlertAsync(
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
                await Shell.Current.DisplayAlertAsync(
                    "Alert",
                    $"You can add credit point when it reaches to zero",
                    "OK");
            }

        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("CurrentUser", out var userObj) || userObj is not Drivers user)
                return;

            // check if user is null
            if (user == null)
            {
                return;
            }

            User = user;

            // Run async logic without blocking navigation. Observe exceptions.
            InitializeUserAsync().ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    System.Diagnostics.Debug.WriteLine($"InitializeUserAsync error: {t.Exception}");
                }
            }, TaskScheduler.Default);
        }
        private async Task InitializeUserAsync()
        {
            try
            {
                // Ensure UI properties are set on the main thread
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ProfileImageUrl = string.IsNullOrEmpty(User.ProfileImageUrl)
                        ? "person.png"
                        : User.ProfileImageUrl;
                });

                var isActiveAndHasCredit = User.IsActive && User.CreditPoint > 0;
                if (isActiveAndHasCredit)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        IsActive = "Active";
                        IsVisible = false;
                    });
                    return;
                }

                if (!string.IsNullOrWhiteSpace(User.FirstName) &&
                    !string.IsNullOrWhiteSpace(User.UserId) &&
                    User.CreditPoint == 0 &&
                    !string.IsNullOrWhiteSpace(User.Mobile))
                {
                    await _db.SaveAsync<Drivers>($"Drivers/{User.UserId}", User);

                    User.IsActive = false;

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        IsActive = "Deactivated";
                        IsVisible = true;
                        await Shell.Current.DisplayAlertAsync(
                            "Low Credit",
                            $"Your current credit points are {User.CreditPoint}. Please recharge to keep your account active.",
                            "OK");
                    });

                    return;
                }

                // Deactivated cases
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    IsActive = "Deactivated";
                    IsVisible = true;
                    await Shell.Current.DisplayAlertAsync(
                        "Account Deactivated",
                        "Your account has been deactivated due to a compliance reason. Please contact support.",
                        "OK");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"User init error: {ex}");
            }
        }
        // add profile photo command
        [RelayCommand]
        public async Task AddProfilePhotoAsync()
        {
            if (User == null)
                return;

            IsBusy = true;
            try
            {
                var photos = await MediaPicker.Default.PickPhotosAsync();
                if (photos == null || photos.Count == 0)
                    return;

                var photo = photos.First();
                // Save the file into firebase storage and get the URL
                using var imageStream = await photo.OpenReadAsync();

                var imageUrl = await _firebaseUpload.UploadProfieImagesToCloud(imageStream, User.UserId);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    User.ProfileImageUrl = imageUrl;
                    await MainThread.InvokeOnMainThreadAsync(() => ProfileImageUrl = imageUrl);

                    await _db.SaveAsync<Drivers>($"Drivers/{User.UserId}", User);
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("DriverProfile", "Failed to upload profile photo.", "OK"));
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Shell.Current.DisplayAlertAsync("DriverProfile Error", $"An error occurred: {ex.Message}", "OK"));
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task UpdateProfileAsync()
        {

            var navigationParameter = new Dictionary<string, object>
                    {
                        { "CurrentUser", User }
                    };
            await Shell.Current.GoToAsync(nameof(DriverRegistration), navigationParameter);

        }
    }
}

