using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using NARA.Droid;
using Android.Webkit;
using NARA;
using CustomRenderer.Droid;
using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using Org.Apache.Http.Client.Methods;
using Org.Apache.Http.Impl.Client;
using NARA.Common_p.Util;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using NARA.Util;
using Java.Lang;
using System.Net;
using Java.Interop;
using static CustomRenderer.Droid.WebViewCustomRenderer;
using Android.Net.Http;

[assembly: ExportRenderer(typeof(WebViewCustom), typeof(WebViewCustomRenderer))]
namespace CustomRenderer.Droid
{
    public class WebViewCustomRenderer : WebViewRenderer, IDisposable
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        ChromeClient cc = new ChromeClient();
        bool isConnected = DependencyService.Get<IPlatformSpecific>().CheckConnection();
        OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
        string url = NaraTools.EditView;
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        static WebViewCustom wvc;
        Android.Webkit.WebView nativeWebView;
        public class ChromeClient : WebChromeClient
        {
            public override bool OnJsAlert(Android.Webkit.WebView view, string url, string message, JsResult result)
            {
                return base.OnJsAlert(view, url, message, result);
            }
        }

        public class JavascriptResult : Java.Lang.Object, IValueCallback
        {
            public void OnReceiveValue(Java.Lang.Object value)
            {
            }
        }
        public class ViewClient : WebViewClient
        {
            string lastUrl = "";
            List<string> history = new List<string>();
            JavascriptResult jsResult = new JavascriptResult();
            OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
            WebUtil webUtil = new WebUtil();
            bool refreshed = false;
            bool loginRedirect = false;

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
            {
                CheckCookies();

                if (url.Contains("about:blank"))
                {
                    return true;
                }

                //CheckCookies();
                if (!url.Contains(NaraTools.Host) && !url.Contains("nara://") && !url.Contains("login") && !url.Contains("vimeo"))
                {
                    wvc.BackUrl = url;
                    wvc.ExternalView();
                    return true;
                }

                if (url.Contains("jsHideLoader"))
                {
                    if (!loginRedirect)
                    {
                        wvc.RaiseNavigated();
                        refreshed = false;
                    }
                    loginRedirect = false;
                    return true;
                }

                //Js navigation parameter which indicates the request of the sharing component
                if (url.Contains("jsRegister"))
                {
                    wvc.ShareUrl = lastUrl;
                    wvc.ShowRegisterForm();
                    return true;
                }

                if (url.Contains("jsDownloadImage"))
                {
                    //MessagingCenter.Send<App,string>((App)Xamarin.Forms.Application.Current, "Share", "https://photos-cdn.historypin.org/projects/img/pid/29850/type/project_image,banner,logo/dim/1000x1080/c/1495017460");

                    DependencyService.Get<IPlatformSpecific>().SaveImage("NARA_" + DateTime.Now.ToLongTimeString().Replace(" ", "").Replace(":", ""), url.Split('=')[1], wvc);

                    return true;
                }

                if (url.Contains("jsSignIn"))
                {
                    string last = "";

                    if (history.Count > 0)
                    {
                        last = history.LastOrDefault();
                    }
                    else
                    {
                        last = NaraTools.DvexList;
                    }

                    loginRedirect = true;
                    wvc.BackUrl = last;
                    wvc.ShowSignInForm();
                    return true;
                }

                if (url.Contains("Cookie"))
                {
                    repo.SaveAdditionalData(new AdditionalData() { Id = repo.GetAdditionalData().Count + 1, Name = url.Split('=')[1], Value = "1", Date = DateTime.Now });
                    return true;
                }

                if (url.Contains("jsNavigate"))
                {
                    if (history.Count > 1)
                    {
                        wvc.RaiseNavigating();
                        view.LoadUrl(history.LastOrDefault());
                        history.Remove(history.LastOrDefault());
                        return true;
                    }
                    else if (history.Count == 1)
                    {
                        wvc.RaiseNavigating();
                        view.LoadUrl(NaraTools.ExplorePage);
                        history.Clear();
                        return true;
                    }
                    else
                    {
                        wvc.RaiseHomepage();
                        return true;
                    }
                }
                else if (url.Contains("jsHome"))
                {
                    wvc.RaiseHomepage();
                    return true;
                }
                else if (url.Contains("login"))
                {
                    string last = "";

                    if (history.Count > 0)
                    {
                        last = history.LastOrDefault();
                    }
                    else
                    {
                        last = NaraTools.DvexList;
                    }

                    loginRedirect = true;
                    wvc.RaiseEdit(last);
                    return true;
                }
                else if (url.Contains("jsEditProfile"))
                {
                    wvc.EditProfileView();
                    return true;
                }
                else if (url.ToLower().Contains("logout"))
                {
                    webUtil.ClearLogin();
                    CheckCookies();
                    loginRedirect = true;
                    view.EvaluateJavascript("javascript: window.location.reload(true)", jsResult);
                    return true;
                }
                else if (url.Contains("jsShare"))
                {
                    wvc.ShareUrl = lastUrl;
                    if (url.Contains("shareCollection"))
                    {
                        wvc.ShareCollection = true;
                    }
                    else { wvc.ShareCollection = false; }

                    wvc.ShareMessage = System.Web.HttpUtility.UrlDecode(url.Split('=')[1]);
                    wvc.ShowShareView();

                    return true;
                }

                if (lastUrl == url && !refreshed)
                {
                    wvc.RaiseNavigating();
                    view.EvaluateJavascript("javascript: window.location.reload(true)", jsResult);
                    refreshed = true;
                }
                else
                {
                    refreshed = false;
                }

                if (!url.Contains("nara//") && url.Contains(NaraTools.Host) && history.LastOrDefault() != url)
                {
                    history.Add(url);
                }

                return false;
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                ////NaraGlobal.IsWebViewLoading = false;
                ////Saves js variables for web app, used for making interactions with xamarin
                ////view.EvaluateJavascript("var androidApp = true;", jsResult);
                ////view.EvaluateJavascript("var environment = 'app';", jsResult);
                ////view.EvaluateJavascript("var appCode = 'nara';", jsResult);
                //foreach (var val in repo.GetAdditionalData())
                //{
                //    view.EvaluateJavascript("var " + val.Name + " = '" + val.Value + "';", jsResult);
                //}

                ////view.EvaluateJavascript("init(115);", jsResult);

                ////Retrieves ready state of web page
                //view.EvaluateJavascript("document.readyState", jsResult);

                ////Disables user web interactions
                //view.EvaluateJavascript("document.body.style.webkitTouchCallout='none';", jsResult);
                //view.EvaluateJavascript("document.body.style.webkitUserSelect='none';", jsResult);
                ////view.EvaluateJavascript("HideLoader();", jsResult);
                //lastUrl = url;
                ////wvc.RaiseNavigated();
            }
            public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon)
            {

                if (url.Contains("edit"))
                {
                    view.SetLayerType(LayerType.Software, null);
                }
                else
                {
                    view.SetLayerType(LayerType.Hardware, null);
                }

                //view.EvaluateJavascript("var androidApp = true;", jsResult);
                //view.EvaluateJavascript("var version = '115';", jsResult);

                if (!url.Contains("vimeo"))
                {
                    wvc.RaiseNavigating();
                }
            }
            public override void OnReceivedError(Android.Webkit.WebView view, ClientError errorCode, string description, string failingUrl)
            {
                view.LoadUrl("file:///android_asset/Content/Index.html");
            }
            public override void OnReceivedSslError(Android.Webkit.WebView view, SslErrorHandler handler, SslError error)
            {
                if (NaraTools.Environment == NARA.Util.Environments.Testing)
                {
                    handler.Proceed();
                }
                else
                {
                    base.OnReceivedSslError(view, handler, error);
                }
            }

            /// <summary>
            /// Checks whether user is signed in and if it is not, it
            /// clears cookies. If user is signed in, then it checks if cookies exists and also
            /// checks their validation
            /// </summary>
            /// <returns></returns>
            private void CheckCookies()
            {
                try
                {
                    var cookieManager = CookieManager.Instance;
                    cookieManager.SetAcceptCookie(true);

                    if (!webUtil.CheckLogin())
                    {
                        //Clears cookies from cookieJar
                        cookieManager.RemoveAllCookie();
                    }
                    else
                    {
                        //Checks for cookies, and if they are valid
                        var cookiesR = repo.GetCookies();

                        List<Cookie> cookiesRR = new List<Cookie>();

                        if (cookiesR != null)
                        {
                            foreach (var cookie in cookiesR.Where(i => i.Name != "nara"))
                            {
                                Cookie cookieNet = new Cookie()
                                {
                                    Value = cookie.Value,
                                    Name = cookie.Name,
                                    HttpOnly = cookie.HttpOnly,
                                    Expired = false,
                                    Expires = DateTime.Now.AddDays(1),
                                    Domain = cookie.Domain
                                };

                                cookiesRR.Add(cookieNet);
                            }

                            cookieManager.RemoveAllCookie();
                            for (var i = 0; i < cookiesRR.Count; i++)
                            {
                                string cookieValue = cookiesRR[i].Value;
                                string cookieDomain = cookiesRR[i].Domain;
                                string cookieName = cookiesRR[i].Name;
                                cookieManager.SetCookie(cookieDomain, cookieName + "=" + cookieValue);
                            }

                        }
                    }
                }

                catch (Java.Lang.Exception e)
                {

                }
            }
        }


        //protected override async void OnElementChanged(ElementChangedEventArgs<WebViewCustom> e)
        //{
        //    string filename = System.IO.Path.Combine(path, "Index.html");
        //    var cookies = e.NewElement.cookies;
        //    base.OnElementChanged(e);
        //    wvc = e.NewElement;

        //    if (Control == null)
        //    {
        //        var webView = new Android.Webkit.WebView(Forms.Context);
        //        //webView.SetInitialScale(1);
        //        //webView.SetWebChromeClient(cc);
        //        //webView.SetWebViewClient(new ViewClient());
        //        //webView.Settings.BuiltInZoomControls = true;
        //        //webView.Settings.LoadWithOverviewMode = true;
        //        //webView.Settings.UseWideViewPort = true;
        //        webView.Settings.JavaScriptEnabled = true;
        //        webView.Settings.DomStorageEnabled = true;
        //        //webView.Settings.DatabaseEnabled = true;
        //        webView.Settings.AllowContentAccess = true;
        //        //webView.Settings.SetAppCacheEnabled(true);
        //        webView.Settings.LoadsImagesAutomatically = true;
        //        //webView.Settings.BlockNetworkImage = false;
        //        webView.Settings.AllowFileAccess = true;
        //        webView.Settings.AllowFileAccessFromFileURLs = true;
        //        webView.Settings.AllowUniversalAccessFromFileURLs = true;
        //        //webView.Settings.AllowContentAccess = true;
        //        //webView.Settings.DisplayZoomControls = false;
        //        //webView.Settings.SetSupportZoom(false);

        //        //webView.Settings.SetAppCacheMaxSize(1024 * 1024 * 15);

        //        //webView.AddJavascriptInterface(new CallCSharp(webView), "NARA");

        //        //webView.Settings.SetAppCachePath("/data/data/" + "NARA" + "/cache");
        //        //webView.Settings.AllowFileAccess = true;
        //        //webView.Settings.SetAppCacheEnabled(true);

        //        //webView.Settings.CacheMode = CacheModes.NoCache;

        //        //var cookieManager = CookieManager.Instance;
        //        //cookieManager.SetAcceptCookie(true);
        //        //cookieManager.RemoveAllCookie();

        //        HtmlWebViewSource source = (HtmlWebViewSource)e.NewElement.Source;

        //        //if (cookies != null)
        //        //{
        //        //    if (cookies.Count() > 0)
        //        //    {
        //        //        foreach (var cookie in cookies)
        //        //        {
        //        //            //cookieManager.SetCookie(cookie.Domain, cookie.Name + "=" + cookie.Value);
        //        //        }
        //        //    }
        //        //}

        //        try
        //        {
        //            if (!isConnected)
        //            {
        //                //webView.Settings.CacheMode = CacheModes.CacheElseNetwork;
        //                string latestContent = repo.GetContent(source.BaseUrl).Content;

        //                var n = Directory.GetFiles(path);

        //                if (File.Exists(filename))
        //                    File.Delete(filename);

        //                using (var streamWriter = new StreamWriter(filename, false))
        //                {
        //                    streamWriter.WriteLine(latestContent);
        //                }
        //                webView.LoadUrl("file://" + filename);
        //                SetNativeControl(webView);
        //            }
        //            else
        //            {
        //                webView.LoadUrl("https://nara-test.semantika.eu/dvex/list");
        //                SetNativeControl(webView);


        //                ////webView.LoadUrl("file:///android_asset/Content/Index.html");

        //                //if (source.BaseUrl != NaraTools.EditView)
        //                //{
        //                //    await Task.Run(async () =>
        //                //    {
        //                //        //string filename = Path.Combine(path, "Index.html");
        //                //        List<string> result = await webUtil.GetContent(source.BaseUrl);
        //                //        string content = result.FirstOrDefault();
        //                //        result.Remove(result.FirstOrDefault());

        //                //        var downloaded = await DependencyService.Get<IPlatformSpecific>().getPageDependencies(result);

        //                //        var offlineContent = webUtil.CorrectContent(downloaded, result, content);
        //                //        repo.SaveContent(new OfflineContent() { Content = offlineContent, Date = DateTime.Now, Url = source.BaseUrl });
        //                //    });
        //                //}

        //                //webView.LoadUrl("javascript:alert(GetObject())");

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            webView.LoadUrl("file:///android_asset/Content/Index.html");
        //        }
        //    }
        //}

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
            }
            if (e.OldElement != null)
            {
            }
            if (e.NewElement != null)
            {
                wvc = Element as WebViewCustom;
                nativeWebView = Control as Android.Webkit.WebView;
                nativeWebView.SetBackgroundColor(wvc.BackgroundColor.ToAndroid());
                nativeWebView.SetWebViewClient(new ViewClient());
                nativeWebView.Settings.JavaScriptEnabled = true;
                nativeWebView.Settings.DomStorageEnabled = true;
                nativeWebView.Settings.LoadsImagesAutomatically = true;
                nativeWebView.Settings.AllowFileAccess = true;
                nativeWebView.Settings.AllowContentAccess = true;
                nativeWebView.Settings.AllowFileAccessFromFileURLs = true;
                nativeWebView.Settings.CacheMode = CacheModes.Default;
                nativeWebView.Settings.DatabaseEnabled = true;
                nativeWebView.SetLayerType(LayerType.Hardware, null);
                nativeWebView.Settings.SetAppCacheEnabled(true);
                var path = Context.CacheDir.AbsolutePath;
                Java.IO.File f = new Java.IO.File(path);
                Console.WriteLine(path);

                if (!f.Exists())
                {
                    f.Mkdirs();
                }

                nativeWebView.Settings.SetAppCachePath(f.AbsolutePath);

                nativeWebView.AddJavascriptInterface(new CallCSharp(nativeWebView, wvc), "CSharp");
                HtmlWebViewSource source = (HtmlWebViewSource)wvc.Source;

                wvc.EvalJS += EvaluateJavascript;

                //if (source.BaseUrl.Contains("Loading"))
                //{
                //    nativeWebView.LoadUrl("file:///android_asset/Loading.html");
                //}
                //else
                //{
                nativeWebView.LoadUrl(source.BaseUrl);
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (nativeWebView != null)
            {
                nativeWebView.Dispose();
            }
        }

        private void EvaluateJavascript(object sender, string e)
        {
            JavascriptResult jsResult = new JavascriptResult();
            nativeWebView.EvaluateJavascript(e, jsResult);
        }
    }

    public class CallCSharp : Java.Lang.Object
    {
        Android.Webkit.WebView webView;
        WebViewCustom wvc;
        JavascriptResult jsResult = new JavascriptResult();
        OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
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
                }
            }
            catch (System.Exception exce)
            { }

            return cookiesS;
        }

        public CallCSharp(Android.Webkit.WebView webView, WebViewCustom p_wvc)
        {
            this.webView = webView;
            wvc = p_wvc;
        }

        [Export]
        [JavascriptInterface]
        public async void HideLoader()
        {


            ////Saves js variables for web app, used for making interactions with xamarin
            //webView.EvaluateJavascript("var androidApp = true;", jsResult);
            //foreach (var val in repo.GetAdditionalData())
            //{
            //    webView.EvaluateJavascript("var " + val.Name + " = '" + val.Value + "';", jsResult);
            //}

            ////webView.EvaluateJavascript("init(115);", jsResult);

            ////Retrieves ready state of web page
            //webView.EvaluateJavascript("document.readyState", jsResult);

            ////Disables user web interactions
            //webView.EvaluateJavascript("document.body.style.webkitTouchCallout='none';", jsResult);
            //webView.EvaluateJavascript("document.body.style.webkitUserSelect='none';", jsResult);


            Device.BeginInvokeOnMainThread(() =>
            {
                wvc.Eval("var version = 116;");
                wvc.Eval("var environment = 'app';");
                wvc.Eval("var appCode = 'nara';");
                wvc.Eval("var androidApp = true;");
                wvc.Eval(CheckCookies());

                foreach (var val in repo.GetAdditionalData())
                {
                    wvc.Eval("var " + val.Name + " = '" + val.Value + "';");
                }

                wvc.Eval("checkHelpCookies();");
                wvc.Eval("checkLogin();");

                wvc.RaiseNavigated();

            });


        }
    }
}