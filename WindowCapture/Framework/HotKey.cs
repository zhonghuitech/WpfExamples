using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowCapture
{
    public class HotKey
    {

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd, // handle to window   
            int id, // hot key identifier   
            KeyModifiers fsModifiers, // key-modifier options   
            int vk // virtual-key code   
        );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd, // handle to window   
            int id // hot key identifier   
        );

        [Flags]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8,
            ESC = 27,
            Space = 32,
            Left = 37,
            Up = 38,
            Right = 39,
            Down = 40,
            A = 65,
            B = 66,
            C = 67,
            D = 68,
            Q = 81,
            S = 83,
            W = 87,
            N1 = 49,
            N2 = 50,
            N3 = 51,
            N4 = 52
        }
    }
}
