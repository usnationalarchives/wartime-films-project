using NARA.Common_p.Model;
using NARA.Common_p.Repository;
using NARA.Common_p.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NARA
{
    public partial class Home : ContentPage
    {
        double m_width = 0;
        double m_height = 0;

        public Home()
        {
            InitializeComponent();
        }

        void wv_Page_Navigating(object sender, WebNavigatingEventArgs e)
        {
        }

        void wv_Page_Navigated(object sender, WebNavigatedEventArgs e)
        {
        }
    }
}
