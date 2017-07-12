using CoreAnimation;
using NARA;
using NARA.iOS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(ButtonCustom), typeof(ButtonCustomRenderer))]

namespace NARA.iOS
{
    /// <summary>
    /// Custom renderer implemented for xamarin button control
    /// </summary>
    public class ButtonCustomRenderer : ButtonRenderer
    {
        UIButton btn;
        public ButtonCustomRenderer()
        {
        }

        /// <summary>
        /// Triggers when any of the element property changes
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                //Handles touch event of the button

                Control.TouchUpInside += (sender, el) =>
                {
                    UIView ctl = Control;
                    while (true)
                    {
                        ctl = ctl.Superview;
                        //Dismisses keyboard when UIView takes over
                        if (ctl.Description.Contains("UIView"))
                            break;
                    }
                    ctl.EndEditing(true);
                };
            }
        }
    }

}
