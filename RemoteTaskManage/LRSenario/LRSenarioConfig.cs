using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace RemoteTaskManage
{
    class LRSenarioConfig
    {
        public String ScenarioPrivateConfig(XmlNode node, String dicStr)
        {
            foreach (XmlNode xm in node.ChildNodes)
            {
                Regex rg = new Regex("(?<=\r\n|^)" + xm.Name + "=(?<" + xm.Name + ">(.*))(?=\r\n|$)");
                Match m = rg.Match(dicStr);
                dicStr = dicStr.Replace(m.Groups[xm.Name].Value, xm.InnerText);
            }
            return dicStr;
        }
        public String HostChief(XmlNode node, String dicStr)
        {
            String str = ""; 
            foreach (XmlNode xm in node.ChildNodes)
            {
                String dic = dicStr;
                foreach (XmlNode xm_node in xm.ChildNodes)
                {
                    dic = dic.Replace("<<Scenario_HostChief_" + xm_node.Name + ">>", xm_node.InnerText);
                }
                str = str + dic ;
            }
            str = str.Substring(0, str.Length - 2);
            return str;
        }
        public String TestChief(XmlNode node, String dicStr)
        {
            String str = "";
            foreach (XmlNode xm in node.ChildNodes)
            {
                String dic = dicStr;
                foreach (XmlNode xm_node in xm.ChildNodes)
                {
                    if (xm_node.Name.Equals("Config"))
                    {
                        if (File.Exists(xm.SelectSingleNode("Path").InnerText))
                        {
                            dic = dic.Replace("<<Scenario_TestChief_" + xm_node.Name + ">>", readScriptCfg(xm.SelectSingleNode("Path").InnerText, xm_node));
                        }

                    }
                    else if (xm_node.Name.Equals("ConfigUsp"))
                    {
                        if (File.Exists(xm.SelectSingleNode("Path").InnerText))
                        {
                            dic = dic.Replace("<<Scenario_TestChief_" + xm_node.Name + ">>", readScriptUsp(xm.SelectSingleNode("Path").InnerText, xm_node));
                        }
                    }
                    else
                    {
                        dic = dic.Replace("<<Scenario_TestChief_" + xm_node.Name + ">>", xm_node.InnerText);
                    }                    
                }
                str = str + dic;
            }
            str = str.Substring(0, str.Length - 2);
            return str;
        }
        public String readScriptCfg(String path, XmlNode xm_node)
        {
            String dirPath = Path.GetDirectoryName(path);
            FileStream fs = new FileStream(dirPath + "\\default.cfg", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            String cfgStr=sr.ReadToEnd();
            sr.Close();
            fs.Close();
            foreach (XmlNode xm in xm_node.ChildNodes)
            {
                foreach (XmlNode xm_new in xm.ChildNodes)
                {
                    Regex rg = new Regex("(?<=\r\n|^)" + xm_new.Name + "=(?<" + xm_new.Name + ">(.*))(?=\r\n|$)");
                    Match m = rg.Match(cfgStr);
                    if (m.Length == 0)
                    {
                        cfgStr = cfgStr.Replace("[" + xm.Name + "]", "[" + xm.Name + "]" + "\r\n" + xm_new.Name + "=" + xm_new.InnerText);
                    }
                    else
                    {
                        cfgStr=rg.Replace(cfgStr, xm_new.Name + "=" + xm_new.InnerText);
                    }
                }
            }
            Regex at = new Regex("(?<=\r\n|^)AutomaticTransactions=(.*)(?=\r\n|$)");
            Match m1 = at.Match(cfgStr);
            if (m1.Length == 0)
            {
                cfgStr = cfgStr.Replace("[General]", "[General]\r\nAutomaticTransactions=0");
            }

            Regex rgtmp = new Regex("(\r\n)+");
            cfgStr = rgtmp.Replace(cfgStr, "\\r\\n");
            return cfgStr;
        }
        public String readScriptUsp(String path, XmlNode xm_node)
        {
            String dirPath = Path.GetDirectoryName(path);
            FileStream fs = new FileStream(dirPath + "\\default.usp", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            String cfgStr = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            //foreach (XmlNode xm in xm_node.ChildNodes)
            //{
            //    foreach (XmlNode xm_new in xm.ChildNodes)
            //    {
            //        Regex rg = new Regex("(?<=\r\n|^)" + xm_new.Name + "=(?<" + xm_new.Name + ">(.*))(?=\r\n|$)");
            //        Match m = rg.Match(cfgStr);
            //        if (m.Length == 0)
            //        {
            //            cfgStr = cfgStr.Replace("[" + xm.Name + "]", "[" + xm.Name + "]" + "\r\n" + xm_new.Name + "=" + xm_new.InnerText);
            //        }
            //        else
            //        {
            //            cfgStr = rg.Replace(cfgStr, xm_new.Name + "=" + xm_new.InnerText);
            //        }
            //    }
            //}
            Regex rgtmp = new Regex("(\r\n)+");
            cfgStr = rgtmp.Replace(cfgStr, "\\r\\n");
            return cfgStr;
        }
        public String ScenarioSchedulerConfig(XmlNode node, String dicStr)
        {
            String str = dicStr.Replace("<<DynamicScheduling>>", getXmlString(node));

            return str;
        }
        public String getXmlString(XmlNode node)
        {
            String str="";
            foreach (XmlNode xm in node.ChildNodes)
            {
                if (xm.Name.Equals("DynamicScheduling"))
                {
                    str = xm.OuterXml;
                    break;
                }
                else
                {
                    str = getXmlString(xm);
                }             
            }
            return str;
        }
        public String ScenarioGroupsData(XmlNode node, String dicStr)
        {
            String str = ""; 
            foreach (XmlNode xm in node.ChildNodes)
            {
                String dic = dicStr;
                foreach (XmlNode xm_node in xm.ChildNodes)
                {
                    Regex rg = new Regex("(?<=\r\n|^)" + xm_node.Name + "=(?<" + xm_node.Name + ">(.*))(?=\r\n|$)");
                    dic = rg.Replace(dic, xm_node.Name + "=" + xm_node.InnerText);
                }
                str = str + dic;
            }
            str = str.Substring(0, str.Length - 2);
            return str;
        }
    }
}
