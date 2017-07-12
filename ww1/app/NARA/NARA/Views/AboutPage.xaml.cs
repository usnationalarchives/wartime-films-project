using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Util;
using NARA.Util;
using Plugin.Connectivity;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NARA.Views
{
    /// <summary>
    /// AboutPage class inherits xamarin ContentPage and contains basic info of the app
    /// </summary>
    ///
    public partial class AboutPage : ContentPage
    {
        Dictionary<Label, ContentView> SubMenuItems;
        private double Height, Width;
        OfflineRepository offlineRepo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
        StackLayout stck;
        StackLayout userMenu = new StackLayout() { IsVisible = false, Spacing = 0 };
        RoundedImage profilePic;
        public AboutPage()
        {
            //Initialization of the xaml components
            InitializeComponent();
            LoadSubMenuItems();
            NavigationPage.SetHasNavigationBar(this, false);
            //grid_SubMenu.IsClippedToBounds = true;

            lb_About.FontFamily = Device.OnPlatform("Orpheus Pro", "OrpheusPro.otf#OrpheusPro", "");
            lbl_Overview.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");

            ClearMenu();
            bv_OverView.BackgroundColor = Color.FromHex("#e0665e");
            lbl_Overview.FontAttributes = FontAttributes.Bold;

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

            var overview_Tap = new TapGestureRecognizer();
            overview_Tap.Tapped += async (s, e) =>
            {
                stack_Submenu.IsVisible = false;
                ClearMenu();
                grid_SubMenu.HeightRequest = 30;
                bv_OverView.BackgroundColor = Color.FromHex("#e0665e");
                lbl_Overview.FontAttributes = FontAttributes.Bold;
                stack_Submenu.IsVisible = false;
                if (Device.OS == TargetPlatform.iOS)
                {
                    await Task.Delay(250);
                }
                await scrollView_Main.ScrollToAsync(0, 0, true);

            };
            var aboutTheApp_Tap = new TapGestureRecognizer();
            aboutTheApp_Tap.Tapped += async (s, e) =>
            {
                ClearMenu();
                ClearSubMenu();
                grid_SubMenu.HeightRequest = 340;
                SetLayout();
                bv_AboutTheApp.BackgroundColor = Color.FromHex("#e0665e");
                lbl_AboutTheApp.FontAttributes = FontAttributes.Bold;
                stack_Submenu.IsVisible = true;
                if (Device.OS == TargetPlatform.iOS)
                {
                    await Task.Delay(250);
                }
                await scrollView_Main.ScrollToAsync(cv_Privacy.Bounds.X, cv_Privacy.Bounds.Y + 30, true);
            };
            var contactUs_Tap = new TapGestureRecognizer();
            contactUs_Tap.Tapped += async (s, e) =>
            {

                ClearMenu();
                stack_Submenu.IsVisible = false;
                grid_SubMenu.HeightRequest = 30;
                bv_ContactUs.BackgroundColor = Color.FromHex("#e0665e");
                lbl_ContactUs.FontAttributes = FontAttributes.Bold;
                if (Device.OS == TargetPlatform.iOS)
                {
                    await Task.Delay(250);
                }
                await scrollView_Main.ScrollToAsync(cv_Contact.Bounds.X, cv_Contact.Bounds.Y - 10, true);
            };

            lbl_ContactUs.GestureRecognizers.Add(contactUs_Tap);
            lbl_AboutTheApp.GestureRecognizers.Add(aboutTheApp_Tap);
            lbl_Overview.GestureRecognizers.Add(overview_Tap);
            lbl_ContactUs.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
            lbl_AboutTheApp.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
            lbl_Overview.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");

            foreach (var item in SubMenuItems)
            {
                item.Key.FontSize = 15;
                SetUpSubMenu(item.Key, item.Value);
            }

            stack_Submenu.IsVisible = false;
            grid_SubMenu.HeightRequest = 30;

            var tgr_ContactUs = new TapGestureRecognizer();
            tgr_ContactUs.Tapped += Tgr_ContactUs_Tapped;

            lbl_ContactUsHeader.GestureRecognizers.Add(tgr_ContactUs);

            var tapGestureRecognizerSignUp = new TapGestureRecognizer();
            tapGestureRecognizerSignUp.Tapped += async (s, e) =>
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    await Navigation.PushAsync(new Registration());
                }
                else
                {
                    await this.DisplayAlert("Error", "Check your internet connection", "Proceed");
                }

            };
            var tapGestureRecognizerLogin = new TapGestureRecognizer();
            tapGestureRecognizerLogin.Tapped += async (s, e) =>
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    await Navigation.PushAsync(new Login());
                }
                else
                {
                    await this.DisplayAlert("Error", "Check your internet connection", "Proceed");
                }

            };

            LoadMenu();
            CheckLogin();

            var tapGestureRecognizerLogout = new TapGestureRecognizer();
            tapGestureRecognizerLogout.Tapped += (s, e) =>
            {
                offlineRepo.ClearUser();
                webUtil.ClearLogin();
                CheckLogin();
            };

            LoadSubItems();

            lbl_SignUp.GestureRecognizers.Add(tapGestureRecognizerSignUp);
            lbl_SignIn.GestureRecognizers.Add(tapGestureRecognizerLogin);

            TapGestureRecognizer tgr_Reg = new TapGestureRecognizer();
            tgr_Reg.Tapped += Tgr_Reg_Tapped;
            lbl_SignInOnBottom.GestureRecognizers.Add(tgr_Reg);

        }

        private async void Tgr_Reg_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registration());
        }

        private void LoadMenu()
        {

            int picSize = 0;

            if (Device.OS == TargetPlatform.iOS)
            {
                picSize = 40;
            }
            else { picSize = 60; }

            profilePic = new RoundedImage() { Aspect = Aspect.AspectFill, HeightRequest = picSize, WidthRequest = picSize };
            TapGestureRecognizer tprProfilePic = new TapGestureRecognizer();
            tprProfilePic.Tapped += TprProfilePic_Tapped;
            profilePic.GestureRecognizers.Add(tprProfilePic);

            stck = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = Device.OnPlatform(5, 0, 5), VerticalOptions = LayoutOptions.FillAndExpand };
            stck.Children.Add(profilePic);
            //TODO : use Device.OnPlatform for size & padding/margin
            var carrot = new Image() { Source = "arrow_down.png", WidthRequest = Device.OnPlatform(10, 12, 20), HeightRequest = Device.OnPlatform(10, 12, 20), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center }; //IOS = HorizontalOptions = Center
            carrot.GestureRecognizers.Add(tprProfilePic);
            stck.Children.Add(carrot);

            int carrotPosition = 0;

            if (Device.OS == TargetPlatform.iOS)
            {
                carrotPosition = 75;
            }
            else { carrotPosition = 85; }

            rl_Main.Children.Add(stck,
            Constraint.RelativeToParent((parent) =>
            {
                return parent.Width - carrotPosition;
            }),
            Constraint.Constant(10), Constraint.RelativeToParent((parent) =>
            {
                //IOS = 40
                return 70;
            }),
            Constraint.Constant(40));



            userMenu.Children.Add(new Image() { Source = "arrow_up.png", HorizontalOptions = LayoutOptions.End, Margin = new Thickness(0, 0, Device.OnPlatform(15, 25, 0), 0) });
            StackLayout linkContainer = new StackLayout() { BackgroundColor = Color.White, Spacing = 25, Padding = new Thickness(20) };
            var lbl_ViewProfile = new Label() { Text = "My Collections", TextColor = Color.Black, FontSize = 19 };
            TapGestureRecognizer tgr_ViewProfile = new TapGestureRecognizer();
            tgr_ViewProfile.Tapped += Tgr_ViewProfile_Tapped;
            lbl_ViewProfile.GestureRecognizers.Add(tgr_ViewProfile);
            linkContainer.Children.Add(lbl_ViewProfile);
            var lbl_EditProfile = new Label() { Text = "Edit Profile", TextColor = Color.Black, FontSize = 19 };
            linkContainer.Children.Add(lbl_EditProfile);
            var lbl_SignOut = new Label() { Text = "Sign Out", TextColor = Color.FromHex("#E0665E"), FontSize = 19 };
            TapGestureRecognizer tgr_SignOut = new TapGestureRecognizer();
            tgr_SignOut.Tapped += Tgr_SignOut_Tapped;
            lbl_SignOut.GestureRecognizers.Add(tgr_SignOut);
            linkContainer.Children.Add(lbl_SignOut);
            TapGestureRecognizer tgr_EditProfile = new TapGestureRecognizer();
            tgr_EditProfile.Tapped += async (s, e) => { await Navigation.PushAsync(new UpdateUser()); userMenu.IsVisible = false; };
            lbl_EditProfile.GestureRecognizers.Add(tgr_EditProfile);
            userMenu.Children.Add(linkContainer);

            int widthM = 205;
            if (Device.OS == TargetPlatform.Android)
            {
                widthM = 200;
            }

            rl_Main.Children.Add(userMenu,

            Constraint.RelativeToParent((parent) =>
            {
                return (parent.Width - widthM);
            }),
            Constraint.RelativeToParent((parent) =>
            {
                return (65);
            }),
            Constraint.RelativeToParent((parent) =>
            {
                return (170);
            }),
            Constraint.Constant(181)
            );
        }

        private void TprProfilePic_Tapped(object sender, EventArgs e)
        {
            if (!userMenu.IsVisible)
            {
                userMenu.IsVisible = true;
            }
            else
            {
                userMenu.IsVisible = false;
            }
        }

        private void LoadSubItems()
        {
            span_Message.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
            span_Message.FontSize = 20;
            span_Mail.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
            span_Mail.FontSize = 20;
            lbl_link.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
            lbl_link.FontSize = 20;
            foreach (var child in stack_Text.Children)
            {
                if (child is ContentView)
                {
                    var cv = (ContentView)child;
                    if (cv.Content is Label)
                    {
                        var lbl = (Label)cv.Content;

                        lbl.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
                        lbl.FontSize = 20;

                    }
                }
            }

            lbl_Privacy.FontSize = 60;
            lbl_Privacy.FontFamily = Device.OnPlatform("Orpheus Pro", "OrpheusPro.otf#OrpheusPro", "");

        }

        private void Tgr_SignOut_Tapped(object sender, EventArgs e)
        {
            userMenu.IsVisible = false;
            webUtil.ClearLogin();
            CheckLogin();
        }

        private async void Tgr_ViewProfile_Tapped(object sender, EventArgs e)
        {
            userMenu.IsVisible = false;
            if (CrossConnectivity.Current.IsConnected)
            {
                await Navigation.PushAsync(new WebContentPage(NaraTools.ProfileView));
            }
            else
            {
                await this.DisplayAlert("Error", "Check your internet connection", "Proceed");
            }
        }

        /// <summary>
        /// Method that checks, if user is signed in and sets the appropriate user controls
        /// </summary>
        private async void CheckLogin()
        {
            if (webUtil.CheckLogin())
            {
                upperToolbar.IsVisible = false;
                if (offlineRepo.GetUser() != null)
                {
                    try
                    {
                        var n = offlineRepo.GetUser();
                        if (!(n.ImageUrl.Contains("no_image")))
                        {
                            profilePic.Source = ImageSource.FromUri(new Uri(n.ImageUrl));
                        }
                        else
                        {
                            profilePic.Source = "no_image.jpg";
                        }
                    }
                    catch
                    {
                        profilePic.Source = "no_image.jpg";
                    }
                }
                stck.IsVisible = true;
                await scrollView_Main.LayoutTo(new Rectangle(scrollView_Main.X, scrollView_Main.Y, rl_Main.Width * 0.7, scrollView_Main.Height));
            }
            else
            {
                await scrollView_Main.LayoutTo(new Rectangle(scrollView_Main.X, scrollView_Main.Y, rl_Main.Width * 0.58, scrollView_Main.Height));
                upperToolbar.IsVisible = true;
                stck.IsVisible = false;
            }
        }

        private void Tgr_ContactUs_Tapped(object sender, EventArgs e)
        {
            try
            {
                var lbl = (Label)sender;

                var emailTask = MessagingPlugin.EmailMessenger;
                if (emailTask.CanSendEmail)
                {
                    // Send simple e-mail to single receiver without attachments, CC, or BCC.
                    emailTask.SendEmail("rememberingwwi@nara.gov", "", "");
                }
            }
            catch { }
        }

        private void SetUpSubMenu(Label label, ContentView view)
        {
            TapGestureRecognizer tgr = new TapGestureRecognizer();
            label.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
            //label.FontSize = 19;

            tgr.Tapped += async (s, e) =>
            {
                ClearSubMenu();
                label.TextColor = Color.FromHex("#e0665e");
                await scrollView_Main.ScrollToAsync(view.Bounds.X, view.Bounds.Y + 30, true);
            };

            label.GestureRecognizers.Add(tgr);
        }

        private void ClearSubMenu()
        {
            foreach (var item in SubMenuItems)
            {
                item.Key.TextColor = Color.White;
            }
        }

        private void LoadSubMenuItems()
        {
            SubMenuItems = new Dictionary<Label, ContentView>();
            //{ new KeyValuePair<Label, ContentView>(lbl_PersonalInfo, cv_PersonalInfo), lbl_StoringCollecting, lbl_Cookies, lbl_ExternalLinks, lbl_Copyright, lbl_Accessibility, lbl_ProtectingInfo, lbl_NotificationOfChanges };

            SubMenuItems.Add(lbl_PersonalInfo, cv_PersonalInfo);
            SubMenuItems.Add(lbl_StoringCollecting, cv_InforCollAndStored);
            SubMenuItems.Add(lbl_Cookies, cv_WebBrowserCookiesAndTools);
            SubMenuItems.Add(lbl_ExternalLinks, cv_ExternalLinksAndDisclaimer);
            SubMenuItems.Add(lbl_Copyright, cv_CopyRightRestrictions);
            SubMenuItems.Add(lbl_Accessibility, cv_Accessibility);
            SubMenuItems.Add(lbl_ProtectingInfo, cv_ProtectingInfo);
            SubMenuItems.Add(lbl_NotificationOfChanges, cv_NotificationOfChanges);

        }

        private void ClearMenu()
        {
            bv_AboutTheApp.BackgroundColor = Color.FromHex("#1d1d1d");
            bv_ContactUs.BackgroundColor = Color.FromHex("#1d1d1d");
            bv_OverView.BackgroundColor = Color.FromHex("#1d1d1d");

            lbl_AboutTheApp.FontAttributes = FontAttributes.None;
            lbl_ContactUs.FontAttributes = FontAttributes.None;
            lbl_Overview.FontAttributes = FontAttributes.None;
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

            if (height != 0 && width != 0)
            {
                if (Device.OS == TargetPlatform.Android)
                {
                    scrollView_Navigation.HeightRequest = height - 200;
                }
                Height = height;
                Width = width;

                icons.WidthRequest = width;
                if (webUtil.CheckLogin())
                {
                    stack_Text.WidthRequest = width * 0.7;
                    scrollView_Main.LayoutTo(new Rectangle(scrollView_Main.X, scrollView_Main.Y, rl_Main.Width * 0.7, scrollView_Main.Height));
                }
                else
                {
                    stack_Text.WidthRequest = width * 0.58;
                    scrollView_Main.LayoutTo(new Rectangle(scrollView_Main.X, scrollView_Main.Y, rl_Main.Width * 0.58, scrollView_Main.Height));
                }
                //grid_Logos.WidthRequest = width * 0.4;

                if (lbl_AboutTheApp.FontAttributes == FontAttributes.Bold)
                {
                    SetLayout();
                }

                if (Height > Width)
                {
                    stack_partnerPresenter.IsVisible = false;
                    stack_partnerPresenterSlim.IsVisible = true;
                }
                else
                {
                    stack_partnerPresenter.IsVisible = true;
                    stack_partnerPresenterSlim.IsVisible = false;
                }

                //if (!(lbl_AboutTheApp.FontAttributes == FontAttributes.Bold))
                //{
                //    stack_Submenu.IsVisible = false;
                //    grid_SubMenu.HeightRequest = 30;
                //    //if (grid_SubMenu.RowDefinitions.ElementAt(1).Height.Value != 0)
                //    //{
                //    //    grid_SubMenu.RowDefinitions.ElementAt(1).Height = new GridLength(0);
                //    //}
                //}
            }
        }

        private void SetLayout()
        {

            if (Height > Width)
            {
                scrollView_Main.Padding = new Thickness(30, 0, 0, 0);
                stack_partnerPresenter.IsVisible = false;
                stack_partnerPresenterSlim.IsVisible = true;
                if (grid_SubMenu.RowDefinitions.ElementAt(1).Height.Value != 350)
                {
                    grid_SubMenu.RowDefinitions.ElementAt(1).Height = new GridLength(350);
                    grid_SubMenu.HeightRequest = 390;
                }
            }
            else
            {
                stack_partnerPresenter.IsVisible = true;
                stack_partnerPresenterSlim.IsVisible = false;
                scrollView_Main.Padding = new Thickness(0);
                if (grid_SubMenu.RowDefinitions.ElementAt(1).Height.Value != 310)
                {
                    grid_SubMenu.RowDefinitions.ElementAt(1).Height = new GridLength(310);
                    grid_SubMenu.HeightRequest = 340;
                }
            }
        }

        /// <summary>
        /// Method that navigates to the homepage of the app
        /// </summary>
        private async void NavigateHomepage()
        {

            await Navigation.PopAsync();
        }
        protected override void OnDisappearing()
        {
            MessagingCenter.Send<App>((App)Xamarin.Forms.Application.Current, "end");
            base.OnDisappearing();
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckLogin();
        }
    }
}
