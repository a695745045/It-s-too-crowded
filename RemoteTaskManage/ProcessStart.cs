using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace RemoteTaskManage
{
    class ProcessStart
    {
        public void Execute(Context context)
        {
            try
            {
                System.Diagnostics.Process listps = System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft Expression\Encoder 4\EEScreen.exe");
                
            }
            catch (Exception e)
            {
                simpleLog.Log(e.Message, "ProcessStart");
            }
            
        }
    }
}
