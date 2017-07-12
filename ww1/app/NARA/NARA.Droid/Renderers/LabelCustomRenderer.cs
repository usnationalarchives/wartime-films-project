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
using Android.Graphics.Drawables.Shapes;
using Android.Graphics;

[assembly: ExportRenderer(typeof(LabelCustom), typeof(LabelCustomRenderer))]
namespace NARA.Droid.Renderers
{
    public class LabelCustomRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                LabelCustom formsLabel = new LabelCustom();

                if (e.NewElement != null)
                {
                    formsLabel = e.NewElement as LabelCustom;
                    //Control.SetLineSpacing((float)formsLabel.LineSpacing, 1);
                    Control.TextAlignment = Android.Views.TextAlignment.Center;

                    if (formsLabel.Padding != null)
                    {
                        Control.SetPadding((int)formsLabel.Padding.Left, (int)formsLabel.Padding.Top, (int)formsLabel.Padding.Right, (int)formsLabel.Padding.Bottom);
                    }

                    GradientDrawable gd = new GradientDrawable();

                    if (formsLabel.RequestedHeight == 1)
                    {
                        gd.SetColor(formsLabel.BorderColor.ToAndroid());
                    }
                    else
                    {
                        gd.SetColor(formsLabel.BackgroundColor.ToAndroid());
                    }

                    gd.SetCornerRadius(formsLabel.BorderRadius);
                    gd.SetStroke(2, formsLabel.BorderColor.ToAndroid());
                    Control.SetBackground(gd);
                }
            }

        }
    }
}