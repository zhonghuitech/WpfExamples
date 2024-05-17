using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace WindowCapture.Framework
{
    public class ControlsHelper : DependencyObject
    {
        private static WindowHelper.Win32.DeskTopSize size;
        public static Brush Brush = (Brush)Application.Current.TryFindResource((object)"WD.BackgroundSolidColorBrush");
        public static Brush PrimaryNormalBrush = (Brush)Application.Current.TryFindResource((object)"WD.PrimaryNormalSolidColorBrush");
        public static Brush WindowForegroundBrush = (Brush)Application.Current.TryFindResource((object)"WD.PrimaryTextSolidColorBrush");
        private static bool _IsCurrentDark;
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlsHelper), new PropertyMetadata((object)new CornerRadius(4.0)));

        public static void OnSubThemeChanged()
        {
            if (ControlsHelper._IsCurrentDark)
                return;
            ControlsHelper.PrimaryNormalBrush = (Brush)Application.Current.TryFindResource((object)"WD.PrimaryNormalSolidColorBrush");
            Application.Current.Resources[(object)"WD.WindowBorderBrushSolidColorBrush"] = (object)ControlsHelper.PrimaryNormalBrush;
        }

        public static void ThemeRefresh() => ControlsHelper.OnSubThemeChanged();

        public static CornerRadius GetCornerRadius(DependencyObject obj) => (CornerRadius)obj.GetValue(ControlsHelper.CornerRadiusProperty);

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value) => obj.SetValue(ControlsHelper.CornerRadiusProperty, (object)value);

        public static void WindowShake(Window window = null)
        {
            if (window == null && Application.Current.Windows.Count > 0)
                window = Application.Current.Windows.OfType<Window>().FirstOrDefault<Window>((Func<Window, bool>)(o => o.IsActive));
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = new double?(window.Left);
            doubleAnimation.To = new double?(window.Left + 15.0);
            doubleAnimation.Duration = (Duration)TimeSpan.FromMilliseconds(50.0);
            doubleAnimation.AutoReverse = true;
            doubleAnimation.RepeatBehavior = new RepeatBehavior(3.0);
            doubleAnimation.FillBehavior = FillBehavior.Stop;
            DoubleAnimation animation = doubleAnimation;
            window.BeginAnimation(Window.LeftProperty, (AnimationTimeline)animation);
            new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/WPFDevelopers;component/Resources/Audio/shake.wav")).Stream).Play();
        }

        public static BitmapFrame CreateResizedImage(
          ImageSource source,
          int width,
          int height,
          int margin)
        {
            Rect rect = new Rect((double)margin, (double)margin, (double)(width - margin * 2), (double)(height - margin * 2));
            DrawingGroup target = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode((DependencyObject)target, BitmapScalingMode.Fant);
            target.Children.Add((Drawing)new ImageDrawing(source, rect));
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing((Drawing)target);
            RenderTargetBitmap source1 = new RenderTargetBitmap(width, height, 96.0, 96.0, PixelFormats.Default);
            source1.Render((Visual)drawingVisual);
            return BitmapFrame.Create((BitmapSource)source1);
        }

        public static BitmapSource Capture()
        {
            IntPtr dc = WindowHelper.Win32.GetDC(WindowHelper.Win32.GetDesktopWindow());
            IntPtr compatibleDc = WindowHelper.Win32.CreateCompatibleDC(dc);
            ControlsHelper.size.cx = WindowHelper.Win32.GetSystemMetrics(0);
            ControlsHelper.size.cy = WindowHelper.Win32.GetSystemMetrics(1);
            IntPtr compatibleBitmap = WindowHelper.Win32.CreateCompatibleBitmap(dc, ControlsHelper.size.cx, ControlsHelper.size.cy);
            if (compatibleBitmap == IntPtr.Zero)
                return (BitmapSource)null;
            IntPtr hgdiobj = WindowHelper.Win32.SelectObject(compatibleDc, compatibleBitmap);
            WindowHelper.Win32.BitBlt(compatibleDc, 0, 0, ControlsHelper.size.cx, ControlsHelper.size.cy, dc, 0, 0, WindowHelper.Win32.TernaryRasterOperations.SRCCOPY);
            WindowHelper.Win32.SelectObject(compatibleDc, hgdiobj);
            WindowHelper.Win32.DeleteDC(compatibleDc);
            WindowHelper.Win32.ReleaseDC(WindowHelper.Win32.GetDesktopWindow(), dc);
            BitmapSource sourceFromHbitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(compatibleBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            WindowHelper.Win32.DeleteObject(compatibleBitmap);
            GC.Collect();
            return sourceFromHbitmap;
        }

        public static AdornerLayer GetAdornerLayer(Visual visual)
        {
            object obj;
            switch (visual)
            {
                case AdornerDecorator adornerDecorator:
                    return adornerDecorator.AdornerLayer;
                case ScrollContentPresenter contentPresenter:
                    return contentPresenter.AdornerLayer;
                case Window window:
                    obj = window.Content;
                    break;
                default:
                    obj = (object)null;
                    break;
            }
            if (!(obj is Visual visual1))
                visual1 = visual;
            return AdornerLayer.GetAdornerLayer(visual1);
        }

        public static Window GetDefaultWindow()
        {
            Window defaultWindow = (Window)null;
            if (Application.Current.Windows.Count > 0)
                defaultWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault<Window>((Func<Window, bool>)(o => o.IsActive)) ?? Application.Current.Windows.OfType<Window>().FirstOrDefault<Window>();
            return defaultWindow;
        }

        public static Thickness GetPadding(FrameworkElement element)
        {
            PropertyInfo property = ((object)element).GetType().GetProperty("Padding");
            return property != (PropertyInfo)null ? (Thickness)property.GetValue((object)element, (object[])null) : new Thickness();
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, childIndex);
                if (child is T visualChild1)
                    return visualChild1;
                T visualChild2 = ControlsHelper.FindVisualChild<T>(child);
                if ((object)visualChild2 != null)
                    return visualChild2;
            }
            return default(T);
        }

        public static object GetXmlReader(object Content)
        {
            using (StringReader input = new StringReader(XamlWriter.Save((object)(Content as UIElement))))
            {
                using (XmlReader reader = XmlReader.Create((TextReader)input))
                {
                    if (XamlReader.Load(reader) is UIElement xmlReader)
                        return (object)xmlReader;
                }
            }
            return (object)null;
        }
    }
}
