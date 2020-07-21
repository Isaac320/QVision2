using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace QVision.Tools
{
    class CSVManger
    {
        private static object _lock = new object();
        public static void WriteCSV(string text, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filename = "xx.csv";

            //string filename = path + "\\" + Global.LotNum + ".csv";
            //if (!File.Exists(filename))
            //{
            //    string tempss = "日期,时间,产品编号,产品名称,检测日期,标签类型,标签模板类型,机台,制造订单,检测流水号码,检测时间,检测结果,故障";
            //    File.AppendAllText(filename, tempss, Encoding.Default);
            //}
            lock (_lock)
            {
                try
                {

                    //File.AppendAllText(filename, "\n" + DateTime.Now.ToString("yyyy-MM-dd,HH:mm:ss:,") + text, Encoding.Default);
                    File.AppendAllText(filename, text + "\n", Encoding.Default);
                }
                catch
                {
                    foreach (Process process in System.Diagnostics.Process.GetProcesses())
                    {
                        if (process.ProcessName.ToUpper().Equals("EXCEL"))
                            process.Kill();
                    }
                    GC.Collect();
                    Thread.Sleep(200);
                    File.AppendAllText(filename, text, Encoding.Default);
                }
            }
        }
    }
}
