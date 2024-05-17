using System.Windows;
using System.Windows.Interop;
using WindowCapture.ScreenSelect;

namespace WindowCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private IntPtr _myHandle;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _myHandle = new WindowInteropHelper(this).Handle;
            //组合键
            var ctrHotKey = (uint)(HotKey.KeyModifiers.Alt | HotKey.KeyModifiers.Control);
            // Alt+CTRL+4
            HotKey.RegisterHotKey(_myHandle, 103, (HotKey.KeyModifiers)ctrHotKey, (int)HotKey.KeyModifiers.N4);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        // 快捷键响应事件
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handle)
        {
            string sid = wParam.ToString();
            switch (sid)
            {
                case "103":
                    ScreenRectSelect();
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private void ScreenRecordRectCallback(object sender, RoutedEventArgs e)
        {
            ScreenRectSelect();
        }

        private void ScreenRectSelect()
        {   
            // this.Hide();
            App.Current.MainWindow.WindowState = WindowState.Minimized;
            Thread.Sleep(150);  // 待当前窗口最小化
            new ScreenReactSelect(this, _vm).Capture();
        }
    }
}