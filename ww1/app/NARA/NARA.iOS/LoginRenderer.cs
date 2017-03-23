using NARA;
using NARA.iOS;
using NARA.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExternalLogin), typeof(LoginRenderer))]

namespace NARA.iOS
{
    /// <summary>
    /// Custom renderer implemented for xamarin page control for authentication with social networks such as Facebook
    /// </summary>
    public class LoginRenderer : PageRenderer
    {
        bool actionCompleted = false;
        public override void ViewDidAppear(bool animated)
        {
            if (!actionCompleted)
            {
                base.ViewDidAppear(animated);

                var auth = new OAuth2Authenticator(
                    clientId: "1017278055057999", // your OAuth2 client id
                    scope: "public_profile+email+user_about_me", // the scopes for the particular API you're accessing, delimited by "+" symbols
                    authorizeUrl: new Uri("https://www.facebook.com/dialog/oauth"), // the auth URL for the service
                    redirectUrl: new Uri(NaraTools.Domain)); // the redirect URL for the service


                auth.AllowCancel = true;

                auth.Completed += (sender, eventArgs) =>
                {
                    // Dimiss UI

                    DismissViewController(true, null);

                    if (eventArgs.IsAuthenticated)
                    {
                        // Retrieve fb token
                        App.SaveToken(eventArgs.Account.Properties["access_token"]);
                        actionCompleted = true;
                        App.SuccessfulLoginAction.Invoke();
                    }
                    else
                    {
                        // The user cancelled
                        actionCompleted = true;
                        DismissViewController(true, null);
                        App.SuccessfulLoginAction.Invoke();
                    }
                };

                PresentViewController(auth.GetUI(), true, null);
            }
        }
    }
}
