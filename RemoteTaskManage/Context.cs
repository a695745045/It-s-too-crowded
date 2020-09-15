using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteTaskManage
{
    class Context
    {
        private Dictionary<string, object> dict = new Dictionary<string, object>();

        public void add(string key,object value)
        {
            if (this.dict.Keys.Contains(key))
            {
                this.dict.Remove(key);
            }
            this.dict.Add(key, value);
        }

        public object get(string key)
        {
            object value = null;
            this.dict.TryGetValue(key, out value);
            return value;
        }
    }
}
