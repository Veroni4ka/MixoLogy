using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Essentials;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
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

	    private async void ScanBtn_OnClicked(object sender, EventArgs e)
	    {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());

            if (file == null)
                return;

            await DisplayAlert("File Location", file.Path, "OK");

            photoImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });
        }
	}
}