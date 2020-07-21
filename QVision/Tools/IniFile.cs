using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace QVision.Tools
{
    class IniFile
    {
        public string path;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder reVal, int size, string filePath);

        public IniFile(string iniPath)
        {
            path = iniPath;
        }

        public void IniWriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.path);
        }

        public string IniReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
            return temp.ToString();
        }
    }
}
