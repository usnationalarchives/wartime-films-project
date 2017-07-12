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

[assembly: ExportRenderer(typeof(BorderedImageCustom), typeof(BorderedImageRenderer))]

namespace NARA.Droid.Renderers
{
    public class BorderedImageRenderer : ImageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            BorderedImageCustom formsImage;

            if (Control != null)
            {
                if (e.NewElement != null)
                {
                    formsImage = e.NewElement as BorderedImageCustom;
                    GradientDrawable gd = new GradientDrawable();
                    gd.SetCornerRadius(2);
                    gd.SetStroke(0, Color.White.ToAndroid());
                    Control.SetBackground(gd);
                }
            }
        }
    }
}