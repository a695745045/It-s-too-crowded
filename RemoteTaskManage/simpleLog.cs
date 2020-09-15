using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace RemoteTaskManage
{
    class simpleLog
    {
        public static void Log(string content,string type)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + "\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + "\\log_" + DateTime.Now.ToString("yyyyMMdd")+".txt";
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }
            if (File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.GetEncoding("GB2312"));
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff  ") + type + "-->" + content);
                sw.Close();
            }
        }
    }
}
