using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NARA
{
    public class RoundedImage : Image
    {
        public RoundedImage()
        {
            BorderWidth = 0;
            BorderColor = Color.White;
        }
        public Color BorderColor { get; set; }
        public int BorderWidth { get; set; }
    }
}
