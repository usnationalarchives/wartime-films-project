using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using ImageCircle.Forms.Plugin.iOS;
using Xamarin.Forms;
using Plugin.Share;
using System.IO;
using System.Drawing;
using System.Net;
using NARA.Util;

namespace NARA.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();
            LoadApplication(new App());

            if (NaraTools.Environment == Environments.Testing)
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            }

            //Sets app status bar theme
            app.StatusBarStyle = UIStatusBarStyle.LightContent;

            ShareImplementation.ExcludedUIActivityTypes = new List<NSString> { };

            //Sets bar button text color and state
            UIBarButtonItem.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                TextColor = UIColor.White
            }, UIControlState.Normal);
            UITextField.Appearance.TintColor = UIColor.White;
            UIApplication.SharedApplication.StatusBarHidden = true;

            MessagingCenter.Subscribe<App, string>(this, "Share", (sender, arg) => {
                Share(arg);
            });

            return base.FinishedLaunching(app, options);
        }

        public void Share(string fileName)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                var directoryname = Path.Combine(documents, "FolderName" + "/" + fileName);

                var ii = NSUrl.FromFilename(directoryname);

                var item = ii.Copy();

                var activityItems = new[] { item };

                var activityController = new UIActivityViewController(activityItems, null);

                var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

                while (topController.PresentedViewController != null)
                {
                    topController = topController.PresentedViewController;
                }

                UIButton menuButton = new UIButton(UIButtonType.Custom);
                menuButton.Frame = new RectangleF(0, 0, 24, 24);

                UIBarButtonItem menuItem = new UIBarButtonItem(menuButton);

                activityController.PopoverPresentationController.BarButtonItem = menuItem;

                topController.PresentViewController(activityController, true, () => { });
            }catch(Exception e)
            {

            }
        }

    }
}
