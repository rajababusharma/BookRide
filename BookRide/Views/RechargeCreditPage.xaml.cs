using BookRide.ViewModels;

namespace BookRide.Views;

public partial class RechargeCreditPage : ContentPage
{
	public RechargeCreditPage(RechargeCreditVM vM)
	{
		BindingContext = vM;
        InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..//..");
        return true;
    }
}