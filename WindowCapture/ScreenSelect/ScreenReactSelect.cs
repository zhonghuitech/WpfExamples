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
                // ScreenWindow screenWindow = new ScreenWindow(mainWnd);
                screenWindow.ScreenShootCompleted += ScreenCaptureCompleted;
                screenWindow.ScreenShootCanceled += ScreenCaptureCanceled;
                screenWindow.Show();
                screenWindow.Activate();
            }));
        }

        private void ScreenCaptureCanceled()
        {
            //Canceled
        }

        private void ScreenCaptureCompleted(int x, int y, int w, int h)
        {
            this.vm.ShowMessage = string.Format("录屏区域设置完成：（{2}, {3}），宽*高：{0}x{1}", w, h, x, y);
            mainWnd.Show();
            mainWnd.Activate();
            mainWnd.WindowState = WindowState.Normal;
        }
    }
}
