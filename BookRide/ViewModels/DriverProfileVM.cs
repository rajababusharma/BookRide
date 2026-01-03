using BookRide.Interfaces;
using BookRide.Models;
using BookRide.Services;
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
        private readonly RealtimeDatabaseService _db;
        [ObservableProperty]
        public Users user;
        public DriverProfileVM() 
        {
            _db = new RealtimeDatabaseService();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Start a timer to deduct credit points every hour
                // Repeating timer that runs on the UI thread (MAUI)
                Application.Current?.Dispatcher.StartTimer(TimeSpan.FromHours(2), () =>
                {
                    // runs on main thread — safe to update observable properties
                    if (user.CreditPoint > 0)
                    {
                        user.CreditPoint -= 1;
                        // avoid .Wait() on async; fire-and-forget safely:
                        _ = _db.SaveAsync<Users>($"Users/{user.UserId}", user);
                    }
                    return true; // keep repeating
                });
            });
        }

        [RelayCommand]
        public async Task AddCreditAsync()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "CurrentUser", User }
            };
            await Shell.Current.GoToAsync(nameof(Views.RechargeCreditPage), navigationParameter);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            User = query["CurrentUser"] as Users;
            if(User.CreditPoint<5)
            {
               Shell.Current.DisplayAlert(
                  "Info",
                   $"Your current credit points are {User.CreditPoint}. Please recharge to add credit points to keep your account active.",
                  "OK");
            }
           
        }
    }
}
