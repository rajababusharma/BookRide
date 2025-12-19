using System.Threading.Tasks;

namespace BookRide.Views;

public partial class DriverRegistration : ContentPage
{
	public DriverRegistration()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        // Custom logic before navigating back
        Shell.Current.GoToAsync("..");

        // true = we handled it, stop default behavior
        return true;
    }
}