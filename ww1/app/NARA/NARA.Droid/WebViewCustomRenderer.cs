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

[assembly: ExportRenderer(typeof(WebViewCustom), typeof(WebViewCustomRenderer))]
namespace CustomRenderer.Droid
{
    public class WebViewCustomRenderer : WebViewRenderer
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        ChromeClient cc = new ChromeClient();
        bool isConnected = DependencyService.Get<IPlatformSpecific>().CheckConnection();
        OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
        WebUtil webUtil = new WebUtil();
        string url = NaraTools.EditView;
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        static WebViewCustom wvc;

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
            JavascriptResult jsResult = new JavascriptResult();
            OfflineRepository repo = new OfflineRepository(DependencyService.Get<IPlatformSpecific>().ConnectionString());
            WebUtil webUtil = new WebUtil();
            bool refreshed = false;
            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
            {

                if (url.Contains("jsHome") || url.Contains("jsNavigate"))
                {
                    wvc.RaiseHomepage();
                    return true;
                }
                else if (url.Contains("login"))
                {
                    wvc.RaiseEdit(lastUrl);
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
                    view.EvaluateJavascript("javascript: window.location.reload(true)", jsResult);
                    return true;
                }
                else if (url.Contains("jsShare"))
                {
                    wvc.ShareUrl = lastUrl;
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
                    lastUrl = url;
                }

                return false;
            }
            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                //NaraGlobal.IsWebViewLoading = false;
                wvc.RaiseNavigated();
                //Saves js variables for web app, used for making interactions with xamarin
                view.EvaluateJavascript("var environment = 'app';", jsResult);
                view.EvaluateJavascript("var appCode = 'nara';", jsResult);

                //Retrieves ready state of web page
                view.EvaluateJavascript("document.readyState", jsResult);

                //Disables user web interactions
                view.EvaluateJavascript("document.body.style.webkitTouchCallout='none';", jsResult);
                view.EvaluateJavascript("document.body.style.webkitUserSelect='none';", jsResult);
            }
            public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon)
            {
                CheckCookies();
                wvc.RaiseNavigating();
            }
            public override void OnReceivedError(Android.Webkit.WebView view, ClientError errorCode, string description, string failingUrl)
            {
                view.LoadUrl("file:///android_asset/Content/Index.html");
            }

            /// <summary>
            /// Checks whether user is signed in and if it is not, it
            /// clears cookies. If user is signed in, then it checks if cookies exists and also
            /// checks their validation
            /// </summary>
            /// <returns></returns>
            private void CheckCookies()
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

            //if (wvc == null)
            //{
                wvc = e.NewElement as WebViewCustom;
            //}

            if (wvc != null)
            {
                var nativeWebView = Control as Android.Webkit.WebView;
                nativeWebView.SetWebViewClient(new ViewClient());
                nativeWebView.Settings.JavaScriptEnabled = true;
                nativeWebView.Settings.DomStorageEnabled = true;
                nativeWebView.Settings.LoadsImagesAutomatically = true;
                nativeWebView.Settings.AllowFileAccess = true;
                nativeWebView.Settings.AllowContentAccess = true;
                nativeWebView.Settings.AllowFileAccessFromFileURLs = true;
                nativeWebView.Settings.CacheMode = CacheModes.NoCache;

                nativeWebView.SetLayerType(LayerType.Hardware, null);

                //nativeWebView.AddJavascriptInterface(new CallCSharp(nativeWebView), "CSharp");
                HtmlWebViewSource source = (HtmlWebViewSource)wvc.Source;

                if (source.BaseUrl.Contains("Loading"))
                {
                    nativeWebView.LoadUrl("file:///android_asset/Loading.html");
                }
                else
                {
                    nativeWebView.LoadUrl(source.BaseUrl);
                }

                //view.LoadUrl("file:///android_asset/Content/Index.html");
            }
        }

        private async Task SaveContent()
        {
        }

    }

    public class CallCSharp : Java.Lang.Object
    {
        Android.Webkit.WebView webView;
        public CallCSharp(Android.Webkit.WebView webView)
        {
            this.webView = webView;
        }
        public void JsNavigate()
        {
        }
    }
}