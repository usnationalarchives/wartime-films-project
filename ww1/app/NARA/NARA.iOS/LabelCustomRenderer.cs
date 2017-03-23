using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using NARA.iOS;
using Xamarin.Forms.Platform.iOS;
using ObjCRuntime;
using NARA;
using UIKit;
using Foundation;

[assembly: ExportRenderer(typeof(LabelCustom), typeof(LabelCustomRenderer))]
namespace NARA.iOS
{
    /// <summary>
    /// Custom renderer implemented for xamarin label control so it supports gif format
    /// </summary>
    public class LabelCustomRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var data = Element as LabelCustom;
            if (data == null || Control == null)
            {
                return;
            }

            //Creates NSMutableAttributedString from a C# string
            var text = Control.Text;
            var attributedString = new NSMutableAttributedString(text);

            //Sets letter spacing
            var nsKern = new NSString("NSKern");
            var spacing = NSObject.FromObject(data.LetterSpacing);
            var range = new NSRange(0, text.Length - 1);

            //Sets line spacing
            var paragraphStyle = new NSMutableParagraphStyle { LineSpacing = (nfloat)data.LineSpacing };
            var style = UIStringAttributeKey.ParagraphStyle;

            //Sets text alignment
            paragraphStyle.Alignment = UITextAlignment.Center;
            attributedString.AddAttribute(nsKern, spacing, range);
            attributedString.AddAttribute(style, paragraphStyle, range);
            Control.TextAlignment = UITextAlignment.Center;

            //Binds text to the control
            Control.AttributedText = attributedString;
        }
    }
}
