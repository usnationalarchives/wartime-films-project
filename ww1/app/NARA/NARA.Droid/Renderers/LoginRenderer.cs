using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NARA.Droid.Renderers;
using NARA;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using NARA.Util;

[assembly: ExportRenderer(typeof(ExternalLogin), typeof(LoginRenderer))]
namespace NARA.Droid.Renderers
{
    public class LoginRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            // this is a ViewGroup - so should be able to load an AXML file and FindView<>
            var activity = this.Context as Activity;

            var auth = new Xamarin.Auth.OAuth2Authenticator(
                    clientId: "1017278055057999", // your OAuth2 client id
                    scope: "public_profile+email+user_about_me", // the scopes for the particular API you're accessing, delimited by "+" symbols
                    authorizeUrl: new Uri("https://www.facebook.com/dialog/oauth"), // the auth URL for the service
                    redirectUrl: new Uri(NaraTools.Domain)); // the redirect URL for the service

            auth.Completed += (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {
                    // Use eventArgs.Account to do wonderful things
                    App.SaveToken(eventArgs.Account.Properties["access_token"]);
                    App.SuccessfulLoginAction.Invoke();
                }
                else
                {
                    // The user cancelled
                    App.SuccessfulLoginAction.Invoke();
                }
            };

            activity.StartActivity(auth.GetUI(activity));
        }
    }
}