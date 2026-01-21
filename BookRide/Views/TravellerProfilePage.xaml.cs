using BookRide.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Threading.Tasks;

namespace BookRide.Views;

public partial class TravellerProfilePage : ContentPage
{
	public TravellerProfilePage(TravellerProfileVM vM)
    {
        BindingContext = vM;
        
		InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..");
        return true;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TravellerProfileVM vm)
            await vm.InitializeAsync();
        }
}