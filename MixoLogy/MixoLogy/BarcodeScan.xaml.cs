using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;

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
            //await CrossMedia.Current.Initialize();

            //if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            //{
            //    await DisplayAlert("No Camera", ":( No camera available.", "OK");
            //    return;
            //}

            //var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());

            //if (file == null)
            //    return;

            //await DisplayAlert("File Location", file.Path, "OK");

            //photoImage.Source = ImageSource.FromStream(() =>
            //{
            //    var stream = file.GetStream();
            //    return stream;
            //});
        }

	    private void Handle_OnScanResult(Result result)
	    {
	        string curType = string.Empty;
	        Device.BeginInvokeOnMainThread(async () =>
	        {
	            var product = await RefreshDataAsync(result.Text);
	            foreach (string type in LiquorTypes.List())
	            {
	                if (product.ToLower().Contains(type))
	                {
	                    curType = type;
                        break;
	                }
	            }
                await DisplayAlert("Scanned result", curType, "OK");
	        });
        }

	    public async Task<string> RefreshDataAsync(string code)
	    {
	        var result = string.Empty;
	        HttpClient client = new HttpClient();
	        var uri = new Uri("https://api.barcodelookup.com/v2/products?barcode="+ code + "&formatted=y&key=ifDzhmKslKav42OD93NE");
	        var response = await client.GetAsync(uri);
	        if (response.IsSuccessStatusCode)
	        {
	            var content = await response.Content.ReadAsStringAsync();
	            dynamic dobj = JsonConvert.DeserializeObject<dynamic>(content);
	            result = dobj["products"][0]["product_name"];
            }

	        return result;
	    }
    }
}