namespace BookRide.Views;

public partial class TravellerProfilePage : ContentPage
{
	public TravellerProfilePage()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..");
        return true;
    }
}