using BookRide.ViewModels;
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


    //protected override void OnAppearing()
    // {
    //     base.OnAppearing();
    //     //Task.Run(async () =>
    //     //{
    //     //    if (BindingContext is TravellerProfileVM vm)
    //     //    {
    //     //        var location = await vm.GetLocationsWithinRadiusAsync();
    //     //        if (location != null)
    //     //        {
    //     //            vm._nearbyLocationsTask = location;
    //     //        }
    //     //    }
    //     //}); // <-- Moved closing parenthesis here to properly close Task.Run
    // }


    //private void pkrdistrict_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    var dist = sender as string;
    //    if (BindingContext is TravellerProfileVM vm)
    //    {
    //        vm.SelectedDistrict = dist;

    //         vm.LoadUsersByDistrictAsync(vm.SelectedDistrict);
    //    }
    //}
}