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
using NARA;
using Xamarin.Forms;
using NARA.Droid.Renderers;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;

[assembly: ExportRenderer(typeof(EntryCustom), typeof(EntryCustomRenderer))]
namespace NARA.Droid.Renderers
{
    public class EntryCustomRenderer : EntryRenderer
    {
        EntryCustom formsEntry;
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                if (e.NewElement != null)
                {
                    formsEntry = e.NewElement as EntryCustom;
                    Control.Background = Resources.GetDrawable(Resource.Drawable.no_borders);
                    Control.SetPadding(0, 0, 0, 0);
                    Control.Gravity = GravityFlags.Bottom;
                }
            }
        }

        protected override void OnFocusChanged(bool gainFocus, [GeneratedEnum] FocusSearchDirection direction, Rect previouslyFocusedRect)
        {
            base.OnFocusChanged(gainFocus, direction, previouslyFocusedRect);
            if (gainFocus)
            {
                if (Control != null)
                {

                }
            }
        }
    }
}