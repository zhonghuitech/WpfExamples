using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowCapture.Framework;
using static WindowCapture.Framework.WindowHelper;
using Size = System.Drawing.Size;
namespace WindowCapture.ScreenSelect
{

    public static class ScreenExtensions
    {
        public static void GetDpi(int x, int y, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(1, 1);
            var mon = MonitorFromPoint(pnt, 2);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }
        [DllImport(Win32.User32)]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);
        [DllImport(Win32.Shcore)]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
    }
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }

    public struct ScreenDPI
    {
        public uint dpiX;
        public uint dpiY;
        public float scaleX;
        public float scaleY;
    }

    public enum ScreenCutMouseType
    {
        Default,
        DrawMouse,
        MoveMouse,
    }

    /// <summary>
    /// ScreenReactView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenReactView : Window
    {
        private MainWindow mainWnd;
        private AdornerLayer adornerLayer;
        private FrameworkElement frameworkElement;
        private bool isMouseUp;
        private Point? pointStart, pointEnd;
        private const string _tag = "Draw";

        private Rect rect;
        private ScreenCutAdorner screenCutAdorner;
        private ScreenCutMouseType screenCutMouseType = ScreenCutMouseType.Default;

        /// <summary>
        /// 截图完成委托
        /// </summary>
        public delegate void ScreenShootDone(int x, int y, int w, int h);
        /// <summary>
        /// 截图完成事件
        /// </summary>
        public event ScreenShootDone ScreenShootCompleted;

        /// <summary>
        /// 截图取消委托
        /// </summary>
        public delegate void ScreenShotCanceled();
        /// <summary>
        /// 截图取消事件
        /// </summary>
        public event ScreenShotCanceled ScreenShootCanceled;

        private ScreenDPI screenDPI;
        private double ratio = 1;

        public ScreenReactView(MainWindow mainWnd)
        {
            Size size = WindowHelper.GetMonitorSize();

            screenDPI = GetScreenDPI();
            InitializeComponent();
            // 设置canvas的背景为当前截图
            canvas.Background = new ImageBrush(ControlsHelper.Capture());
            // 宽高点完，实现初始化蒙层效果
            _rectangleLeft.Width = canvas.Width;
            _rectangleLeft.Height = canvas.Height;
            ratio = (size.Width / canvas.Width);
            _border.Opacity = 0;
            this.mainWnd = mainWnd;
        }

        public void Dispose()
        {
            canvas.Background = null;
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Dispose();
        }

        private ScreenDPI GetScreenDPI()
        {
            ScreenDPI dpi = new ScreenDPI();
            Size size = WindowHelper.GetMonitorSize();

            int y = 2;
            int x = 2;


            //dpi.dpiX = (uint)size.Width;
            //dpi.dpiY = (uint)size.Height;
            ScreenExtensions.GetDpi(x, y, DpiType.Effective, out dpi.dpiX, out dpi.dpiY);

            dpi.scaleX = (float)((double)dpi.dpiX / 0.95999997854232788 / 100.0);
            dpi.scaleY = (float)((double)dpi.dpiY / 0.95999997854232788 / 100.0);
            return dpi;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            OnCanceled();
        }

        private void Restore()
        {
            _border.Cursor = Cursors.SizeAll;
            if (screenCutMouseType == ScreenCutMouseType.Default) return;
            screenCutMouseType = ScreenCutMouseType.Default;
        }

        private void _border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isMouseUp)
            {
                var left = Canvas.GetLeft(_border);
                var top = Canvas.GetTop(_border);
                var beginPoint = new Point(left, top);
                var endPoint = new Point(left + _border.ActualWidth, top + _border.ActualHeight);
                rect = new Rect(beginPoint, endPoint);
                pointStart = beginPoint;
                MoveAllRectangle(endPoint);
            }
            EditBarPosition();
            EditRectInfoPosition();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (screenCutMouseType == ScreenCutMouseType.Default)
                screenCutMouseType = ScreenCutMouseType.MoveMouse;
        }

        private void ButtonComplete_Click(object sender, RoutedEventArgs e)
        {
            Restore();
            DisposeControl();
            this.Close();
            if (ScreenShootCompleted != null)
                ScreenShootCompleted((int)(rect.TopLeft.X * ratio), (int)(rect.TopLeft.Y * ratio),
                    (int)(rect.Width * ratio), (int)(rect.Height * ratio));

        }

        public CroppedBitmap CutBitmap(int x, int y, int w, int h)
        {
            _border.Visibility = Visibility.Collapsed;
            _editBar.Visibility = Visibility.Collapsed;
            _rectangleLeft.Visibility = Visibility.Collapsed;
            _rectangleTop.Visibility = Visibility.Collapsed;
            _rectangleRight.Visibility = Visibility.Collapsed;
            _rectangleBottom.Visibility = Visibility.Collapsed;
            DpiScale dpiScale = VisualTreeHelper.GetDpi(canvas);
            // 屏幕等比截图需要考虑缩放率进行等比放大canvas
            var source = new RenderTargetBitmap(
                (int)(canvas.Width * screenDPI.scaleX),
                (int)(canvas.Height * screenDPI.scaleY),
                screenDPI.dpiX, screenDPI.dpiY, PixelFormats.Default);
            source.Render((Visual)canvas);
            // 屏幕等比截图需要考虑缩放率进行等比放大截图区域
            var realrect = new Int32Rect(
                (int)(rect.X * screenDPI.scaleX),
                (int)(rect.Y * screenDPI.scaleY),
                (int)(rect.Width * screenDPI.scaleX),
                (int)(rect.Height * screenDPI.scaleY));
            return new CroppedBitmap((BitmapSource)source, realrect);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            OnCanceled();
        }

        void OnCanceled()
        {
            this.Close();
            mainWnd.Show();
            mainWnd.Activate();
            mainWnd.WindowState = WindowState.Normal;
            if (ScreenShootCanceled != null)
            {
                ScreenShootCanceled();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OnCanceled();
            }
            else if (e.Key == Key.Delete)
            {
                if (canvas.Children.Count > 0)
                    canvas.Children.Remove(frameworkElement);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (canvas.Children.Count > 0)
                    canvas.Children.Remove(canvas.Children[canvas.Children.Count - 1]);
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source is RadioButton)
                return;

            var vPoint = e.GetPosition(canvas);
            if (!isMouseUp)
            {
                pointStart = vPoint;
                screenCutMouseType = ScreenCutMouseType.DrawMouse;
                _editBar.Visibility = Visibility.Hidden;
                pointEnd = pointStart;
                rect = new Rect(pointStart.Value, pointEnd.Value);
            }
            else
            {
                if (vPoint.X < rect.Left || vPoint.X > rect.Right)
                    return;

                if (vPoint.Y < rect.Top || vPoint.Y > rect.Bottom)
                    return;
                pointStart = vPoint;

                switch (screenCutMouseType)
                {
                    default:
                        Focus();
                        break;
                }
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.Source is RadioButton)
                return;

            if (pointStart is null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var current = e.GetPosition(canvas);
                switch (screenCutMouseType)
                {
                    case ScreenCutMouseType.DrawMouse:
                        MoveAllRectangle(current);
                        break;
                    case ScreenCutMouseType.MoveMouse:
                        MoveRect(current);
                        break;
                }
            }
        }

        private void SelectElement()
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(canvas); i++)
            {
                var child = VisualTreeHelper.GetChild(canvas, i);
                if (child is FrameworkElement frameworkElement && frameworkElement.Tag != null)
                    if (frameworkElement.Tag.ToString() == _tag)
                        frameworkElement.Opacity = 1;
            }
        }

        private void MoveRect(Point current)
        {
            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            if (current != vPoint)
            {
                var vector = Point.Subtract(current, vPoint);
                var left = Canvas.GetLeft(_border) + vector.X;
                var top = Canvas.GetTop(_border) + vector.Y;
                if (left <= 0)
                    left = 0;
                if (top <= 0)
                    top = 0;
                if (left + _border.Width >= canvas.ActualWidth)
                    left = canvas.ActualWidth - _border.ActualWidth;
                if (top + _border.Height >= canvas.ActualHeight)
                    top = canvas.ActualHeight - _border.ActualHeight;
                pointStart = current;

                Canvas.SetLeft(_border, left);
                Canvas.SetTop(_border, top);
                rect = new Rect(new Point(left, top), new Point(left + _border.Width, top + _border.Height));
                _rectangleLeft.Height = canvas.ActualHeight;
                _rectangleLeft.Width = left <= 0 ? 0 : left >= canvas.ActualWidth ? canvas.ActualWidth : left;


                Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
                _rectangleTop.Height = top <= 0 ? 0 : top >= canvas.ActualHeight ? canvas.ActualHeight : top;

                Canvas.SetLeft(_rectangleRight, left + _border.Width);
                var wRight = canvas.ActualWidth - (_border.Width + _rectangleLeft.Width);
                _rectangleRight.Width = wRight <= 0 ? 0 : wRight;
                _rectangleRight.Height = canvas.ActualHeight;

                Canvas.SetLeft(_rectangleBottom, _rectangleLeft.Width);
                Canvas.SetTop(_rectangleBottom, top + _border.Height);
                _rectangleBottom.Width = _border.Width;
                var hBottom = canvas.ActualHeight - (top + _border.Height);
                _rectangleBottom.Height = hBottom <= 0 ? 0 : hBottom;
                EditRectInfoPosition();
                EditBarPosition();
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.Source is ToggleButton)
                return;
            if (pointStart == pointEnd)
            {
                return;
            }
            var fElement = e.Source as FrameworkElement;
            if (fElement != null && fElement.Tag == null)
                SelectElement();
            isMouseUp = true;
            if (screenCutMouseType != ScreenCutMouseType.Default)
            {
                if (screenCutMouseType == ScreenCutMouseType.MoveMouse)
                {
                    EditBarPosition();
                }

                screenCutMouseType = ScreenCutMouseType.Default;
                DisposeControl();
            }
        }

        private void DisposeControl()
        {
            pointStart = null;
            pointEnd = null;
        }

        private void EditRectInfoPosition()
        {
            rectInfo.Visibility = Visibility.Visible;
            string i = string.Format("{0} x {1}", (int)(rect.Width * ratio), (int)(rect.Height * ratio));
            rectInfoText.Text = i;
            Canvas.SetLeft(rectInfo, rect.X + 5);
            double target = rect.Y - 20;
            rectInfoText.Foreground = target < 0 ?
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e60012"))
                 : new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));

            Canvas.SetTop(rectInfo, target > 0 ? target : rect.Y + 5);
        }

        private void EditBarPosition()
        {
            _editBar.Visibility = Visibility.Visible;
            Canvas.SetLeft(_editBar, rect.X + rect.Width - _editBar.ActualWidth);
            var y = Canvas.GetTop(_border) + _border.ActualHeight + _editBar.ActualHeight +
                    24;
            if (y > canvas.ActualHeight && Canvas.GetTop(_border) > _editBar.ActualHeight)
                y = Canvas.GetTop(_border) - _editBar.ActualHeight - 8;
            else if (y > canvas.ActualHeight && Canvas.GetTop(_border) < _editBar.ActualHeight)
                y = _border.ActualHeight - _editBar.ActualHeight - 8;
            else
                y = Canvas.GetTop(_border) + _border.ActualHeight + 8;
            Canvas.SetTop(_editBar, y);
        }

        private void MoveAllRectangle(Point current)
        {
            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            pointEnd = current;
            var vEndPoint = current;

            rect = new Rect(vPoint, vEndPoint);
            _rectangleLeft.Width = rect.X < 0 ? 0 : rect.X > canvas.ActualWidth ? canvas.ActualWidth : rect.X;
            _rectangleLeft.Height = canvas.Height;

            Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
            _rectangleTop.Width = rect.Width;
            var h = 0.0;
            if (current.Y < vPoint.Y)
                h = current.Y;
            else
                h = current.Y - rect.Height;

            _rectangleTop.Height = h < 0 ? 0 : h > canvas.ActualHeight ? canvas.ActualHeight : h;

            Canvas.SetLeft(_rectangleRight, _rectangleLeft.Width + rect.Width);
            var rWidth = canvas.Width - (rect.Width + _rectangleLeft.Width);
            _rectangleRight.Width = rWidth < 0 ? 0 : rWidth > canvas.ActualWidth ? canvas.ActualWidth : rWidth;

            _rectangleRight.Height = canvas.Height;

            Canvas.SetLeft(_rectangleBottom, _rectangleLeft.Width);
            Canvas.SetTop(_rectangleBottom, rect.Height + _rectangleTop.Height);
            _rectangleBottom.Width = rect.Width;
            var rBottomHeight = canvas.Height - (rect.Height + _rectangleTop.Height);
            _rectangleBottom.Height = rBottomHeight < 0 ? 0 : rBottomHeight;

            _border.Height = rect.Height;
            _border.Width = rect.Width;
            Canvas.SetLeft(_border, rect.X);
            Canvas.SetTop(_border, rect.Y);

            if (adornerLayer != null) return;
            _border.Opacity = 1;
            adornerLayer = AdornerLayer.GetAdornerLayer(_border);

            screenCutAdorner = new ScreenCutAdorner(_border);
            screenCutAdorner.PreviewMouseDown += (s, e) =>
            {
                Restore();
            };
            adornerLayer.Add(screenCutAdorner);
            _border.SizeChanged += _border_SizeChanged;
        }
    }
}
