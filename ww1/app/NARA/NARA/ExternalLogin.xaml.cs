using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace NARA
{
    /// <summary>
    /// ExternalLogin class inherits xamarin ContentPage and is used for logins from the social networks,
    /// such as Facebook.
    /// All the logic is handled in the custom renderer for the external login page
    /// </summary>
    public partial class ExternalLogin : ContentPage
    {
        public ExternalLogin()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            this.BackgroundColor = Color.FromHex("#1d1d1d");
            InitializeComponent();
        }
    }
}
