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

        public List<Location> _nearbyLocationsTask;

        [ObservableProperty]
        public Users user;
        [ObservableProperty]
        public string hi="Hi";
        [ObservableProperty]
        public string selectedDistrict;
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
          //  _locationService = new LocationService();
             LoadUsersByDistrictAsync("");
        }

        public async Task LoadUsersByDistrictAsync(string district)
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
                    UsersList = users;
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
                    UsersList = users;
                }).GetAwaiter().GetResult();
            }
           
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            User = query["CurrentUser"] as Users;
        }

        public async Task<List<Location>> GetLocationsWithinRadiusAsync()
        {
            var currentLocation = await _locationService.GetCurrentLocationAsync();
            if (currentLocation == null)
            {
                return new List<Location>();
            }

            List<Location> targetLocations = new List<Location>();
            foreach (var usr in usersList)
            {
                targetLocations.Add(usr.Location);
            }
            var locationsInRadius = new List<Location>();
            double radiusKm = 5.0;

            foreach (var location in targetLocations)
            {
                // Calculate the distance in kilometers
                double distance = currentLocation.CalculateDistance(location, DistanceUnits.Kilometers);

                if (distance <= radiusKm)
                {
                    locationsInRadius.Add(location);
                }
            }

            return locationsInRadius;
        }

       
    }
}
