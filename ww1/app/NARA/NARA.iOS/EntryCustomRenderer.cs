using CoreGraphics;
using Foundation;
using NARA;
using NARA.iOS;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EntryCustom), typeof(EntryCustomRenderer))]
namespace NARA.iOS
{
    /// <summary>
    /// Custom renderer implemented for xamarin entry control
    /// </summary>
    public class EntryCustomRenderer : EntryRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {

                //Sets border of the UIText to Line and changes it's width and color
                Control.BorderStyle = UITextBorderStyle.Line;
                Control.Layer.BorderColor = Color.FromHex("#717171").ToCGColor();
                Control.Layer.BorderWidth = 1;

                //Sets left margin of the control
                Control.LeftView = new UIView(new CGRect(0, 0, 15, 0));
                Control.LeftViewMode = UITextFieldViewMode.Always;

                var n = (EntryCustom)e.NewElement;
            }
		}
	}
}

