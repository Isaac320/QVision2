using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace QVision.Tools
{
    class SerPort
    {
        static SerialPort sp = new SerialPort();


        static private void OpenPort()
        {
            sp.PortName = "com2";
            sp.BaudRate = 19200;
            sp.DataBits = 8;
            sp.StopBits = (StopBits)1;
            sp.Open();
        }

        static private void ClosePort()
        {
            sp.Close();
        }

        static public void SendData(string s)
        {
            if(!sp.IsOpen)
            {
                OpenPort();
            }
            sp.Write(s);
        }
    }
}
