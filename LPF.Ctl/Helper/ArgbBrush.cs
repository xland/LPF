using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace LPF.Ctl.Helper
{
    public class ArgbBrush:MarkupExtension
    {
        public ArgbBrush(byte r, byte g, byte b)
        {
            A = 255;
            R = r;
            G = g;
            B = b;
        }
        //public ArgbBrush(byte r, byte g, byte b, byte a)
        //{
        //    A = a;
        //    R = r;
        //    G = g;
        //    B = b;
        //}
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new SolidColorBrush(Color.FromArgb(A, R, G, B));
        }
    }
}
