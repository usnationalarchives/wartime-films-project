using NARA.Common_p.Model;
using NARA.Util;
using Plugin.Share;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NARA
{
    /// <summary>
    /// WebContentPage class inherits xamarin ContentPage and contains a custom webview, that handles the web-part of the app
    /// </summary>
    public partial class WebContentPage : ContentPage
    {
        ImageViewCustom activityIndicator = new ImageViewCustom() { ImageSourceInByteArray = App.LoaderImageInByteArray, Source = "loader.gif", Aspect = Aspect.AspectFit, HeightRequest = 100, WidthRequest = 100 };
        BoxView loadingView = new BoxView() { BackgroundColor = Color.FromHex("#1d1d1d") };
        StackLayout log = new StackLayout() { BackgroundColor = Color.FromHex("#1d1d1d") };
        Label lbl_Log = new Label() { TextColor = Color.White };
        Label lbl_LoadingData;
        WebViewCustom wvc;
        double m_width = 0;
        double m_height = 0;

        /// <summary>
        /// Method that handles navigation to the user profile edit page
        /// </summary>
        private async void Wvc_EditProfile(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateUser());
        }

        /// <summary>
        /// Handles share component of the app, which represents a mail composer. It takes a url from the webview,
        /// and set a title and message. 
        /// </summary>
        private async void Wvc_ShowShare(object sender, EventArgs e)
        {
            var title = "The US National Archives’ Remembering WWI app";
            var message = "I’ve found a great resource within the new Remembering WWI app from the US National Archives, check it out:";
            //var url = wvc.ShareUrl;
            var url = "https://itunes.apple.com/us/app/remembering-wwi/id1153610517";

            //if (url.Contains("?"))
            //{
            //    url += "&share=1";
            //}
            //else
            //{
            //    url += "?share=1";
            //}

            //try
            //{
            //    if (url.Contains("nara.semantika.si"))
            //    {
            //        url = url.Replace("nara.semantika.si", "museu.ms");
            //    }
            //}
            //catch(Exception excp) { }

            // Share a link and an optional title and message.
            await CrossShare.Current.ShareLink(url, message, title);
        }

        /// <summary>
        /// Redirects to the login page
        /// </summary>
        async void wvc_GoToEdit(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login(wvc.BackUrl));
        }

        /// <summary>
        /// Navigates to the home page in a way, that clears the navigation stack (PopToRootAsync())
        /// </summary>
        async void wvc_GoToHomepage(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Method that handles appearing of the loading indicator from the webview.
        /// Logging is also implemented for testing the performance of the app
        /// </summary>
        void wvc_MyNavigating(object sender, EventArgs e)
        {

            this.BackgroundColor = Color.FromHex("#1d1d1d");
            loadingView.Opacity = 1;
            loadingView.IsVisible = true;
            activityIndicator.IsVisible = true;
            lbl_LoadingData.IsVisible = false;
            DependencyService.Get<IPlatformSpecific>().SaveText("Loader is presented : " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);

        }

        /// <summary>
        /// Method that handles the hiding of the loading indicator from the webview
        /// Logging is also implemented for testing the performance of the app
        /// </summary>
        async void wvc_MyNavigated(object sender, EventArgs e)
        {
            await Task.Delay(150);
            activityIndicator.IsVisible = false;
            loadingView.IsVisible = false;
            lbl_LoadingData.IsVisible = false;
            DependencyService.Get<IPlatformSpecific>().SaveText("Loader is hidden : " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);
            DependencyService.Get<IPlatformSpecific>().SaveText("------------------------------" + Environment.NewLine, true);
        }

        /// <summary>
        /// Navigates back, when user taps back icon on the web
        /// </summary>
        async void wvc_OnJsNavigation(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }


        /// <summary>
        /// Constructor that takes in the url of the webpage, to be redirected to
        /// </summary>
        public WebContentPage(string url)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            this.BackgroundColor = Color.FromHex("#1d1d1d");

            activityIndicator.IsVisible = true;

            //Instantiating the custom webview with received url 
            wvc = new WebViewCustom
            {
                Source = new HtmlWebViewSource() { BaseUrl = url },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsLoading = true,
                Url = new Uri(url)
            };
            WebView wv = new WebView()
            {
                Source = url,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            //Binding events
            wvc.OnJsNavigation += wvc_OnJsNavigation;
            wvc.Navigated += wvc_Navigated;
            wvc.Navigating += wvc_Navigating;
            wvc.MyNavigating += wvc_MyNavigating;
            wvc.MyNavigated += wvc_MyNavigated;
            wvc.GoToHomepage += wvc_GoToHomepage;
            wvc.GoToEdit += wvc_GoToEdit;
            wvc.ShowShare += Wvc_ShowShare;
            wvc.EditProfile += Wvc_EditProfile;
            wvc.ExternalLink += Wvc_ExternalLink;

            //Implemented logging for performance testing purposes
            //DependencyService.Get<IPlatformSpecific>().SaveText("Setting a layout of webview page : " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);

            //Setting the content of the main relative layout on the page
            Content = SetLayout(wvc);

        }

        private async void Wvc_ExternalLink(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WebContentForLinks(wvc.BackUrl));
        }

        /// <summary>
        /// Method that handles the appearing of the loading indicator from the webview
        /// Logging is also implemented for testing the performance of the app
        /// </summary>
        void wvc_Navigating(object sender, WebNavigatingEventArgs e)
        {
            activityIndicator.IsVisible = true;
            loadingView.IsVisible = true;
            lbl_LoadingData.IsVisible = true;
            DependencyService.Get<IPlatformSpecific>().SaveText("Loader is presented : " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);
        }

        /// <summary>
        /// Method that handles the hiding of the loading indicator from the webview
        /// Logging is also implemented for testing the performance of the app
        /// </summary>
        async void wvc_Navigated(object sender, WebNavigatedEventArgs e)
        {
            await Task.Delay(100);
            activityIndicator.IsVisible = false;
            loadingView.IsVisible = false;
            lbl_LoadingData.IsVisible = false;
            DependencyService.Get<IPlatformSpecific>().SaveText("Loader is hidden : " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);
        }

        /// <summary>
        /// Method that handles interaction when the view is appearing, it also containes javascript for reload of the
        /// page in case it is redirected from login. 
        /// TODO: Additional check for reload  
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrEmpty(wvc.BackUrl) || Device.OS == TargetPlatform.iOS)
            {
                wvc.Refresh();
                //wvc.Eval("javascript:window.location.reload( true )");
            }
        }

        /// <summary>
        /// Method that handles the binding of the controls (wvc - webview that is core of the view, loading indicator, ...)
        /// </summary>
        /// <param name="wvc"></param>
        /// <returns></returns>
        private RelativeLayout SetLayout(WebView wvc)
        {
            int act_size = 0;
            int minusActivator = 0;

            //ScrollView container of the controls for logging
            ScrollView log_Scroll = new ScrollView() { IsVisible = false };
            log_Scroll.Content = log;
            log.Children.Add(lbl_Log);

            //Tap gestures
            TapGestureRecognizer tgr_lbl_log = new TapGestureRecognizer();
            tgr_lbl_log.Tapped += (s, e) => { lbl_Log.Text = DependencyService.Get<IPlatformSpecific>().LoadText(); log_Scroll.IsVisible = true; };
            TapGestureRecognizer tgr_lbl_logHide = new TapGestureRecognizer();
            tgr_lbl_logHide.Tapped += (s, e) => { log_Scroll.IsVisible = false; };

            Label log_Hide = new Label() { Text = "Hide log", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand };
            log_Hide.GestureRecognizers.Add(tgr_lbl_logHide);
            log.Children.Add(log_Hide);

            Label log_Show = new Label() { Text = "Show log", TextColor = Color.White };
            log_Show.GestureRecognizers.Add(tgr_lbl_log);

            if (Device.OS == TargetPlatform.iOS)
                minusActivator = 20;
            else
                minusActivator = 40;

            act_size = 50;

            //
            //The main relative view control, that containes all of the child controls (webview, loading indicator, logging controls, ... )
            //

            RelativeLayout main_stack = new RelativeLayout() { BackgroundColor = Color.FromHex("#1d1d1d") };
            lbl_LoadingData = new Label() { Text = "Loading data ...", FontFamily = NaraTools.SelectedFont, FontSize = 25, TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };

            main_stack.Children.Add(wvc, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) =>
            {
                return parent.Width;
            }),
            Constraint.RelativeToParent((parent) =>
            {
                return parent.Height;
            })
            );

            main_stack.Children.Add(loadingView, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) =>
            {
                return parent.Width;
            }),
            Constraint.RelativeToParent((parent) =>
            {
                return parent.Height;
            })
            );

            main_stack.Children.Add(activityIndicator,

            Constraint.RelativeToParent((parent) =>
            {
                return (parent.Width / 2) - 80;
            }),
            Constraint.RelativeToParent((parent) =>
            {
                return (parent.Height / 2) - 80;
            }),

            Constraint.Constant(160),
            Constraint.Constant(160)
            );

            ////////////////////////////////////////////////////////////////
            //activityIndicator.IsVisible = false;
            //loadingView.IsVisible = false;
            ///////////////////////////////////////////////////////////////

            ////////////// Commented out the binding of the logging controls ////////////////////////////////////////////
            //TODO: Custom implementation of logging and option to enable/ disable

            if (NaraTools.LogingEnabled)
            {
                main_stack.Children.Add(log_Scroll,

                Constraint.RelativeToParent((parent) =>
                {
                    return 80;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return 80;
                }),

                            Constraint.RelativeToParent((parent) =>
                            {
                                return parent.Width - 160;
                            }),
                            Constraint.RelativeToParent((parent) =>
                            {
                                return parent.Height - 160;
                            })
                );

                main_stack.Children.Add(log_Show,

                Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width / 2) - 30;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return 30;
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return 80;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return 30;
                })
                );
            }

            return main_stack;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            //wvc.Eval("javascript:window.location.reload( true )");
            wvc.WidthRequest = width;
            wvc.HeightRequest = height;
        }
    }
}
