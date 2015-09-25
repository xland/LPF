using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace LPF.Ctl
{
    public partial class WindowL : Window
    {
        public WindowL()
        {

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SourceInitialized += WindowL_SourceInitialized;
            this.SizeChanged += WindowL_SizeChanged;
            this.LocationChanged += WindowL_LocationChanged;
        }

        private void WindowL_LocationChanged(object sender, EventArgs e)
        {
            cur_left = this.Left;
            cur_top = this.Top;
        }

        private void WindowL_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
            {
                WindowGrowBorder.Margin = new Thickness(0);
            }
            else
            {
                WindowGrowBorder.Margin = new Thickness(10);
                var old = (WindowL)e.OriginalSource;
                if (old.WindowState == WindowState.Maximized)
                {
                    this.Top = 300;
                }
            }
        }

        private void WindowL_SourceInitialized(object sender, EventArgs e)
        {
            var hs = PresentationSource.FromVisual(this) as HwndSource;
            hs.AddHook(new HwndSourceHook(WndProc));
        }

        static WindowL()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowL), new FrameworkPropertyMetadata(typeof(WindowL)));
        }
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

        private const int WM_GETMINMAXINFO = 0x0024;
        private const int WM_NCHITTEST = 0x0084;
        private readonly int agWidth = 12; //拐角宽度  
        private readonly int bThickness = 10; // 边框宽度  
        private Point mousePoint = new Point(); //鼠标坐标
        private double cur_top;
        private double cur_left;

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                case WM_NCHITTEST:
                    {
                        this.mousePoint.X = (lParam.ToInt32() & 0xFFFF);
                        this.mousePoint.Y = (lParam.ToInt32() >> 16);
                        if(WindowState == WindowState.Maximized)
                        {
                            cur_top = 0;
                            cur_left = 0;
                        }
                        // 窗口左上角  
                        if (this.mousePoint.Y - cur_top <= this.agWidth && this.mousePoint.X - cur_left <= this.agWidth)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOPLEFT);
                        }
                        // 窗口左下角      
                        else if (this.ActualHeight + cur_top - this.mousePoint.Y <= this.agWidth
                           && this.mousePoint.X - cur_left <= this.agWidth)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOMLEFT);
                        }
                        // 窗口右上角  
                        else if (this.mousePoint.Y - cur_top <= this.agWidth
                           && this.ActualWidth + cur_left - this.mousePoint.X <= this.agWidth)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOPRIGHT);
                        }
                        // 窗口右下角  
                        else if (this.ActualWidth + cur_left - this.mousePoint.X <= this.agWidth
                           && this.ActualHeight + cur_top - this.mousePoint.Y <= this.agWidth)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOMRIGHT);
                        }
                        // 窗口左侧  
                        else if (this.mousePoint.X - cur_left <= this.bThickness)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTLEFT);
                        }
                        // 窗口右侧  
                        else if (this.ActualWidth + cur_left - this.mousePoint.X <= this.bThickness)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTRIGHT);
                        }
                        // 窗口上方  
                        else if (this.mousePoint.Y - cur_top <= this.bThickness)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOP);
                        }
                        // 窗口下方  
                        else if (this.ActualHeight + cur_top - this.mousePoint.Y <= this.bThickness)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOM);
                        }
                        else if (cur_left < this.mousePoint.X && this.mousePoint.X < cur_left + WindowTitleContent.ActualWidth + 32 && this.mousePoint.Y > cur_top && this.mousePoint.Y < cur_top + 32)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTCAPTION);
                        }
                        else // 窗口移动  
                        {
                            return IntPtr.Zero;
                        }
                    }
            }
            return IntPtr.Zero;
        }
        
        Grid WindowContainer;
        ContentControl WindowTitleContent;
        Border WindowGrowBorder;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            WindowTitleContent = GetTemplateChild("WindowTitleContent") as ContentControl;
            WindowContainer = GetTemplateChild("WindowContainer") as Grid;
            WindowGrowBorder = GetTemplateChild("WindowGrowBorder") as Border;
            WindowTitleContent.MouseMove += WindowTitleContent_MouseMove;
            //WindowTitleContent.MouseDoubleClick += WindowTitleContent_MouseDoubleClick;
            var WindowTitleIcon = GetTemplateChild("WindowTitleIcon") as ContentControl;
            WindowTitleIcon.MouseMove += WindowTitleContent_MouseMove;
            //var WindowBorderTop = GetTemplateChild("WindowBorderTop") as Border;
            //WindowBorderTop.MouseLeftButtonDown += WindowBorderTop_MouseLeftButtonDown;
            //WindowBorderTop.MouseMove += WindowBorderTop_MouseMove;
            //WindowBorderTop.MouseLeftButtonUp += WindowBorderTop_MouseLeftButtonUp;
        }

        //private void WindowTitleContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (WindowState == WindowState.Maximized)
        //    {
        //        WindowContainer.Margin = new Thickness(10);
        //        WindowState = WindowState.Normal;
        //    }
        //    else
        //    {
        //        WindowContainer.Margin = new Thickness(0);
        //        this.WindowState = WindowState.Maximized;
        //    }
        //}

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
        #region 这一部分用于最大化时不遮蔽任务栏
        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };
        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        #endregion
    }
}
