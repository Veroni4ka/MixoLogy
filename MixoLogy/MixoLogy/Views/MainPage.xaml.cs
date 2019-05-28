﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using Xamarin.Forms;
using Xamarin.Forms.StyleSheets;

namespace MixoLogy
{
    public partial class MainPage : ContentPage
    {
        private int _pg = 1;
        private List<Cocktail> _cocktails;
        public MainPage()
        {
            InitializeComponent();
            Resources.Add(StyleSheet.FromAssemblyResource(
                typeof(MainPage).GetTypeInfo().Assembly,
                "MixoLogy.Assets.styles.css"));

            _cocktails = Cocktail.LoadCocktailsCollection();
            CreateGrid(_cocktails);
            var button = new Button
            {
                Text = "Load More",
                BackgroundColor = Color.HotPink,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(5, 5, 5, 40)
            };
            button.Clicked += LoadMore;
            stackLayout.Children.Add(button);
        }

        protected override void OnAppearing()
        {
            Shell.SetNavBarIsVisible(this, false);

            base.OnAppearing();
        }

        public interface IFileService
        {
            StreamReader GetFileStream(string name);
        }

        

        private void CreateGrid(List<Cocktail> cocktails)
        {
            if (cocktails != null && cocktails.Any())
            {
                foreach (var cocktail in cocktails.Skip((_pg - 1) * 10).Take(_pg * 10))
                {
                    StackLayout stackLayout = new StackLayout();
                    var image = new ImageButton
                    {
                        Source = ImageSource.FromUri(new Uri(cocktail.StrDrinkThumb)),
                        WidthRequest = 140,
                        BorderColor = Color.Aquamarine,
                        BorderWidth = 1,
                        CommandParameter = cocktail
                    };
                    image.Margin = new Thickness(15, 15, 15, 15);
                    image.VerticalOptions = LayoutOptions.CenterAndExpand;
                    image.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    image.Clicked += ImageOnClicked;
                    stackLayout.Children.Add(image);
                    flexLayout.Children.Add(stackLayout);
                }
                

                _pg++;
            }
        }

        private async void ImageOnClicked(object sender, EventArgs e)
        {
            var btn = sender as ImageButton;
            var cocktail = btn?.CommandParameter as Cocktail;
            FieldInfo[] fields = typeof(Cocktail).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            var ingredients = fields.Where(x => x.Name.Contains("StrIngredient") && x.GetValue(cocktail).ToString() != "").ToList();
            var measurements = fields.Where(x => x.Name.Contains("StrMeasure") && x.GetValue(cocktail).ToString() != "").ToList();

            var instructions = "Ingredients:\n";
            for (var i = 0; i < ingredients.Count; i++)
            {
                instructions += ingredients[i].GetValue(cocktail) + " " + measurements[i].GetValue(cocktail) + "\n";
            }

            if (cocktail != null)
            {
                var answer = await DisplayAlert(cocktail.StrDrink, instructions, "Continue", "Close");
                if (answer)
                {
                    await DisplayAlert(cocktail.StrDrink, "Recipe: " + cocktail.StrInstructions, "Close");
                }
            }
        }

        private void LoadMore(object sender, EventArgs e)
        {
            CreateGrid(_cocktails);
        }

        private void RandomRecipe(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RandomRecipe());
        }

        private void AlcoholRecognize(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BarcodeScan());
        }
    }
}
