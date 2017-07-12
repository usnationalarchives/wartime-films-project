using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Util;
using NARA.Util;
using NARA.Views;
using Plugin.Connectivity;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NARA
{
    /// <summary>
    /// HomeScreen class inherits xamarin ContentPage and presents home screen of the app
    /// </summary>
    /// 
    public partial class HomeScreen : ContentPage
    {
        MuseumsImageProvider imageProvider = new MuseumsImageProvider();
        bool isConnected = DependencyService.Get<IPlatformSpecific>().CheckConnection();
        string offlineConnection = DependencyService.Get<IPlatformSpecific>().ConnectionString();
        BoxView loadingView = new BoxView() { BackgroundColor = Color.FromHex("#1d1d1d") };
        Label lbl_LoadingData = new Label();
        WebView loadingWV;
        OfflineRepository offlineRepo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
        Button btnRetry = new Button() { Text = "Retry", WidthRequest = 100, BorderColor = Color.FromHex("#E0665E"), FontSize = 25, BorderRadius = 20, BorderWidth = 2, BackgroundColor = Color.Transparent, TextColor = Color.White };
        Label lbl_SignOut;
        Label lbl_SignIn;
        Label lbl_SignUp;
        StackLayout stack_CenterMainText;
        Label logging = new Label() { TextColor = Color.Red };

        BoxView bx_ToolbarBottomBorder;
        BoxView bx_ToolbarBottomBorder2;
        BoxView bx_ToolbarBottomBorder3;
        StackLayout stck;
        StackLayout cv_ButtonHolder;
        double initialY = 0;
        bool rotated = false;
        StackLayout upperToolbarLoader = new StackLayout();

        StackLayout stack_Confirmation = new StackLayout() { BackgroundColor = Color.FromHex("#e0c95e"), HeightRequest = 50, IsVisible = false };
        Label lbl_Confirmation = new Label() { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.Black, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };

        double _width, _height;

        StackLayout stck_HelpTip;


        string html = @"
         <html>
            <head>
                <style>

                    body, html {
                          background: #1d1d1d;
                          width: 100%;
                          height: 100%;
                          margin: 0;
                          padding: 0;
                          opacity:0.5;
                        }

                        .loader {
                          width: 120px;
                          height: 120px;
                          position: relative;
                          top: 50%;
                          margin: -60px auto;
                        }

                        .loader:after {
                          content: "";
                          display: block;
                          background: url('loading-overlay.png');
                          background-size: 100%;
                          width: 100%;
                          height: 100%;
                          position: absolute;
                          top: 0;
                          left: 0;
                        }

                        .loader svg {
                          display: block;
                          height: 100%;
                          width: 100%;
                          transform: rotate(-90deg);
                        }

                        .loader svg circle {
                          fill: rgba(0, 0, 0, 0);
                          stroke-width: 24;
                          stroke: rgba(255, 255, 255, 0.25);
                          stroke-dasharray: 90;
                          stroke-dashoffset: 180;
                          animation: loading 2s linear infinite forwards;
                          -webkit-animation: loading 2s linear infinite forwards;
                        }

                        keyframes loading {
                          to {
                            stroke-dashoffset: 0
                          }
                        }

                        @-webkit-keyframes loading {
                          to {
                            stroke-dashoffset: 0
                          }
                        }
                     </style>
                </head>

            <body>
                        <div class='loader'>	
                              <svg viewBox='0 0 48 48'>
		                           <circle class='pie1' cx='24' cy='24' r='12'/>
	                          </svg>
                        </div>
            </body>
        </html>";

        int fontSize_Header = 40;
        int fontSize_Description = 15;
        int fontSize_BottomHeader = 25;
        RoundedImage profilePic;
        StackLayout userMenu = new StackLayout() { IsVisible = false, Spacing = 0 };
        Grid upperToolbar;
        Grid signOutToolbar;
        bool showMessage = false;
        Grid grid_CenteredContent = new Grid();
        WebContentPage _ExplorePage;
        AboutPage _AboutPage;
        TeachersPage _TeachersPage;
        CuratorsPage _CuratorsPage;
        /// <summary>
        /// Constructor takes in property:
        ///  - refresh, which indicates if it needs to refresh data from api
        ///  - message, to be shown in a notification bar
        /// </summary>
        /// <param name="refresh"></param>
        /// <param name="message"></param>
        ///
        public HomeScreen(bool refresh = false, FormattedString message = null)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            InitWebView();
            btnRetry.Clicked += BtnRetry_Clicked;
            //logging.Text += "BeforeLoadHomepage(): " + sw.ElapsedMilliseconds.ToString();
            LoadHomepage(refresh, message);
            //logging.Text += "LoadHomepage(): " + sw.ElapsedMilliseconds.ToString();
            //sw.Stop();
            //logging.Text += "App: " + App.sw.ElapsedMilliseconds.ToString() + Environment.NewLine + "Homescreen: " + sw.ElapsedMilliseconds.ToString();
        }

        private async void InitWebView()
        {
            await Task.Run(() =>
            {
                _ExplorePage = new WebContentPage(NaraTools.ExplorePage);
                _AboutPage = new AboutPage();
                _TeachersPage = new TeachersPage();
                _CuratorsPage = new CuratorsPage();
            }
            );
        }

        /// <summary>
        /// Method, that reloads home screen when user clicks retry if error occurs
        /// </summary>
        private void BtnRetry_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new HomeScreen()) { BarBackgroundColor = Color.FromHex("#1d1d1d"), BarTextColor = Color.White, BackgroundColor = Color.FromHex("#1d1d1d") };
        }
        /// <summary>
        /// LoadHomepage builds view of a homescreen, binds all of the controls and work out positioning, such as top
        /// navigation bar, middle section, bottom items and handles navigation using TapGestures
        /// </summary>
        /// <param name="refresh"></param>
        /// <param name="message"></param>
        private void LoadHomepage(bool refresh = false, FormattedString message = null)
        {
            try
            {

                //IMPLEMENTING HOMESCREEN
                Stopwatch sw = new Stopwatch();
                sw.Start();
                //LoadingScreen();

                upperToolbarLoader.Children.Add(new ImageViewCustom() { ImageSourceInByteArray = App.LoaderImageInByteArray, Source = "loader.gif", Aspect = Aspect.AspectFit, HeightRequest = 50, WidthRequest = 50, HorizontalOptions = LayoutOptions.End, Margin = new Thickness(0, 0, 50, 0) });


                //Building components of the top navigation bar, which consists of SIGN IN/OUT, SIGN UP and context menu of user
                stack_Confirmation.Children.Add(lbl_Confirmation);
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



                var bckImage = new Image() { Source = "homebackground.png", Aspect = Aspect.AspectFill, Opacity = 0.6 };
                TapGestureRecognizer tgr_BckImage = new TapGestureRecognizer();
                tgr_BckImage.Tapped += Tgr_BckImage_Tapped;
                bckImage.GestureRecognizers.Add(tgr_BckImage);
                rl_Main.Children.Add(bckImage, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width;
                }), Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height;
                }));


                //SIGN IN and SIGN UP labels for the upper toolbar

                int lblHeight = 38;

                if (Device.OS == TargetPlatform.Android)
                {
                    lblHeight = 45;
                }

                lbl_SignUp = new LabelCustom() { RequestedHeight = 1, HeightRequest = lblHeight, WidthRequest = 90, BorderWidth = 2, BorderRadius = 19, BorderColor = Color.FromHex("#E0665E"), LetterSpacing = 1, Text = "SIGN UP", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
                lbl_SignIn = new LabelCustom() { HeightRequest = lblHeight, WidthRequest = 90, BorderWidth = 2, BorderRadius = 19, BackgroundColor = Color.Transparent, BorderColor = Color.FromHex("#E0665E"), LetterSpacing = 1, Text = "SIGN IN", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
                lbl_SignOut = new LabelCustom() { HeightRequest = lblHeight, WidthRequest = 90, BorderWidth = 2, BorderRadius = 19, BorderColor = Color.FromHex("#E0665E"), LetterSpacing = 1, Text = "SIGN OUT", TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.EndAndExpand };


                //Tap gesture recognizers which also checks if user has network connection and then navigates it

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
                var tapGestureRecognizerLogout = new TapGestureRecognizer();
                tapGestureRecognizerLogout.Tapped += (s, e) =>
                {
                    offlineRepo.ClearUser();
                    webUtil.ClearLogin();
                    CheckLogin();
                };

                lbl_SignUp.GestureRecognizers.Add(tapGestureRecognizerSignUp);
                lbl_SignIn.GestureRecognizers.Add(tapGestureRecognizerLogin);
                lbl_SignOut.GestureRecognizers.Add(tapGestureRecognizerLogout);


                //Grid for the upper toolbar items

                int gridHeight = 38;
                int gridSpacing = 5;
                int topGridWidth = 170;

                if (Device.OS == TargetPlatform.Android)
                {
                    gridHeight = 38;
                    gridSpacing = 5;
                    topGridWidth = 160;
                }

                upperToolbar = new Grid()
                {
                    //HorizontalOptions = LayoutOptions.EndAndExpand,
                    IsVisible = false,
                    ColumnSpacing = gridSpacing,
                    WidthRequest = topGridWidth,
                    ColumnDefinitions = { new ColumnDefinition() { Width = GridLength.Auto }, new ColumnDefinition() { Width = new GridLength(23, GridUnitType.Absolute) }, new ColumnDefinition() { Width = GridLength.Auto } },
                    RowDefinitions = { new RowDefinition() { Height = new GridLength(gridHeight, GridUnitType.Absolute) }, new RowDefinition() { Height = new GridLength(2, GridUnitType.Absolute) } }
                };

                signOutToolbar = new Grid()
                {
                    IsVisible = false,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    WidthRequest = 50,
                    HeightRequest = 50,
                    ColumnDefinitions = { new ColumnDefinition() { Width = GridLength.Auto } },
                    RowDefinitions = { new RowDefinition() { Height = new GridLength(gridHeight, GridUnitType.Absolute) }, new RowDefinition() { Height = new GridLength(2, GridUnitType.Absolute) } }
                };


                //Borders : Changed background color
                bx_ToolbarBottomBorder = new BoxView() { HeightRequest = 2, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.StartAndExpand, BackgroundColor = Color.Transparent };
                bx_ToolbarBottomBorder2 = new BoxView() { HeightRequest = 2, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.StartAndExpand, BackgroundColor = Color.Transparent };
                bx_ToolbarBottomBorder3 = new BoxView() { HeightRequest = 2, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.StartAndExpand, BackgroundColor = Color.Transparent };

                int minusFont = 0;

                if (Device.OS == TargetPlatform.iOS)
                {
                    //lbl_SignIn.FontSize -= 3;
                    //lbl_SignUp.FontSize -= 3;
                    //lbl_SignOut.FontSize -= 3;
                    lbl_SignIn.FontSize = 15.2;
                    lbl_SignUp.FontSize = 15.2;
                    lbl_SignOut.FontSize = 15.2;
                }
                else
                {
                    //lbl_SignIn.FontSize += 1;
                    //lbl_SignUp.FontSize += 1;
                    //lbl_SignOut.FontSize += 1;
                    lbl_SignIn.FontSize = 15.2;
                    lbl_SignUp.FontSize = 15.2;
                    lbl_SignOut.FontSize = 15.2;
                }


                upperToolbar.Children.Add(lbl_SignUp, 2, 0);
                upperToolbar.Children.Add(new Label() { Text = "or", FontAttributes = FontAttributes.Italic, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White, FontSize = 15 }, 1, 0);
                upperToolbar.Children.Add(lbl_SignIn, 0, 0);
                upperToolbar.Children.Add(bx_ToolbarBottomBorder, 0, 1);
                upperToolbar.Children.Add(bx_ToolbarBottomBorder2, 1, 1);

                int authGridPositionMargin = 0;

                if (Device.OS == TargetPlatform.iOS)
                {
                    authGridPositionMargin = 223;
                }
                else { authGridPositionMargin = 225; }

                rl_Main.Children.Add(upperToolbar,
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - authGridPositionMargin;
                }),
                Constraint.Constant(10), Constraint.RelativeToParent((parent) =>
                {
                    return 250;
                }),
                Constraint.Constant(48));

                rl_Main.Children.Add(upperToolbarLoader,
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - authGridPositionMargin;
                }),
                Constraint.Constant(10), Constraint.RelativeToParent((parent) =>
                {
                    return 250;
                }),
                Constraint.Constant(48));

                //Components of user rounded profile picture

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
                stck = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = Device.OnPlatform(5, 0, 5), VerticalOptions = LayoutOptions.FillAndExpand, IsVisible = false };
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

                //Grid which centers all of the main content of the homescreen

                grid_CenteredContent = new Grid()
                {
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    RowDefinitions =
                {
                    new RowDefinition(){Height = new GridLength(550,GridUnitType.Absolute)}
                },
                    ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3.8, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) }
                }
                };

                grid_CenteredContent.GestureRecognizers.Add(tgr_BckImage);

                // Pre-title text
                stack_CenterMainText = new StackLayout() { Spacing = 14, Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand };
                stack_CenterMainText.Children.Add(new Label() { FontSize = Device.OnPlatform(fontSize_Description, fontSize_Description + 2, fontSize_Description), TextColor = Color.FromHex("#E0665E"), Text = "The National Archives & Records Administration", VerticalOptions = LayoutOptions.EndAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center });

                //Title
                ContentView mainheader = new ContentView() { Content = new Label { FontSize = Device.OnPlatform(65, 67, 65), TextColor = Color.White, FontFamily = Device.OnPlatform("Orpheus Pro", "OrpheusPro.otf#OrpheusPro", ""), Text = "Remembering WWI", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand }, Padding = new Thickness(0, 10, 0, 0) };
                stack_CenterMainText.Children.Add(mainheader);

                //Main text, which is center aligned
                ContentView cv_LabelPadding = new ContentView() { Padding = new Thickness(0, 14, 0, 0) };
                cv_LabelPadding.Content = new LabelCustom()
                {
                    LineSpacing = 8,
                    FontSize = Device.OnPlatform(fontSize_Description, fontSize_Description + 2, fontSize_Description),
                    TextColor = Color.White,
                    Text = @"The National Archives and Records Administration (NARA) invites you to participate, collaborate, and engage with the Archive's extensive collection of World War I moving and still images. Together with other national and local partners, you have at your fingertips an unprecedented amount of newly digitized and expertly preserved content. We welcome you to explore, interpret and reuse this selection of America’s most important cultural heritage.",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                };

                BottomItems();

                stack_CenterMainText.Children.Add(cv_LabelPadding);

                // Padding = new Thickness(0, Device.OnPlatform(50, 45, 20), 0, 30)
                cv_ButtonHolder = new StackLayout() { VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand };

                //Explore the archives button, which navigates to the Archives page; 19.1.2017 : 180
                Button btn_Explore = new Button() { HeightRequest = 60, WidthRequest = Device.OnPlatform(300, 320, 180), FontSize = Device.OnPlatform(20, 22, 13), FontAttributes = Device.OnPlatform(FontAttributes.Bold, FontAttributes.None, FontAttributes.None), BackgroundColor = Color.FromHex("#E0665E"), BorderColor = Color.FromHex("#E0665E"), BorderRadius = 20, BorderWidth = 2, TextColor = Color.White, Text = "EXPLORE THE ARCHIVE", VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand };
                btn_Explore.Clicked += btn_Explore_Click;
                cv_ButtonHolder.Children.Add(btn_Explore);

                stack_CenterMainText.Children.Add(cv_ButtonHolder);

                //, BackgroundColor = Color.Gray 
                ContentView cv = new ContentView() { VerticalOptions = LayoutOptions.FillAndExpand };
                cv.Content = stack_CenterMainText;

                grid_CenteredContent.Children.Add(stack_CenterMainText, 1, 0);


                //Working out the positioning of all controls, using main RelativeLayout
                rl_Main.Children.Add(grid_CenteredContent, Constraint.RelativeToParent((parent) =>
                {
                    return 0;
                }), Constraint.RelativeToParent((parent) =>
                {
                    //return (parent.Height / 6) - 100;
                    return (parent.Height * 0.05);
                })
                , Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height * 0.85);
                }));


                //Method that takes care of botton items 
                //BottomItems();


                loadingView.IsVisible = false;
                //loadingWV.IsVisible = false;
                lbl_LoadingData.IsVisible = false;

                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    fontSize_Header = 70;
                    fontSize_Description = 12;
                    fontSize_BottomHeader = 30;
                }

                rl_Main.Children.Add(stack_Confirmation,

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

                stck_HelpTip = new StackLayout() { IsVisible = false, Spacing = 0 };
                var childs = userMenu.Children.ToList();
                stck_HelpTip.Children.Add(childs.FirstOrDefault());

                var stack_HelpTip = new StackLayout()
                {
                    HeightRequest = 110,
                    WidthRequest = 255,
                    BackgroundColor = Color.White
                };

                stack_HelpTip.Children.Add(new Label()
                {
                    FormattedText = new FormattedString()
                    {
                        Spans = { new Span() { Text = "Sign up for free", FontAttributes = FontAttributes.Bold, ForegroundColor = Color.Black, FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", ""), FontSize = 14
            } }
                    },
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Margin = new Thickness(0, 10, 0, 0)
                });
                stack_HelpTip.Children.Add(new Label()
                {
                    FormattedText = new FormattedString()
                    {
                        Spans = { new Span() { Text = "to create your own collections!", ForegroundColor = Color.Black, FontFamily = Device.OnPlatform("Freight", "freightsans.otf#Freight", ""), FontSize = 14
            } }
                    },
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                });


                var btn_CloseHelpTip = new Button() { BackgroundColor = Color.Transparent, HeightRequest = Device.OnPlatform(33, 40, 0), BorderWidth = 0, TextColor = Color.Black, FontSize = 14, Text = "OK", HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(0,0,0, Device.OnPlatform(20, 20, 0)) };
                //TapGestureRecognizer tgr_CloseHelpTip = new TapGestureRecognizer();

                //tgr_CloseHelpTip.Tapped += Tgr_CloseHelpTip_Tapped;
                btn_CloseHelpTip.Clicked += Btn_CloseHelpTip_Clicked;

                stack_HelpTip.Children.Add(new BoxView() { HeightRequest = 2, WidthRequest = 195, HorizontalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Color.LightGray, Margin = new Thickness(0,10,0,0)});

                stack_HelpTip.Children.Add(btn_CloseHelpTip);

                stck_HelpTip.Children.Add(stack_HelpTip);

                rl_Main.Children.Add(stck_HelpTip,

                Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - 250);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (65);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (220);
                }),
                Constraint.Constant(115)
                );



                if (NaraTools.LogingEnabled)
                {
                    rl_Main.Children.Add(logging,

                    Constraint.RelativeToParent((parent) =>
                    {
                        return 0;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return 0;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width * 0.6;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Height / 2;
                    }));
                }

                if (message != null)
                {
                    lbl_Confirmation.FormattedText = message;
                    showMessage = true;
                }



                //Method that checks whether the user is signed in or not
                initialY = cv_ButtonHolder.TranslationY;

            }
            catch (Exception e)
            {
                //btnRetry.IsVisible = true;
                ////loadingWV.IsVisible = false;
                //loadingView.IsVisible = true;
                lbl_LoadingData.FontAttributes = FontAttributes.Italic;
                lbl_LoadingData.TextColor = Color.FromHex("#E0665E");
                //MessagingCenter.Send<HomeScreen>(this, "DisableMenu");
                lbl_LoadingData.Text = "Please, check your internet connection";
                lbl_LoadingData.IsVisible = true;
            }

        }

        private void Btn_CloseHelpTip_Clicked(object sender, EventArgs e)
        {
            offlineRepo.SaveAdditionalData(new AdditionalData() { Date = DateTime.Now, Id = offlineRepo.GetAdditionalData().Count + 1, Name = "homescreentip", Value = "1" });
            stck_HelpTip.IsVisible = false;
        }

        /// <summary>
        /// Hides user menu context
        /// </summary>
        private void Tgr_BckImage_Tapped(object sender, EventArgs e)
        {
            userMenu.IsVisible = false;
        }

        /// <summary>
        /// Sign out user
        /// </summary>
        private void Tgr_SignOut_Tapped(object sender, EventArgs e)
        {
            userMenu.IsVisible = false;
            webUtil.ClearLogin();
            CheckLogin();
        }

        /// <summary>
        /// Navigates user to the profile view
        /// </summary>
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
        /// Shows or hides the user menu context, wheter is it opened or closed
        /// </summary>
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

        /// <summary>
        /// Navigates to the view with webview control, based on the url
        /// </summary>
        /// <param name="url"></param>
        async void MyNavigation(string url)
        {
            //await Navigation.PushAsync(new WebContentPage(url));
        }

        /// <summary>
        /// Method that displays message if it was send to the view
        /// </summary>
        protected async void ShowMessage()
        {
            stack_Confirmation.IsVisible = true;
            await stack_Confirmation.LayoutTo(new Rectangle(0, 0, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
            await Task.Delay(5000);
            await stack_Confirmation.LayoutTo(new Rectangle(0, -50, stack_Confirmation.Width, stack_Confirmation.Height), 250, Easing.Linear);
            stack_Confirmation.IsVisible = false;
        }

        /// <summary>
        /// Method that handles navigation of the EXPLORE THE ARCHIVES button
        /// </summary>
        async void btn_Explore_Click(object sender, EventArgs e)
        {
            try
            {
                MessagingCenter.Send<App>((App)Xamarin.Forms.Application.Current, "makesure");
                if (CrossConnectivity.Current.IsConnected)
                {
                    if (_ExplorePage != null)
                    {
                        await Navigation.PushAsync(_ExplorePage);
                    }
                }
                else
                {
                    ShowNetworkError();
                }
            }
            catch (Exception exc)
            {
                ShowNetworkError();
            }
        }

        /// <summary>
        /// Shows alert message if an error related to network occurs
        /// </summary>
        private async void ShowNetworkError()
        {
            try
            {
                await this.DisplayAlert("Error", "Check your internet connection", "Proceed");
            }
            catch { }
        }

        /// <summary>
        /// Method that handles implementation of bottom rectangles, which containes text and navigation to the
        /// About,Curators and Teacher's page
        /// </summary>
        private void BottomItems()
        {
            //logging.Text += "BottomItems(): " + sw.ElapsedMilliseconds.ToString();
            Thickness bottomItemsPadding = new Thickness(15, 0, 15, 0);

            var tapGestureRecognizerEditor = new TapGestureRecognizer();
            tapGestureRecognizerEditor.Tapped += async (s, e) =>
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
            var tapGestureRecognizerAbout = new TapGestureRecognizer();
            tapGestureRecognizerAbout.Tapped += (s, e) =>
            {
                Navigation.PushAsync(new AboutPage());
            };

            var tapGestureRecognizerForTeachers = new TapGestureRecognizer();
            tapGestureRecognizerForTeachers.Tapped += (s, e) =>
            {
                Navigation.PushAsync(new TeachersPage());
            };

            //logging.Text += "Gestures: " + sw.ElapsedMilliseconds.ToString();

            Grid grid_BottomItems = new Grid()
            {
                ColumnSpacing = 10,
                Padding = new Thickness(10, 10, 10, 10),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            Grid grid_BottomLeftItem = new Grid()
            {
                Padding = bottomItemsPadding,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                RowDefinitions =
                {
                    new RowDefinition(){Height = GridLength.Auto},
                    new RowDefinition(){Height = GridLength.Auto},
                    new RowDefinition(){Height = GridLength.Auto}
                },
            };

            grid_BottomItems.Children.Add(BuildBottomItems("About", "Learn more about Remembering WWI", "thumb_about.png", 0), 0, 0);
            grid_BottomItems.Children.Add(BuildBottomItems("For Teachers", "Tips for educators on using this app in the classroom", "thumb_teachers.png", 1), 1, 0);
            grid_BottomItems.Children.Add(BuildBottomItems("For Institutions", "Tips for local institutions on utilizing this app for community activities", "thumb_curators.png", 2), 2, 0);

            //logging.Text += "EndBuildBottomItems(): " + sw.ElapsedMilliseconds.ToString();

            rl_Main.Children.Add(grid_BottomItems, Constraint.Constant(0)
            , Constraint.RelativeToParent((parent) =>
            {
                //return (parent.Height / 3.5) + (parent.Height / 2);
                return (parent.Height * 0.80);
            })
            , Constraint.RelativeToParent((parent) =>
            {
                return parent.Width;
            }),
            Constraint.RelativeToParent((parent) =>
            {
                //return parent.Height - ((parent.Height / 3.5) + (parent.Height / 2));
                return parent.Height * 0.20;
            }));

            //logging.Text += "EndBottomItems(): " + sw.ElapsedMilliseconds.ToString();
            //+ (parent.Height / 2)
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
            if (height > 0 && width > 0)
            {
                _height = height;
                _width = width;
            }


            base.OnSizeAllocated(width, height);


            try
            {
                //if (Device.OS == TargetPlatform.iOS)
                //{
                //if (!rotated)
                //{
                //    cv_ButtonHolder.TranslationY += 400;
                //}

                grid_CenteredContent.TranslationY = 0;

                var gRow = grid_CenteredContent.RowDefinitions.FirstOrDefault().Height;

                if (gRow.Value != (_height * 0.65))
                {
                    grid_CenteredContent.RowDefinitions.FirstOrDefault().Height = _height * 0.65;
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Method that implements loading component of the application
        /// </summary>
        private void LoadingScreen()
        {
            int act_size = 0;
            int minusActivator = 0;

            if (Device.OS == TargetPlatform.iOS)
                minusActivator = 20;
            else
                minusActivator = 40;

            act_size = 50;

            lbl_LoadingData = new Label() { Text = "Loading data ...", FontFamily = NaraTools.SelectedFont, FontSize = 12, TextColor = Color.White, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };

            rl_Main.Children.Add(loadingView, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) =>
            {
                return parent.Width;
            }),
            Constraint.RelativeToParent((parent) =>
            {
                return parent.Height;
            })
            );

            //rl_Main.Children.Add(loadingWV,

            //            Constraint.Constant(0),
            //            Constraint.Constant(0),

            //            Constraint.RelativeToParent((parent) =>
            //            {
            //                return parent.Width;
            //            }),
            //            Constraint.RelativeToParent((parent) =>
            //            {
            //                return parent.Height;
            //            })
            //            );

            lbl_LoadingData.IsVisible = false;

            rl_Main.Children.Add(lbl_LoadingData,

            Constraint.Constant(10),
            Constraint.RelativeToParent((parent) =>
            {
                return 12;
            }),

            Constraint.RelativeToParent((parent) =>
            {
                return parent.Width;
            }),
            Constraint.Constant(act_size)
            );

            btnRetry.IsVisible = false;
            //rl_Main.Children.Add(btnRetry,

            //Constraint.RelativeToParent((parent) =>
            //{
            //    return (parent.Width / 2) - 50;
            //}),
            //Constraint.RelativeToParent((parent) =>
            //{
            //    return (parent.Height / 2) + 25;
            //}),
            //Constraint.RelativeToParent((parent) =>
            //{
            //    return 100;
            //}),
            //Constraint.Constant(50)
            //);
        }

        /// <summary>
        /// Method that is used for dynamical builing of the bottom controls and takes in properties of title,
        /// description, background picture and page, which it navigates to, using tap gesture recognizer
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="picture"></param>
        /// <param name="p_NavigateTo"></param>
        /// <returns></returns>
        private RelativeLayout BuildBottomItems(string title, string description, string picture, int pageType)
        {

            var tapGestureRecognizerAbout = new TapGestureRecognizer();
            tapGestureRecognizerAbout.Tapped += async (s, e) =>
            {
                //MessagingCenter.Send<App>((App)Xamarin.Forms.Application.Current, "start");
                Page p_NavigateTo;
                if (pageType == 0)
                {
                    p_NavigateTo = _AboutPage;
                }
                else if (pageType == 1)
                {
                    p_NavigateTo = _TeachersPage;
                }
                else { p_NavigateTo = _CuratorsPage; }

                await Navigation.PushAsync(p_NavigateTo);
            };

            Thickness bottomItemsPadding = new Thickness(15, 0, 15, 0);
            Grid grid_BottomLeftItem = new Grid()
            {
                Padding = bottomItemsPadding,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                RowDefinitions =
                {
                    new RowDefinition(){Height = GridLength.Auto},
                    new RowDefinition(){Height = GridLength.Auto},
                    new RowDefinition(){Height = GridLength.Auto}
                },
            };

            StackLayout stack_LeftHolder = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.CenterAndExpand };
            stack_LeftHolder.Children.Add(grid_BottomLeftItem);
            FrameCustom frame_BottomLeft = new FrameCustom() { VerticalOptions = LayoutOptions.FillAndExpand, OutlineColor = Color.FromHex("#4a4a4a"), BackgroundColor = Color.Transparent };
            frame_BottomLeft.GestureRecognizers.Add(tapGestureRecognizerAbout);
            frame_BottomLeft.Content = stack_LeftHolder;
            //frame_BottomLeft.GestureRecognizers.Add(tapGestureRecognizerEditor);
            stack_LeftHolder.Children.Add(new Label() { Text = title, FontFamily = Device.OnPlatform("Orpheus Pro", "OrpheusPro.otf#OrpheusPro", ""), FontSize = Device.OnPlatform(27, 31, 30), VerticalOptions = LayoutOptions.EndAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center });
            stack_LeftHolder.Children.Add(new LabelCustom() { LineSpacing = 5, Text = description, FontSize = Device.OnPlatform(fontSize_Description - 2, fontSize_Description, fontSize_Description), TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.StartAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalTextAlignment = TextAlignment.Center });

            RelativeLayout rel_LeftItem = new RelativeLayout() { IsClippedToBounds = true, BackgroundColor = Color.FromHex("#1d1d1d") };
            rel_LeftItem.Children.Add(new Image() { Aspect = Aspect.AspectFill, Source = picture, Opacity = 1 }, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) =>
            {
                return parent.Width;
            }), Constraint.RelativeToParent((parent) =>
            {
                return parent.Height;
            }));
            rel_LeftItem.Children.Add(frame_BottomLeft, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) =>
            {
                return parent.Width;
            }), Constraint.RelativeToParent((parent) =>
            {
                return parent.Height;
            }));

            return rel_LeftItem;
        }

        /// <summary>
        /// Method that checks, if user is signed in and sets the appropriate user controls
        /// </summary>
        private async void CheckLogin()
        {
            //await Task.Run(() =>
            //{
            //Retrieves token
            try
            {
                var tv = new TokenVerification();
                if (string.IsNullOrEmpty(NaraTools.Token))
                {
                    await Task.Run(() => NaraTools.Token = tv.GetToken(NaraTools.ApiUsername, NaraTools.ApiPassword).access_token);
                }
                upperToolbarLoader.IsVisible = false;

                if (webUtil.CheckLogin())
                {
                    stck_HelpTip.IsVisible = false;
                    upperToolbar.IsVisible = false;
                    if (offlineRepo.GetUser() != null)
                    {
                        try
                        {
                            var n = offlineRepo.GetUser();
                            if (!(n.ImageUrl.Contains("no_image")) && !(n.ImageUrl.Contains("no-profile-image")))
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
                }
                else
                {
                    stck_HelpTip.IsVisible = offlineRepo.GetAdditionalData().Where(i=>i.Name == "homescreentip").Count() == 0;
                    upperToolbar.IsVisible = true;
                    stck.IsVisible = false;
                }
            }
            catch (Exception exc)
            {
                lbl_LoadingData.FontAttributes = FontAttributes.Italic;
                lbl_LoadingData.TextColor = Color.FromHex("#E0665E");
                lbl_LoadingData.Text = "Please, check your internet connection";
                lbl_LoadingData.IsVisible = true;
            }
            //});
        }

        /// <summary>
        /// Method that is triggered when view is in state of appearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            CheckLogin();

            if (showMessage)
            {
                showMessage = false;
                ShowMessage();
            }

        }

    }
}
