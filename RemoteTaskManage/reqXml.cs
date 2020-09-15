using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RemoteTaskManage
{
    class reqXml
    {
        private string strXml = null;
        private Dictionary<string, string> map = new Dictionary<string, string>();
        public reqXml(string str)
        {
            this.strXml = str;
        }
        public void analysisXml()
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(this.strXml);
            XmlElement xmlRootEl =xd.DocumentElement;
            nodesPath(xmlRootEl.Name, xmlRootEl.ChildNodes);
        }
        private void nodesPath(string Name,XmlNodeList xmlnodelist)
        {
            foreach (XmlNode xm in xmlnodelist)
            {
                if (xm.HasChildNodes)
                {
                    nodesPath(Name + "|" + xm.Name, xm.ChildNodes);
                }else{
                map.Add(Name+"|"+xm.Name, xm.Value);
                }
            }
        }
        public string getValue(string key)
        {
            string value = null;
            this.map.TryGetValue(key, out value);
            return value;
        }
        public List<string> getKeys()
        {
            List<string> keys=new List<string>();
            foreach (string str in this.map.Keys)
            {
                keys.Add(str.ToString());
            }
            return keys;
        }
    }
}
