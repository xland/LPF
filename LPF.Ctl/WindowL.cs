using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using LPF.Ctl.Helper;

namespace LPF.Ctl
{
    public  partial class WindowL : Window
    {
        public WindowL()
        {
            FullScreenManager.RepairWpfWindowFullScreenBehavior(this);
            Loaded += WindowL_Loaded;
        }

        private void WindowL_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
        }

        static WindowL()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowL), new FrameworkPropertyMetadata(typeof(WindowL)));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(this.WndProc));
            }
        }
        
        private const int WM_NCHITTEST = 0x0084;
        private readonly int agWidth = 8; //拐角宽度  
        private readonly int bThickness = 8; // 边框宽度  
        private Point mousePoint = new Point(); //鼠标坐标  
        public enum HitTest : int
        {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21,
        }
        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_NCHITTEST:
                    {
                        this.mousePoint.X = (lParam.ToInt32() & 0xFFFF);
                        this.mousePoint.Y = (lParam.ToInt32() >> 16);
                        #region 测试鼠标位置  
                        // 窗口左上角  
                        if (this.mousePoint.Y - this.Top <= this.agWidth
                                         && this.mousePoint.X - this.Left <= this.agWidth)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOPLEFT);
                        }
                        // 窗口左下角　　  
                        else if (this.ActualHeight + this.Top - this.mousePoint.Y <= this.agWidth
                                         && this.mousePoint.X - this.Left <= this.agWidth)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOMLEFT);
                        }
                        // 窗口右上角  
                        else if (this.mousePoint.Y - this.Top <= this.agWidth
                         && this.ActualWidth + this.Left - this.mousePoint.X <= this.agWidth)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOPRIGHT);
                        }
                        // 窗口右下角  
                        else if (this.ActualWidth + this.Left - this.mousePoint.X <= this.agWidth
                         && this.ActualHeight + this.Top - this.mousePoint.Y <= this.agWidth)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOMRIGHT);
                        }
                        // 窗口左侧  
                        else if (this.mousePoint.X - this.Left <= this.bThickness)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTLEFT);
                        }
                        // 窗口右侧  
                        else if (this.ActualWidth + this.Left - this.mousePoint.X <= this.bThickness)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTRIGHT);
                        }
                        // 窗口上方  
                        else if (this.mousePoint.Y - this.Top <= this.bThickness)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOP);
                        }
                        // 窗口下方  
                        else if (this.ActualHeight + this.Top - this.mousePoint.Y <= this.bThickness)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOM);
                        }
                        else
                        {
                            handled = false;
                            return IntPtr.Zero;
                        }
                        #endregion
                    }
            }
            return IntPtr.Zero;
        }
        Grid WindowContainer;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var WindowTitleContent = GetTemplateChild("WindowTitleContent") as ContentControl;
            WindowContainer = GetTemplateChild("WindowContainer") as Grid;
            WindowTitleContent.MouseMove += WindowTitleContent_MouseMove;
            WindowTitleContent.MouseDoubleClick += WindowTitleContent_MouseDoubleClick;
            var WindowTitleIcon = GetTemplateChild("WindowTitleIcon") as ContentControl;
            WindowTitleIcon.MouseMove += WindowTitleContent_MouseMove;
            //var WindowBorderTop = GetTemplateChild("WindowBorderTop") as Border;
            //WindowBorderTop.MouseLeftButtonDown += WindowBorderTop_MouseLeftButtonDown;
            //WindowBorderTop.MouseMove += WindowBorderTop_MouseMove;
            //WindowBorderTop.MouseLeftButtonUp += WindowBorderTop_MouseLeftButtonUp;
        }

        private void WindowTitleContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WindowContainer.Margin = new Thickness(0);
            this.WindowState = WindowState.Maximized;

        }

        private void WindowTitleContent_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void WindowBorderTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        //private void WindowBorderTop_MouseMove(object sender, MouseEventArgs e)
        //{
        //    var mousePoint = Mouse.GetPosition(this);
        //    var ySpan = mousePoint.Y - curMousePoint.Y;
        //}

        //private void WindowBorderTop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    curMousePoint = Mouse.GetPosition(this);
        //}

        protected void TopBarMouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
