using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
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

        /// <summary>
        /// 发送蓝牙内容 https://github.com/inthehand/32feet/blob/main/Samples/BluetoothClassicConsole/Program.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendFileCallback(object sender, RoutedEventArgs e)
        {
            BluetoothClient client = new BluetoothClient();
            IReadOnlyCollection<BluetoothDeviceInfo> devices = client.DiscoverDevices();

            BluetoothDeviceInfo device = null;
            foreach (BluetoothDeviceInfo dev in devices) //设备搜寻           
            {
                if ("Nax".Equals(dev.DeviceName))
                {
                    device = dev;
                    break;
                }
            }
            if (device == null) { return; }

            if (!device.Authenticated)
            {
                BluetoothSecurity.PairRequest(device.DeviceAddress, "1234");
            }

            device.Refresh();
            Console.WriteLine("Connecting..." + device.Authenticated);
            try
            {
                if (!device.Connected)
                {
                    client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                    Console.WriteLine("Connected!");

                }
                else
                {
                    Console.WriteLine("Has Been Connected!");
                }

                var stream = client.GetStream();

                if (stream != null)
                {
                    byte[] buffer = File.ReadAllBytes(@"D:\1.jpg");
                    //stream.Write(buffer, 0, buffer.Length);

                    StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.ASCII);
                    sw.Write(buffer);
                    //sw.WriteLine("Hello world!\r\n\r\n");
                    sw.Close();
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// https://tisyang.github.io/post/2016-05-27-bluetooth-serialport-with-32feet-net-library/
        /// https://blog.csdn.net/IFuWantMe/article/details/110658115
        /// </summary>
        /// <param name="bluetoothClient"></param>
        private void ReceiveData(BluetoothClient bluetoothClient)
        {
            bool isConnected = bluetoothClient.Connected;
            while (isConnected)
            {
                try
                {
                    string receive = string.Empty;
                    Stream peerStream = bluetoothClient.GetStream();
                    byte[] buffer = new byte[255];
                    peerStream.Read(buffer, 0, 255);
                    receive = Encoding.UTF8.GetString(buffer).ToString().Replace("\0", "");
                    //Console.ReadKey();
                    Console.Write(receive);
                }
                catch (Exception e)
                {
                    Console.Write("Error:" + e);
                    break;
                }
            }
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