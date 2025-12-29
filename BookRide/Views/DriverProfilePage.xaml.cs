using BookRide.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace BookRide.Views;

public partial class DriverProfilePage : ContentPage
{
	public DriverProfilePage(DriverProfileVM vM)
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