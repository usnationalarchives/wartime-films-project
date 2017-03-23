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
    /// TeachersPage class inherits xamarin ContentPage and contains info for the curators
    /// </summary>
    public partial class TeachersPage : ContentPage
    {
        public TeachersPage()
        {
            //Initialization of the xaml components
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            //Tap gesture for back navigation
            var Back_Tap = new TapGestureRecognizer();
            Back_Tap.Tapped += (s, e) =>
            {
                NavigateHomepage();
            };

            img_Previous.GestureRecognizers.Add(Back_Tap);

            //Event that handles links in the text
            TapGestureRecognizer tgr_Links = new TapGestureRecognizer();
            tgr_Links.Tapped += Tgr_Links_Tapped;

            lbl_Link1.GestureRecognizers.Add(tgr_Links);
            lbl_Link2.GestureRecognizers.Add(tgr_Links);
        }

        /// <summary>
        /// Method that represents the mail composer component to the user
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
            catch(Exception exc)
            { }
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
            await Navigation.PopToRootAsync();
        }
    }
}
