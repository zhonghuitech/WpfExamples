using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowCapture
{
    public class MainViewModel : ViewModelBase
    {
        private string _showMessage = "";

        public string ShowMessage
        {
            get { return _showMessage; }
            set
            {
                if (value == _showMessage)
                {
                    return;
                }

                _showMessage = value;
                NotifyPropertyChange(nameof(ShowMessage));
            }
        }
    }
}
