using NARA;
using NARA.iOS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using static System.Net.Mime.MediaTypeNames;

[assembly: ExportRenderer(typeof(BorderedImageCustom), typeof(BorderedImage))]
namespace NARA.iOS
{
    public class BorderedImage : ImageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            SetBorder();
        }
        /// <summary>
        /// Sets the border of the image
        /// </summary>
        private void SetBorder()
        {
            try
            {
                Control.Layer.BorderColor = ((BorderedImageCustom)Element).BorderColor.ToCGColor();
                Control.Layer.BorderWidth = ((BorderedImageCustom)Element).BorderWidth;
                Control.ClipsToBounds = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to create circle image: " + ex);
            }
        }
    }
}
