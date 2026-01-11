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

        [ObservableProperty]
        public Users user;

        [ObservableProperty]
        public bool isVisible;

        public DriverProfileVM() 
        {
           
        }

        [RelayCommand]
        public async Task AddCreditAsync()
        {
            // Navigate to RechargeCreditPage with User as parameter if credit points are less than 1
            if(User.CreditPoint < 1)
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
            User = query["CurrentUser"] as Users;
            if (User.CreditPoint > 0)
            {
                IsVisible = false;
            }
            else
            {
                IsVisible = true;
            }
            //if(User.CreditPoint<5)
            //{
            //   Shell.Current.DisplayAlert(
            //      "Info",
            //       $"Your current credit points are {User.CreditPoint}. Please recharge to add credit points to keep your account active.",
            //      "OK");
            //}

        }
    }
}
