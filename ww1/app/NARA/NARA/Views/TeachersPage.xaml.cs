using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Util;
using NARA.Util;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NARA
{
    /// <summary>
    /// TeachersPage class inherits xamarin ContentPage and contains info for the curators
    /// </summary>
    /// 
    public partial class TeachersPage : ContentPage
    {
        Dictionary<Label, ContentView> SubMenuItems;
        OfflineRepository offlineRepo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
        StackLayout stck;
        StackLayout userMenu = new StackLayout() { IsVisible = false, Spacing = 0 };
        RoundedImage profilePic;
        public TeachersPage()
        {
            //Initialization of the xaml components
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            lbl_ForTeachers.FontFamily = Device.OnPlatform("Orpheus Pro", "OrpheusPro.otf#OrpheusPro", "");
            lbl_Overview.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");

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

            var overview_Tap = new TapGestureRecognizer();
            overview_Tap.Tapped += async (s, e) =>
            {
                ClearMenu();
                ClearSubMenu();
                ClearBorder();
                bv_OverView.BackgroundColor = Color.FromHex("#e0665e");
                lbl_Overview.FontAttributes = FontAttributes.Bold;

                await scrollView_Main.ScrollToAsync(0, 0, true);

            };

            lbl_Overview.GestureRecognizers.Add(overview_Tap);

            LoadSubMenuItems();
            ClearBorder();

            foreach (var item in SubMenuItems)
            {
                SetUpSubMenu(item.Key, item.Value);
            }

            bv_OverView.BackgroundColor = Color.FromHex("#e0665e");
            lbl_Overview.FontAttributes = FontAttributes.Bold;


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

            lbl_SignUp.GestureRecognizers.Add(tapGestureRecognizerSignUp);
            lbl_SignIn.GestureRecognizers.Add(tapGestureRecognizerLogin);
            LoadSubItems();

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

        private async void Delay(int ms)
        {
            await Task.Delay(ms);
            ForceLayout();
        }

        private void SetUpSubMenu(Label label, ContentView view)
        {
            var parent = ((StackLayout)label.Parent);
            label.LineBreakMode = LineBreakMode.WordWrap;
            label.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
            //parent.HeightRequest = 55;
            TapGestureRecognizer tgr = new TapGestureRecognizer();
            tgr.Tapped += async (s, e) =>
            {
                ClearMenu();
                ClearSubMenu();
                ClearBorder();
                SetBorder(label, Grid.GetRow(label));
                label.FontAttributes = FontAttributes.Bold;

                await Task.Delay(250);

                await scrollView_Main.ScrollToAsync(view.Bounds.X, view.Bounds.Y - 30, true);
            };

            label.GestureRecognizers.Add(tgr);
        }

        private void SetBorder(Label label, int p_row)
        {
            var parent = ((StackLayout)label.Parent).Children[0].BackgroundColor = Color.FromHex("#e0665e");
        }

        private void ClearBorder()
        {
            bv_OverView.BackgroundColor = Color.Transparent;

            foreach (var label in SubMenuItems)
            {
                ((StackLayout)label.Key.Parent).Children[0].BackgroundColor = Color.Transparent;
            }
        }

        private void ClearSubMenu()
        {
            foreach (var item in SubMenuItems)
            {
                item.Key.FontAttributes = FontAttributes.None;
                item.Key.TextColor = Color.White;
            }
        }

        private void ClearMenu()
        {
            bv_ContactUs.BackgroundColor = Color.FromHex("#1d1d1d");
            bv_OverView.BackgroundColor = Color.FromHex("#1d1d1d");

            //lbl_AboutTheApp.FontAttributes = FontAttributes.None;
            lbl_ContactUs.FontAttributes = FontAttributes.None;
            lbl_Overview.FontAttributes = FontAttributes.None;
        }

        private void LoadSubMenuItems()
        {
            SubMenuItems = new Dictionary<Label, ContentView>();

            //{ new KeyValuePair<Label, ContentView>(lbl_PersonalInfo, cv_PersonalInfo), lbl_StoringCollecting, lbl_Cookies, lbl_ExternalLinks, lbl_Copyright, lbl_Accessibility, lbl_ProtectingInfo, lbl_NotificationOfChanges };

            SubMenuItems.Add(lbl_ForTeachersStudents, TeachersStudents);
            SubMenuItems.Add(lbl_WhatType, cv_WhatTypeOfPrimary);
            SubMenuItems.Add(lbl_WhatKind, cv_WhatKindOfColl);
            SubMenuItems.Add(lbl_Register, cv_DoIHaveToRegister);
            SubMenuItems.Add(lbl_Explore, cv_HowToExplore);
            SubMenuItems.Add(lbl_Favoriting, cv_Favoriting);
            SubMenuItems.Add(lbl_EditCollection, cv_HowDoIEdit);
            SubMenuItems.Add(lbl_AddingToNotes, cv_Adding);
            SubMenuItems.Add(lbl_Downloads, cv_Downloads);
            SubMenuItems.Add(lbl_Discuss, cv_Discuss);
            SubMenuItems.Add(lbl_CreateAndAdd, cv_HowToCreate);
            SubMenuItems.Add(lbl_Downloading, cv_Downloading);

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
            catch (Exception exc)
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

            if (height != 0 && width != 0)
            {
                scrollView_Navigation.HeightRequest = height - 60;
                icons.WidthRequest = width;
                //stack_Text.WidthRequest = width;
                stack_Outer.WidthRequest = width;
                scrollView_Main.WidthRequest = width;

                if (webUtil.CheckLogin() && scrollView_Main.Width != (rl_Main.Width * 0.7))
                {
                    stack_Text.WidthRequest = width * 0.7;
                    scrollView_Main.LayoutTo(new Rectangle(scrollView_Main.X, scrollView_Main.Y, rl_Main.Width * 0.7, scrollView_Main.Height));
                }
                else
                {
                    if (scrollView_Main.Width != (rl_Main.Width * 0.58))
                    {
                        stack_Text.WidthRequest = width * 0.58;
                        scrollView_Main.LayoutTo(new Rectangle(scrollView_Main.X, scrollView_Main.Y, rl_Main.Width * 0.58, scrollView_Main.Height));
                    }
                }

                img1.HorizontalOptions = LayoutOptions.CenterAndExpand;

                if (img1.Width > width)
                {
                    img1.WidthRequest = width - 50;
                }
                //grid_Logos.WidthRequest = width * 0.4;
            }
        }

        /// <summary>
        /// Method that navigates to the homepage of the app
        /// </summary>
        private async void NavigateHomepage()
        {
            await Navigation.PopToRootAsync();
        }
        protected override void OnDisappearing()
        {
            MessagingCenter.Send<App>((App)Xamarin.Forms.Application.Current, "end");
            base.OnDisappearing();
        }
        private void LoadSubItems()
        {
            int i = 0;
            foreach (var child in stack_Text.Children)
            {
                if (child is ContentView)
                {
                    var cv = (ContentView)child;
                    if (cv.Content is Label)
                    {
                        var lbl = (Label)cv.Content;
                        if (i != 0)
                        {
                            lbl.FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "");
                        }
                        i++;
                        if (lbl.TextColor == Color.FromHex("#e0665e") && lbl.FontSize == 20)
                        {

                        }
                    }
                }
            }
        }
    }
}
