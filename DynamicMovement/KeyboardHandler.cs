using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyboardHookLite;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Diagnostics;

namespace DynamicMovement
{
    internal class KeyboardHandler
    {
        private static KeyboardHook kb;
        public KeyboardHandler() {
            kb = new KeyboardHook();
            Debug.WriteLine("Keyboard handler initialized.");
            kb.KeyboardPressed += Kb_KeyboardPressed;
        }

        public static void Dispose()
        {
            Debug.WriteLine("Keyboard handler disposed.");
            kb.Dispose();
        }

        private static void Kb_KeyboardPressed(object? sender, KeyboardHookEventArgs e)
        {
            Debug.WriteLine("Key pressed: " + e.InputEvent.Key);
        }
    }
}
