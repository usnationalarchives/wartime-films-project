using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Service;
using NARA.Common_p.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace NARA
{
    public partial class ChangePassword : ContentPage
    {
        OAuthClient authClient = new OAuthClient();
        OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        UserRepository userRepo;
        WebUtil webUtil = new WebUtil();
        string content;
        CookieCollection responseCookiesString;
        List<Cookie> responseCookies = new List<Cookie>();
        //WebViewCustom loading_Indicator = new WebViewCustom() { Source = new HtmlWebViewSource() { BaseUrl = "Content/Loading.html" }, IsVisible = false };
        ImageViewCustom loading_IndicatorImage = new ImageViewCustom() { BackgroundColor = Color.FromHex("#1d1d1d"), ImageSourceInByteArray = App.LoaderImageInByteArray, Source = "loader.gif", Aspect = Aspect.AspectFit, HeightRequest = 160, WidthRequest = 160, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

        StackLayout loading_Indicator = new StackLayout() { BackgroundColor = Color.FromHex("#1d1d1d"), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

        StackLayout stack_Confirmation = new StackLayout() { BackgroundColor = Color.FromHex("#e0c95e"), HeightRequest = 50 };
        Label lbl_Confirmation = new Label() { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.Black, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };

        EntryCustom entry_OldPassword;
        EntryCustom entry_Password;
        EntryCustom entry_PasswordRepeat;
        Button btn_SaveChanges;
        Frame frame;
        StackLayout btnHolder;
        StackLayout stackCB;
        StackLayout stackCB2;

        Encrypt encrypt = new Encrypt();
        string encryptKey = "B374A26A71490437AA024E4FADD5B497";

        double _width = 0;
        double _height = 0;
        string returnUrlL = "";
        string imgUrl = "";
        string facebookToken = "";
        byte[] imageInBytes;
        User user = new User();

        public ChangePassword()
        {
            try
            {
                //Hides the navigation bar
                NavigationPage.SetHasNavigationBar(this, false);

                //Initialize the xaml controls
                InitializeComponent();

                stack_Confirmation.Children.Add(lbl_Confirmation);

                //Positioning of the loading indicator IOS then ANDROID
                loading_Indicator.Children.Add(loading_IndicatorImage);
                main.Children.Add(loading_Indicator,
                Constraint.RelativeToParent((parent) =>
                {
                    return (0);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (0);
                }), Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width);
                }), Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height);
                })
                );

                loading_Indicator.IsVisible = true;

                //Tap gesture for back navigation
                var tapGestureRecognizerBack = new TapGestureRecognizer();
                tapGestureRecognizerBack.Tapped += (s, e) =>
                {
                    NavigateBack();
                };

                img_Previous.GestureRecognizers.Add(tapGestureRecognizerBack);

                //Building of the main entry controls such as name, surname, checkboxes, profile picture upload

                entry_OldPassword = new EntryCustom() { IsPassword = true, Placeholder = "Old password", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.CenterAndExpand };
                entry_Password = new EntryCustom() { IsPassword = true, Placeholder = "New password", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.CenterAndExpand };
                entry_PasswordRepeat = new EntryCustom() { IsPassword = true, Placeholder = "Repeat your new password", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.CenterAndExpand };

                btn_SaveChanges = new Button() { Text = "SAVE CHANGES", WidthRequest = 175, FontAttributes = FontAttributes.Bold, BorderColor = Color.FromHex("#E0665E"), FontSize = 20, BorderRadius = 20, BorderWidth = 2, BackgroundColor = Color.FromHex("#E0665E"), TextColor = Color.White, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };

                btn_SaveChanges.Clicked += btn_SaveChanges_Clicked;

                btnHolder = new StackLayout() { Spacing = 14, HorizontalOptions = LayoutOptions.CenterAndExpand, Padding = new Thickness(10, 0, 10, 0) };
                btnHolder.Children.Add(entry_OldPassword);
                btnHolder.Children.Add(entry_Password);
                btnHolder.Children.Add(entry_PasswordRepeat);

                ContentView cv_Btn = new ContentView() { Padding = new Thickness(0, 15, 0, 0) };
                cv_Btn.Content = btn_SaveChanges;
                btnHolder.Children.Add(cv_Btn);

                //positioning of the top bar
                main.Children.Add(stack_Confirmation,

                Constraint.RelativeToParent((parent) =>
                {
                    return (0);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (-50);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width);
                }),
                Constraint.Constant(50)
                );

                stack_Forms.Children.Add(btnHolder);

                Grid.SetColumnSpan(btnHolder, 2);

                //Retrieves token from the API
                string token = repo.GetToken();

                userRepo = new UserRepository(token, new RestServiceProvider(token), true, DependencyService.Get<IPlatformSpecific>().ConnectionString());

                SetUserData();

                loading_Indicator.IsVisible = false;

            }
            catch (Exception e)
            {
            }
        }

        private async void SetUserData()
        {
            try
            {
                user = repo.GetUser();
                user.RegistrationUserName = user.UserName;
                user = await userRepo.GetUserByUsername(user.UserName);
            }
            catch (Exception e)
            {

            }
        }

        private void ImagePicker_Clicked()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that handles back navigation
        /// </summary>
        private async void NavigateBack()
        {
            await Navigation.PopAsync();
        }

        private async void btn_SaveChanges_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Hiding the loading indicator and setting inserted user info to a object that will be send to the API
                loading_Indicator.IsVisible = true;
                user.OldPassword = entry_OldPassword.Text;
                user.RegistrationPassword = entry_Password.Text;
                user.RegistrationConfirmPassword = entry_PasswordRepeat.Text;
                user.RegistrationUserName = user.UserName;

                if (entry_Password.Text == entry_PasswordRepeat.Text)
                {
                    //Sending data to the API
                    var response = await userRepo.UserChangePassword(user);

                    if (response.Success)
                    {
                        //If profile image is selected it proceeds to the uploading of the image 
                        ShowMessage("Password has been successfully changed", null, 2500, true);
                        loading_Indicator.IsVisible = false;
                    }
                    else
                    {
                        //In case of an error, the error message is displayed
                        loading_Indicator.IsVisible = false;
                        ShowMessage("Error occured. Please validate inserted passwords");
                    }
                }
                else
                {
                    //In case of an error, the error message is displayed
                    loading_Indicator.IsVisible = false;
                    ShowMessage("Passwords doesn't match");
                }
            }
            catch (Exception excp)
            {
                //In case of an error, the error message is displayed
                loading_Indicator.IsVisible = false;
                ShowMessage("An error occured, please check your internet connection");
            }
        }

        /// <summary>
        /// Method that represents the message that is shown in the top bar
        /// </summary>
        /// <param name="message"></param>
        /// <param name="formatedString"></param>
        /// <param name="delay"></param>
        /// <param name="navigate"></param>
        protected async void ShowMessage(string message, FormattedString formatedString = null, int delay = 5000, bool navigate = false)
        {
            if (formatedString != null)
            {
                lbl_Confirmation.FormattedText = formatedString;
            }
            else
            {
                lbl_Confirmation.Text = message;
            }

            await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
            await Task.Delay(delay);
            await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);

            if (navigate)
            {
                NavigateBack();
            }
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
            try
            {
                _width = width;
                _height = height;

                loading_Indicator.WidthRequest = width;
                loading_Indicator.HeightRequest = height;

                stack_Overlay.WidthRequest = width;

                var requestedWidth = (width / 2) - 80;

                btnHolder.WidthRequest = requestedWidth;

                entry_OldPassword.WidthRequest = requestedWidth;
                entry_Password.WidthRequest = requestedWidth;
                entry_PasswordRepeat.WidthRequest = requestedWidth;

                stackCB.WidthRequest = requestedWidth;
                stackCB2.WidthRequest = requestedWidth;

            }
            catch { }
        }

        private void Tgr_CheckBox_Tapped(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
