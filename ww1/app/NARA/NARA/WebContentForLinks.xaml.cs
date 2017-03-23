using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace NARA
{
    /// <summary>
    /// WebContentForLinks class inherits xamarin ContentPage and contains a webview which navigates to the page of the url parameter
    /// that is received through constructor
    /// </summary>
    public partial class WebContentForLinks : ContentPage
    {
        /// <summary>
        /// Webview of the page navigates to the received url
        /// </summary>
        /// <param name="url"></param>
        public WebContentForLinks(string url)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            //Back navigation
            var Back_Tap = new TapGestureRecognizer();
            Back_Tap.Tapped += (s, e) =>
            {
                NavigateHomepage();
            };

            img_Previous.GestureRecognizers.Add(Back_Tap);

            //Setting the webview to the url, that is received through constructor
            webView.Source = url;
        }

        /// <summary>
        /// Back navigation
        /// </summary>
        private async void NavigateHomepage()
        {
            await Navigation.PopAsync();
        }
    }
}
