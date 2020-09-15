using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Reflection;
using System.IO;
namespace RemoteTaskManage
{
    public partial class FormMain : Form
    {
        public FormMain()
        {

            ThreadPool.SetMaxThreads(1000,1000);
            InitializeComponent();

        }
        private Socket Server = null;
        private IPEndPoint ServerIP = null;
        private bool serverState=true;
        static string path = null;
        static string TextEncoding = "GB2312";
        HttpListenerDemo hd;
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Socket:9991&&&&Http:9992"))
            {
                serverState = true;
                try{
                    ServerIP = new IPEndPoint(IPAddress.Any, 9991);
                    Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Server.Bind(ServerIP);
                    Server.Listen(1000);
                    Thread listenTh = new Thread(new ThreadStart(ListenEvent));
                    listenTh.IsBackground = true;
                    listenTh.Start();
                    hd = new HttpListenerDemo();
                    hd.HttpMain();
                }catch (Exception a)
                {
                    simpleLog.Log(a.Message, "FormMain");
                }
                if (Server != null) 
                    button1.Text = "停止";
            }
            else if (button1.Text.Contains("停止"))
            {
                try
                {
                   // ServerAccept.Close();
                    serverState = false;
                    Server.Close();
                    hd.HttpStop();
                }
                catch (Exception a)
                {
                    simpleLog.Log(a.Message, "FormMain");
                }
                button1.Text = "Socket:9991&&&&Http:9992";
            }
        }

        private void ListenEvent()
        {
            while (serverState)
            {
                try
                {
                    Socket ServerAccept = Server.Accept();
                    clientThread cltr = new clientThread(ServerAccept);
                    Thread td = new Thread(new ThreadStart(cltr.Execute));
                    td.IsBackground = true;
                    td.Start();
                }catch { }   
             }
        }
        public class clientThread
        {
            private Socket socket = null;
            public clientThread(Socket socket)
            {
                this.socket = socket;
            }
            public void Execute()
            {
                try
                {
                    byte[] header = new byte[8];
                    this.socket.Receive(header);
                    int len = int.Parse(System.Text.Encoding.GetEncoding(TextEncoding).GetString(header));
                    byte[] body = new byte[len];
                    int index = 0, lentmp = len;
                    while (lentmp > index)
                    {
                        index = this.socket.Receive(body, index, lentmp, SocketFlags.None);
                        lentmp = lentmp - index;
                    }
                    string str = System.Text.Encoding.GetEncoding(TextEncoding).GetString(body);
                    reqXml rx = new reqXml(str);
                    rx.analysisXml();
                    string assemblyStr = "RemoteTaskManage." + rx.getValue("root|req|code|#text");
                    
                    Context context = new Context();

                    context.add("socket", this.socket);
                    context.add("reqXml", rx);

                    Assembly assembly = Assembly.GetExecutingAssembly();
                    dynamic obj = assembly.CreateInstance(assemblyStr);
                    obj.Execute(context);
                     
                   // byte[] msg = System.Text.Encoding.GetEncoding(TextEncoding).GetBytes(path);
                   // this.socket.Send(msg);
                }catch(Exception e){
                    simpleLog.Log(e.Message, "FormMain");
                }
                finally { this.socket.Close(); }
               
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.Visible == true)
            {
                this.notifyIcon1.Visible = true;
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.ShowInTaskbar = false;
            }
            try
            {
                hd = new HttpListenerDemo();
                hd.HttpMain();
            }
            catch (Exception a)
            {
                simpleLog.Log(a.Message, "FormMain");
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                hd.HttpStop();
            }
            catch (Exception a)
            {
                simpleLog.Log(a.Message, "FormMain");
            }
            System.Environment.Exit(0);
        }

           // System.IO.DriveInfo[] aa = System.IO.DriveInfo.GetDrives();
          // string aa1= aa[0].Name;

    }
}
