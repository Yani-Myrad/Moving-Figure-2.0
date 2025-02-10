using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;


namespace BackgroundApp
{
    class Program
    {
        // API calls to get and set keyboard properties
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] lpKeyState);
        // Import the user32.dll to allow hiding the console window
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        

        private const uint SPI_GETKEYBOARDDELAY = 0x0016;
        private const uint SPI_GETKEYBOARDSPEED = 0x0013;
        private const uint SPI_SETKEYBOARDDELAY = 0x0017;
        private const uint SPI_SETKEYBOARDSPEED = 0x0014;

        private static uint originalDelay = 0;
        private static uint originalSpeed = 0;

        static void Main(string[] args)
        {
            IntPtr handle = GetConsoleWindow();
            int delay = args.Length > 0 && int.TryParse(args[0], out int seconds) ? seconds : 5;

            string mainAppName = "Moving Figure";  
            SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0, ref originalDelay, 0);
            SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0, ref originalSpeed, 0);
            uint newDelay = 0;
            uint newSpeed = 31;
            SystemParametersInfo(SPI_SETKEYBOARDDELAY, newDelay, ref originalDelay, 0);
            SystemParametersInfo(SPI_SETKEYBOARDSPEED, newSpeed, ref originalSpeed, 0);


            while (true)
            {
                var mainAppProcess = Process.GetProcessesByName(mainAppName);

                if (mainAppProcess.Length == 0)
                {
                    ResetKeyboardProperties();

                    Thread.Sleep(delay * 1000);

                    break; 
                }
                

                Thread.Sleep(1000);
            }

        }

        private static void ResetKeyboardProperties()
        {
            try
            {
                int resultDelay = SystemParametersInfo(SPI_SETKEYBOARDDELAY, originalDelay, ref originalDelay, 0);
                int resultSpeed = SystemParametersInfo(SPI_SETKEYBOARDSPEED, originalSpeed, ref originalSpeed, 0);

               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while resetting keyboard properties: {ex.Message}");
            }
        }
    }
}
