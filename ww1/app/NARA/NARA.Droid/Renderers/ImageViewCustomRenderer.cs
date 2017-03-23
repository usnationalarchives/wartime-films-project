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
using Felipecsl.GifImageViewLibrary;
using Android.Graphics;
using System.Net.Http;
using System.IO;
using Java.Nio;

[assembly: ExportRenderer(typeof(ImageViewCustom), typeof(ImageViewCustomRenderer))]
namespace NARA.Droid.Renderers
{
    public class ImageViewCustomRenderer : ImageRenderer
    {
        GifImageView gifImageView;
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                if (e.NewElement != null)
                {

                    gifImageView = new GifImageView(Forms.Context);
                    ImageViewCustom formsImageView = e.NewElement as ImageViewCustom;
                    
                    try
                    {
                        gifImageView.SetBytes(formsImageView.ImageSourceInByteArray);
                        gifImageView.StartAnimation();
                        SetNativeControl(gifImageView);
                    }
                    catch (Exception ex)
                    {
                    }

                }
            }
        }
    }
}