using BookRide.ViewModels;
using System.Threading.Tasks;

namespace BookRide.Views;

public partial class DriverRegistration : ContentPage
{
    public DriverRegistration(DriverRegistrationVM vM)
    {
        BindingContext = vM;
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
        //  return base.OnBackButtonPressed();
        return true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        //Task.Run(async () =>
        //{
        //    if (BindingContext is DriverRegistrationVM vm)
        //    {
        //       // await vm.GetCurrentLocationAsync();
               
        //    }
        //}); // <-- Moved closing parenthesis here to properly close Task.Run
    }
}