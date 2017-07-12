using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NARA
{
    public class LabelCustom : Label
    {
        public double LetterSpacing { get; set; }
        public double LineSpacing { get; set; }
        private Color borderColor { get; set; }
        public Color BorderColor { get { if (borderColor == null) { return Color.Transparent; } else { return borderColor; } } set { borderColor = value; } }
        public int BorderRadius { get; set; }
        public int BorderWidth { get; set; }
        public int RequestedHeight { get; set; }
        public Thickness Padding { get; set; }
        public bool FillColor { get; set; }
    }
}
