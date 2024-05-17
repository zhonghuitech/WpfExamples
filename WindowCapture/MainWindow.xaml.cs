using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowCapture.ScreenSelect;

namespace WindowCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ScreenRecordRectCallback(object sender, RoutedEventArgs e)
        {
            // this.Hide();
            App.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
            new ScreenReactSelect(this).Capture();
        }
    }
}