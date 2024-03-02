using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace MerryTestFramework.testitem.Computer
{
    /// <summary>
    /// 控制窗体类
    /// </summary>
    public class WindowControl
    {
        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        public struct HWND__
        {
            public int unused;
        }

        private readonly List<string> _clollection = new List<string>();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr hDeskop, EnumDelegate lpenumcallbackFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr handle, StringBuilder text, int maxlen);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowW([MarshalAs(UnmanagedType.LPTStr)][In] string lpClassName, [MarshalAs(UnmanagedType.LPTStr)][In] string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow([In] IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern int SendMessageW([In] IntPtr hWnd, uint Msg, [MarshalAs(UnmanagedType.U4)] uint wParam, [MarshalAs(UnmanagedType.I4)] int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessageW([In] IntPtr hWnd, uint Msg, [MarshalAs(UnmanagedType.U4)] uint wParam, [MarshalAs(UnmanagedType.I4)] int lParam);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseWindow([In] IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow([In] IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int SetCursorPos(int x, int y);

        [DllImport("user32")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// 获取窗体名
        /// </summary>
        /// <param name="WinName"></param>
        /// <param name="IsFullName"></param>
        /// <param name="NameFound"></param>
        /// <returns></returns>
        public bool GetWindowName(string WinName, bool IsFullName, out string NameFound)
        {
            bool result = false;
            NameFound = "";
            _clollection.Clear();
            Regex regex = new Regex(WinName);

            EnumDelegate lpenumcallbackFunc = new EnumDelegate(EnumWindowName);
            if (EnumDesktopWindows(IntPtr.Zero, lpenumcallbackFunc, IntPtr.Zero))
            {
                foreach (string current in this._clollection)
                {
                    if (IsFullName)
                    {
                        if (current.Equals(WinName))
                        {
                            NameFound = current;
                            result = true;
                            break;
                        }
                    }
                    else if (regex.Match(current).Success)
                    {
                        NameFound = current;
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private bool EnumWindowName(IntPtr hWnd, int lParam)
        {
            StringBuilder stringBuilder = new StringBuilder(255);
            WindowControl.GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity + 1);
            string text = stringBuilder.ToString();
            if (WindowControl.IsWindowVisible(hWnd) && !string.IsNullOrEmpty(text))
            {
                this._clollection.Add(text);
            }
            return true;
        }
        /// <summary>
        /// 选中窗体并按键
        /// </summary>
        /// <param name="WinName"></param>
        /// <param name="fullName"></param>
        /// <param name="sKey"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SendKeyToWindow(string WinName, bool fullName, string sKey, Point point, out string error)
        {
            bool flag = false;
            error = "";
            try
            {
                SetCursorPos(point.X, point.Y); ;
                mouse_event(2, 10, 10, 0, 0);
                Thread.Sleep(50);
                mouse_event(4, 10, 10, 0, 0);
                string lpWindowName = "";
                int i = 0;
                while (!GetWindowName(WinName, fullName, out lpWindowName))
                {
                    if (i > 100) return false;
                    Thread.Sleep(500);
                    i++;
                }
                IntPtr foregroundWindow = IntPtr.Zero;
                flag = true;
                foregroundWindow = FindWindowW(null, lpWindowName);
                if (foregroundWindow.ToInt32() == 0)
                {
                    error = "find window fail";
                    return true;
                }
                if (!SetForegroundWindow(foregroundWindow))
                {
                    Thread.Sleep(20);
                    if (!SetForegroundWindow(foregroundWindow))
                    {
                        error = "Set window to front";
                        return true;
                    }
                }
                Thread.Sleep(80);
                byte expr_81 = (byte)((Keys)Enum.Parse(typeof(Keys), sKey));
                //按键
                keybd_event(expr_81, 0, 0u, 0u);
                Thread.Sleep(20);
                keybd_event(expr_81, 0, 2u, 0u);

            }
            catch (Exception ex)
            {
                error = "error->" + ex.Message;
            }
            return flag;
        }

        public bool SendKeyToWindow(string WinName, bool fullName, int x, int y, out string error)
        {
            bool flag = false;
            error = "";
            try
            {
                string lpWindowName = "";
                if (!this.GetWindowName(WinName, fullName, out lpWindowName))
                {
                    error = "Get window name fail";
                    bool result = flag;
                    return result;
                }
                IntPtr intPtr = WindowControl.FindWindowW(null, lpWindowName);
                if (intPtr.ToInt32() == 0)
                {
                    error = "find window fail";
                    bool result = flag;
                    return result;
                }
                if (!WindowControl.SetForegroundWindow(intPtr))
                {
                    error = "Set window to front";
                    bool result = flag;
                    return result;
                }
                WindowControl.ShowWindow(intPtr, 3);
                WindowControl.SetCursorPos(x, y);
                Thread.Sleep(300);
                WindowControl.mouse_event(2, 0, 0, 0, 0);
                Thread.Sleep(200);
                WindowControl.mouse_event(4, 0, 0, 0, 0);
                flag = true;
            }
            catch (Exception ex)
            {
                error = "error->" + ex.Message;
            }
            return flag;
        }

        public bool SetWindowFront(string WinName, bool fullName, out string error)
        {
            bool flag = false;
            error = "";
            try
            {
                string lpWindowName = "";
                if (!this.GetWindowName(WinName, fullName, out lpWindowName))
                {
                    error = "Get window name fail";
                    bool result = flag;
                    return result;
                }
                IntPtr foregroundWindow = WindowControl.FindWindowW(null, lpWindowName);
                if (foregroundWindow.ToInt32() == 0)
                {
                    error = "find window fail";
                    bool result = flag;
                    return result;
                }
                if (!WindowControl.SetForegroundWindow(foregroundWindow))
                {
                    error = "Set window to front";
                    bool result = flag;
                    return result;
                }
                SendKeys.Send("D");
                flag = true;
            }
            catch (Exception ex)
            {
                error = "error->" + ex.Message;
            }
            return flag;
        }

        public IntPtr FindWindowHd(string WinName, bool fullName, out string error)
        {
            IntPtr intPtr = IntPtr.Zero;
            error = "";
            try
            {
                string lpWindowName = "";
                if (!this.GetWindowName(WinName, fullName, out lpWindowName))
                {
                    error = "Get window name fail";
                    IntPtr result = intPtr;
                    return result;
                }
                intPtr = WindowControl.FindWindowW(null, lpWindowName);
                if (intPtr.ToInt32() == 0)
                {
                    error = "find window fail";
                    IntPtr result = intPtr;
                    return result;
                }
            }
            catch (Exception ex)
            {
                error = "error->" + ex.Message;
            }
            return intPtr;
        }

        public bool SendKeyToWindow(IntPtr hd, string sKey, out string error)
        {
            bool result = false;
            error = "";
            try
            {
                if (!WindowControl.SetForegroundWindow(hd))
                {
                    Thread.Sleep(50);
                    if (!WindowControl.SetForegroundWindow(hd))
                    {
                        error = "Set window to front";
                        return result;
                    }
                }
                byte expr_41 = (byte)((Keys)Enum.Parse(typeof(Keys), sKey));
                WindowControl.keybd_event(expr_41, 0, 0u, 0u);
                Thread.Sleep(20);
                WindowControl.keybd_event(expr_41, 0, 2u, 0u);
                result = true;
            }
            catch (Exception ex)
            {
                error = "error->" + ex.Message;
            }
            return result;
        }

        public bool DestroyWindow(string WinName, bool fullName, out string error)
        {
            bool flag = false;
            error = "";
            try
            {
                string lpWindowName = "";
                if (!this.GetWindowName(WinName, fullName, out lpWindowName))
                {
                    error = "Get window name fail";
                    bool result = flag;
                    return result;
                }
                IntPtr hWnd = WindowControl.FindWindowW(null, lpWindowName);
                if (hWnd.ToInt32() == 0)
                {
                    error = "find window fail";
                    bool result = flag;
                    return result;
                }
                flag = WindowControl.PostMessageW(hWnd, 16u, 0u, 0);
            }
            catch (Exception ex)
            {
                error = "error->" + ex.Message;
            }
            return flag;
        }

        public bool CloseWindow(string WinName, bool fullName, out string error)
        {
            bool flag = false;
            error = "";
            try
            {
                string lpWindowName = "";
                if (!this.GetWindowName(WinName, fullName, out lpWindowName))
                {
                    error = "Get window name fail";
                    bool result = flag;
                    return result;
                }
                IntPtr hWnd = WindowControl.FindWindowW(null, lpWindowName);
                if (hWnd.ToInt32() == 0)
                {
                    error = "find window fail";
                    bool result = flag;
                    return result;
                }
                flag = CloseWindow(hWnd);
            }
            catch (Exception ex)
            {
                error = "error->" + ex.Message;
            }
            return flag;
        }
    }
}
