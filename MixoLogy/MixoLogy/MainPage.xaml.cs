using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MixoLogy
{
    public partial class MainPage : ContentPage
    {
        private int _pg = 1;
        private List<Cocktail> _cocktails;
        public MainPage()
        {
            InitializeComponent();
            _cocktails = LoadCocktailsCollection();
            CreateGrid(_cocktails);
            
        }

        public interface IFileService
        {
            StreamReader GetFileStream(string name);
        }

        private List<Cocktail> LoadCocktailsCollection()
        {
            int imageDimension = Device.RuntimePlatform == Device.iOS ||
                                 Device.RuntimePlatform == Device.Android ? 240 : 120;

            var assembly = typeof(Cocktail).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MixoLogy.Resources.all_drinks.csv");
            //Stream stream = assembly.GetManifestResourceStream(fileName);
            //var csvData = DataTable.New.ReadLazy(stream);
            //return csvData.RowsAs<Cocktail>().ToList();
            var list = new List<Cocktail>();
            using (stream)
            {
                if (stream != null)
                    using (var reader = new StreamReader(stream))
                    {
                        using (var csv = new CsvReader(reader))
                        {

                            csv.Configuration.Delimiter = ",";
                            csv.Configuration.PrepareHeaderForMatch = (header, index) => header.ToLower();
                            while (csv.Read())
                            {
                                var row = csv.GetRecord<Cocktail>();
                                if (row == null)
                                {
                                    break;
                                }
                                list.Add(row);
                            }
                        }
                    }
            }

            return list;
        }

        private void CreateGrid(List<Cocktail> cocktails)
        {
            if (cocktails != null && cocktails.Any())
            {
                if (flexLayout.Children.Count > 1)
                {
                    flexLayout.Children.RemoveAt(flexLayout.Children.Count - 1);
                }

                // Create an Image object for each bitmap
                foreach (var cocktail in cocktails.Skip((_pg - 1) * 10).Take(_pg * 10))
                {
                    StackLayout stackLayout = new StackLayout();
                    var image = new ImageButton()
                    {
                        Source = ImageSource.FromUri(new Uri(cocktail.StrDrinkThumb)),
                        WidthRequest = 140,
                        BorderColor = Color.Aquamarine,
                        BorderWidth = 1
                    };
                    stackLayout.Children.Add(image);
                    image.Margin = new Thickness(15, 15, 15, 15);
                    image.VerticalOptions = LayoutOptions.CenterAndExpand;
                    image.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    flexLayout.Children.Add(stackLayout);
                }
                var button = new Button
                {
                    Text = "Load More",
                    Visual = new VisualMarker.MaterialVisual(),
                    BackgroundColor = Color.HotPink,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    WidthRequest = 330,
                    Margin = new Thickness(5, 5, 5, 5)
                };
                button.Clicked += LoadMore;
                flexLayout.Children.Add(button);

                _pg++;
            }
        }

        private void LoadMore(object sender, EventArgs e)
        {
            CreateGrid(_cocktails);
        }
    }
}
