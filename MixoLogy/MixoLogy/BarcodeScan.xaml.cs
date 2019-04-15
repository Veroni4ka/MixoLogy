using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MixoLogy
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BarcodeScan : ContentPage
	{
		public BarcodeScan ()
		{
			InitializeComponent ();
		}

	    protected override void OnAppearing()
	    {
	        NavigationPage.SetHasNavigationBar(this, false);

	        base.OnAppearing();
	    }

        private void CocktailListBtn_OnClicked(object sender, EventArgs e)
	    {
	        Navigation.PushAsync(new MainPage());
	    }

	    private void RandomRecipeBtn_OnClicked(object sender, EventArgs e)
	    {
	        Navigation.PushAsync(new RandomRecipe());
        }
	}
}