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

        public ScreenReactSelect(MainWindow mainWindow)
        {
            this.mainWnd = mainWindow;
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
            mainWnd.Show();
            mainWnd.Activate();
            mainWnd.WindowState = WindowState.Normal;
        }
    }
}
