namespace BookRide.Views;

public partial class DriverProfilePage : ContentPage
{
	public DriverProfilePage()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..");
        return true;
    }
}