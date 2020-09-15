using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;

namespace RemoteTaskManage
{
    class ScreenShot
    {
        [DllImport("user32.dll")]
        static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, UInt32 nFlags);
        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowNmae);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll")]
        static extern int DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hObject);
        [DllImport("user32.dll")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        System.Text.Encoding encoding;
        public void Execute(Context context)
        {          
            if (((HttpListenerRequest)context.get("HttpReq")).QueryString["WaitTime"] != null)
            {
                string wt=((HttpListenerRequest)context.get("HttpReq")).QueryString["WaitTime"];
                int WaitTime;
                if(int.TryParse(wt, out WaitTime))
                {
                    invokeCMD();
                    System.Threading.Thread.Sleep(WaitTime);
                }
                else
                {
                    StreamWriter sw = new StreamWriter(((HttpListenerResponse)context.get("HttpRsp")).OutputStream, encoding);
                    sw.Write("{\"rspCode\":\"1\",\"rspMsg\":\"Parameter unavailable , Please use normal numbers\"}");
                    ((HttpListenerResponse)context.get("HttpRsp")).Headers.Add("Content-Type", "application/json;charset=UTF-8;");
                    sw.Close();
                }
                return;
            }
            invokeCMD();
            System.Threading.Thread.Sleep(3000);
            Bitmap bmp = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height));
            string path = Application.StartupPath + "\\ScreenShot\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
            if (!Directory.Exists(Path.GetDirectoryName(Application.StartupPath + "\\ScreenShot\\")))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Application.StartupPath + "\\ScreenShot\\"));
            }
            //FileStream file = new FileStream(path, FileMode.Create);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 25L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            ImageCodecInfo myImageCodecInfo = getImageCodecInfo("image/jpeg");
            bmp.Save(path, myImageCodecInfo, myEncoderParameters);
            g.Dispose();
            bmp.Dispose();
            //file.Close();

            FileStream file1 = new FileStream(path, FileMode.Open,FileAccess.Read);
            Stream rqstream=((HttpListenerResponse)context.get("HttpRsp")).OutputStream;
            ((HttpListenerResponse)context.get("HttpRsp")).Headers.Add("Content-Type", "application/x-www-form-urlencoded;");
            byte[] buf = new byte[file1.Length];
            int byteR = 0;
            while ((byteR = file1.Read(buf, 0, buf.Length)) != 0)
            {
                rqstream.Write(buf, 0, byteR);
            }
            rqstream.Close();
            file1.Close();
        }
        private ImageCodecInfo getImageCodecInfo(string str)
        {
            int i;
            ImageCodecInfo[] myImageCodecInfo = ImageCodecInfo.GetImageEncoders();
            for (i = 0; i < myImageCodecInfo.Length; i++)
            {
                if (myImageCodecInfo[i].MimeType == str)
                    return myImageCodecInfo[i];
            }
            return null;
        }

        private void test()
        {
            invokeCMD();
            IntPtr hWnd = FindWindow(null, "HP LoadRunner Controller - Scenario1.lrs - [Design(3)]");//HP LoadRunner Controller - Scenario1.lrs - [Design(3)]
            IntPtr mytest = GetDC(hWnd);
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);
            IntPtr hbmp = CreateCompatibleBitmap(mytest, rect.Right - rect.Left, rect.Bottom - rect.Top);
            IntPtr hmemdc = CreateCompatibleDC(mytest);
            SelectObject(hmemdc, hbmp);
            bool re = PrintWindow(hWnd, hmemdc, 0);
            Bitmap bmp = null;
             if(re)
             {
            bmp = Bitmap.FromHbitmap(hbmp);
             }
            DeleteObject(hbmp);
            DeleteDC(hmemdc);
            ReleaseDC(hWnd, mytest);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 0);
            myEncoderParameters.Param[0] = myEncoderParameter;
            ImageCodecInfo myImageCodecInfo = getImageCodecInfo("image/bmp");
            bmp.Save("D:\\CWTTEST.bmp", myImageCodecInfo, myEncoderParameters);
        }
        public void invokeCMD()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.StandardInput.WriteLine(@"@%windir%\System32\tscon.exe 0 /dest:console");
            process.StandardInput.WriteLine(@"@%windir%\System32\tscon.exe 1 /dest:console");
            process.StandardInput.WriteLine(@"@%windir%\System32\tscon.exe 2 /dest:console");
            process.StandardInput.WriteLine(@"@%windir%\System32\tscon.exe rdp-tcp#0 /dest:console");
            process.StandardInput.WriteLine(@"@%windir%\System32\tscon.exe rdp-tcp#1 /dest:console");
            process.StandardInput.WriteLine(@"@%windir%\System32\tscon.exe rdp-tcp#2 /dest:console");
            process.StandardInput.WriteLine("exit");
            process.StandardInput.AutoFlush = true;
            process.Close();
        }
    }
}
