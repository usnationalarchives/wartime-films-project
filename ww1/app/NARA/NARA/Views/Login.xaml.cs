using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Service;
using NARA.Common_p.Util;
using NARA.Util;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XLabs.Forms.Controls;

namespace NARA
{
    /// <summary>
    /// Login class inherits xamarin ContentPage and contains the login mechanism for the app
    /// </summary>
    /// 
    public partial class Login : ContentPage
    {
        OAuthClient authClient = new OAuthClient();
        OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
        string content;
        CookieCollection responseCookiesString;
        List<Cookie> responseCookies = new List<Cookie>();
        //WebView loading_Indicator = new WebView() { Source = new HtmlWebViewSource() { BaseUrl = "Content/Loading.html" }, IsVisible = false };
        ImageViewCustom loading_IndicatorImage = new ImageViewCustom() { BackgroundColor = Color.FromHex("#1d1d1d"), ImageSourceInByteArray = App.LoaderImageInByteArray, Source = "loader.gif", Aspect = Aspect.AspectFit, HeightRequest = 160, WidthRequest = 160, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

        StackLayout loading_Indicator = new StackLayout() { BackgroundColor = Color.FromHex("#1d1d1d"), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
        Label lbl_ForgotYourPassword = new Label() { IsVisible = true, Text = "Forgot your password?", TextColor = Color.FromHex("#E0665E"), HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
        UserRepository userRepo;

        EntryCustom entry_Username = new EntryCustom();
        EntryCustom entry_Password = new EntryCustom();
        ButtonCustom btn_Login = new ButtonCustom();

        StackLayout stack_Confirmation = new StackLayout() { BackgroundColor = Color.FromHex("#e0c95e"), HeightRequest = 50 };
        Label lbl_Confirmation = new Label() { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.Black, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };

        Encrypt encrypt = new Encrypt();
        string encryptKey = "B374A26A71490437AA024E4FADD5B497";

        BoxView bv_Username = new BoxView()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            HeightRequest = 1,
            Color = Color.FromHex("#717171")
        };
        BoxView bv_Password = new BoxView()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            HeightRequest = 1,
            Color = Color.FromHex("#717171")
        };

        double _width = 0;
        double _height = 0;
        string returnUrlL = "";
        bool showMessage = false;
        bool crash = false;

        /// <summary>
        /// Constructor takes in properties of 
        ///  - returnUrl is the url of the page, that the user is redirected to
        ///  - message, to be shown in a notification bar
        ///  and takes care of build-ing and positioning of the view
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="message"></param>
        public Login(string returnUrl = "", string message = "")
        {
            try
            {
                NavigationPage.SetHasNavigationBar(this, false);
                InitializeComponent();
                //btn_SignUp = new LabelCustom() { BorderWidth = 3, RequestedHeight = 35, BorderRadius = 8, BorderColor = Color.FromHex("#E0665E"), LetterSpacing = 1, Text = "SIGN UP", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand };

                //if (Device.OS == TargetPlatform.iOS)
                //{
                //    btn_SignUp = new LabelCustom() { HeightRequest = 45, WidthRequest = 100, BorderWidth = 3, BorderRadius = 8, BorderColor = Color.FromHex("#E0665E"), LetterSpacing = 1, Text = "SIGN UP", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.EndAndExpand };
                //    btn_SignUp.FontSize = 8;
                //}

                if (Device.OS == TargetPlatform.Android)
                {
                    btn_Cancel.FontSize = 12;
                    //btn_SignUp.FontSize = 12;
                    btn_Cancel.WidthRequest += 10;
                    //btn_SignUp.WidthRequest += 25;
                    //btn_SignUp.HeightRequest += 10;

                    //grid_SignUp.RowDefinitions.FirstOrDefault().Height = new GridLength(45, GridUnitType.Absolute);

                    //btn_SignUp.BorderWidth = 0;
                    //btn_Cancel.BorderWidth = 0;
                    //btn_SignUp.VerticalOptions = LayoutOptions.Start;
                    btn_Cancel.VerticalOptions = LayoutOptions.CenterAndExpand;
                }

                //btn_SignUp.BackgroundColor = Color.FromHex("#E0665E");

                //Correct -cancel grid- padding for android
                if (Device.OS == TargetPlatform.Android)
                {
                    //grid_Cancel.Padding = new Thickness(0, 10, 60, 0);
                    //grid_Cancel.RowDefinitions.FirstOrDefault().Height = new GridLength(25, GridUnitType.Absolute);
                }

                //Tap event which takes user to the sign up page
                btn_Facebook.Clicked += Btn_Facebook_Clicked;

                if (Device.OS == TargetPlatform.Android)
                {
                    //var row = grid_SignUp.RowDefinitions.FirstOrDefault();
                    //row.Height = 30;
                    btn_SignUp.FontSize = 15;
                }

                TapGestureRecognizer tgr_SignUp = new TapGestureRecognizer();
                tgr_SignUp.Tapped += Tgr_SignUp_Tapped;

                btn_SignUp.GestureRecognizers.Add(tgr_SignUp);
                lbl_SignUp.Clicked += async (s, e) =>
                {
                    await Navigation.PushAsync(new Registration(returnUrl));
                };

                TapGestureRecognizer tgr_ForgotPassword = new TapGestureRecognizer();
                tgr_ForgotPassword.Tapped += Tgr_ForgotPassword_Tapped;

                lbl_ForgotYourPassword.GestureRecognizers.Add(tgr_ForgotPassword);

                //Positioning of the loading indicator IOS then ANDROID
                //if (Device.OS == TargetPlatform.iOS)
                //{
                //main.Children.Add(loading_Indicator,
                //Constraint.RelativeToParent((parent) =>
                //{
                //    return (parent.Width / 2) - 25;
                //}),
                //Constraint.RelativeToParent((parent) =>
                //{
                //    return (parent.Height / 2) - 25;
                //}),
                //Constraint.Constant(50),
                //Constraint.Constant(50)
                //);
                //}
                //else
                //{
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
                //}

                if (returnUrl != "")
                    returnUrlL = returnUrl;

                loading_Indicator.IsVisible = false;

                var tapGestureRecognizerBack = new TapGestureRecognizer();
                tapGestureRecognizerBack.Tapped += (s, e) =>
                {
                    NavigateBack();
                };

                img_Previous.GestureRecognizers.Add(tapGestureRecognizerBack);

                //Creating and positioning of the controls such as entry for the username/password, login button
                entry_Username = new EntryCustom() { Placeholder = "Username             ", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 30, HorizontalOptions = LayoutOptions.CenterAndExpand, FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "") };
                entry_Password = new EntryCustom() { Placeholder = "Password             ", TextColor = Color.FromHex("#717171"), IsPassword = true, FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.Transparent, HeightRequest = 30, HorizontalOptions = LayoutOptions.Center, FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", "") };
                btn_Login = new ButtonCustom() { Text = "SIGN IN", WidthRequest = 160, BorderColor = Color.FromHex("#E0665E"), FontSize = 25, BorderRadius = 20, BorderWidth = 2, BackgroundColor = Color.FromHex("#E0665E"), TextColor = Color.White, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };

                //Event for the login
                btn_Login.Clicked += btn_Login_Clicked;

                //StackLayout that holds the required controls
                StackLayout btnHolder = new StackLayout() { Spacing = 14, Padding = new Thickness(0, 50, 0, 50), HorizontalOptions = LayoutOptions.CenterAndExpand };
                btnHolder.Children.Add(entry_Username);
                btnHolder.Children.Add(bv_Username);
                btnHolder.Children.Add(entry_Password);
                btnHolder.Children.Add(bv_Password);
                btnHolder.Children.Add(lbl_ForgotYourPassword);
                ContentView cv_BtnHolder = new ContentView() { Content = btn_Login, Padding = new Thickness(0, 15, 0, 0) };
                btnHolder.Children.Add(cv_BtnHolder);

                //Positioning of the navigation bar that holds confirmation/error message text
                stack_Confirmation.Children.Add(lbl_Confirmation);
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

                if (message != "")
                {
                    lbl_Confirmation.Text = message;
                    showMessage = true;
                }

                stack_Forms.Children.Add(btnHolder);
                Grid.SetColumnSpan(btnHolder, 2);

                //Retrieving of the token required for auth. with the API
                string token = NaraTools.Token;
                //var tv = new TokenVerification();
                //token = tv.GetToken(NaraTools.ApiUsername, NaraTools.ApiPassword).access_token;

                //User repository
                userRepo = new UserRepository(token, new RestServiceProvider(token), true, DependencyService.Get<IPlatformSpecific>().ConnectionString());

                //Tap gesture for back navigation
                TapGestureRecognizer tgr_Back = new TapGestureRecognizer();
                tgr_Back.Tapped += Tgr_Back_Tapped;

                //Setting properties and tap gesture rec. for the cancel button
                int minusFont = 0;
                if (Device.OS == TargetPlatform.iOS) { btn_Cancel.FontSize -= 3; } else { btn_Cancel.FontSize += 1; }

                //btn_Cancel.GestureRecognizers.Add(tgr_Back);
                btn_Cancel.Clicked += Btn_Cancel_Clicked;

                if (Device.OS == TargetPlatform.iOS)
                {
                    btn_SignUp.IsVisible = false;
                    lbl_SignUp.HeightRequest = 30;
                    lbl_SignUp.IsVisible = true;
                    lbl_SignUp.WidthRequest = 75;
                }

                ForceLayout();

                entry_Username.Focused += Bv_Username_Focused;
                entry_Password.Focused += Bv_Password_Focused;
                entry_Username.Unfocused += Entry_Username_Unfocused;
                entry_Password.Unfocused += Entry_Username_Unfocused;
            }
            catch (Exception excp)
            {
                //In case of an error, crash flag is set
                crash = true;
            }
        }

        private void Entry_Username_Unfocused(object sender, FocusEventArgs e)
        {
            ClearBorders();
        }

        private void ClearBorders()
        {
            bv_Username.Color = Color.FromHex("#717171");
            bv_Password.Color = Color.FromHex("#717171");
        }

        private void Bv_Password_Focused(object sender, FocusEventArgs e)
        {
            ClearBorders();
            bv_Password.Color = Color.FromHex("#E0665E");
        }

        private void Bv_Username_Focused(object sender, FocusEventArgs e)
        {
            ClearBorders();
            bv_Username.Color = Color.FromHex("#E0665E");
        }

        private async void Tgr_SignUp_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registration(returnUrlL));
        }

        private async void Btn_Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void Tgr_ForgotPassword_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPassword());
        }

        /// <summary>
        /// Method that handles back navigation on the navigation stack
        /// </summary>
        private async void Tgr_Back_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Method displays alert box with the text from the message property
        /// </summary>
        /// <param name="message"></param>
        private async void ShowAlert(string message)
        {
            await DisplayAlert("Error", message, "Proceed");
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Navigates to the view for external login, such as Facebook
        /// </summary>
        private async void Btn_Facebook_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExternalLogin());
        }

        /// <summary>
        /// Shows the navigation bar with message, set in lbl_Message control
        /// </summary>
        /// <param name="navigate"></param>
        protected async void ShowMessage(bool navigate = false)
        {
            await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
            await Task.Delay(5000);
            await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
        }

        /// <summary>
        /// Method that handles interaction when the view is appearing, it checks whether facebook token is set, and if it is,
        /// then the external login is in progress and it continues with the processing of the token
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                //If the user is already signed in, it cannot access this view, but to be sure, it also checks if the user is signed in
                //and navigates back if it is
                if (webUtil.CheckLogin())
                {
                    await Navigation.PopAsync();
                }

                string facebookToken;

                //Message that was included is presented 
                if (showMessage)
                {
                    ShowMessage();
                }

                //Checking if token exists and if it does, then it proceeds with the sign in process
                if (App.Token != null)
                {
                    loading_Indicator.IsVisible = true;

                    facebookToken = App.Token;
                    App.Token = null;

                    //Update facebook user in the DB
                    var response = await userRepo.AddUserViaFacebook(new User() { Token = facebookToken });

                    if (response.Success)
                    {
                        //Retrieves cookies
                        var success = await webUtil.SaveCookies(new List<Cookie>(), response.RegistrationUserName, response.RegistrationPassword);
                        if (success)
                        {
                            var message = new FormattedString();
                            message.Spans.Add(new Span() { Text = "You are signed in as " });
                            message.Spans.Add(new Span() { Text = response.RegistrationUserName, FontAttributes = FontAttributes.Bold });
                            string token = repo.GetToken();
                            if (!string.IsNullOrEmpty(token))
                            {
                                userRepo = new UserRepository(token, new RestServiceProvider(token), true, DependencyService.Get<IPlatformSpecific>().ConnectionString());
                                //Saves the user info. to the app DB so it can access the user profile picture/username in the app
                                repo.SaveUser(await userRepo.GetUserByUsername(response.RegistrationUserName));

                                if (returnUrlL == "")
                                {
                                    App.Current.MainPage = new NavigationPage(new HomeScreen(false, message)) { BarBackgroundColor = Color.FromHex("#1d1d1d"), BarTextColor = Color.White, BackgroundColor = Color.FromHex("#1d1d1d") };
                                }
                                else
                                {
                                    NavigateBack();
                                }
                                loading_Indicator.IsVisible = false;
                            }
                            else
                            {
                                //In case of an error, the error message is displayed
                                lbl_Confirmation.Text = "Error occured when retrieving user profile";

                                await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                                await Task.Delay(3000);
                                await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                                loading_Indicator.IsVisible = false;
                            }

                        }
                    }
                    else
                    {
                        //In case of an error, the error message is displayed
                        loading_Indicator.IsVisible = false;
                        if (!response.Message.Contains("400"))
                        {
                            lbl_Confirmation.Text = response.Message;

                            await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                            await Task.Delay(5000);
                            await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //In case of an error, the error message is displayed
                loading_Indicator.IsVisible = false;
                lbl_Confirmation.Text = "Something went wrong, please check your internet connection and try again";
                ShowMessage();
            }
        }

        /// <summary>
        /// Method that handles back navigation on the navigation stack
        /// </summary>
        private async void NavigateBack()
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Metod that handles username/password login 
        /// </summary>
        async void btn_Login_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Adjusting height for the tablets
                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    stack_loading.HeightRequest = 200;
                }

                grid_InnerAll.IsVisible = false;
                loading_Indicator.IsVisible = true;
                lbl_Result.TextColor = Color.White;
                lbl_Result.Text = "";
                loading_Indicator.IsVisible = true;


                //Checking if user is connected to the internet using dependency service
                if (DependencyService.Get<IPlatformSpecific>().CheckConnection())
                {
                    try
                    {
                        var formContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("Username", entry_Username.Text),
                    new KeyValuePair<string, string>("Password", entry_Password.Text)
                });

                        //Setting up cookie container and handler for retrieving of the cookies from the request
                        CookieContainer cookies = new CookieContainer();
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.CookieContainer = cookies;

                        using (HttpClient authClient = new HttpClient(handler))
                        {
                            //Setting up the request
                            var uri = new Uri(NaraTools.Domain);
                            authClient.BaseAddress = uri;
                            authClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            //Sending the request
                            HttpResponseMessage authenticationResponse = await authClient.PostAsync("/cms/account/login?brand=dvex&ReturnUrl=%2Fdvex%2Fedit", formContent);

                            //Retrieving and storing of the cookies
                            responseCookiesString = cookies.GetCookies(uri);
                            responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        //In case of an error, the error message is displayed
                        lbl_Confirmation.Text = "Sign in failed, please check your credentials and try again";

                        await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                        await Task.Delay(5000);
                        await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                    }
                }
                else
                {
                    //Checking the db for stored cookies
                    foreach (var cookie in repo.GetCookies())
                    {
                        Cookie netCookie = new Cookie()
                        {
                            Expires = cookie.Expires,
                            HttpOnly = cookie.HttpOnly,
                            Name = cookie.Name,
                            Value = cookie.Value,
                            Domain = cookie.Domain,
                            Expired = cookie.Expired,
                            Secure = cookie.Secure,
                            Discard = cookie.Discard
                        };

                        responseCookies.Add(netCookie);
                    }
                }

                //If login was successfull, cookies are stored in the database, message is displayed and user is redirected
                if (responseCookies.Count > 1)
                {
                    string token = responseCookies.Where(i => i.Name == "_museums_access_token").FirstOrDefault().Value;
                    userRepo = new UserRepository(token, new RestServiceProvider(token), true, DependencyService.Get<IPlatformSpecific>().ConnectionString());
                    repo.SaveUser(await userRepo.GetUserByUsername(entry_Username.Text));

                    repo.ClearCookies();

                    foreach (var cookie in responseCookies)
                    {
                        repo.SaveCookie(entry_Username.Text, DateTime.Now.AddDays(1), cookie.HttpOnly, cookie.Name, cookie.TimeStamp, cookie.Value, cookie.Domain, cookie.Secure, cookie.Discard, cookie.Expired);
                    }

                    //Cookie nara is saved for easier identification is cookies has expired
                    repo.SaveCookie(entry_Username.Text, DateTime.Now.AddDays(1), true, "nara", DateTime.Now, "", NaraTools.Host, true, false, false);

                    if (returnUrlL != "")
                    {
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var message = new FormattedString();
                        message.Spans.Add(new Span() { Text = "You are signed in as " });
                        message.Spans.Add(new Span() { Text = entry_Username.Text, FontAttributes = FontAttributes.Bold });
                        App.Current.MainPage = new NavigationPage(new HomeScreen(false, message)) { BarBackgroundColor = Color.FromHex("#1d1d1d"), BarTextColor = Color.White, BackgroundColor = Color.FromHex("#1d1d1d") };
                    }

                    loading_Indicator.IsVisible = false;

                    if (DependencyService.Get<IPlatformSpecific>().CheckConnection())
                    {
                        await Task.Run(() => SaveData());
                    }
                }
                else
                {
                    //In case of and error, the error message is displayed
                    loading_Indicator.IsVisible = false;
                    lbl_Confirmation.Text = "Sign in failed, please check your credentials and try again";

                    await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                    await Task.Delay(5000);
                    await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);

                    NavigationPage.SetHasNavigationBar(this, false);
                }
            }
            catch (Exception exc)
            {
                //In case of and error, the error message is displayed
                loading_Indicator.IsVisible = false;
                lbl_Confirmation.Text = "Error occured, please check your internet connection";
                ShowMessage();
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
                if ((width != _width || height != _height) && (height > 0 && width > 0))
                {
                    _width = width;
                    _height = height;

                    loading_Indicator.WidthRequest = width;
                    loading_Indicator.HeightRequest = height;

                    main.WidthRequest = Width;
                    main.HeightRequest = Height;

                    stack_Overlay.WidthRequest = width;
                    stack_Overlay.HeightRequest = height - 220;
                    stack_SocialHolder.WidthRequest = (width / 2) - 30;
                    stack_Forms.WidthRequest = (width / 2) - 30;

                    grid_CenterHolder.WidthRequest = 60;

                    var requestedWidth = (width / 2) - 80;

                    entry_Password.WidthRequest = requestedWidth;
                    entry_Username.WidthRequest = requestedWidth;
                    btn_Facebook.WidthRequest = (width / 2) - 70;

                    btn_Cancel.HeightRequest = 45;

                    if ((height + width) < 1490 && Device.OS == TargetPlatform.Android)
                    {
                        btn_Facebook.FontSize = 12;
                        btn_Facebook.HeightRequest = 35;
                        img_Facebook.TranslationY += 2;
                        img_Facebook.HeightRequest = 20;
                        img_Facebook.HeightRequest = 20;
                        img_Facebook.VerticalOptions = LayoutOptions.CenterAndExpand;
                    }

                    //btn_SignUp.HeightRequest = 35;

                }
                else
                    return;
            }
            catch (Exception e)
            {
                //In case of and error, the crash flag is set
                crash = true;
            }
        }

        /// <summary>
        /// Metod that saves the html and all of the dependencies for the offline representation
        /// </summary>
        async void SaveData()
        {
            try
            {
                var dependencies = webUtil.getDependenciesFromHtml(content);
                dependencies.Remove(dependencies.FirstOrDefault());

                var downloaded = await DependencyService.Get<IPlatformSpecific>().getPageDependencies(dependencies);

                var offlineContent = webUtil.CorrectContent(downloaded, dependencies, content);
            }
            catch (Exception e) { }
        }
    }
}
