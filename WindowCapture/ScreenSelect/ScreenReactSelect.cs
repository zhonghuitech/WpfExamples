using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowCapture.ScreenSelect
{
    public class ScreenReactSelect
    {
        private MainWindow mainWnd;
        private MainViewModel vm;

        public ScreenReactSelect(MainWindow mainWindow, MainViewModel vm)
        {
            this.mainWnd = mainWindow;
            this.vm = vm;
        }

        public void Capture()
        {
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                ScreenReactView screenWindow = new ScreenReactView(mainWnd);
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
        }
    }
}
