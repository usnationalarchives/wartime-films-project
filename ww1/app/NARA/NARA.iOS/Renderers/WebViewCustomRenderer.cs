using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Linq;
using FormsCustomWebViewClient.iOS;
using UIKit;
using NARA;
using Foundation;
using NARA.Common_p.Model;
using System.IO;
using System.Collections.Generic;
using NARA.Common_p.Repository;
using NARA.Common_p.Util;
using System.Threading.Tasks;
using System.Net;
using NARA.Util;
using System.Drawing;
using WebKit;
using ObjCRuntime;
using System.Diagnostics;
using System.Text;

[assembly: ExportRenderer(typeof(WebViewCustom), typeof(WebViewCustomRenderer))]

namespace FormsCustomWebViewClient.iOS
{
    /// <summary>
    /// Custom renderer implemented for xamarin webview control, to create a rounded one
    /// </summary>
    public class WebViewCustomRenderer : ViewRenderer<WebViewCustom, WKWebView>, IWKScriptMessageHandler, IDisposable
    {

        bool isConnected = DependencyService.Get<IPlatformSpecific>().CheckConnection();
        OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
        string url = NaraTools.Dvex;
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        WebViewCustom custom_WebView;
        List<string> history = new List<string>();
        WKWebView webView;
        Stopwatch sw = new Stopwatch();

        protected override void OnElementChanged(ElementChangedEventArgs<WebViewCustom> e)
        {
            base.OnElementChanged(e);

            sw.Start();

            DependencyService.Get<IPlatformSpecific>().SaveText("Method ElementChanged started" + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);
            DependencyService.Get<IPlatformSpecific>().SaveText("OnElement changed" + sw.ElapsedMilliseconds.ToString() + Environment.NewLine, true);

            if (Control == null)
            {
                custom_WebView = Element as WebViewCustom;
                custom_WebView.EvalJS += EvaluateJavascript;
                var config = new WKWebViewConfiguration { };
                webView = new WKWebView(Frame, config);
                webView.BackgroundColor = Color.FromHex("#1d1d1d").ToUIColor();
                DependencyService.Get<IPlatformSpecific>().SaveText("+Before navigation delegate" + sw.ElapsedMilliseconds.ToString() + Environment.NewLine, true);
                webView.NavigationDelegate = new NavigationDelegate(custom_WebView);
                DependencyService.Get<IPlatformSpecific>().SaveText("+After navigation delegate" + sw.ElapsedMilliseconds.ToString() + Environment.NewLine, true);
                //webView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
            }
            if (e.NewElement != null)
            {
                if (Element.Source != null)
                {
                    var source = (HtmlWebViewSource)Element.Source;

                    //if (source.BaseUrl.Contains("dvex/list"))
                    //{
                    //    var content = repo.GetContent(source.BaseUrl);
                    //    if (content != null)
                    //    {
                    //        if (content != null)
                    //        {
                    //            if ((DateTime.Now - content.Date).TotalMinutes < NaraTools.ContentIntervalCheck)
                    //            {
                    //                webView.LoadHtmlString(new NSString(content.Content), new NSUrl(content.Url));
                    //                //byte[] bytes = Encoding.UTF8.GetBytes(content.Content);
                    //                //Control.LoadData(new NSData(Convert.ToBase64String(bytes), NSDataBase64DecodingOptions.None), "text/html", source.BaseUrl, new NSUrl(source.BaseUrl));
                    //            }
                    //            else
                    //            {
                    //                Control.LoadRequest(new NSUrlRequest(new NSUrl(source.BaseUrl), NSUrlRequestCachePolicy.ReturnCacheDataElseLoad, 1440));
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Control.LoadRequest(new NSUrlRequest(new NSUrl(source.BaseUrl), NSUrlRequestCachePolicy.ReturnCacheDataElseLoad, 1440));
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Control.LoadRequest(new NSUrlRequest(new NSUrl(source.BaseUrl), NSUrlRequestCachePolicy.ReturnCacheDataElseLoad, 1440));
                    //    }
                    //}
                    //else
                    //{
                        Control.LoadRequest(new NSUrlRequest(new NSUrl(source.BaseUrl), NSUrlRequestCachePolicy.ReturnCacheDataElseLoad, 1440));
                    //}

                }
            }
        }

        private void EvaluateJavascript(object sender, string e) {
            webView.EvaluateJavaScript(e, handler);
        }

        WKJavascriptEvaluationResult handler = (NSObject result, NSError err) =>
        {
            if (err != null)
            {
                System.Console.WriteLine(err);
            }
            if (result != null)
            {
                System.Console.WriteLine(result);
            }
        };

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (webView != null)
            {
                webView.Dispose();
            }
        }
    }



    public class NavigationDelegate : WKNavigationDelegate
    {
        bool isConnected = DependencyService.Get<IPlatformSpecific>().CheckConnection();
        OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
        string url = NaraTools.Dvex;
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        WebViewCustom custom_WebView;
        string lastUrl = "";
        List<string> history = new List<string>();
        string content = "";
        WKWebView wkWebView;
        string reqUrl = "";
        bool loadedFromDb = false;
        #region Error page content
        string errorPage = @"<!DOCTYPE html>
<html>
<head>
    <style>
        body {
            /*background-image: url('Images/background.png');*/
            text-align: center;
            font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
            background: #1d1d1d;
            /*background: #E0665E;*/
        }

        h1 {
            margin-top: 50px;
            color: white;
            font-family: 'Trebuchet MS';
        }

        p {
            color: white;
            font-family: 'Trebuchet MS';
        }

        .btn-back{
            border: 2px solid #E0665E;
            border-radius: 20px;
            background-color: transparent;
            color:white;
            font-family: 'Trebuchet MS';
            text-transform: uppercase;
            height:30px;
            width: 80px;
        }
    </style>
</head>
<body>
    <script>


    function navigateBack()
    {
        location.href = 'NARA://action?jsNavigate=true';
        }
    </script>


    <h1>An error has occured</h1>
    <p>Please check your internet connection</p>
    <button class='btn-back' onclick='navigateBack()'>Retry</button>

</body>
</html>
";
        #endregion
        public NavigationDelegate(WebViewCustom webView)
        {
            custom_WebView = webView;
            if (custom_WebView != null)
            {
                custom_WebView.RefreshContent += Custom_WebView_RefreshContent;
            }
        }
        
        private void Custom_WebView_RefreshContent(object sender, EventArgs e)
        {
            try
            {
                if (wkWebView != null)
                {
                    if (!webUtil.CheckLogin())
                    {
                        var setOfContent = new NSSet<NSString>(new[]
                        {
                    //Choose which ones you want to remove
                    WKWebsiteDataType.Cookies,
                });
                        wkWebView.Configuration.WebsiteDataStore.RemoveDataOfTypes(setOfContent, NSDate.DistantPast, CookieHandler);
                    }
                    else
                    {
                        wkWebView.EvaluateJavaScript(CheckCookies(), handler);
                    }
                }

                wkWebView.EvaluateJavaScript("checkLogin();", handler);
            }
            catch (Exception exc)
            {

            }
        }

        WKJavascriptEvaluationResult handler = (NSObject result, NSError err) =>
        {
            if (err != null)
            {
                System.Console.WriteLine(err);
            }
            if (result != null)
            {
                System.Console.WriteLine(result);
            }
        };

        Action CookieHandler = () =>
        {
        };

        public override void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
        {
            try
            {
                NSHttpUrlResponse response = (NSHttpUrlResponse)navigationResponse.Response;
                if (response.StatusCode < 400)
                {
                    decisionHandler(WKNavigationResponsePolicy.Allow);
                }
                else
                {
                    decisionHandler(WKNavigationResponsePolicy.Cancel);
                    webView.LoadHtmlString(errorPage, new NSUrl("file:///"));
                }
            }
            catch (Exception e)
            {
                decisionHandler(WKNavigationResponsePolicy.Allow);
            }
        }

        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            try
            {

                if (loadedFromDb)
                {
                    loadedFromDb = false;
                    decisionHandler(WKNavigationActionPolicy.Allow);
                    return;
                }

                string _backUrl = "";
                wkWebView = webView;
                var request = navigationAction.Request;
                

                //WKUserContentController userController = new WKUserContentController();
                //WKUserScript script = new WKUserScript(new NSString(CheckCookies()), WKUserScriptInjectionTime.AtDocumentStart, false);
                //userController.AddUserScript(script);
                //webView.Configuration.UserContentController = userController;

                webView.EvaluateJavaScript(CheckCookies(), handler);

                DependencyService.Get<IPlatformSpecific>().SaveText("Start of url navigation override-ing " + request.Url.AbsoluteString + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);

                //Since some pages are loaded in parts (iframes, ... ), we check, that the whole content of the page is saved only once, since some parts of the page are being refreshed in the process.
                //if ((lastUrl != request.Url.AbsoluteString && request.Url.AbsoluteString != "about:blank" && !lastUrl.Contains("vimeo") && !request.Url.AbsoluteString.Contains("vimeo") && !request.Url.AbsoluteString.Contains("file://") && !request.Url.AbsoluteString.Contains("about:blank")))
                //{
                reqUrl = request.Url.AbsoluteString;
                if (request.Url.AbsoluteString.Contains("jsHideLoader"))
                {
                    custom_WebView.RaiseNavigated();
                    decisionHandler(WKNavigationActionPolicy.Cancel);
                    return;
                }

                bool forward = false;
                bool back = false;
                bool canGoForward = true;

                //    //if(history.Count > 0)
                //    //{
                //    //    if(history.LastOrDefault() == request.Url.AbsoluteString)
                //    //    {
                //    //        canGoForward = false;
                //    //    }
                //    //}

                if (request.Url.AbsoluteString.Contains("vimeo") && request.Url.AbsoluteString.Contains("jsDownloadImage"))
                {
                    DependencyService.Get<IPlatformSpecific>().SaveImage("NARA_" + DateTime.Now.ToLongTimeString().Replace(" ", "").Replace(":", ""), request.Url.AbsoluteString.Split('=')[1], custom_WebView);
                    decisionHandler(WKNavigationActionPolicy.Cancel);
                    return;
                }


                if (request.Url.AbsoluteString != "about:blank" && canGoForward && !request.Url.AbsoluteString.Contains("vimeo") && !request.Url.AbsoluteString.StartsWith("file:///"))
                {

                    if (!request.Url.AbsoluteString.Contains(NaraTools.Host) && !request.Url.AbsoluteString.ToLower().Contains("nara://") && !request.Url.AbsoluteString.Contains("login") && !request.Url.AbsoluteString.Contains("vimeo"))
                    {

                        custom_WebView.BackUrl = request.Url.AbsoluteString;
                        custom_WebView.ExternalView();
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }


                    //Disable scroll of the page
                    webView.ScrollView.ScrollEnabled = false;

                    //Js navigation parameter which indicates the logout operation
                    //User data/cookies are cleared and refresh of the page is triggered
                    if (request.Url.AbsoluteString.Contains("jsLogout"))
                    {
                        repo.ClearUser();
                        webUtil.ClearLogin();
                        //var cookieUrl = new Uri(NaraTools.Domain);

                        //var cookieJar = NSHttpCookieStorage.SharedStorage;
                        //cookieJar.AcceptPolicy = NSHttpCookieAcceptPolicy.Always;
                        //foreach (var aCookie in cookieJar.Cookies)
                        //{
                        //    cookieJar.DeleteCookie(aCookie);
                        //}

                        webView.EvaluateJavaScript("javascript:window.location.reload( true )", handler);
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }

                    if (request.Url.AbsoluteString.Contains("Cookie"))
                    {
                        //var cookieValue = request.Url.AbsoluteString.Split('=')[1];

                        repo.SaveAdditionalData(new AdditionalData() { Id = repo.GetAdditionalData().Count + 1, Name = request.Url.AbsoluteString.Split('=')[1], Value = "1", Date = DateTime.Now });

                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }

                    //Js navigation parameter which indicates the navigation to the profile edit page,
                    //which is a xamarin form
                    if (request.Url.AbsoluteString.Contains("jsEditProfile"))
                    {
                        custom_WebView.EditProfileView();
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }
                    if (request.Url.AbsoluteString.Contains("jsDownloadImage"))
                    {
                        //MessagingCenter.Send<App,string>((App)Xamarin.Forms.Application.Current, "Share", "https://photos-cdn.historypin.org/projects/img/pid/29850/type/project_image,banner,logo/dim/1000x1080/c/1495017460");

                        DependencyService.Get<IPlatformSpecific>().SaveImage("NARA_" + DateTime.Now.ToLongTimeString().Replace(" ", "").Replace(":", ""), request.Url.AbsoluteString.Split('=')[1], custom_WebView);

                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }
                    //Js navigation parameter which indicates that the menu should be presented
                    if (request.Url.AbsoluteString.Contains("jsShowMenu"))
                    {
                        custom_WebView.RaiseMenu();
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }

                    //Js navigation parameter which indicates the request of the sharing component
                    if (request.Url.AbsoluteString.Contains("jsShare"))
                    {
                        custom_WebView.ShareUrl = lastUrl;

                        var parsed = System.Web.HttpUtility.ParseQueryString(request.Url.AbsoluteString);

                        if (request.Url.AbsoluteString.Contains("shareCollection"))
                        {
                            custom_WebView.ShareCollection = true;
                            custom_WebView.ShareMessage = parsed[1];
                        }
                        else
                        {
                            custom_WebView.ShareCollection = false;
                            custom_WebView.ShareMessage = parsed[1];
                            custom_WebView.ShareSource = parsed[2];
                        }

                        //custom_WebView.ShareMessage = System.Web.HttpUtility.UrlDecode(request.Url.AbsoluteString.Split('=')[1]);

                        custom_WebView.ShowShareView();
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }

                    //Js navigation parameter which indicates the request of the sharing component
                    if (request.Url.AbsoluteString.Contains("jsRegister"))
                    {
                        custom_WebView.ShareUrl = lastUrl;
                        custom_WebView.ShowRegisterForm();
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }

                    //        //Js navigation parameter which indicates the navigation to the homepage
                    if (request.Url.AbsoluteString.Contains("jsHome"))
                    {
                        custom_WebView.RaiseHomepage();
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }


                    if (request.Url.AbsoluteString.Contains("jsSignIn"))
                    {
                        lastUrl = request.Url.AbsoluteString;
                        custom_WebView.BackUrl = lastUrl;
                        custom_WebView.ShowSignInForm();
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }


                    //        //Js navigation parameter which indicates, that user clicked login/sign-in button,
                    //        //and navigates to the sign-in form
                    if (request.Url.AbsoluteString.Contains("login"))
                    {
                        custom_WebView.RaiseEdit(history.LastOrDefault());
                        lastUrl = request.Url.AbsoluteString;
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                    }
                    //Back navigation triggered in web. Navigation history is implemented, so that the app knows,
                    //whether to navigate from the app (to the homescreen app view), or perform web navigation (navigating between webpage-s)
                    else if (request.Url.AbsoluteString.Contains("jsNavigate"))
                    {
                        back = true;
                        if (lastUrl.ToLower().Contains("edit"))
                        {
                            if (history.Count < 2)
                                _backUrl = NaraTools.DvexList;
                            else
                            {
                                history.Remove(history.LastOrDefault());
                                _backUrl = history.LastOrDefault();
                            }
                            forward = false;
                        }
                        else if (history.Count > 1)
                        {
                            history.Remove(history.LastOrDefault());
                            _backUrl = history.LastOrDefault();
                        }
                        else
                        {
                            if (history.Count == 0 && lastUrl.Contains("list"))
                            {
                                history = new List<string>();
                                custom_WebView.RaiseHomepage();
                                //custom_WebView.RaiseNavigating();
                                decisionHandler(WKNavigationActionPolicy.Allow);
                                return;

                            }
                            else if (history.Count == 0)
                                _backUrl = NaraTools.DvexList;
                            else
                            {
                                history = new List<string>();
                                custom_WebView.RaiseHomepage();
                                decisionHandler(WKNavigationActionPolicy.Allow);
                                return;
                            }
                        }
                        if (_backUrl.Contains("edit") && repo.GetUser() == null)
                        {
                            _backUrl = NaraTools.DvexList;
                            lastUrl = _backUrl;
                            history.Clear();
                        }
                        else
                        {
                            lastUrl = request.Url.AbsoluteString;
                        }
                    }
                    //If navigated to the edit form, scroll is enabled, current url is stored, so it 
                    //then navigates to page, where the user was before it navigated to the edit form
                    else if (request.Url.AbsoluteString.Contains("edit"))
                    {
                        lastUrl = request.Url.AbsoluteString;
                        //history.Add(request.Url.AbsoluteString);
                        _backUrl = request.Url.AbsoluteString;
                        webView.ScrollView.ScrollEnabled = true;
                        custom_WebView.RaiseNavigating();
                        decisionHandler(WKNavigationActionPolicy.Allow);
                        return;
                    }


                    //Checks whether is new page or page is being refreshed
                    if (_backUrl == "")
                    {
                        _backUrl = request.Url.AbsoluteString;
                        forward = true;
                    }

                    //Represents loading indicator
                    //if (_backUrl != lastUrl || navigationAction.NavigationType == WKNavigationType.Reload)
                    //{
                    custom_WebView.RaiseNavigating();
                    //}


                    //        #region Offline Content
                    //        //////////////!!!!!!!!!!!!!!! Logging implemented for performance testing purposes
                    //        //DependencyService.Get<IPlatformSpecific>().SaveText("Checking DB for content : " + DateTime.Now.ToString("HH:mm:ss") + "| URL : " + _backUrl + " |" + Environment.NewLine, true);
                    //        //var contentObj = repo.GetContent(_backUrl);
                    //        //if (contentObj != null)
                    //        //{
                    //        //    if ((DateTime.Now - contentObj.Date).TotalMinutes < NaraTools.ContentIntervalCheck)
                    //        //    {
                    //        //        content = contentObj.Content;
                    //        //        DependencyService.Get<IPlatformSpecific>().SaveText("Content added from DB : " + DateTime.Now.ToString("HH:mm:ss") + "| URL : " + _backUrl + " |" + Environment.NewLine, true);
                    //        //    }
                    //        //    else
                    //        //    {
                    //        //        content = RetrieveContent(_backUrl);
                    //        //    }
                    //        //}
                    //        //else
                    //        //{

                    //        //if (_backUrl.Contains("dvex/list"))
                    //        //{
                    //        //    DependencyService.Get<IPlatformSpecific>().SaveText("Checking DB for content : " + DateTime.Now.ToString("HH:mm:ss") + "| URL : " + _backUrl + " |" + Environment.NewLine, true);
                    //        //    var contentObj = repo.GetContent(_backUrl);
                    //        //    if (contentObj != null)
                    //        //    {
                    //        //        if (!string.IsNullOrEmpty(contentObj.Content))
                    //        //        {
                    //        //            if ((DateTime.Now - contentObj.Date).TotalMinutes < NaraTools.ContentIntervalCheck)
                    //        //            {
                    //        //                content = contentObj.Content;
                    //        //                DependencyService.Get<IPlatformSpecific>().SaveText("Content added from DB : " + DateTime.Now.ToString("HH:mm:ss") + "| URL : " + _backUrl + " |" + Environment.NewLine, true);
                    //        //            }
                    //        //            else
                    //        //            {
                    //        //                content = RetrieveContent(_backUrl);
                    //        //            }
                    //        //        }
                    //        //        else
                    //        //        {
                    //        //            content = RetrieveContent(_backUrl);
                    //        //        }
                    //        //    }
                    //        //    else
                    //        //    {
                    //        //        content = RetrieveContent(_backUrl);
                    //        //    }
                    //        //}
                    //        //else
                    //        //{
                    //            //content = RetrieveContent(_backUrl);
                    //        //}
                    //        #endregion
                    //        content = RetrieveContent(_backUrl);
                    //        try
                    //        {

                    //            if (content != "")
                    //            {
                    //                ////////////////!!!!!!!!!!!!!!!!! Logging implemented for performance testing purposes
                    //                DependencyService.Get<IPlatformSpecific>().SaveText("Loading content into webview : " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);

                    //                webView.LoadHtmlString(content, new NSUrl(_backUrl));

                    //                DependencyService.Get<IPlatformSpecific>().SaveText("End of loading content into webview : " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);

                    //                //Checks whether navigation has been performed to the new page
                    if (forward && !_backUrl.Contains("vimeo"))
                    {
                        if (history.Count == 0)
                        {
                            history.Add(_backUrl);
                        }
                        else
                        {
                            if (history.LastOrDefault() != _backUrl)
                            {
                                history.Add(_backUrl);
                            }
                        }
                    }
                    else if (back)
                    {
                        //if (repo.GetContent(_backUrl) != null)
                        //{
                        //    var content = repo.GetContent(_backUrl);
                        //    if (content != null)
                        //    {
                        //        if ((DateTime.Now - content.Date).TotalMinutes < NaraTools.ContentIntervalCheck)
                        //        {
                        //            //webView.LoadHtmlString(new NSString(content.Content), new NSUrl(content.Url));
                        //            webView.LoadData(new NSData(content.Content, NSDataBase64DecodingOptions.None), "text/html", _backUrl, new NSUrl(_backUrl));
                        //            decisionHandler(WKNavigationActionPolicy.Cancel);
                        //            return;
                        //        }
                        //        else
                        //        {
                        //            webView.LoadRequest(new NSUrlRequest(new NSUrl(_backUrl)));
                        //            decisionHandler(WKNavigationActionPolicy.Cancel);
                        //            return;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        webView.LoadRequest(new NSUrlRequest(new NSUrl(_backUrl)));
                        //        decisionHandler(WKNavigationActionPolicy.Cancel);
                        //        return;
                        //    }
                        //}
                        //else
                        //{
                        webView.LoadRequest(new NSUrlRequest(new NSUrl(_backUrl), NSUrlRequestCachePolicy.ReturnCacheDataElseLoad, 1440));
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        return;
                        //}

                    }
                    lastUrl = _backUrl;

                    //            }
                    //            else
                    //            {
                    //                //If there was no content recieved, custom error page is displayed
                    //                string fileName = "Content/Index.html";
                    //                string localHtmlUrl = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);
                    //                webView.LoadRequest(new NSUrlRequest(new NSUrl(localHtmlUrl, false)));
                    //                DependencyService.Get<IPlatformSpecific>().SaveText("Error - no content: " + _backUrl + " ---- " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);
                    //                //webView.ScalesPageToFit = false;
                    //            }
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            lastUrl = request.Url.AbsoluteString;
                    //            decisionHandler(WKNavigationActionPolicy.Allow);
                    //            return;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        decisionHandler(WKNavigationActionPolicy.Cancel);
                    //        return;
                    //    }
                    //}
                    //else if (request.Url.AbsoluteString.EndsWith("/dvex/list"))
                    //{
                    //    custom_WebView.RaiseNavigating();
                    //}

                    //else if (request.Url.AbsoluteString.Contains("about:blank"))
                    //{
                    //    //Checks if navigated page is blank
                    //    decisionHandler(WKNavigationActionPolicy.Cancel);
                    //    return;
                }
                DependencyService.Get<IPlatformSpecific>().SaveText("End of url navigation override-ing " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);
                if (request.Url.AbsoluteString.Contains("about:blank"))
                {
                    //    //Checks if navigated page is blank
                    decisionHandler(WKNavigationActionPolicy.Cancel);
                    return;
                }

                //if (_backUrl.Contains("dvex/list"))
                //{
                //    var content = repo.GetContent(_backUrl);
                //    if (content != null)
                //    {
                //        if (content != null)
                //        {
                //            if ((DateTime.Now - content.Date).TotalMinutes < NaraTools.ContentIntervalCheck)
                //            {
                //                webView.LoadHtmlString(new NSString(content.Content), new NSUrl(content.Url));
                //                loadedFromDb = true;
                //                //byte[] bytes = Encoding.UTF8.GetBytes(content.Content);
                //                //webView.LoadData(new NSData(Convert.ToBase64String(bytes), NSDataBase64DecodingOptions.None), "text/html", _backUrl, new NSUrl(_backUrl));
                //                decisionHandler(WKNavigationActionPolicy.Cancel);
                //                Console.WriteLine("Loaded from DB");
                //                return;
                //            }
                //            else
                //            {
                //                decisionHandler(WKNavigationActionPolicy.Allow);
                //            }
                //        }
                //        else
                //        {
                //            decisionHandler(WKNavigationActionPolicy.Allow);
                //        }
                //    }
                //    else
                //    {
                //        decisionHandler(WKNavigationActionPolicy.Allow);
                //    }
                //}
                //else
                //{
                    decisionHandler(WKNavigationActionPolicy.Allow);
                //}
            }
            catch (Exception e)
            {
                decisionHandler(WKNavigationActionPolicy.Allow);
            }

            //decisionHandler(WKNavigationActionPolicy.Allow);
            //decisionHandler(WKNavigationActionPolicy.Cancel);    
            //decisionHandler(WKNavigationActionPolicy.Allow);
        }
        public void SaveContent(string url)
        {
            repo.SaveContent(new OfflineContent() { Date = DateTime.Now, Content = RetrieveContent(url), Url = url });
        }
        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            // If navigation fails, this gets called
        }
        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            //try
            //{
            //    if (error.Code != -999 && error.Code != 101)
            //    {
            //        string fileName = "Content/Index.html";
            //        string localHtmlUrl = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);
            //        webView.LoadRequest(new NSUrlRequest(new NSUrl(localHtmlUrl, false)));
            //        DependencyService.Get<IPlatformSpecific>().SaveText("Error code: " + error.Code + ". Description:" + error.Description + " Debug description: " + error.DebugDescription + " ---- " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);
            //    }
            //}
            //catch { custom_WebView.RaiseHomepage(); }
        }
        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {

        }
        public override void ContentProcessDidTerminate(WKWebView webView)
        {
            webView.LoadRequest(new NSUrlRequest(new NSUrl(lastUrl)));
        }
        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            DependencyService.Get<IPlatformSpecific>().SaveText("Load finished " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);

            if (!webUtil.CheckLogin())
            {
                var setOfContent = new NSSet<NSString>(new[]
                {
                    //Choose which ones you want to remove
                    WKWebsiteDataType.Cookies,
                });
                webView.Configuration.WebsiteDataStore.RemoveDataOfTypes(setOfContent, NSDate.DistantPast, CookieHandler);
            }
            else
            {
                webView.EvaluateJavaScript(CheckCookies(), handler);
            }

            if (webView.IsLoading == false)
            {
                //Saves js variables for web app, used for making interactions with xamarin
                webView.EvaluateJavaScript("var environment = 'app';", handler);
                webView.EvaluateJavaScript("var appCode = 'nara';", handler);
                webView.EvaluateJavaScript("var version = '116';", handler);

                foreach (var val in repo.GetAdditionalData())
                {
                    webView.EvaluateJavaScript("var " + val.Name + " = '" + val.Value + "';", handler);
                }

                //Retrieves ready state of web page
                //var js = webView.EvaluateJavascript("document.readyState");

                //Disables user web interactions
                webView.EvaluateJavaScript("document.body.style.webkitTouchCallout='none';", handler);
                webView.EvaluateJavaScript("document.body.style.webkitUserSelect='none';", handler);

                if (custom_WebView != null)
                {
                    //await Task.Delay(100);
                    wkWebView.EvaluateJavaScript("checkLogin();", handler);
                    custom_WebView.RaiseNavigated();
                    Task.Run(() =>
                    {
                        repo.SaveContent(new OfflineContent() { Content = RetrieveContent(reqUrl), Date = DateTime.Now, Url = reqUrl });
                        DependencyService.Get<IPlatformSpecific>().SaveText("End of call for WS : " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine, true);
                    });
                }
            }

        }

        public override void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
        {
            // This seems to be needed, I didn't manage to track down what its used for
        }

        private string RetrieveContent(string url)
        {
            string content;
            DependencyService.Get<IPlatformSpecific>().SaveText("Calling WS for content : " + DateTime.Now.ToString("HH:mm:ss") + "| URL : " + url + " |" + Environment.NewLine, true);
            content = Task.Run(() => webUtil.GetContentHtml(url)).Result;
            return content;
        }

        private string CheckCookies()
        {
            string cookiesS = "";

            try
            {
                if (webUtil.CheckLogin())
                {
                    //Checks for cookies, and if they are valid
                    var cookiesR = repo.GetCookies();

                    if (cookiesR != null)
                    {
                        foreach (var cookie in cookiesR.Where(i => i.Name != "nara"))
                        {
                            cookiesS += string.Format("document.cookie = '{0}={1}; expires={2}; domain={3};';", cookie.Name, cookie.Value, cookie.Expires.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"), cookie.Domain);
                            //Cookie cookieNet = new Cookie()
                            //{
                            //    Value = cookie.Value,
                            //    Name = cookie.Name,
                            //    HttpOnly = cookie.HttpOnly,
                            //    Expired = false,
                            //    Expires = DateTime.Now.AddDays(1),
                            //    Domain = cookie.Domain
                            //};
                        }
                        var user = repo.GetUser();

                        if (user != null)
                        {
                            cookiesS += "currentUser=" + user.Id + ";";
                        }
                    }
                }
                else
                {
                    var cookieUrl = new Uri(NaraTools.Domain);
                    var cookieJar = NSHttpCookieStorage.SharedStorage;
                    cookieJar.AcceptPolicy = NSHttpCookieAcceptPolicy.Always;
                    foreach (var aCookie in cookieJar.Cookies)
                    {
                        cookieJar.DeleteCookie(aCookie);
                    }
                    cookiesS = ClearCookies;
                }
            }
            catch (Exception exc)
            { }

            return cookiesS;
        }

        private string ClearCookies = "function clearListCookies(){   var cookies = document.cookie.split(';');for (var i = 0; i<cookies.length; i++){   var spcook = cookies[i].split('=');deleteCookie(spcook[0]);}function deleteCookie(cookiename){var d = new Date();d.setDate(d.getDate() - 1);var expires = ';expires=' + d;var name = cookiename;var value = '';document.cookie = name + '=' + value + expires + '; path=/acc/html';}window.location = '';}";
    }

    public class UIDelegate : WKUIDelegate
    {
        public override void RunJavaScriptAlertPanel(WKWebView webView, string message, WKFrameInfo frame, Action completionHandler)
        {
            // Custom Alert() code here
        }
    }

}