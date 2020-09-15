using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace RemoteTaskManage
{
    class HttpListenerDemo
    {
        static public Context context = new Context();
        HttpListener httpLsn;
        public HttpListenerDemo()
        {
            context.add("Encoding", System.Text.Encoding.GetEncoding("UTF-8"));
            String BaseCoinfg = File.ReadAllText(Application.StartupPath + "\\config\\BaseConfig.xml", System.Text.Encoding.GetEncoding("UTF-8"));
            reqXml rx = new reqXml(BaseCoinfg);
            rx.analysisXml();
            context.add("LocalPort", rx.getValue("root|LocalHost|Port|#text"));
            context.add("ServerPort", rx.getValue("root|Server|Port|#text"));
            context.add("SqlServerIP", rx.getValue("root|SqlServer|IP|#text"));
            context.add("SqlServerDatabase", rx.getValue("root|SqlServer|Database|#text"));
            context.add("SqlServerUid", rx.getValue("root|SqlServer|Uid|#text"));
            context.add("SqlServerPwd", rx.getValue("root|SqlServer|Pwd|#text"));
        }
        public void HttpMain()
        {

            httpLsn = new HttpListener();
            httpLsn.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpLsn.Prefixes.Add("http://*:" + (string)context.get("LocalPort") + "/");
            httpLsn.Start();

            Thread thmain=new Thread(new ThreadStart(delegate
            {
                try
                {
                    loop(httpLsn);
                }
                catch (Exception)
                {
                    httpLsn.Stop();
                }
            }));
            thmain.IsBackground = true;
            thmain.Start();
        }
        public void HttpStop()
        {
            httpLsn.Stop();
        }
        private void loop(HttpListener httpLsn)
        {
            while (true)
            {

                HttpListenerContext httpContext = httpLsn.GetContext();
                Thread th=new Thread(new ThreadStart(delegate
                {                    
                    HttpListenerRequest httpReq = httpContext.Request;
                    HttpListenerResponse httpRsp = httpContext.Response;
                    
                    if (httpReq.HttpMethod == "POST")
                    {
                        onPost(httpReq, httpRsp);
                    }
                    else if (httpReq.HttpMethod == "GET")
                    {
                        onGet(httpReq, httpRsp);
                    }
                }));
                th.IsBackground = true;
                th.Start();
            }
        }
        public virtual void onGet(HttpListenerRequest request, HttpListenerResponse response)
        {           
            if (request.QueryString["method"] != null)
            {                
                try
                {
                    string assemblyStr = "RemoteTaskManage." + request.QueryString["method"];
                    Assembly assembly = Assembly.GetExecutingAssembly();

                    context.add("HttpReq", request);
                    context.add("HttpRsp", response);
                    context.add("HttpReqAddress", request.RemoteEndPoint.Address.ToString());

                    dynamic obj = assembly.CreateInstance(assemblyStr);
                    obj.Execute(context);
                }
                catch (Exception e)
                {
                    simpleLog.Log(e.Message, "HttpListenerDemo");
                    StreamWriter sw = new StreamWriter(response.OutputStream, (Encoding)context.get("Encoding"));
                    sw.Write("{\"rspCode\":\"1\",\"rspMsg\":\"" + e.Message + "\"}");
                    response.Headers.Add("Content-Type", "application/json;charset=UTF-8;");
                    sw.Close();
                }
                finally
                {
                    response.Close();
                }
            }
            else
            {
                StreamWriter sw = new StreamWriter(response.OutputStream, (Encoding)context.get("Encoding"));
                sw.Write("{\"rspCode\":\"1\",\"rspMsg\":\"Get请求错误缺少Method参数\"}");
                response.Headers.Add("Content-Type", "application/json;charset=UTF-8;");
                sw.Close();
                response.Close();
            }

        }
        public virtual void onPost(HttpListenerRequest request, HttpListenerResponse response)
        {
            StreamWriter sw = new StreamWriter(response.OutputStream, (Encoding)context.get("Encoding"));
            sw.Write("暂不支持POST请求");
            response.Headers.Add("Content-Type", "application/json;charset=UTF-8;");
            sw.Close();
            response.Close();
        }
    }
}
