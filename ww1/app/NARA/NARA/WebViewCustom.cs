using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebKit;
using Xamarin.Forms;

namespace NARA
{
    //WebViewCustom is a class, that inherits xamarin WebView and is used for custom renderer,
    //it is used for platform specific code
    public class WebViewCustom : WebView
    {
        public IEnumerable<Cookie> cookies { get; set; }
        public bool IsLoading { get; set; }

        public event EventHandler OnJsNavigation;
        public Uri Url { get; set; }
        public double Opacity { get; set; }

        //Event that is triggered, when webview ends loading content
        public event EventHandler MyNavigated;

        //Event that is triggered, when webview starts loading content
        public event EventHandler MyNavigating;

        //Event that is triggered, when user navigates to the homepage
        public event EventHandler GoToHomepage;

        //Event that is triggered, when webview is redirected to edit
        public event EventHandler GoToEdit;

        //Event that is triggered, when menu is tapped
        public event EventHandler ShowMenu;

        //Event that is triggered, when share is triggered
        public event EventHandler ShowShare;

        //Event that is triggered, when webview navigates to the edit profile
        public event EventHandler EditProfile;

        public event EventHandler ExternalLink;

        public event EventHandler RefreshContent;

        //Property for storing the last url, to be redirected to when webview is to be shown again
        public string BackUrl { get; set; }

        //Property for storing the url used for sharing of the content
        public string ShareUrl { get; set; }

        //Implementation of events
        public void RaiseEntered()
        {
            OnJsNavigation?.Invoke(this, EventArgs.Empty);
        }
        public void RaiseNavigating()
        {
            MyNavigating?.Invoke(this, EventArgs.Empty);
        }
        public void RaiseNavigated()
        {
            MyNavigated?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseHomepage()
        {
            GoToHomepage?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseEdit(string backUrl = "")
        {
            BackUrl = backUrl;
            GoToEdit?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseMenu()
        {
            ShowMenu?.Invoke(this, EventArgs.Empty);
        }

        public void ShowShareView()
        {
            ShowShare?.Invoke(this, EventArgs.Empty);
        }

        public void EditProfileView()
        {
            EditProfile.Invoke(this, EventArgs.Empty);
        }
 
        public void ExternalView()
        {
            ExternalLink.Invoke(this, EventArgs.Empty);
        }
        public void Refresh()
        {
            RefreshContent.Invoke(this, EventArgs.Empty);
        }

    }
}
