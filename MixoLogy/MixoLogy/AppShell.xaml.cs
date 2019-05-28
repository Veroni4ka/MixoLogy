using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MixoLogy
{
	public partial class AppShell : Shell
	{
	    Dictionary<string, Type> routes = new Dictionary<string, Type>();
	    public Dictionary<string, Type> Routes { get { return routes; } }

        public AppShell ()
		{
		    InitializeComponent();
		    RegisterRoutes();
		    BindingContext = this;
        }


        void RegisterRoutes()
	    {
	        routes.Add("main", typeof(MainPage));
	        routes.Add("barcode", typeof(BarcodeScan));
	        routes.Add("random", typeof(Random));

	        foreach (var item in routes)
	        {
	            Routing.RegisterRoute(item.Key, item.Value);
	        }
	    }

    }
}