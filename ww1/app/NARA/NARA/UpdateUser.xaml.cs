using ImageCircle.Forms.Plugin.Abstractions;
using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Service;
using NARA.Common_p.Util;
using NARA.Util;
using Plugin.Connectivity;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace NARA
{
    /// <summary>
    /// UpdateUser class inherits xamarin ContentPage and enables the user to change profile data
    /// </summary>
    public partial class UpdateUser : ContentPage
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

        EntryCustom entry_Name;
        EntryCustom entry_Surname;
        EntryCustom entry_Password;
        EntryCustom entry_PasswordRepeat;
        EntryCustom entry_Email;
        Button btn_Login;
        CircleImage circleImage = new CircleImage() { HeightRequest = 50, WidthRequest = 50, Aspect = Aspect.AspectFill };
        Frame frame;
        StackLayout btnHolder;
        StackLayout stackCB0;
        StackLayout stackCB;
        StackLayout stackCB2;
        Label lbl_Teacher;
        Label lbl_Newsletter;
        Label lbl_Terms;
        Switch cb_Terms;

        CustomCheckBox cb_Teacher = new CustomCheckBox();
        CustomCheckBox cb_Newsletter = new CustomCheckBox();

        Encrypt encrypt = new Encrypt();
        string encryptKey = "B374A26A71490437AA024E4FADD5B497";

        double _width = 0;
        double _height = 0;
        string returnUrlL = "";
        string imgUrl = "";
        string facebookToken = "";
        byte[] imageInBytes;
        User user = new User();
        public UpdateUser()
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

                //Tap gestures for custom checkboxes
                TapGestureRecognizer tgr_CheckBox = new TapGestureRecognizer();
                tgr_CheckBox.Tapped += Tgr_CheckBox_Tapped;

                cb_Teacher.GestureRecognizers.Add(tgr_CheckBox);
                cb_Newsletter.GestureRecognizers.Add(tgr_CheckBox);

                loading_Indicator.IsVisible = true;

                //Tap gesture for back navigation
                var tapGestureRecognizerBack = new TapGestureRecognizer();
                tapGestureRecognizerBack.Tapped += (s, e) =>
                {
                    NavigateBack();
                };

                img_Previous.GestureRecognizers.Add(tapGestureRecognizerBack);

                //Building of the main entry controls such as name, surname, checkboxes, profile picture upload

                entry_Name = new EntryCustom() { Placeholder = "Name", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.CenterAndExpand };
                entry_Surname = new EntryCustom() { Placeholder = "Surname", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.CenterAndExpand };
                entry_Email = new EntryCustom() { Placeholder = "Email", TextColor = Color.FromHex("#717171"), FontSize = 16, PlaceholderColor = Color.FromHex("#717171"), BackgroundColor = Color.Transparent, HeightRequest = 50, HorizontalOptions = LayoutOptions.CenterAndExpand };

                btn_Login = new Button() { Text = "SAVE PROFILE", WidthRequest = 160, FontAttributes = FontAttributes.Bold, BorderColor = Color.FromHex("#E0665E"), FontSize = 20, BorderRadius = 20, BorderWidth = 2, BackgroundColor = Color.FromHex("#E0665E"), TextColor = Color.White, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };

                btn_Login.Clicked += btn_Login_Clicked;

                btnHolder = new StackLayout() { Spacing = 14, HorizontalOptions = LayoutOptions.CenterAndExpand, Padding = new Thickness(10, 0, 10, 0) };
                btnHolder.Children.Add(entry_Name);
                btnHolder.Children.Add(entry_Surname);
                btnHolder.Children.Add(entry_Email);

                Button btn_ChangePassword = new Button() { Text = "Change password", HeightRequest = 15, WidthRequest = 140, FontAttributes = FontAttributes.None, BorderColor = Color.FromHex("#E0665E"), FontSize = 16, BorderRadius = 0, BorderWidth = 0, BackgroundColor = Color.Transparent, TextColor = Color.FromHex("#E0665E"), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
                btn_ChangePassword.Clicked += Btn_ChangePassword_Clicked;

                lbl_Teacher = new Label() { Text = "I'm a teacher", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End, FontSize = 16, TextColor = Color.FromHex("#717171") };
                stackCB0 = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, Padding = new Thickness(0, 10, 0, 0) };
                stackCB = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, Padding = new Thickness(0, 0, 0, 0) };
                stackCB0.Children.Add(btn_ChangePassword);
                stackCB.Children.Add(lbl_Teacher);
                stackCB.Children.Add(cb_Teacher);

                lbl_Newsletter = new Label() { Text = "Would you like to receive emails regarding this app?", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End, FontSize = 16, TextColor = Color.FromHex("#717171") };
                stackCB2 = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, Padding = new Thickness(0, 0, 0, 10) };
                stackCB2.Children.Add(lbl_Newsletter);
                stackCB2.Children.Add(cb_Newsletter);

                btnHolder.Children.Add(stackCB0);
                btnHolder.Children.Add(stackCB);
                btnHolder.Children.Add(stackCB2);

                Button imagePicker = new Button() { Text = "TAP TO UPLOAD A PROFILE PHOTO", TextColor = Color.FromHex("#717171"), FontSize = 12 };

                frame = new Frame() { OutlineColor = Color.FromHex("#717171"), BackgroundColor = Color.FromHex("#1d1d1d") };
                StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                stack.Children.Add(circleImage);
                stack.Children.Add(new Label() { Text = "TAP TO UPLOAD A PROFILE PHOTO", TextColor = Color.FromHex("#717171"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, FontSize = 12, FontAttributes = FontAttributes.Bold });
                frame.Content = stack;

                //Tap gesture for the profile image picker
                var tapGestureAvatar = new TapGestureRecognizer();
                tapGestureAvatar.Tapped += (s, e) =>
                {
                    ImagePicker_Clicked();
                };

                frame.GestureRecognizers.Add(tapGestureAvatar);

                btnHolder.Children.Add(frame);

                ContentView cv_Btn = new ContentView() { Padding = new Thickness(0, 15, 0, 0) };
                cv_Btn.Content = btn_Login;
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

            }
            catch (Exception e)
            {
            }
        }

        private async void Btn_ChangePassword_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangePassword());
        }

        /// <summary>
        /// Method that handles custom checkbox states (checked, unchecked)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Retireves data and populates user controles with profile info
        /// </summary>
        private async void SetUserData()
        {
            try
            {
                user = repo.GetUser();
                user.RegistrationUserName = user.UserName;
                user = await userRepo.GetUserByUsername(user.UserName);


                if (!string.IsNullOrEmpty(user.Name))
                {
                    entry_Name.Text = user.Name;
                }
                if (!string.IsNullOrEmpty(user.Surname))
                {
                    entry_Surname.Text = user.Surname;
                }
                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    circleImage.Source = ImageSource.FromUri(new Uri(user.ImageUrl));
                }
                if (!string.IsNullOrEmpty(user.Email))
                {
                    entry_Email.Text = user.Email;
                }
                if (user.IsCurator.HasValue)
                {
                    cb_Teacher.IsChecked = user.IsCurator.Value;
                }

                loading_Indicator.IsVisible = false;
            }
            catch (Exception e)
            {
                loading_Indicator.IsVisible = false;
                ShowMessage("Error occured while retrieving user data. Please, try again later.");
            }
        }

        /// <summary>
        /// Retrieves image from the gallery
        /// </summary>
        private async void ImagePicker_Clicked()
        {
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync();
                imageInBytes = ReadFully(file.GetStream());
                imgUrl = file.Path;
                circleImage.Source = file.Path;
            }
            catch (Exception e)
            { }
        }

        /// <summary>
        /// Method that handles back navigation
        /// </summary>
        private async void NavigateBack()
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Event that handles user update process, sends data to the API
        /// </summary>
        async void btn_Login_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Hiding the loading indicator and setting inserted user info to a object that will be send to the API
                loading_Indicator.IsVisible = true;
                user.Name = entry_Name.Text;
                user.Surname = entry_Surname.Text;
                user.RegistrationUserName = user.UserName;
                user.IsCurator = cb_Teacher.IsChecked;
                user.Email = entry_Email.Text;

                //Sending data to the API
                var response = await userRepo.UpdateUser(user);

                if (response.Success)
                {
                    //If profile image is selected it proceeds to the uploading of the image 
                    if (imgUrl != "")
                    {
                        try
                        {
                            response.ImgUrl = System.IO.Path.GetFileName(imgUrl);

                            //Uploads image to the API
                            var imageUpload = await userRepo.UpdateUserProfilePicture(response, imageInBytes);

                            //Saves the user info. to the app DB so it can access the user profile picture/username in the app
                            repo.SaveUser(await userRepo.GetUserByUsername(user.UserName));
                            loading_Indicator.IsVisible = false;
                            ShowMessage("Your profile was successfully updated");
                        }
                        catch (Exception excp)
                        {
                            //In case of an error, the error message is displayed
                            loading_Indicator.IsVisible = false;
                            lbl_Error.Text = excp.Message + " ::: " + excp.StackTrace;
                            ShowMessage("Your profile was updated, but an error occured while uploading profile picture", null, 4000, true);
                        }

                    }
                    else
                    {
                        //In case of an error, the error message is displayed
                        loading_Indicator.IsVisible = false;
                        ShowMessage("Your profile was successfully updated");
                    }


                }
                else
                {
                    //In case of an error, the error message is displayed
                    loading_Indicator.IsVisible = false;
                    ShowMessage("An error occured, please check your internet connection");
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

                loading_Indicator.WidthRequest = width;
                loading_Indicator.HeightRequest = height;

                stack_Overlay.WidthRequest = width;

                var requestedWidth = (width / 2) - 80;

                btnHolder.WidthRequest = requestedWidth;

                entry_Surname.WidthRequest = requestedWidth;
                entry_Name.WidthRequest = requestedWidth;
                entry_Email.WidthRequest = requestedWidth;

                stackCB.WidthRequest = requestedWidth;
                stackCB0.WidthRequest = requestedWidth;
                stackCB2.WidthRequest = requestedWidth;


                lbl_Teacher.WidthRequest = requestedWidth - 35;
                lbl_Newsletter.WidthRequest = requestedWidth - 35;

            }
            catch { }
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
    }
}
