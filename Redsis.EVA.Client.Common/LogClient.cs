using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Common
{
    public class LogClient
    {
        //public static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetMethodFromHandle().DeclaringType);

        #region Error
        public static void Error(string message)
        {
            Task.Run(() =>
            {
                //Logger.Error(message);
            });
        }

        public static void Error(string format, params string[] message)
        {
            string msj = string.Format(format, message);
            Error(msj);
        }


        #endregion

        #region info
        public static void Info(string message)
        {
            Task.Run(() =>
            {
                //Logger.Info(message);
            });
        }

        public static void Info(string format, params string[] message)
        {
            string msj = string.Format(format, message);
            Info(msj);
        }
        #endregion

        #region Debug
        public static void Debug(string message)
        {
            Task.Run(() =>
            {
                //Logger.Debug(message);
            });
        }

        public static void Debug(string format, params string[] message)
        {
            string msj = string.Format(format, message);
            Debug(msj);
        }

        #endregion
    }
}
