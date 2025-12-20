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
    public partial class ConfirmRegistrationVM : ObservableObject
    {
        [RelayCommand]
        private async Task BackToLogin()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}
