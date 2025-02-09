using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WootingAnalogSDKNET;
using System.Threading;
using System.Diagnostics;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DynamicMovement
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        // instance variables
        private static readonly List<ushort> keycodes = new List<ushort> { 26, 4, 22, 7 };
        private static readonly List<string> keynames = new List<string> { "W", "A", "S", "D" };
        private static List<float> pressures = new List<float> { 0, 0, 0, 0 };
        private static bool shiftDown = false;
        private static bool ctrlDown = false;
        public static bool active = false;

        // modifiers
        const ushort VK_CTRL = 0xA2;  // Virtual key code for Ctrl
        const ushort VK_SHIFT = 0xA0; // Virtual key code for Shift



        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // write to console
            Debug.WriteLine("App started.");

            var (numDevices, error) = WootingAnalogSDK.Initialise();
            if (numDevices >= 0)
            {
                Debug.WriteLine($"Analog SDK Successfully initialised with {numDevices} devices!");

                // Get a list of the connected devices and Associated information
                var (devices, infoErr) = WootingAnalogSDK.GetConnectedDevicesInfo();
                if (infoErr != WootingAnalogResult.Ok)
                    Debug.WriteLine($"Error getting devices: {infoErr}");

                //foreach (DeviceInfo device in devices)
                //{
                //    Debug.WriteLine($"Device info has: {device}");
                //}

                ThreadPool.QueueUserWorkItem(o => runLoop());

            }
            else
            {
                Debug.WriteLine($"Analog SDK failed to initialise: {error}");
            }
        }

        private void runLoop()
        {

            while (true)
            {
                // read WASD keys
                for(int i = 0; i < keycodes.Count; i++)
                {
                    var pressure = WootingAnalogSDK.ReadAnalog(keycodes[i]);

                    if (pressure.Item2 == WootingAnalogResult.Ok)
                    {
                        pressures[i] = pressure.Item1;
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to read analog value for key {keycodes[i]}: {pressure.Item2}");
                    }

                }

                // update text on window`
                var text = $"W: {pressures[0]:F2}\tA: {pressures[1]:F2}\tS: {pressures[2]:F2}\tD: {pressures[3]:F2}";
                try
                {
                    m_window?.DispatcherQueue.TryEnqueue(() => m_window.updateText(text));
                } catch (Exception e)
                {
                    Debug.WriteLine($"Failed to update text: {e}");
                }

                // calculate movement modifiers
                var maxPressure = pressures.Max();
                // if max is below 0.3 hold down ctrl
                if (maxPressure > 0 && active)
                {
                    if (maxPressure < 0.5)
                    {
                        if (!ctrlDown)
                        {
                            // TODO: make it queue holding down ctrl in a few milliseconds, to prevent accidental presses when doing full press
                            // TODO: make it only press CTRL once a movement key is actually pressed once above is correct
                            KeyboardSimulator.PressKey(VK_CTRL);
                            ctrlDown = true;
                        }
                    }
                    else
                    {
                        if (ctrlDown)
                        {
                            KeyboardSimulator.ReleaseKey(VK_CTRL);
                            ctrlDown = false;
                        }
                    }

                    if (maxPressure > 0.98)
                    {
                        if (!shiftDown)
                        {
                            KeyboardSimulator.PressKey(VK_SHIFT);
                            shiftDown = true;
                        }
                    }
                    else
                    {
                        if (shiftDown)
                        {
                            KeyboardSimulator.ReleaseKey(VK_SHIFT);
                            shiftDown = false;
                        }
                    }
                }
                else {
                    if (ctrlDown)
                    {
                        KeyboardSimulator.ReleaseKey(VK_CTRL);
                        ctrlDown = false;
                    }

                    if (shiftDown)
                    {
                        KeyboardSimulator.ReleaseKey(VK_SHIFT);
                        shiftDown = false;
                    }
                }
                Debug.WriteLine("Shift active: " + shiftDown + "\tCtrl active: " + ctrlDown);

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private MainWindow? m_window;
    }
}
