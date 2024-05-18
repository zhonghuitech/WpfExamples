using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static WindowCapture.Framework.WindowHelper.NativeMethods;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Win32;

namespace WindowCapture.ScreenSelect
{
    public class ScreenReactSelect
    {
        private MainWindow mainWnd;
        private MainViewModel vm;
        private ScreenReactView screenWindow;

        public ScreenReactSelect(MainWindow mainWindow, MainViewModel vm)
        {
            this.mainWnd = mainWindow;
            this.vm = vm;
        }

        public void Capture()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                screenWindow = new ScreenReactView(mainWnd);
                screenWindow.ScreenShootCompleted += ScreenCaptureCompleted;
                screenWindow.ScreenShootCanceled += ScreenCaptureCanceled;
                screenWindow.Show();
                screenWindow.Activate();
            }));
        }

        private void ScreenCaptureCanceled()
        {
            // Canceled
        }

        private void ScreenCaptureCompleted(int x, int y, int w, int h)
        {
            this.vm.ShowMessage = string.Format("Screen select finished：（{0}, {1}），width*height：{2}x{3}", x, y, w, h);
            mainWnd.Show();
            mainWnd.Activate();
            mainWnd.WindowState = WindowState.Normal;

            var dlg = new SaveFileDialog();
            dlg.FileName = $"ScreenCapture{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "image file|*.jpg";

            if (dlg.ShowDialog() == true)
            {
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(screenWindow.CutBitmap(x, y, w, h)));
                using (var fs = File.OpenWrite(dlg.FileName))
                {
                    pngEncoder.Save(fs);
                    fs.Dispose();
                    fs.Close();
                }
            }
        }

    }
}
