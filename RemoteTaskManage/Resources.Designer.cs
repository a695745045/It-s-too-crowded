﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18408
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace RemoteTaskManage {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RemoteTaskManage.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 {&lt;&lt;Scenario_HostChief_IP&gt;&gt;
        ///PLATFORM=&lt;&lt;Scenario_HostChief_PLATFORM&gt;&gt;
        ///IS_REMOTE_LOGIN=0
        ///REMOTE_LOGIN=
        ///IS_LOCAL_LOGIN=0
        ///LOCAL_LOGIN=
        ///DONT_USE_RSH=0
        ///REMOTE_TMP_DIR=
        ///MAX_VUSER_XL=5000
        ///MAX_VUSER_RTE=1000
        ///ALLOW_XL=1
        ///ALLOW_RTE=1
        ///MAX_VUSER_RUNNER=1
        ///LOAD_QUOTA_GLOBAL=50
        ///KILL_QUOTA_GLOBAL=50
        ///Enable=1
        ///RUNNER_FILE_TYPE=0
        ///RUNNER_FILE=
        ///NFS_LESS_MODE=1
        ///DEFAULT_SCENARIO_STORAGE_FLAG=1
        ///USE_FW_MONITOR=0
        ///ENABLE_FIREWALL=0
        ///ENABLE_SSL=0
        ///ROUTER_NAME=
        ///HOST_WAN_EMULATION_SETTINGS=HOST_WAN_EMULATION_SETTINGS        /// [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string HostChief {
            get {
                return ResourceManager.GetString("HostChief", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 
        ///{Product
        ///Product=HP LoadRunner Controller
        ///Version=11.0.0.0
        ///}
        ///
        ///{ScenarioGeneralConfig
        ///ScenarioType=2
        ///Is_mix=1
        ///Is_runner_file=0
        ///Runner_file=
        ///Host_kill_type=0
        ///GlobalDir=
        ///}
        ///
        ///&lt;&lt;ScenarioPrivateConfig&gt;&gt;
        ///
        ///{ScenarioCommonConfig
        ///GlobalNlq=999
        ///GlobalNls=999
        ///HostUnixNLQ=5
        ///HostWinNLQ=5
        ///EchoDlgRefresh=1000
        ///EchoMaxOpen=10
        ///VuRunTO=120
        ///VuPauseTO=120
        ///VuQuitTO=120
        ///}
        ///
        ///{ScenarioIniFlags
        ///2:DeleteQuotaOfOutputMessages=2000
        ///3:FlagLimitOutputMessages=1
        ///4:FlagResetLog=0
        ///5:OutputIsExported=1
        ///6:Outpu [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string Scenario {
            get {
                return ResourceManager.GetString("Scenario", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {SCHED_GROUP_DATA
        ///SCHED_GROUP_NAME=&lt;&lt;SCHED_GROUP_NAME&gt;&gt;
        ///TEST_PERCENT_DISTRIBUTION=&lt;&lt;TEST_PERCENT_DISTRIBUTION&gt;&gt;
        ///SCHED_USE_ANY_HOST=&lt;&lt;SCHED_USE_ANY_HOST&gt;&gt;
        ///TEST_HOSTS_NAMES=
        ///}
        ///
        /// 的本地化字符串。
        /// </summary>
        internal static string ScenarioGroupsData {
            get {
                return ResourceManager.GetString("ScenarioGroupsData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {ScenarioPrivateConfig
        ///Path=&lt;&lt;Scenario_ScenarioPrivateConfig_Path&gt;&gt;
        ///Author=Administrator
        ///Subject=&lt;&lt;Scenario_ScenarioPrivateConfig_Subject&gt;&gt;
        ///Description=
        ///Hosts=5
        ///Scripts=2
        ///Groups=2
        ///Vusers=75
        ///Tranzactions=31
        ///Rendezvous=0
        ///Result_file=&lt;&lt;Scenario_ScenarioPrivateConfig_Result_file&gt;&gt;
        ///ResCleanName=&lt;&lt;Scenario_ScenarioPrivateConfig_ResCleanName&gt;&gt;
        ///ResSymName=
        ///DEFAULT_NFS_LESS_MODE=1
        ///AutoSetResults=0
        ///AutoOverwriteResults=0
        ///BaseResultsName=
        ///MaxVuserSeedNumberUsed=0
        ///PlannedNumOfVusers=&lt;&lt;Scenario_Scenar [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string ScenarioPrivateConfig {
            get {
                return ResourceManager.GetString("ScenarioPrivateConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {ScenarioSchedulerConfig
        ///&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-16&quot;?&gt;
        ///&lt;LoadTest&gt;
        ///  &lt;Schedulers&gt;
        ///    &lt;CurrentSchedulerId&gt;1&lt;/CurrentSchedulerId&gt;
        ///    &lt;StartMode&gt;
        ///      &lt;StartModeType&gt;Immediately&lt;/StartModeType&gt;
        ///      &lt;StartModes&gt;
        ///        &lt;Immediately /&gt;
        ///        &lt;DelayAfterLTStart&gt;0&lt;/DelayAfterLTStart&gt;
        ///        &lt;StartAt&gt;2018-07-02T18:25:24.515625+08:00&lt;/StartAt&gt;
        ///      &lt;/StartModes&gt;
        ///    &lt;/StartMode&gt;
        ///    &lt;Scheduler ID=&quot;1&quot;&gt;
        ///      &lt;Name&gt;Schedule 1&lt;/Name&gt;
        ///      &lt;Manual&gt;
        ///        &lt;SchedulerType&gt;Global&lt;/Sched [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string ScenarioSchedulerConfig {
            get {
                return ResourceManager.GetString("ScenarioSchedulerConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {&lt;&lt;Scenario_TestChief_UiName&gt;&gt;
        ///UiName=&lt;&lt;Scenario_TestChief_UiName&gt;&gt;
        ///Type=1
        ///SubType=&lt;&lt;Scenario_TestChief_SubType&gt;&gt;
        ///Path=&lt;&lt;Scenario_TestChief_Path&gt;&gt;
        ///Config=&lt;&lt;Scenario_TestChief_Config&gt;&gt;
        ///ConfigUsp=&lt;&lt;Scenario_TestChief_ConfigUsp&gt;&gt;
        ///Param=
        ///TDPath=
        ///TDServer=
        ///TDDatabase=
        ///Platform=All
        ///AstraSubType=0
        ///}
        ///
        /// 的本地化字符串。
        /// </summary>
        internal static string TestChief {
            get {
                return ResourceManager.GetString("TestChief", resourceCulture);
            }
        }
    }
}
