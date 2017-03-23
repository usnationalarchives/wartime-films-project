using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Service;
using NARA.Common_p.Util;
using NARA.Util;
using Plugin.Media;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Platform.Services.Media;

namespace NARA
{
    /// <summary>
    /// Registration class inherits xamarin ContentPage and contains the register/sign up mechanism for the app
    /// </summary>
    public partial class Registration : ContentPage
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

        StackLayout stack_Background = new Xamarin.Forms.StackLayout() { IsVisible = false, Opacity = 0.8, BackgroundColor = Color.FromHex("#1d1d1d") };

        StackLayout stack_TermsAndServices = new StackLayout() { BackgroundColor = Color.FromHex("#1d1d1d"), Padding = new Thickness(10) };
        StackLayout stack_HelpText = new StackLayout() { IsVisible = true, Padding = new Thickness(10) };

        EntryCustom entry_Username;
        EntryCustom entry_Email;
        EntryCustom entry_Password;
        EntryCustom entry_PasswordRepeat;
        Button btn_SignUp;
        RoundedImage circleImage = new RoundedImage() { Margin = new Thickness(10, 0, 0, 0), HeightRequest = 50, WidthRequest = 80, Aspect = Aspect.AspectFill, Source = "BorderImage.png" };
        Frame frame;
        StackLayout btnHolder;
        StackLayout stackCB;
        StackLayout stackCB2;
        StackLayout stackCB3;
        Label lbl_Teachers;
        Label lbl_Terms;
        Label lbl_Newsletter;
        //Switch cb_Terms;
        CustomCheckBox cb_TermsCustom = new CustomCheckBox();
        CustomCheckBox cb_Teacher = new CustomCheckBox();
        CustomCheckBox cb_Newsletter = new CustomCheckBox() { };

        Encrypt encrypt = new Encrypt();
        string encryptKey = "B374A26A71490437AA024E4FADD5B497";

        double _width = 0;
        double _height = 0;
        string returnUrlL = "";
        string imgUrl = "";
        string facebookToken = "";
        byte[] imageInBytes;
        bool HelpShown = false;

        /// <summary>
        ///  Constructor takes in properties of 
        ///  - returnUrl is the url of the page, that the user is redirected to
        ///  and takes care of build-ing and positioning of the view
        /// </summary>
        /// <param name="returnUrl"></param>
        public Registration(string returnUrl = "")
        {
            try
            {
                NavigationPage.SetHasNavigationBar(this, false);
                InitializeComponent();

                //Correct -cancel grid- padding for android
                if (Device.OS == TargetPlatform.Android)
                {
                    grid_Cancel.Padding = new Thickness(0, 0, 60, 0);
                    grid_Cancel.RowDefinitions.FirstOrDefault().Height = new GridLength(20, GridUnitType.Absolute);
                    cv_Title.Padding = new Thickness(0, 0, 0, 0);
                    stack_Overlay.Padding = new Thickness(20, 0, 20, 20);
                }

                //Defining button events and tap gesture recognizers
                stack_Confirmation.Children.Add(lbl_Confirmation);
                btn_Facebook.Clicked += Btn_Facebook_Clicked;

                TapGestureRecognizer tgr_Terms = new TapGestureRecognizer();
                tgr_Terms.Tapped += Tgr_Terms_Tapped;
                lbl_TermsPopup.GestureRecognizers.Add(tgr_Terms);

                TapGestureRecognizer tgr_CheckBox = new TapGestureRecognizer();
                tgr_CheckBox.Tapped += Tgr_CheckBox_Tapped;

                cb_Teacher.GestureRecognizers.Add(tgr_CheckBox);
                cb_TermsCustom.GestureRecognizers.Add(tgr_CheckBox);
                cb_Newsletter.GestureRecognizers.Add(tgr_CheckBox);

                //Positioning of the loading indicator IOS then ANDROID
                //if (Device.OS == TargetPlatform.iOS)
                //{
                main.Children.Add(stack_Background,
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
                //else
                //{
                //    main.Children.Add(loading_Indicator,
                //    Constraint.RelativeToParent((parent) =>
                //    {
                //        return (0);
                //    }),
                //    Constraint.RelativeToParent((parent) =>
                //    {
                //        return (0);
                //    }), Constraint.RelativeToParent((parent) =>
                // {
                //        return (parent.Width);
                //    }), Constraint.RelativeToParent((parent) =>
                // {
                //        return (parent.Height);
                //    })
                //    );
                //}


                //Setting the url of the page, that app navigates to when sign in is complete, if it is defined
                if (returnUrl != "")
                {
                    returnUrlL = returnUrl;
                }

                loading_Indicator.IsVisible = false;

                //Defining tap gesture for back navigation and linking it to the specific controls
                var tapGestureRecognizerBack = new TapGestureRecognizer();
                tapGestureRecognizerBack.Tapped += (s, e) =>
                {
                    NavigateBack();
                };
                img_Previous.GestureRecognizers.Add(tapGestureRecognizerBack);


                //Building of the main controls for the sign up process, such as entry controls for username, email, password, sign up btn, checboxes ... 
                //TEMPORARY FIX : Extra spaces were added to the placeholder, cause ios added shading if there were < 6 char
                entry_Username = new EntryCustom() { Placeholder = "Username                    ", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.CenterAndExpand };
                entry_Email = new EntryCustom() { Placeholder = "Email                     ", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.CenterAndExpand };

                entry_Password = new EntryCustom() { Placeholder = "Create a password", TextColor = Color.FromHex("#717171"), IsPassword = true, FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.Center };
                entry_PasswordRepeat = new EntryCustom() { Placeholder = "Confirm password", TextColor = Color.FromHex("#717171"), IsPassword = true, FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.Center };

                btn_SignUp = new Button() { Text = "SAVE PROFILE", WidthRequest = 180, BorderColor = Color.FromHex("#E0665E"), FontSize = 20, BorderRadius = 20, BorderWidth = 2, BackgroundColor = Color.FromHex("#E0665E"), TextColor = Color.White, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };

                btn_SignUp.Clicked += btn_SignUp_Clicked;

                btnHolder = new StackLayout() { Spacing = 14, HorizontalOptions = LayoutOptions.CenterAndExpand, Padding = new Thickness(10, 0, 10, 0) };
                btnHolder.Children.Add(entry_Username);
                btnHolder.Children.Add(entry_Email);
                btnHolder.Children.Add(entry_Password);
                btnHolder.Children.Add(entry_PasswordRepeat);

                Button imagePicker = new Button() { Text = "TAP TO UPLOAD A PROFILE PHOTO", TextColor = Color.FromHex("#717171"), FontSize = 12 };

                frame = new Frame() { OutlineColor = Device.OnPlatform(Color.FromHex("#717171"), Color.White, Color.White), BackgroundColor = Color.FromHex("#1d1d1d"), HeightRequest = 80 };
                StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                stack.Children.Add(circleImage);
                stack.Children.Add(new Label() { Text = "TAP TO UPLOAD A PROFILE PHOTO", TextColor = Color.FromHex("#717171"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, FontSize = 12, FontAttributes = FontAttributes.Bold });
                frame.Content = stack;

                var tapGestureAvatar = new TapGestureRecognizer();
                tapGestureAvatar.Tapped += (s, e) =>
                {
                    ImagePicker_Clicked();
                };

                frame.GestureRecognizers.Add(tapGestureAvatar);

                stackCB = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, Padding = new Thickness(0, 10, 0, 0) };
                lbl_Teachers = new Label() { Text = "I'm a teacher", HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, TextColor = Color.FromHex("#717171") };
                stackCB.Children.Add(lbl_Teachers);

                stackCB.Children.Add(cb_Teacher);
                stackCB2 = new StackLayout() { Padding = new Thickness(0, 0, 0, 0), Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
                FormattedString formatedString = new FormattedString();
                formatedString.Spans.Add(new Span() { FontSize = 16, Text = "I agree to NARA's " });
                formatedString.Spans.Add(new Span() { FontSize = 16, Text = "privacy and use policies", ForegroundColor = Color.FromHex("#559FE3") });

                lbl_Terms = new Label() { FormattedText = formatedString, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End, FontSize = 16, TextColor = Color.FromHex("#717171") };
                lbl_Terms.GestureRecognizers.Add(tgr_Terms);
                stackCB2.Children.Add(lbl_Terms);
                stackCB2.Children.Add(cb_TermsCustom);

                stackCB3 = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, Padding = new Thickness(0, 0, 0, 10) };
                lbl_Newsletter = new Label() { Text = "Would you like to receive emails regarding this app?", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End, FontSize = 16, TextColor = Color.FromHex("#717171") };
                stackCB3.Children.Add(lbl_Newsletter);
                stackCB3.Children.Add(cb_Newsletter);

                btnHolder.Children.Add(stackCB);
                btnHolder.Children.Add(stackCB2);
                btnHolder.Children.Add(stackCB3);
                btnHolder.Children.Add(frame);

                var topPading = new Thickness();

                if (Device.OS == TargetPlatform.Android)
                {
                    topPading = new Thickness(0);
                }
                else { topPading = new Thickness(0, 15, 0, 0); }

                ContentView cv_Btn = new ContentView() { Padding = topPading };
                cv_Btn.Content = btn_SignUp;
                btnHolder.Children.Add(cv_Btn);


                //Adding stack with confirmation bar to the main relative layout
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

                //Retrieving of the token from the API
                //var tv = new TokenVerification();
                //var token = tv.GetToken(NaraTools.ApiUsername, NaraTools.ApiPassword).access_token;
                string token = NaraTools.Token;

                userRepo = new UserRepository(token, new RestServiceProvider(token), true, DependencyService.Get<IPlatformSpecific>().ConnectionString());

                BuildPopUp();
                BuildHelpPopUp();

                //Adding the terms and services popup to the relative view
                main.Children.Add(stack_TermsAndServices,
                    Constraint.RelativeToParent((parent) =>
                    {
                        return (50);
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        //var n = this.Height;
                        //var o = this.HeightRequest;
                        return (-3350);
                    }),

                    Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 100);
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Height - 100);
                    })
                );

                //Adding the help text popup to the relative view
                main.Children.Add(stack_HelpText,
                    Constraint.RelativeToParent((parent) =>
                    {
                        return (50);
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        //var n = this.Height;
                        //var o = this.HeightRequest;
                        return (-3350);
                    }),

                    Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 100);
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Height - 100);
                    })
                );

                //Tap gesture for back navigation
                TapGestureRecognizer tgr_Back = new TapGestureRecognizer();
                tgr_Back.Tapped += Tgr_Back_Tapped;

                //Setting properties and tap gesture rec. for the cancel button
                int minusFont = 0;
                if (Device.OS == TargetPlatform.iOS) { btn_Cancel.FontSize -= 3; } else { btn_Cancel.FontSize += 1; }

                //Adjusting size of the cancel button
                btn_Cancel.GestureRecognizers.Add(tgr_Back);

                ForceLayout();
            }
            catch (Exception e)
            { }
        }

        /// <summary>
        /// Method that handles back navigation
        /// </summary>
        private async void Tgr_Back_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Method, that handles states of the custom checkboxes (checked/unchecked)
        /// </summary>
        private void Tgr_CheckBox_Tapped(object sender, EventArgs e)
        {
            try
            {
                var cb = (CustomCheckBox)sender;
                cb.CheckedChanged();
            }
            catch { }
        }

        /// <summary>
        /// Method that reprisents the stackview that holds the text for terms and services
        /// </summary>
        private async void Tgr_Terms_Tapped(object sender, EventArgs e)
        {
            await stack_TermsAndServices.LayoutTo(new Rectangle(50, 50, stack_TermsAndServices.Width, stack_TermsAndServices.Height));
        }

        /// <summary>
        /// Redirects the user to the external login form, such as Facebook
        /// </summary>
        private async void Btn_Facebook_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExternalLogin());
        }

        /// <summary>
        /// Method is called when user selects the image from the gallery using xamarin plugin for media 
        /// </summary>
        private async void ImagePicker_Clicked()
        {
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync();
                imageInBytes = ReadFully(file.GetStream());
                imgUrl = file.Path;
                circleImage.Source = file.Path;

                //var mediaPicker = DependencyService.Get<IMediaPicker>();
                //CameraMediaStorageOptions options = new CameraMediaStorageOptions() { DefaultCamera = CameraDevice.Rear };
                //var image = await mediaPicker.SelectPhotoAsync(options);

            }
            catch (Exception e)
            { }
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

                if (App.Token != null)
                {
                    //Hides the loading indicator
                    loading_Indicator.IsVisible = true;

                    facebookToken = App.Token;
                    App.Token = null;

                    var response = await userRepo.AddUserViaFacebook(new User() { Token = facebookToken });

                    //Checking if token exists and if it does, then it proceeds with the sign in process
                    if (response.Success)
                    {
                        //Retrieve cookies
                        var success = await webUtil.SaveCookies(new List<Cookie>(), response.RegistrationUserName, response.RegistrationPassword);
                        if (success)
                        {
                            //Setting up formated message that the user is signed in
                            var message = new FormattedString();
                            message.Spans.Add(new Span() { Text = "Registration was successfull, you are signed in as " });
                            message.Spans.Add(new Span() { Text = response.RegistrationUserName, FontAttributes = FontAttributes.Bold });

                            //Gets the token that is used for retrieving of user data
                            string token = repo.GetToken();
                            if (!string.IsNullOrEmpty(token))
                            {
                                userRepo = new UserRepository(token, new RestServiceProvider(token), true, DependencyService.Get<IPlatformSpecific>().ConnectionString());
                                //Saves the user info. to the app DB so it can access the user profile picture/username in the app
                                repo.SaveUser(await userRepo.GetUserByUsername(response.RegistrationUserName));

                                //Message is displayed
                                lbl_Confirmation.FormattedText = message;
                                //await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                                //await Task.Delay(3000);
                                //await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                                //Navigate();
                                ShowHelp();
                            }
                            else
                            {
                                //In case of an error, the error message is displayed
                                lbl_Confirmation.Text = "Registration was successfull but an error occured when retrieving profile data";

                                await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                                await Task.Delay(5000);
                                await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                            }
                        }

                        loading_Indicator.IsVisible = false;
                    }
                    else
                    {
                        //In case of an error, the error message is displayed
                        lbl_Confirmation.Text = "Something went wrong, please check your internet connection and try again";

                        await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                        await Task.Delay(5000);
                        await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
                    }
                }
                else
                {
                    ShowHelp();
                }
            }
            catch (Exception e)
            {
                //In case of an error, loading indicator is hidden
                loading_Indicator.IsVisible = false;
            }
        }

        /// <summary>
        /// Method that handles back navigation
        /// </summary>
        private async void NavigateBack()
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Method that handles registration on user SIGN UP button click
        /// </summary>
        async void btn_SignUp_Clicked(object sender, EventArgs e)
        {
            //Hides the loading indicator
            loading_Indicator.IsVisible = true;

            try
            {
                if (cb_TermsCustom.IsChecked)
                {
                    if (EmailIsValid(entry_Email.Text))
                    {
                        if (entry_Password.Text == entry_PasswordRepeat.Text)
                        {
                            User user = new User()
                            {
                                RegistrationUserName = entry_Username.Text,
                                RegistrationPassword = entry_Password.Text,
                                RegistrationConfirmPassword = entry_PasswordRepeat.Text,
                                Email = entry_Email.Text,
                                IsCurator = cb_Teacher.IsChecked,
                                ImgUrl = ""
                            };

                            //Sends user data to API for sign up
                            var response = await userRepo.AddUser(user);

                            if (response.Success)
                            {
                                //If image is selected, then it continuous to sign in and upload of the profile picture of the user
                                if (imgUrl != "")
                                {
                                    try
                                    {
                                        //Display message of success
                                        ShowMessage("Registration was successfull. Signing in ...");

                                        //Retrieves cookies
                                        var success = await webUtil.SaveCookies(new List<Cookie>(), entry_Username.Text, entry_Password.Text);


                                        if (success)
                                        {
                                            //Gets token required for retrieving of the user profile
                                            string token = repo.GetToken();
                                            if (!string.IsNullOrEmpty(token))
                                            {
                                                userRepo = new UserRepository(token, new RestServiceProvider(token), true, DependencyService.Get<IPlatformSpecific>().ConnectionString());
                                                //Saves the user info. to the app DB so it can access the user profile picture/username in the app
                                                repo.SaveUser(await userRepo.GetUserByUsername(response.RegistrationUserName));

                                                //Retrieves file name of the selected image path
                                                response.ImgUrl = System.IO.Path.GetFileName(imgUrl);

                                                //Uploads image for the profile of the user
                                                var imageUpload = await userRepo.UpdateUserProfilePicture(response, imageInBytes);

                                                //Displays message
                                                var message = new FormattedString();
                                                //message.Spans.Add(new Span() { Text = "Registration was successfull, you are signed in as " });
                                                //message.Spans.Add(new Span() { Text = entry_Username.Text, FontAttributes = FontAttributes.Bold });
                                                repo.SaveUser(await userRepo.GetUserByUsername(response.RegistrationUserName));
                                                //ShowMessage("", message, 2500, true);
                                                ShowHelp();
                                            }
                                            else
                                            {
                                                //In case of an error, the error message is displayed
                                                loading_Indicator.IsVisible = false;
                                                ShowMessage("Registration was successfull, but upload of a profile picture failed", null, 4000, true);
                                            }
                                        }
                                        else
                                        {
                                            //In case of an error, the error message is displayed
                                            ShowMessage("Registration was successfull, but there was an error when signing in. Please try to sign in again later", null, 3500, true);
                                            loading_Indicator.IsVisible = false;
                                        }
                                    }
                                    catch (Exception excp)
                                    {
                                        //In case of an error, the error message is displayed
                                        ShowMessage("Registration was successfull, but an error occured while uploading profile picture", null, 4000, true);
                                        loading_Indicator.IsVisible = false;
                                    }

                                }
                                else
                                {
                                    //If pic. was not selected, then it continous with sign-ing in
                                    var success = await webUtil.SaveCookies(new List<Cookie>(), entry_Username.Text, entry_Password.Text);

                                    if (success)
                                    {
                                        var message = new FormattedString();
                                        message.Spans.Add(new Span() { Text = "Registration was successfull, you are signed in as " });
                                        message.Spans.Add(new Span() { Text = entry_Username.Text, FontAttributes = FontAttributes.Bold });
                                        string token = repo.GetToken();
                                        if (!string.IsNullOrEmpty(token))
                                        {
                                            userRepo = new UserRepository(token, new RestServiceProvider(token), true, DependencyService.Get<IPlatformSpecific>().ConnectionString());
                                            //Saves the user info. to the app DB so it can access the user profile picture/username in the app
                                            repo.SaveUser(await userRepo.GetUserByUsername(response.RegistrationUserName));
                                        }
                                        else
                                        {
                                            //In case of an error, the error message is displayed
                                            ShowMessage("Registration was successfull, but SIGN IN has failed. Please, try again later");
                                        }

                                        //ShowMessage("", message, 2500, true);
                                        ShowHelp();

                                    }
                                }
                            }
                            else
                            {
                                //In case of an error, the error message is displayed
                                loading_Indicator.IsVisible = false;
                                ShowMessage(response.Message);
                            }
                        }
                        else
                        {
                            //In case of an error, the error message is displayed
                            loading_Indicator.IsVisible = false;
                            ShowMessage("Your passwords do not match, please check your password");
                        }
                    }
                    else
                    {
                        //In case of an error, the error message is displayed
                        loading_Indicator.IsVisible = false;
                        ShowMessage("Your email address is not valid");
                    }
                }
                else
                {
                    //In case of an error, the error message is displayed
                    loading_Indicator.IsVisible = false;
                    ShowMessage("You cannot sign up if you don't agree to the terms of use");
                }
            }
            catch (Exception excp)
            {
                //In case of an error, the error message is displayed
                loading_Indicator.IsVisible = false;
                ShowMessage("Error occured, please check your inputs");
            }
        }

        private async void ShowHelp()
        {
            HelpShown = true;
            stack_Background.IsVisible = true;
            stack_HelpText.IsVisible = true;
            await stack_HelpText.LayoutTo(new Rectangle(50, (stack_HelpText.Height / 3), stack_HelpText.Width, stack_HelpText.Height));
        }

        /// <summary>
        /// Method that navigates the user to the correct page, preventing that te user could be redirected to login when it is already sign-ed-in by
        /// inspecting navigation stack
        /// </summary>
        private void Navigate()
        {
            if (returnUrlL != "")
            {
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

                loading_Indicator.IsVisible = false;
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new HomeScreen()) { BarBackgroundColor = Color.FromHex("#1d1d1d"), BarTextColor = Color.White, BackgroundColor = Color.FromHex("#1d1d1d") };
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
            //Setting the text for the message
            if (formatedString != null)
            {
                lbl_Confirmation.FormattedText = formatedString;
            }
            else
            {
                lbl_Confirmation.Text = message;
            }

            //Displaying the message in the top bar
            await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
            await Task.Delay(delay);
            await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);

            if (navigate)
            {
                Navigate();
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

                //if(Device.OS == TargetPlatform.Android)
                //{
                //    scrollView_Main.HeightRequest = height + 150;
                //}

                loading_Indicator.WidthRequest = width;
                loading_Indicator.HeightRequest = height;

                if (height > width)
                {
                    circleImage.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    circleImage.Margin = new Thickness(10, 0, 0, 0);
                }

                stack_Overlay.WidthRequest = width;
                stack_Overlay.HeightRequest = height - 180;
                stack_SocialHolder.WidthRequest = (width / 2) - 30;
                stack_Forms.WidthRequest = (width / 2) - 30;

                grid_CenterHolder.WidthRequest = 60;

                var requestedWidth = (width / 2) - 80;

                btnHolder.WidthRequest = requestedWidth;

                entry_Email.WidthRequest = requestedWidth;
                entry_Password.WidthRequest = requestedWidth;
                entry_PasswordRepeat.WidthRequest = requestedWidth;
                entry_Username.WidthRequest = requestedWidth;
                stackCB.WidthRequest = requestedWidth;
                stackCB2.WidthRequest = requestedWidth;
                stackCB3.WidthRequest = requestedWidth;
                btn_Facebook.WidthRequest = (width / 2) - 70;

                lbl_Teachers.WidthRequest = requestedWidth - 35;
                lbl_Terms.WidthRequest = requestedWidth - 35;
                lbl_Newsletter.WidthRequest = requestedWidth - 35;

                if (HelpShown)
                {
                    stack_HelpText.TranslationX = 0;
                    stack_HelpText.TranslationY = 50;
                }
            }
            catch (Exception exc)
            { }
        }

        /// <summary>
        /// //Reads stream and return a byte array of the selected image
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Metod that saves the html and all of the dependencies for the offline representation
        /// </summary>
        async void SaveData()
        {
            var dependencies = webUtil.getDependenciesFromHtml(content);
            dependencies.Remove(dependencies.FirstOrDefault());

            var downloaded = await DependencyService.Get<IPlatformSpecific>().getPageDependencies(dependencies);

            var offlineContent = webUtil.CorrectContent(downloaded, dependencies, content);
        }

        static Regex ValidEmailRegex = CreateValidEmailRegex();

        /// <summary>
        /// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
        /// Checks for a valid email format
        /// </summary>
        /// <returns></returns>
        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Checks for a valid email format
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        internal static bool EmailIsValid(string emailAddress)
        {
            bool isValid = ValidEmailRegex.IsMatch(emailAddress);
            return isValid;
        }

        /// <summary>
        /// Builds the popup, that holds the text of the terms and conditions and inserts it to the relative layout
        /// </summary>
        private void BuildPopUp()
        {
            int headerFontSize = 60;
            int firstTitle = 20;
            int secondTitle = 21;
            int text = 15;
            Color textColor = Color.White;

            FrameCustom overlayFrame = new FrameCustom() { OutlineColor = Color.FromHex("#717171"), BackgroundColor = Color.FromHex("#1d1d1d") };

            ScrollView scrollView = new ScrollView();
            StackLayout stack_InnerHolder = new StackLayout() { Padding = new Thickness(1), BackgroundColor = Color.FromHex("#1d1d1d") };
            StackLayout stack_OuterHolder = new StackLayout() { BackgroundColor = Color.FromHex("#1d1d1d") };

            Thickness noPadding = new Thickness(0);
            Thickness paddingTop10 = new Thickness(0, 25, 0, 0);

            Label lbl_Header = new Label() { FontAttributes = FontAttributes.Bold, FontSize = headerFontSize, TextColor = textColor, FontFamily = "OrpheusPro" };
            lbl_Header.Text = "Privacy and Use Policies";
            FormattedString lbl_TitleOfTermsAndConditions = new FormattedString();
            lbl_TitleOfTermsAndConditions.Spans.Add(new Span() { FontSize = firstTitle, Text = "Terms and Conditions for Using ", ForegroundColor = textColor });
            lbl_TitleOfTermsAndConditions.Spans.Add(new Span() { FontSize = firstTitle, Text = "Remembering WWI", FontAttributes = FontAttributes.Italic, ForegroundColor = textColor });

            TapGestureRecognizer tgr_ClosePopUp = new TapGestureRecognizer();
            tgr_ClosePopUp.Tapped += (s, e) =>
            {
                Task.Factory.StartNew(async () =>
                {
                    await stack_TermsAndServices.LayoutTo(new Rectangle(50, -3350, stack_TermsAndServices.Width, stack_TermsAndServices.Height));
                }
                );
            };

            Image img_Close = new Image() { Aspect = Aspect.AspectFit, Source = "remove.png", HorizontalOptions = LayoutOptions.EndAndExpand };
            img_Close.GestureRecognizers.Add(tgr_ClosePopUp);
            stack_OuterHolder.Children.Add(img_Close);

            stack_InnerHolder.Children.Add(lbl_Header);
            stack_InnerHolder.Children.Add(new Label() { FormattedText = lbl_TitleOfTermsAndConditions });

            stack_InnerHolder.Children.Add(CreateTextForPopUp("We are committed to protecting your privacy. We do not collect personally identifying information about you when you visit our site unless you choose to provide such information to us. Providing such information is strictly voluntary. This policy is your guide to how we will handle information we learn about you from your visit to our website.", text, paddingTop10));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("This mobile app was created by the National Archives and Records Administration (NARA).", text, paddingTop10));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Personal Information", secondTitle, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("You may elect to sign in so that your saved information is available when you return. The site requires your name, email address and a password to create an account. If you choose to login using Facebook, we access your public profile on Facebook to obtain your name and email address.", text, noPadding));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("We do not collect any other personally identifiable information about you unless you voluntarily submit such information to us, by, for example, filling out a registration form or survey. We use this information to assist us in improving existing or future products, tools, services, and communication materials.", text, paddingTop10));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("This information is not used for any other purpose, nor is the information sold to third parties. Please do not send us sensitive information, such as your credit card or social security numbers, via email.", text, paddingTop10));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("If there are any issues related to your user account, we may use your name and email address to contact you. If you give us permission, we will use your name and email address to send information about the app.", text, paddingTop10));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Information Collected and Stored Automatically", secondTitle, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("We may collect non-personal identification information about users whenever they interact with our app. Non-personal identification information may include the browser name, the type of computer, and technical information about users’ means of connection to our site, such as the operating system, Internet service provider utilized, and other similar information.", text, noPadding));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("We use the IP address from your Internet connection to analyze trends, administer the site, track users’ movements, and gather broad demographic information for aggregate use. IP addresses are not linked to any personally identifiable information.", text, paddingTop10));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Web browser cookies and analytics tools", secondTitle, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("Our site may use “cookies” to enhance user experience. A user’s web browser places cookies on their hard drive for record-keeping purposes and sometimes to track information about them. Although persistent cookies help us create a better experience for you, most of this website will work without them. You may choose to set your web browser to refuse cookies, or to alert you when cookies are being sent.", text, noPadding));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("This app uses tools (Google Analytics Premium) to collect heatmap, scroll map, and web traffic data to better understand how visitors engage with us. Please refer to the following privacy policies for more information about each tool:", text, paddingTop10));


            // Section that handles presentation and navigation for the google links
            TapGestureRecognizer links = new TapGestureRecognizer();
            links.Tapped += Links_Tapped;

            Label lbl_GooglePrivacyLink = new Label() { TextColor = Color.FromHex("#559FE3"), FontSize = 15 };
            lbl_GooglePrivacyLink.Text = "- Google privacy policy";
            lbl_GooglePrivacyLink.BindingContext = "https://www.google.com/policies/privacy/";
            lbl_GooglePrivacyLink.GestureRecognizers.Add(links);
            Label lbl_CookiesAndGoogleAnalytics = new Label() { TextColor = Color.FromHex("#559FE3"), FontSize = 15 };
            lbl_CookiesAndGoogleAnalytics.Text = "- Cookies and Google Analytics on websites";
            lbl_CookiesAndGoogleAnalytics.BindingContext = "https://developers.google.com/analytics/devguides/collection/analyticsjs/cookie-usage";
            lbl_CookiesAndGoogleAnalytics.GestureRecognizers.Add(links);
            StackLayout stack_GoogleLinks = new StackLayout() { Padding = paddingTop10 };
            stack_GoogleLinks.Children.Add(lbl_GooglePrivacyLink);
            stack_GoogleLinks.Children.Add(lbl_CookiesAndGoogleAnalytics);
            stack_InnerHolder.Children.Add(stack_GoogleLinks);

            stack_InnerHolder.Children.Add(CreateTextForPopUp("External Links and Disclaimer", secondTitle, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("We provide external links that may contain information of interest to our users, including links to records contributed by partner cultural heritage institutions. External links do not constitute an endorsement by NARA of the opinions, products, or services presented on the external site, or of any sites linked to it. NARA has made reasonable efforts to ensure the items in this app are appropriately marked to indicate any possible copyright interest. However, NARA makes no warranties representations regarding the copyright status of the digital images on these web sites, or in the materials in other collections. Likewise, we are not responsible for the legality or accuracy of information on externally linked sites, or for any costs incurred while using externally linked sites.", text, noPadding));

            //stack_InnerHolder.Children.Add(CreateTextForPopUp("Disclaimer of Endorsement", secondTitle, paddingTop10, FontAttributes.Bold));
            //stack_InnerHolder.Children.Add(CreateTextForPopUp("Reference to specific commercial businesses, products, processes, or services by trade name, trademark, manufacturer, or otherwise, on any web site or app administered by NARA does not constitute or imply its endorsement, recommendation, or favoring by the NAF or by NARA.", text, noPadding));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Copyright, Restrictions, and Permissions", secondTitle, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("Primary source documents included on this site come from the holdings of the National Archives and other cultural heritage organizations. Generally, materials produced by federal agencies are in the public domain and may be reproduced without permission. However, not all materials appearing on this website are in the public domain. Each primary source is labeled to indicate its copyright and re-use status:", text, noPadding));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("No Copyright", text, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("Public Domain - This work has been identified as being free of known restrictions under copyright law, including all related and neighboring rights. You can copy, modify, distribute and perform the work, even for commercial purposes, all without asking permission. If you use documents or images from this website, we ask that you include the citation provided or credit the National Archives as the source.", text, noPadding));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("In Copyright", text, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("In Copyright - This Item is protected by copyright and/or related rights. You are free to use this Item in any way that is permitted by the copyright and related rights legislation that applies to your use. For other uses you need to obtain permission from the rights-holder(s).", text, noPadding));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("Unknown Rightsholder - This Item is protected by copyright and/or related rights. However, for this Item, either (a) no rights-holder(s) have been identified or (b) one or more rights-holder(s) have been identified but none have been located. You are free to use this Item in any way that is permitted by the copyright and related rights legislation that applies to your use.", text, paddingTop10));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Other", text, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("Copyright Not Evaluated - The copyright and related rights status of this Item has not been evaluated. You are free to use this Item in any way that is permitted by the copyright and related rights legislation that applies to your use.", text, noPadding));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("No Known Copyright – We believe that the Item is not restricted by copyright or related rights, but a conclusive determination could not be made. You are free to use this Item in any way that is permitted by the copyright and related rights legislation that applies to your use.", text, paddingTop10));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Some materials have been donated or obtained from individuals or organizations and may be subject to restrictions on use. You may consult NARA reference staff for details on specific items; please click through to the National Archives catalog entry from the National Archives Identifier link for specific archival information on any particular item, including the unit in the National Archives where the original item is held. Though we are aware of donor restrictions applicable to our collections, we cannot confirm copyright status for any item. Please note that because we cannot guarantee the status of specific items, you use materials found in our holdings at your own risk. NARA does not claim any rights to the digital reproductions of items in our holdings.", text, paddingTop10));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Certain individuals depicted may claim rights in their likenesses and images. Use of photographs or other materials found on this website may be subject to these claims. Anyone who intends to use these materials commercially should contact the individuals depicted or their representatives.", text, paddingTop10));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Registered users of this website can save activities and choose to share them with others or keep them private. Shared, or published activities, are available to all logged-in users of the site and receive the CC0 Public Domain Dedication; by sharing or publishing the activity, you waive all copyright and related rights to the extent possible under the law. We may contact authors of shared or published activities to make them available to the general public.", text, paddingTop10));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Accessibility", secondTitle, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("This app is designed to be accessible to visitors with disabilities, and to comply with federal guidelines concerning accessibility. We welcome your comments. If you have suggestions on how to make the site more accessible, please contact us at :", text, noPadding));

            //Setting the event for the email composer
            TapGestureRecognizer mailLink = new TapGestureRecognizer();
            mailLink.Tapped += MailLink_Tapped;

            string bindingContext = "rememberingwwi@nara.gov";
            stack_InnerHolder.Children.Add(CreateTextForPopUp("- rememberingwwi@nara.gov", text, noPadding, FontAttributes.None, null, mailLink, bindingContext));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Protecting Your Information", secondTitle, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("We adopt appropriate data collection, storage and processing practices and security measures to protect against unauthorized access, alteration, disclosure or destruction of your personal information, username, password, and data stored on our website. Sensitive and private data exchange between the website and its users happens over a SSL secured communication channel and is encrypted and protected with digital signatures.", text, noPadding));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Notification of Changes", secondTitle, paddingTop10, FontAttributes.Bold));
            stack_InnerHolder.Children.Add(CreateTextForPopUp("If we decide to change our privacy policy, we will post those changes to this page so our users are always aware of what information we collect and how we use it. Your continued use of the app after such change is made constitutes acceptance of and agreement to be bound by the terms as modified.", text, noPadding));

            stack_InnerHolder.Children.Add(CreateTextForPopUp("Last updated: September 29, 2016", text, paddingTop10, FontAttributes.Italic));

            scrollView.Content = stack_InnerHolder;
            stack_OuterHolder.Children.Add(scrollView);
            overlayFrame.Content = stack_OuterHolder;
            stack_TermsAndServices.Children.Add(overlayFrame);

            //await stack_TermsAndServices.LayoutTo(new Rectangle(50, -stack_TermsAndServices.Height, stack_TermsAndServices.Width, stack_TermsAndServices.Height));
        }

        /// <summary>
        /// Method that represents the mail composer component to the user
        /// </summary>
        private void MailLink_Tapped(object sender, EventArgs e)
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
            catch (Exception exc)
            { }
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
            {

            }
        }

        /// <summary>
        /// Method that returns a ContentView with the formated text of the terms and services
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <param name="padding"></param>
        /// <param name="fontAtribute"></param>
        /// <param name="formatedString"></param>
        /// <param name="tapEvent"></param>
        /// <param name="BindingContext"></param>
        /// <returns></returns>
        private ContentView CreateTextForPopUp(string text, int fontSize, Thickness padding, FontAttributes fontAtribute = FontAttributes.None, FormattedString formatedString = null, TapGestureRecognizer tapEvent = null, string BindingContext = null)
        {
            try
            {
                if (formatedString == null)
                {
                    var lbl = new Label() { Text = text, FontSize = fontSize, FontAttributes = fontAtribute, TextColor = Color.White };
                    var toReturn = new ContentView() { Padding = padding, Content = lbl };

                    if (tapEvent != null)
                    {
                        lbl.TextColor = Color.FromHex("#559FE3");
                        lbl.GestureRecognizers.Add(tapEvent);
                    }
                    if (!string.IsNullOrEmpty(BindingContext))
                    {
                        lbl.BindingContext = BindingContext;
                    }

                    return toReturn;

                }
                else
                {
                    var lbl = new Label() { FormattedText = formatedString, FontSize = fontSize, FontAttributes = fontAtribute, TextColor = Color.White };
                    var toReturn = new ContentView() { Padding = padding, Content = lbl };

                    if (tapEvent != null)
                    {
                        lbl.GestureRecognizers.Add(tapEvent);
                    }
                    if (!string.IsNullOrEmpty(BindingContext))
                    {
                        lbl.BindingContext = BindingContext;
                    }

                    return toReturn;
                }
            }
            catch { return new ContentView(); }
        }

        /// <summary>
        /// Builds the popup, that holds the text of the terms and conditions and inserts it to the relative layout
        /// </summary>
        private void BuildHelpPopUp()
        {
            int headerFontSize = 30;
            int firstTitle = 16;
            int secondTitle = 21;
            int text = 15;
            int rowHeight = 25;
            Color textColor = Color.White;

            FrameCustom overlayFrame = new FrameCustom() { OutlineColor = Color.FromHex("#717171"), BackgroundColor = Color.FromHex("#1d1d1d") };

            ScrollView scrollView = new ScrollView();
            StackLayout stack_InnerHolder = new StackLayout() { Padding = new Thickness(1), BackgroundColor = Color.FromHex("#1d1d1d") };
            StackLayout stack_OuterHolder = new StackLayout() { BackgroundColor = Color.FromHex("#1d1d1d") };

            Thickness noPadding = new Thickness(0);
            Thickness paddingTop10 = new Thickness(0, 25, 0, 0);

            TapGestureRecognizer tgr_ClosePopUp = new TapGestureRecognizer();
            tgr_ClosePopUp.Tapped += (s, e) =>
            {
                Task.Factory.StartNew(async () =>
                {
                    await stack_HelpText.LayoutTo(new Rectangle(50, -3350, stack_TermsAndServices.Width, stack_TermsAndServices.Height));
                }
                );

                Navigate();
            };

            //Image img_Close = new Image() { Aspect = Aspect.AspectFit, Source = "remove.png", HorizontalOptions = LayoutOptions.EndAndExpand };
            //img_Close.GestureRecognizers.Add(tgr_ClosePopUp);
            //stack_OuterHolder.Children.Add(img_Close);

            Label lbl_Header = new Label() { FontAttributes = FontAttributes.Bold, FontSize = headerFontSize, TextColor = textColor, FontFamily = "OrpheusPro", Margin = new Thickness(10, 10, 0, 5) };
            lbl_Header.Text = "You've successfully created an account. How can I get started?";

            Label lbl_HelpLine1 = new Label() { TextColor = textColor, FontSize = firstTitle, Text = "Read our tips for how you can use this app if you're a teacher or museum curator on our Welcome screen." };
            Label lbl_HelpLine2 = new Label() { TextColor = textColor, FontSize = firstTitle, Text = "Browse popular collections from the Archives and our partners, search content thematically from the Tag section, or view others using the app in the Curators section." };
            Label lbl_HelpLine3 = new Label() { TextColor = textColor, FontSize = firstTitle, Text = "Create your own thematic collection by clicking on 'My Collections' via your profile icon and then on 'Add collection.' Once you save it, make it 'private' if it's still a work in progress." };
            Label lbl_HelpLine4 = new Label() { TextColor = textColor, FontSize = firstTitle, Text = "You can also use the ' + ' buttons on any item in the app to add it to your collection, or start a new collection with that item." };

            Grid grid_MainText = new Grid()
            {
                Margin = new Thickness(10, 0, 0, 5),
                RowDefinitions = { new RowDefinition() { Height = rowHeight }, new RowDefinition() { Height = rowHeight }, new RowDefinition() { Height = rowHeight }, new RowDefinition() { Height = rowHeight }, },
                ColumnDefinitions = { new ColumnDefinition() { Width = 25 }, new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) } }
            };

            grid_MainText.Children.Add(GetAnchor(textColor, firstTitle), 0, 0);
            grid_MainText.Children.Add(lbl_HelpLine1, 1, 0);
            grid_MainText.Children.Add(GetAnchor(textColor, firstTitle), 0, 1);
            grid_MainText.Children.Add(lbl_HelpLine2, 1, 1);
            grid_MainText.Children.Add(GetAnchor(textColor, firstTitle), 0, 2);
            grid_MainText.Children.Add(lbl_HelpLine3, 1, 2);
            grid_MainText.Children.Add(GetAnchor(textColor, firstTitle), 0, 3);
            grid_MainText.Children.Add(lbl_HelpLine4, 1, 3);

            stack_InnerHolder.Children.Add(lbl_Header);
            stack_InnerHolder.Children.Add(grid_MainText);

            Button btn_GotIt = new Button() { TextColor = Color.White, WidthRequest = 160, Text = "Okay, I got it", BackgroundColor = Color.FromHex("#E0665E"), BorderRadius = 20, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0,10,0,20) };
            btn_GotIt.Clicked += Btn_GotIt_Clicked;

            stack_InnerHolder.Children.Add(btn_GotIt);

            scrollView.Content = stack_InnerHolder;
            stack_OuterHolder.Children.Add(scrollView);
            overlayFrame.Content = stack_OuterHolder;
            stack_HelpText.Children.Add(overlayFrame);

            //await stack_TermsAndServices.LayoutTo(new Rectangle(50, -stack_TermsAndServices.Height, stack_TermsAndServices.Width, stack_TermsAndServices.Height));
        }

        private void Btn_GotIt_Clicked(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                await stack_HelpText.LayoutTo(new Rectangle(50, -3350, stack_TermsAndServices.Width, stack_TermsAndServices.Height));
            });

            Navigate();
        }

        private Label GetAnchor(Color textColor, int firstTitle)
        {
            return new Label() { TextColor = textColor, FontSize = firstTitle, Text = "•" };
        }
    }
}
