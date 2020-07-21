using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QVision.Tools
{
    class LogManager
    {
        public static object locker = new object();
        public static void WriteLog(string strLog)
        {
            string sFilePath = Application.StartupPath + "\\Log\\";
            string sFileName = "日志" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            sFileName = sFilePath + sFileName;
            if (!Directory.Exists(sFileName))
            {
                Directory.CreateDirectory(sFilePath);
            }
            StreamWriter sw = null;
            lock (locker)
            {
                using (sw = File.AppendText(sFileName))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: " + strLog));
                }

            }


        }
    }
}
