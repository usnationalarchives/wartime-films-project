using CoreAnimation;
using CoreGraphics;
using Foundation;
using ImageIO;
using NARA;
using NARA.iOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ImageViewCustom), typeof(ImageViewRenderer))]

namespace NARA.iOS
{
    /// <summary>
    /// Custom renderer implemented for xamarin image control so it supports gif format
    /// </summary>
    public class ImageViewRenderer : ImageRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            CreateAnimatedImageView(CGImageSource.FromUrl(new NSUrl("loader.gif", true)), Control);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        /// <summary>
        /// Creates UIImageView instance that supports gif format
        /// </summary>
        /// <param name="nsData">Image byte buffer</param>
        /// <param name="imageView">UIImageView control</param>
        /// <returns>UIImageView control</returns>
        public static UIImageView GetAnimatedImageView(NSData nsData, UIImageView imageView = null)
        {

            var sourceRef = CGImageSource.FromData(nsData);
            return CreateAnimatedImageView(sourceRef, imageView);
        }

        /// <summary>
        /// Creates UIImageView control with support for .GIF format
        /// </summary>
        /// <param name="imageSource"></param>
        /// <param name="imageView"></param>
        /// <returns></returns>
        private static UIImageView CreateAnimatedImageView(CGImageSource imageSource, UIImageView imageView = null)
        {
            //Gets frame count from image
            var frameCount = imageSource.ImageCount;

            var frameImages = new List<NSObject>((int)frameCount);
            var frameCGImages = new List<CGImage>((int)frameCount);
            var frameDurations = new List<double>((int)frameCount);

            var totalFrameDuration = 0.0;

            //Sets duration to the specific frame
            for (int i = 0; i < frameCount; i++)
            {
                var frameImage = imageSource.CreateImage(i, null);

                frameCGImages.Add(frameImage);
                frameImages.Add(NSObject.FromObject(frameImage));

                var properties = imageSource.GetProperties(i, null);
                var duration = properties.Dictionary["{GIF}"];
                var delayTime = duration.ValueForKey(new NSString("DelayTime"));
                duration.Dispose();
                var realDuration = double.Parse(delayTime.ToString());
                frameDurations.Add(realDuration);
                totalFrameDuration += realDuration;
                frameImage.Dispose();
            }
            
            var framePercentageDurations = new List<NSNumber>((int)frameCount);
            var framePercentageDurationsDouble = new List<double>((int)frameCount);
            NSNumber currentDurationPercentage = 0.0f;
            double currentDurationDouble = 0.0f;

            //Sets precentage of the specific frame of the image
            for (int i = 0; i < frameCount; i++)
            {
                if (i != 0)
                {
                    var previousDuration = frameDurations[i - 1];
                    var previousDurationPercentage = framePercentageDurationsDouble[i - 1];

                    var number = previousDurationPercentage + (previousDuration / totalFrameDuration);
                    currentDurationDouble = number;
                    currentDurationPercentage = new NSNumber(number);
                }
                framePercentageDurationsDouble.Add(currentDurationDouble);
                framePercentageDurations.Add(currentDurationPercentage);
            }

            var imageSourceProperties = imageSource.GetProperties(null);
            var imageSourceGIFProperties = imageSourceProperties.Dictionary["{GIF}"];
            var loopCount = imageSourceGIFProperties.ValueForKey(new NSString("LoopCount"));
            var imageSourceLoopCount = float.Parse(loopCount.ToString());
            var frameAnimation = new CAKeyFrameAnimation();
            frameAnimation.KeyPath = "contents";
            if (imageSourceLoopCount <= 0.0f)
            {
                frameAnimation.RepeatCount = float.MaxValue;
            }
            else
            {
                frameAnimation.RepeatCount = imageSourceLoopCount;
            }

            imageSourceGIFProperties.Dispose();
            
            //Sets animation properties to a UIImageView control
            frameAnimation.CalculationMode = CAAnimation.AnimationDescrete;
            frameAnimation.Values = frameImages.ToArray();
            frameAnimation.Duration = totalFrameDuration;
            frameAnimation.KeyTimes = framePercentageDurations.ToArray();
            frameAnimation.RemovedOnCompletion = false;
            var firstFrame = frameCGImages[0];
            if (imageView == null)
                imageView = new UIImageView(new CGRect(0.0f, 0.0f, firstFrame.Width, firstFrame.Height));
            else
                imageView.Layer.RemoveAllAnimations();

            imageView.Layer.AddAnimation(frameAnimation, "contents");

            frameAnimation.Dispose();
            return imageView;
        }
    }
}
