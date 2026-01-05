using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookRide.ViewModels
{
    public partial class TravellerProfileVM : ObservableObject, IQueryAttributable
    {
        private RealtimeDatabaseService _db;
        private ILocation _locationService;
        private readonly IWhatsAppConnect _whatsAppConnect;
        public ObservableCollection<string> Districts { get; }
      

        [ObservableProperty]
        public ObservableCollection<Users> usersList = new();

        [ObservableProperty]
        public Users user;
        [ObservableProperty]
        public string hi="Hi";
        [ObservableProperty]
        public string selectedDistrict;
        private readonly GeolocationRequest _geolocationRequest;

        private Location currentLocation;
        partial void  OnSelectedDistrictChanged(string value)
        {
            if (value == null) return;
            Console.WriteLine($"Selected: {value}");

            LoadUsersByDistrictAsync(value);
        }
        [RelayCommand]
        public async void WhatsappConnect(string phoneNumber)
        {
            if (phoneNumber != null && phoneNumber.Length == 10)
            {
                try
                {
                     _whatsAppConnect.WhatsappConnect("+91" + phoneNumber, $"Hello, my name is {User.FirstName} " + $" {User.LastName}" + " and I want to connect with you.");
                }
                catch (Exception exp)
                {
                    // Handle error
                    await Shell.Current.DisplayAlert(
                           "Error",
                           exp.Message,
                           "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert(
                          "Error",
                          "Incorrect phone number",
                          "OK");
            }
        }

        [RelayCommand]
        public async void Call(string phoneNumber)
        {
            if (phoneNumber != null && phoneNumber.Length == 10)
            {

                try
                {
                    //  PhoneDialer.Default.Open(phoneNumber);
                    await Launcher.Default.OpenAsync("tel:+91" + phoneNumber);

                }
                catch (Exception exp)
                {
                    // Handle error
                    await Shell.Current.DisplayAlert(
                           "Error",
                           exp.Message,
                           "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert(
                          "Error",
                          "Incorrect phone number",
                          "OK");
            }
        }

        public TravellerProfileVM(IWhatsAppConnect whatsApp)
        {
            _whatsAppConnect = whatsApp;
            Districts = new ObservableCollection<string>(UttarPradeshDistricts.All);
            _db = new RealtimeDatabaseService();
            _geolocationRequest = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
           
        }

        public async Task InitializeAsync()
        {
            // async work
            currentLocation = await Geolocation.Default.GetLocationAsync(_geolocationRequest);

           await LoadUsersByDistrictAsync("");
            //  await LoadDataAsync();
        }

        public async Task LoadUsersByDistrictAsync(string district)
        {
            try
                {
                UsersList.Clear();
                if (string.IsNullOrEmpty(district))

                {
                    Task.Run(async () =>
                    {
                        var users = await _db.GetAllAsync<Users>("Users").ContinueWith(t =>
                        {
                            var userList = t.Result.Where<Users>(x => x.CreditPoint > 0 && x.UserType.Equals(eNum.eNumUserType.Driver.ToString()));
                            return new ObservableCollection<Users>(userList);
                        });

                        UsersList = await GetLocationsWithinRadiusAsync(users);
                    }).GetAwaiter().GetResult();
                }
                else
                {
                    Task.Run(async () =>
                    {
                        var users = await _db.GetAllAsync<Users>("Users").ContinueWith(t =>
                        {
                            var userList = t.Result.Where<Users>(x => x.CreditPoint > 0 && x.UserType.Equals(eNum.eNumUserType.Driver.ToString()) && x.District.Equals(district));
                            return new ObservableCollection<Users>(userList);
                        });
                        UsersList = await GetLocationsWithinRadiusAsync(users);
                    }).GetAwaiter().GetResult();
                }

            }
            catch (Exception ex)
                {
                    // Handle exceptions related to geolocation
                    // Console.WriteLine($"Error obtaining location: {ex.Message}");
                    await Shell.Current.DisplayAlert(
                              "Error",
                              $"Error obtaining location: {ex.Message}",
                              "OK");
                }
          
        }

        public async Task<ObservableCollection<Users>> GetLocationsWithinRadiusAsync(ObservableCollection<Users> users)
        {
            try
            {
                 
                    if (currentLocation == null)
                    {
                        return users;
                    }


                    double radiusKm = 5.0;

                    foreach (var usr in users)
                    {
                        if (usr.Location != null)
                        {
                            // Calculate the distance in kilometers
                            double distance = currentLocation.CalculateDistance(usr.Location, DistanceUnits.Kilometers);
                            if (distance <= radiusKm)
                            {
                                UsersList.Add(usr);
                            }
                         }
                        else
                        {
                        UsersList.Add(usr);
                        }
                    }

                    return UsersList;
                
            }
            catch (Exception ex)
            {
                // Handle exceptions related to geolocation
               // Console.WriteLine($"Error obtaining location: {ex.Message}");
               
                return users; // Return the original list if location cannot be obtained

            }
           
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            User = query["CurrentUser"] as Users;
        }
    }
}
