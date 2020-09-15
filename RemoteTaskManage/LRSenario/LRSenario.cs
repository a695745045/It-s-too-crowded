using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Xml;
using System.Net;
using System.Data;
using System.Diagnostics;
using System.Threading;

namespace RemoteTaskManage
{
    class LRSenario
    {
        System.Text.Encoding encoding;
        public void Execute(Context context)
        {
            encoding = (System.Text.Encoding)context.get("Encoding");

            if (((HttpListenerRequest)context.get("HttpReq")).QueryString["F_ID"] == null)
            {
                StreamWriter sw = new StreamWriter(((HttpListenerResponse)context.get("HttpRsp")).OutputStream, encoding);
                sw.Write("{\"rspCode\":\"2\",\"rspMsg\":\"Method(LRSenario) Missing Senario Unique F_ID\"}");
                ((HttpListenerResponse)context.get("HttpRsp")).Headers.Add("Content-Type", "application/json;charset=UTF-8;");
                sw.Close();
            }
            else
            {
                String F_ID = ((HttpListenerRequest)context.get("HttpReq")).QueryString["F_ID"];
                String xmlSql = "select sce_xml from pt_scene where F_ID='" + F_ID + "'";
                SqlCon con = new SqlCon();
                String xmlStr = con.conSql(xmlSql).Rows[0][0].ToString();
                List<String> fileName = new List<String>() { "ScenarioPrivateConfig", "HostChief", "TestChief", "ScenarioSchedulerConfig", "ScenarioGroupsData" };
                System.Resources.ResourceManager rc = Resources.ResourceManager;
                Dictionary<String, String> dic = new Dictionary<String, String>();
                foreach (String liststr in fileName)
                {
                    dic.Add(liststr, (String)rc.GetObject(liststr));
                };

              //  String xmlStr = @"<Scenario><ScenarioPrivateConfig><Path>c:\test.lrs</Path><Subject>test</Subject><Result_file>c:\test\test.lrr</Result_file><ResCleanName>test</ResCleanName><PlannedNumOfVusers>50</PlannedNumOfVusers></ScenarioPrivateConfig><HostChief><HostInfo><IP>8.99.1.141</IP><PLATFORM>Windows</PLATFORM></HostInfo><HostInfo><IP>8.99.1.142</IP><PLATFORM>Windows</PLATFORM></HostInfo><HostInfo><IP>8.99.1.142</IP><PLATFORM>Windows</PLATFORM></HostInfo></HostChief><TestChief><ScriptInfo><UiName>test</UiName><Path>C:\Users\Administrator\Desktop\source\test\test.usr</Path><SubType>WinSock</SubType><Config><Log><LogOptions>LogDisabled</LogOptions><MsgClassParameters>1</MsgClassParameters><MsgClassData>0</MsgClassData><MsgClassFull>0</MsgClassFull><AutoLog>1</AutoLog></Log></Config><ConfigUsp></ConfigUsp></ScriptInfo></TestChief><ScenarioSchedulerConfig><Global><Scheduling><IsDefaultScheduler>true</IsDefaultScheduler><DynamicScheduling><RampUp><StartCondition><PrevAction /></StartCondition><Batch><Count>3</Count><Interval>1</Interval></Batch><TotalVusersNumber>50</TotalVusersNumber></RampUp><Duration><StartCondition><PrevAction /></StartCondition><RunFor>780</RunFor></Duration><RampDownAll><StartCondition><PrevAction /></StartCondition><Batch><Count>3</Count><Interval>1</Interval></Batch></RampDownAll></DynamicScheduling></Scheduling></Global></ScenarioSchedulerConfig><ScenarioGroupsData><SCHED_GROUP_DATA><SCHED_GROUP_NAME>test</SCHED_GROUP_NAME><TEST_PERCENT_DISTRIBUTION>100</TEST_PERCENT_DISTRIBUTION><SCHED_USE_ANY_HOST>1</SCHED_USE_ANY_HOST><TEST_HOSTS_NAMES>8.99.1.141,8.99.9.142</TEST_HOSTS_NAMES></SCHED_GROUP_DATA></ScenarioGroupsData></Scenario>";

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(xmlStr);
                XmlElement xmlRootEl = xd.DocumentElement;
                foreach (XmlNode xm in xmlRootEl.ChildNodes)
                {
                    if (fileName.Contains(xm.Name))
                    {
                        Type t = Type.GetType("RemoteTaskManage.LRSenarioConfig");
                        MethodInfo method = t.GetMethod(xm.Name);
                        object obj = System.Activator.CreateInstance(t);
                        object[] parameters = new object[] { xm, dic[xm.Name] };
                        dic[xm.Name] = (String)method.Invoke(obj, parameters);
                    }
                }

                String Scenario = (String)rc.GetObject("Scenario");
                Regex rg = new Regex("\\<\\<(?<RepValue>.*)\\>\\>");
                MatchCollection m = rg.Matches(Scenario);
                foreach (Match item in m)
                {
                    if (fileName.Contains(item.Groups["RepValue"].Value))
                    {
                        Scenario = Scenario.Replace(item.Value, dic[item.Groups["RepValue"].Value]);
                    }
                }
                if (!Directory.Exists(Path.GetDirectoryName((xd.SelectSingleNode("/Scenario/ScenarioPrivateConfig/Path").InnerText))))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName((xd.SelectSingleNode("/Scenario/ScenarioPrivateConfig/Path").InnerText)));
                }
                FileStream fs = new FileStream(xd.SelectSingleNode("/Scenario/ScenarioPrivateConfig/Path").InnerText, FileMode.Create);
                StreamWriter sr = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
                sr.Write(Scenario);
                sr.Flush();
                sr.Close();
                fs.Close();

                if (Process.GetProcessesByName("Wlrun").Count() > 0)
                {
                    Process.GetProcessesByName("Wlrun")[0].Kill();
                }

                invokeCMD(xd.SelectSingleNode("/Scenario/ScenarioPrivateConfig/Path").InnerText);

                for (int i = 0; i <60; i++)
                {
                    if (Process.GetProcessesByName("Wlrun").Count() > 0)
                    {
                        //con.conSql("update pt_scene set sce_state=1 where F_ID='" + F_ID + "'");
                        StreamWriter sw = new StreamWriter(((HttpListenerResponse)context.get("HttpRsp")).OutputStream, encoding);
                        sw.Write("{\"rspCode\":\"0\",\"rspMsg\":\"success\"}");
                        ((HttpListenerResponse)context.get("HttpRsp")).Headers.Add("Content-Type", "application/json;charset=UTF-8;");
                        sw.Close();

                        while (true)
                        {
                            if (Process.GetProcessesByName("Wlrun").Count() > 0 )
                            {
                                Thread.Sleep(60000);
                            }
                            else
                            {
                                WebRequest req = WebRequest.Create("http://" + (string)context.get("HttpReqAddress") + ":" + (string)context.get("ServerPort") + "/API/SceneEnd?keyValue=" + F_ID);
                                WebRequest req1 = WebRequest.Create("http://" + (string)context.get("HttpReqAddress") + ":" + (string)context.get("ServerPort") + "/API/SceneDataCollectEnd?keyValue=" + F_ID + "&F_Type=success&F_Message=场景执行完毕");
                                req.Method="GET";
                                req1.Method = "GET";
                                StreamReader reqsr = null;
                                StreamReader req1sr = null;
                                try
                                {
                                    reqsr = new StreamReader(req.GetResponse().GetResponseStream(), encoding);
                                    if (!reqsr.ReadLine().Equals("success"))
                                    {
                                        simpleLog.Log("http://" + (string)context.get("HttpReqAddress") + ":" + (string)context.get("ServerPort") + "/API/SceneEnd?keyValue=" + F_ID + " response is " + reqsr.ReadLine(), "LRSenario");
                                    }
                                    req1sr = new StreamReader(req1.GetResponse().GetResponseStream(), encoding);
                                    if (!req1sr.ReadLine().Equals("success"))
                                    {
                                        simpleLog.Log("http://" + (string)context.get("HttpReqAddress") + ":" + (string)context.get("ServerPort") + "/API/SceneDataCollectEnd?keyValue=" + F_ID + "&F_Type=success&F_Message=场景执行完毕" + " response is " + reqsr.ReadLine(), "LRSenario");
                                    }
                                }catch(Exception e)
                                {
                                    simpleLog.Log(e.Message, "LRSenario");
                                }finally
                                {
                                    reqsr.Close();
                                    req1sr.Close();
                                }

                                //con.conSql("update pt_scene set sce_state=2 where F_ID='" + F_ID + "'");
                                break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                    if (i == 59)
                    {
                        StreamWriter sw = new StreamWriter(((HttpListenerResponse)context.get("HttpRsp")).OutputStream, encoding);
                        sw.Write("{\"rspCode\":\"2\",\"rspMsg\":\"process start fail\"}");
                        ((HttpListenerResponse)context.get("HttpRsp")).Headers.Add("Content-Type", "application/json;charset=UTF-8;");
                        sw.Close();
                        break;
                    }
                }
            }
        }
        public void invokeCMD(string senariooPath)
        {
                string str = Environment.GetEnvironmentVariable("LR_PATH", EnvironmentVariableTarget.Machine) + "\\bin";
                string disk=str.Substring(0,2);
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.WriteLine(disk);
                process.StandardInput.WriteLine("cd " + str);
                process.StandardInput.WriteLine("Wlrun -TestPath " + senariooPath + " -Run");
                process.StandardInput.WriteLine("exit");
                process.StandardInput.AutoFlush = true;
                process.Close();
        }
    }
}
