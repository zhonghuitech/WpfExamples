using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace WindowCapture.Framework
{
    public class WindowHelper
    {
        public static System.Drawing.Size GetMonitorSize()
        {
            var window = System.Windows.Application.Current.MainWindow;
            var hwnd = new WindowInteropHelper(window).EnsureHandle();
            var monitor = NativeMethods.MonitorFromWindow(hwnd, NativeMethods.MONITOR_DEFAULTTONEAREST);
            NativeMethods.MONITORINFO info = new NativeMethods.MONITORINFO();
            NativeMethods.GetMonitorInfo(new HandleRef(null, monitor), info);
            return info.rcMonitor.Size;
        }

        public static class Win32
        {
            public const string User32 = "user32.dll";
            public const string Gdi32 = "gdi32.dll";
            public const string GdiPlus = "gdiplus.dll";
            public const string Kernel32 = "kernel32.dll";
            public const string Shell32 = "shell32.dll";
            public const string MsImg = "msimg32.dll";
            public const string NTdll = "ntdll.dll";
            public const string DwmApi = "dwmapi.dll";
            public const string Winmm = "winmm.dll";
            public const string Shcore = "Shcore.dll";
            public const int SM_CXSCREEN = 0;
            public const int SM_CYSCREEN = 1;

            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string className, string winName);

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessageTimeout(
              IntPtr hwnd,
              uint msg,
              IntPtr wParam,
              IntPtr lParam,
              uint fuFlage,
              uint timeout,
              IntPtr result);

            [DllImport("user32.dll")]
            public static extern bool EnumWindows(Win32.EnumWindowsProc proc, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr FindWindowEx(
              IntPtr hwndParent,
              IntPtr hwndChildAfter,
              string className,
              string winName);

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

            [DllImport("user32.dll")]
            public static extern IntPtr SetParent(IntPtr hwnd, IntPtr parentHwnd);

            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("winmm.dll")]
            public static extern long mciSendString(
              string strCommand,
              StringBuilder strReturn,
              int iReturnLength,
              IntPtr hwndCallback);

            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateBitmap(
              int nWidth,
              int nHeight,
              uint cPlanes,
              uint cBitsPerPel,
              IntPtr lpvBits);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern IntPtr DeleteDC(IntPtr hDc);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern int GetSystemMetrics(int abc);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(int ptr);

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(
              IntPtr hdc,
              int nXDest,
              int nYDest,
              int nWidth,
              int nHeight,
              IntPtr hdcSrc,
              int nXSrc,
              int nYSrc,
              Win32.TernaryRasterOperations dwRop);

            [DllImport("user32.dll")]
            public static extern void SetCursorPos(int x, int y);

            public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

            public struct DeskTopSize
            {
                public int cx;
                public int cy;
            }

            public enum TernaryRasterOperations : uint
            {
                BLACKNESS = 66, // 0x00000042
                NOTSRCERASE = 1114278, // 0x001100A6
                NOTSRCCOPY = 3342344, // 0x00330008
                SRCERASE = 4457256, // 0x00440328
                DSTINVERT = 5570569, // 0x00550009
                PATINVERT = 5898313, // 0x005A0049
                SRCINVERT = 6684742, // 0x00660046
                SRCAND = 8913094, // 0x008800C6
                MERGEPAINT = 12255782, // 0x00BB0226
                MERGECOPY = 12583114, // 0x00C000CA
                SRCCOPY = 13369376, // 0x00CC0020
                SRCPAINT = 15597702, // 0x00EE0086
                PATCOPY = 15728673, // 0x00F00021
                PATPAINT = 16452105, // 0x00FB0A09
                WHITENESS = 16711778, // 0x00FF0062
            }
        }

        internal static class NativeMethods
        {
            public const Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;

            [DllImport("user32.dll")]
            public static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool GetMonitorInfo(HandleRef hmonitor, MONITORINFO info);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
            public class MONITORINFO
            {
                internal int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                internal RECT rcMonitor = new RECT();
                internal RECT rcWork = new RECT();
                internal int dwFlags = 0;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;

                public RECT(int left, int top, int right, int bottom)
                {
                    this.left = left;
                    this.top = top;
                    this.right = right;
                    this.bottom = bottom;
                }

                public RECT(System.Drawing.Rectangle r)
                {
                    left = r.Left;
                    top = r.Top;
                    right = r.Right;
                    bottom = r.Bottom;
                }

                public static RECT FromXYWH(int x, int y, int width, int height) => new RECT(x, y, x + width, y + height);

                public System.Drawing.Size Size => new System.Drawing.Size(right - left, bottom - top);
            }
        }
    }
}
