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
using Xamarin.Forms;
using NARA;
using NARA.Droid.Renderers;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(ButtonCustom), typeof(ButtonCustomRenderer))]

namespace NARA.Droid.Renderers
{
    public class ButtonCustomRenderer : ButtonRenderer
    {
        private GradientDrawable _normal, _pressed;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var button = (ButtonCustom)e.NewElement;

                Control.SetHeight(25);
                Control.TextAlignment = Android.Views.TextAlignment.Center;

                button.SizeChanged += (s, args) =>
                {

                };
            }
        }
    }
}