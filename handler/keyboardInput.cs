using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ezafk.handler
{
    internal class keyboardInput
    {
        public static int counter = 0;

        public static string[] keyArray = new string[] { "W", "A", "S", "D" };

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);
        [STAThread]


        public static void antiAfk()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(Data.valProcess);
                while (Data.antiafk == true)
                {

                    foreach (Process proc in processes)
                    {
                        SetForegroundWindow(proc.MainWindowHandle);
                        SendKeys.SendWait("A");
                        Thread.Sleep(300);
                        SendKeys.SendWait("A");
                        Thread.Sleep(300);
                        SendKeys.SendWait("D");
                        Thread.Sleep(300);
                        SendKeys.SendWait("D");
                        Thread.Sleep(300);
                        SendKeys.SendWait("W");
                        Thread.Sleep(300);
                        SendKeys.SendWait("S");
                        Thread.Sleep(300);
                        SendKeys.SendWait("S");
                        Thread.Sleep(300);
                        SendKeys.SendWait("W");
                    }
                    Thread.Sleep(300);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception caught when using anti afk: {ex.Message}", "ezafk");
            }
        }

        public static void randomAntiAfk()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(Data.valProcess);
                while (Data.antiafk == true)
                {
                    foreach (Process proc in processes)
                    {
                        Random random = new Random();
                        int value = random.Next(0, keyArray.Length);
                        SendKeys.SendWait(keyArray[value]);
                    }
                    Thread.Sleep(250); // make it slightly faster than the static afk function
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception caught when using random anti afk: {ex.Message}", "ezafk");
            }
        }
    }
}