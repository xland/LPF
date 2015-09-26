using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LPF.Ctl
{
    public class ButtonL:Button
    {
        public static readonly DependencyProperty LeftIconProperty = 
            DependencyProperty.Register("LeftIcon", 
                typeof(string), 
                typeof(ButtonL));
        public string LeftIcon
        {
            get { return (string)GetValue(LeftIconProperty); }
            set { SetValue(LeftIconProperty, value); }
        }

        public static readonly DependencyProperty RightIconProperty =
    DependencyProperty.Register("RightIcon",
        typeof(string),
        typeof(ButtonL));
        public string RightIcon
        {
            get { return (string)GetValue(RightIconProperty); }
            set { SetValue(RightIconProperty, value); }
        }
       
        static ButtonL()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonL), new FrameworkPropertyMetadata(typeof(ButtonL)));
        }
    }
}
