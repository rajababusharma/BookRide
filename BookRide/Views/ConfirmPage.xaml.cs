using AndroidX.Core.View;
using Microsoft.Maui.Platform;

namespace BookRide.Views;

public partial class ConfirmPage : ContentPage
{
	public ConfirmPage()
	{
		InitializeComponent();

	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        // await Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
        await Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
    }

    protected override bool OnBackButtonPressed()
    {
         Shell.Current.GoToAsync("//MainPage"); // Navigates to the root of the Page
        return true;
    }

    override protected void OnAppearing()
    {
        base.OnAppearing();
    }
}