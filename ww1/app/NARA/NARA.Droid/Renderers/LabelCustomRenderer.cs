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
                    Control.SetLineSpacing((float)formsLabel.LineSpacing, 1);
                }
            }

        }
    }
}