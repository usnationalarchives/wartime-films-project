using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace NARA
{
    /// <summary>
    /// CuratorsPage class inherits xamarin ContentPage and contains info for the curators
    /// </summary>
    public partial class CuratorsPage : ContentPage
    {
        public CuratorsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            

            var Back_Tap = new TapGestureRecognizer();
            Back_Tap.Tapped += (s, e) =>
            {
                NavigateHomepage();
            };

            img_Previous.GestureRecognizers.Add(Back_Tap);

            TapGestureRecognizer tgr_Links = new TapGestureRecognizer();
            tgr_Links.Tapped += Tgr_Links_Tapped;

            lbl_Link.GestureRecognizers.Add(tgr_Links);
        }

        /// <summary>
        /// Method that handles navigation of the links in the "about" text
        /// </summary>
        private async void Tgr_Links_Tapped(object sender, EventArgs e)
        {
            try
            {
                var link = ((Label)sender).Text;

                if (CrossConnectivity.Current.IsConnected)
                {
                    await Navigation.PushAsync(new WebContentForLinks(link));
                }
                else
                {
                    await this.DisplayAlert("Error", "Check your internet connection", "Proceed");
                }
            }
            catch { }
        }

        /// <summary>
        /// Method that is called when size of the device screen is set, and contains
        /// properties of width and height of the screen, and makes some additional
        /// adjustments of the controls
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            icons.WidthRequest = width;
            stack_Text.WidthRequest = width * 0.6;
            grid_Logos.WidthRequest = width * 0.4;
        }

        /// <summary>
        /// Method that navigates to the homepage of the app
        /// </summary>
        private async void NavigateHomepage()
        {
            await Navigation.PopAsync();
        }
    }
}
