using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
//using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
namespace RemoteTaskManage
{
    class MyComputerJson
    {
        
        public void Execute(Context context)
        {
            List<Dictionary<string, object>> map = new List<Dictionary<string, object>>();
            if (((reqXml)context.get("reqXml")).getValue("root|req|method|#text").Equals("GetRemote"))
            { 
                map = GetRemote();
            }
            else if (((reqXml)context.get("reqXml")).getValue("root|req|method|#text").Equals("getFolder_children"))
            {
                map = getFolder_children(((reqXml)context.get("reqXml")).getValue("root|req|path|#text"));
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = Int32.MaxValue;
            string strjson = js.Serialize(map);

            byte[] msg = System.Text.Encoding.GetEncoding("GB2312").GetBytes(strjson);
            ((Socket)context.get("socket")).Send(msg);
        }
        public List<Dictionary<string, object>> GetRemote()
        {
            List<Dictionary<string, object>> map = new List<Dictionary<string, object>>();
            Dictionary<string, object> tmpMap = new Dictionary<string, object>();
            string ip = null;
            NetworkInterface[] nt = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nt)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet && adapter.Name.Contains("本地"))
                {
                    IPInterfaceProperties ipif = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection ipcollection = ipif.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipcollection)
                    {
                        ip = ipadd.Address.ToString();
                    }
                }
            }
            tmpMap.Add("id", ip);
            tmpMap.Add("text", "我的电脑 (" + ip + ")");
            tmpMap.Add("iconCls", "icon-computer");
            tmpMap.Add("children", GetDrive());
            tmpMap.Add("state", "closed");
            map.Add(tmpMap);
            return map;
        }

        public List<Dictionary<string, object>> GetDrive()
        {
            List<Dictionary<string, object>> Map = new List<Dictionary<string, object>>();

            foreach (DriveInfo dr in DriveInfo.GetDrives())
            {
                if(dr.DriveType==DriveType.Fixed)
                {
                    Dictionary<string, object> tmpMap = new Dictionary<string, object>();
                    tmpMap.Add("id", dr.Name);
                    if (dr.VolumeLabel != "") {
                        tmpMap.Add("text", dr.VolumeLabel+" (" + dr.Name + ")");
                    }else {
                        tmpMap.Add("text", "本地磁盘 (" + dr.Name + ")");
                    }
                    tmpMap.Add("iconCls", "icon-disk");
                    List<Dictionary<string, object>> qsh = new List<Dictionary<string, object>>();
                    Dictionary<string, object> qsh_tmp = new Dictionary<string, object>();
                    qsh_tmp.Add("text", "请稍候...");
                    qsh.Add(qsh_tmp);
                    tmpMap.Add("children", qsh);
                    tmpMap.Add("state", "closed");
                    Map.Add(tmpMap);
                }                
            }
            return Map;
        }

        public List<Dictionary<string, object>> getFolder_all(string path, List<Dictionary<string, object>> dict)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] dii = null;
            List<Dictionary<string, object>> dict_new = new  List<Dictionary<string, object>>();
            try
            {
                dii = dir.GetDirectories();
            }
            catch (Exception e)
            {
                simpleLog.Log(e.Message, "MyComputerJson");
                return null;
            }
            if (dii.Length > 0)
            {
                foreach (DirectoryInfo dd in dii)
                {
                    Dictionary<string, object> tmp = new Dictionary<string, object>();
                    tmp.Add("id", dd.FullName);
                    tmp.Add("text", dd.Name);
                    //dict.Add(tmp);
                    dict_new.Add(tmp);
                }
            }
            else
            {
                return null;
            }
            if (dict_new != null)
            {
                foreach (Dictionary<string, object> key in dict_new)
                {
                    object tt = null;
                    key.TryGetValue("id",out tt);
                    List<Dictionary<string, object>> dicr_tmp = getFolder_all((string)tt, dict);
                    if (dicr_tmp != null)
                    { key.Add("children", dicr_tmp); }
                }
            }
            dict = dict_new;
            return dict;
        }

        public List<Dictionary<string, object>> getFolder_children(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] dii = null;
            List<Dictionary<string, object>> dict_new = new List<Dictionary<string, object>>();
            try
            {
                dii = dir.GetDirectories();
            }
            catch (Exception e)
            {
                return null;
            }
            if (dii.Length > 0)
            {
                foreach (DirectoryInfo dd in dii)
                {
                    Dictionary<string, object> tmp = new Dictionary<string, object>();
                    tmp.Add("id", dd.FullName);
                    tmp.Add("text", dd.Name);
                    tmp.Add("iconCls", "icon-folder");
                    DirectoryInfo q = new DirectoryInfo(dd.FullName);
                    DirectoryInfo[] w = null;
                    int falg = 1;
                    try
                    {
                        w = q.GetDirectories();
                    }
                    catch (Exception g)
                    { falg = 0; }
                    if (falg != 0)
                    {
                        if (w.Length > 0)
                        {
                            List<Dictionary<string, object>> qsh = new List<Dictionary<string, object>>();
                            Dictionary<string, object> qsh_tmp = new Dictionary<string, object>();
                            qsh_tmp.Add("text", "请稍候...");
                            qsh.Add(qsh_tmp);
                            tmp.Add("children", qsh);
                            tmp.Add("state", "closed");
                        }
                    }

                    dict_new.Add(tmp);
                }
            }
            else
            {
                return null;
            }
            return dict_new;
        }
    }
}
/*           string hostname= Dns.GetHostName();
            IPHostEntry ipen = Dns.GetHostEntry(hostname);
            for (int i=0; i < ipen.AddressList.Length; i++)
            {
                if (ipen.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ip= ipen.AddressList[i].ToString();
                }
            }*/