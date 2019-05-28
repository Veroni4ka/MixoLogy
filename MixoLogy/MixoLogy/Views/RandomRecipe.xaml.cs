using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MixoLogy
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RandomRecipe : ContentPage
	{
	    List<Cocktail> _cocktails = Cocktail.LoadCocktailsCollection();
	    SensorSpeed speed = SensorSpeed.Game;

        public RandomRecipe ()
		{
			InitializeComponent ();
            DisplayRecipe();
            ToggleAccelerometer();
		    Accelerometer.ShakeDetected += Accelerometer_ShakeDetected;
        }

	    protected override void OnAppearing()
	    {
	        Shell.SetNavBarIsVisible(this, false);

	        base.OnAppearing();
	    }

        private Cocktail GenerateRecipe()
	    {
	        var rnd = new Random();
	        var num = rnd.Next(1, _cocktails.Count);
	        return _cocktails[num];
	    }

	    public void ToggleAccelerometer()
	    {
	        try
	        {
	            if (Accelerometer.IsMonitoring)
	                Accelerometer.Stop();
	            else
	                Accelerometer.Start(speed);
	        }
	        catch (FeatureNotSupportedException fnsEx)
	        {
	            DisplayAlert("Not supported", "Shake detection is not supported on your device", "Ok");
	        }
	        catch (Exception ex)
	        {
	            // Other error has occurred.
	        }
	    }
        private void CocktailListBtn_OnClicked(object sender, EventArgs e)
	    {
	        Navigation.PushAsync(new MainPage());
	    }

	    void Accelerometer_ShakeDetected(object sender, EventArgs e)
	    {
	        DisplayRecipe();
	    }

	    private void DisplayRecipe()
	    {
	        var cocktail = GenerateRecipe();

	        FieldInfo[] fields = typeof(Cocktail).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
	        var ingredients = fields.Where(x => x.Name.Contains("StrIngredient") && x.GetValue(cocktail).ToString() != "")
	            .ToList();
	        var measurements = fields.Where(x => x.Name.Contains("StrMeasure") && x.GetValue(cocktail).ToString() != "")
	            .ToList();

	        var instructions = "Ingredients:\n\n";
	        for (var i = 0; i < ingredients.Count; i++)
	        {
	            instructions += ingredients[i].GetValue(cocktail) + " " + measurements[i].GetValue(cocktail) + "\n";
	        }

	        drinkTitleLbl.Text = cocktail.StrDrink;
	        ingredientsLbl.Text = instructions;
	        recipeLbl.Text = "Instructions:\n\n" + cocktail.StrInstructions;
	        cocktailImg.Source = ImageSource.FromUri(new Uri(cocktail.StrDrinkThumb));
	    }

        private void AlcoholRecognizeBtn_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BarcodeScan());
        }
    }
}