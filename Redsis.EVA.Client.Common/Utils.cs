using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Redsis.EVA.Client.Common
{
    public class Utils
    {
        public static void HideOnScreenKeyboard()
        {
            Process[] localByName = Process.GetProcessesByName("tabtip");

            if (localByName.Count() == 0)
                return;

            Process myProcess = localByName[0];
            //myProcess.CloseMainWindow();
            //myProcess.Close();
        }

        public static void KillOnScreenKeyboard()
        {
            Process myProcess = Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");
            //myProcess.CloseMainWindow();
            //myProcess.Close();
        }

        public static bool IsFileLocked(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            FileStream stream = null;
            bool isLocked = false;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                isLocked = true;
            }
            finally
            {
                if (stream != null)
                {
                    //stream.Close();
                }
            }

            return isLocked;
        }

        public static void EndTask(string taskname)
        {
            string processName = taskname;
            string fixstring = taskname.Replace(".exe", "");

            if (taskname.Contains(".exe"))
            {
                foreach (Process process in Process.GetProcessesByName(fixstring))
                {
                    process.Kill();
                }
            }
            else if (!taskname.Contains(".exe"))
            {
                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                }
            }
        }
    }
}
