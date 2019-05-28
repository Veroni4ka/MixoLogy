using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
	    List<Cocktail> _cocktails = Cocktail.LoadCocktailsCollection();

        public BarcodeScan ()
		{
			InitializeComponent ();
		}

	    protected override void OnAppearing()
	    {
	        Shell.SetNavBarIsVisible(this, false);

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

	            if (curType != null || curType != string.Empty)
	            {
                    DisplayRecipe(AlcCocktail(curType));
	            }
                

	        });
        }

	    public Cocktail AlcCocktail(string alcType)
	    {
            Dictionary<string, string> ingredients = new Dictionary<string, string>();
	        FieldInfo[] fields = typeof(Cocktail).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (var cocktailItem in _cocktails)
	        {
	            List<string> instructions = new List<string>();
                var ingredientsList = fields.Where(x => x.Name.Contains("StrIngredient") && x.GetValue(cocktailItem).ToString() != "")
	            .ToList();
	            foreach (var t in ingredientsList)
	            {
	                instructions.Add(t.GetValue(cocktailItem).ToString());
	            }

	            ingredients.Add(cocktailItem.IdDrink, string.Join(", ", instructions));
	        }

	        var drinkList = ingredients.Where(x => x.Value.ToLower().Contains(alcType)).Select(i => i.Key).ToList();
	        if (drinkList.Any())
	        {
                var rnd = new Random();
	            var num = rnd.Next(1, drinkList.Count());
	            var drinkId = drinkList.ElementAt(num);

	            return _cocktails.First(x => x.IdDrink == drinkId);
	        }
	        
            return new Cocktail();
	    }

	    public void DisplayRecipe(Cocktail cocktail)
	    {
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

	    public async Task<string> RefreshDataAsync(string code)
	    {
	        var result = string.Empty;
	        HttpClient client = new HttpClient();
	        var uri = new Uri("https://api.barcodelookup.com/v2/products?barcode="+ code + "&formatted=y&key=xxx");
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