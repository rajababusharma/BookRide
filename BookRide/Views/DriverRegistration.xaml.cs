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
        Shell.Current.GoToAsync("..//..//..");
        return true;
    }
}