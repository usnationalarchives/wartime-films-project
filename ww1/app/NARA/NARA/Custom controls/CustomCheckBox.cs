using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NARA
{
    public class CustomCheckBox : Image
    {
        public CustomCheckBox()
        {
            Source = "empty_cb.png";
            isChecked = false;
            Aspect = Aspect.AspectFit;
        }
        bool isChecked { get; set; }
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;

                if (value)
                {
                    Source = "fill_cb.png";
                }
                else
                {
                    Source = "empty_cb.png";
                }
            }
        }

        public void CheckedChanged()
        {
            if (IsChecked)
            {
                IsChecked = false;
            }
            else
            {
                IsChecked = true;
            }
        }
    }
}
