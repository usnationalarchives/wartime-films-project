using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace NARA
{
    /// <summary>
    /// AboutPage class inherits xamarin ContentPage and contains basic info of the app
    /// </summary>
    public partial class AboutPage : ContentPage
    {

        public AboutPage()
        {
            //Initialization of the xaml components
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            //Binds context to the mail label, which contains the email address 
            lbl_MailTo.BindingContext = "rememberingwwi@nara.gov";

            //Tap gesture recognizers for links, and mail composer
            TapGestureRecognizer tgrLinks = new TapGestureRecognizer();
            tgrLinks.Tapped += Links_Tapped;
            TapGestureRecognizer tgrMailTo = new TapGestureRecognizer();
            tgrMailTo.Tapped += TgrMailTo_Tapped;

            lbl_MailTo.GestureRecognizers.Add(tgrMailTo);

            lbl_GoogleCookies.BindingContext = "https://developers.google.com/analytics/devguides/collection/analyticsjs/cookie-usage";
            lbl_GooglePrivacyPolicy.BindingContext = "https://www.google.com/policies/privacy/";

            lbl_GoogleCookies.GestureRecognizers.Add(tgrLinks);
            lbl_GooglePrivacyPolicy.GestureRecognizers.Add(tgrLinks);

            var Back_Tap = new TapGestureRecognizer();
            Back_Tap.Tapped += (s, e) =>
            {
                NavigateHomepage();
            };

            img_Previous.GestureRecognizers.Add(Back_Tap);
        }

        /// <summary>
        /// Method that represents the mail composer component to the user
        /// </summary>
        private void TgrMailTo_Tapped(object sender, EventArgs e)
        {
            try
            {
                var lbl = (Label)sender;

                var emailTask = MessagingPlugin.EmailMessenger;
                if (emailTask.CanSendEmail)
                {
                    // Send simple e-mail to single receiver without attachments, CC, or BCC.
                    emailTask.SendEmail(lbl.BindingContext.ToString(), "", "");
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

        /// <summary>
        /// Method that handles navigation of the links in the "about" text
        /// </summary>
        private async void Links_Tapped(object sender, EventArgs e)
        {
            try
            {
                var where = (Label)sender;
                await Navigation.PushAsync(new WebContentForLinks((string)where.BindingContext));
            }
            catch (Exception exc)
            { }
        }
    }
}
