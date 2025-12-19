using BookRide.ViewModels;

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
}