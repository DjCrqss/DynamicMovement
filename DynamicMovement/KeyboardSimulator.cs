using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMovement
{
    internal class KeyboardSimulator
    {
        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);

        public static void PressKey(ushort keyCode)
        {
            keybd_event(keyCode, 0, 0, 0);
        }

        public static void ReleaseKey(ushort keyCode)
        {
            keybd_event(keyCode, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
