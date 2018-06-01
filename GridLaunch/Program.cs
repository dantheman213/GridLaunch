using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Runtime.InteropServices;

namespace GridLaunch
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;


        private const string EXECUTABLE_JAR = "youtubegrid.jar";

        private static string getScreensaverPath()
        {
            string result = null;

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\YouTubeGrid"))
                {
                    if (key != null)
                    {
                        result = (String)key.GetValue("SCREEN_SAVER_PATH");
                    }
                }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                //react appropriately
            }

            return result; 
        }

        private static string getJavaInstallationPath()
        {
            string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentPath))
            {
                return environmentPath;
            }

            string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";
            using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(javaKey))
            {
                string currentVersion = rk.GetValue("CurrentVersion").ToString();
                using (Microsoft.Win32.RegistryKey key = rk.OpenSubKey(currentVersion))
                {
                    return key.GetValue("JavaHome").ToString();
                }
            }
        }

        private static void launchScreensaver(String appPath, String arg)
        {
            string javaBin = getJavaInstallationPath() + "\\bin\\java.exe";
            string jarPath = appPath + "\\" + EXECUTABLE_JAR;


            Process proc = new Process();
            proc.StartInfo.FileName = "java";
            proc.StartInfo.Arguments = "-jar " + jarPath + arg;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            proc.Start();
            proc.WaitForExit();
        }

        private static void hideConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }

        static void Main(string[] args)
        {
            hideConsole();

            string appPath = getScreensaverPath();
            if(!String.IsNullOrEmpty(appPath))
            {
                string arg = "";

                if(args != null && args.Length > 0)
                {
                    arg = " " + args[0];
                }

                launchScreensaver(appPath, arg);
            }
        }
    }
}
