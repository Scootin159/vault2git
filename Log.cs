using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vault2git
{
    class Log : IDisposable
    {
        #region Static Attributes

        private static StreamWriter logfile = null;

        #endregion

        #region Static Properties

        private static string DateStamp { get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); } }

        #endregion

        #region Public Static Methods

        public static void Info(string message)
        {
            Console.Write(DateStamp);
            Console.Write(": ");
            Console.WriteLine(message);

            Debug(message);
        }
        public static void Debug(string message)
        {
            logfile.Write(DateStamp);
            logfile.Write(": ");
            logfile.WriteLine(message);
        }

        #endregion

        #region Constructor

        public Log()
        {
            if (logfile == null)
            {
                logfile = new StreamWriter(Program.options.LogFilePath);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (logfile != null)
            {
                logfile.Close();
                logfile = null;
            }
        }

        #endregion
    }
}
