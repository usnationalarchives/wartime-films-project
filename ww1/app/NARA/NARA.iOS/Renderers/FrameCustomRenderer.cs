using NARA;
using NARA.iOS;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FrameCustom), typeof(FrameCustomRenderer))]
namespace NARA.iOS
{
    /// <summary>
    /// Custom renderer implemented for xamarin frame control
    /// </summary>
    public class FrameCustomRenderer : FrameRenderer
    {
        public FrameCustomRenderer()
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                //It sets corner radious and border width of the frame control
                var frame = this;
                frame.Layer.CornerRadius = 0;
                frame.Layer.BorderWidth = 3;
                frame.IsAccessibilityElement = true;
            }
        }
    }
}
