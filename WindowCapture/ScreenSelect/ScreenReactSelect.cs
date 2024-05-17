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
        public void Capture()
        {
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                ScreenReactView screenWindow = new ScreenReactView();
                // ScreenWindow screenWindow = new ScreenWindow(mainWnd);
                // screenWindow.ScreenShootCompleted += ScreenCaptureCompleted;
                // screenWindow.ScreenShootCanceled += ScreenCaptureCanceled;
                screenWindow.Show();
                screenWindow.Activate();
            }));
        }
    }
}
