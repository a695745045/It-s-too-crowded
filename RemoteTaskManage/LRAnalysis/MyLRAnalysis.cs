using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;

using Analysis.Api;
using Analysis.ApiLib;
using System.Globalization;
using Analysis.Api.Dictionaries;
using System.IO;
using Analysis.ApiLib.Dimensions;
using Analysis.ApiLib.Sla;

namespace RemoteTaskManage
{
    class MyLRAnalysis
    {
        System.Text.Encoding encoding;
        //main object
        private LrAnalysis _analysisApi;
        //next two fields are references to corresponding objects in _analysisApi for easier access
        private Session _currentSession;
        private Run _currentRun;
        //variable for storing current graph's name
        private string _lastGraph = null;
        //constants for comparing usability
        private const int _EmptyGraph = 5;
        private const int _ExcludedGraph = 4;
        //constant for run number
        private const int _usedRun = 0;
        //template manager
        private TemplateManager _tManager = null;
        //SLA object
        SlaResult _slaResult;

        private static int granularity = 16;
        DataTable datatable = null;

        public MyLRAnalysis()
        {
            _analysisApi = new LrAnalysis(); 
            //setup "Silent Mode"
            _analysisApi.RunTimeErrors.IsSilentMode = true;
            _analysisApi.Exporters.CSV.ExportSettings.FractionalLength = 8;
            _tManager = ApiGlobal.GetInstance().Templates;
            //setup Optimization Mode
            _analysisApi.DefaultOptimizationMode = DataOptimization.Performance;
            _analysisApi.Options.ResultCollection.DataSource = ResultCollectionSettings.DataSourceParsingStyle.GenerateSummaryDataOnly;
            InitDataTable();
        }
        public void Execute(Context context)
        {
            encoding = (System.Text.Encoding)context.get("Encoding");

            if (((HttpListenerRequest)context.get("HttpReq")).QueryString["F_SceId"] == null || ((HttpListenerRequest)context.get("HttpReq")).QueryString["F_RunId"] == null)
            {
                StreamWriter sw = new StreamWriter(((HttpListenerResponse)context.get("HttpRsp")).OutputStream, encoding);
                sw.Write("{\"rspCode\":\"2\",\"rspMsg\":\"Method(LRSenario) Missing Senario Unique F_SceId or F_RunId\"}");
                ((HttpListenerResponse)context.get("HttpRsp")).Headers.Add("Content-Type", "application/json;charset=UTF-8;");
                sw.Close();
            }
            else
            {
                String F_SceId = ((HttpListenerRequest)context.get("HttpReq")).QueryString["F_SceId"];
                String F_RunId = ((HttpListenerRequest)context.get("HttpReq")).QueryString["F_RunId"];
                //String xmlSql = "select sce_result_dir,sce_result_file from pt_scene where F_ID='" + F_SceId + "'";
                String xmlSql = "select sce_result_dir,sce_result_file from pt_sce_his where F_SceId='" + F_SceId + "'" + " and F_Id='" + F_RunId + "'";
                SqlCon con = new SqlCon();
                DataTable temp= con.conSql(xmlSql);
                String sce_result_dir_Str = temp.Rows[0][0].ToString();
                String sce_result_file_Str = temp.Rows[0][1].ToString();
                String res_name = sce_result_dir_Str + "\\" + sce_result_file_Str + "\\" + sce_result_file_Str + ".lrr";

                StreamWriter sw = new StreamWriter(((HttpListenerResponse)context.get("HttpRsp")).OutputStream, encoding);
                sw.Write("{\"rspCode\":\"0\",\"rspMsg\":\"数据采集中\"}");
                ((HttpListenerResponse)context.get("HttpRsp")).Headers.Add("Content-Type", "application/json;charset=UTF-8;");
                sw.Close();

                if (CreateSession(sce_result_dir_Str + "\\" + sce_result_file_Str + "\\SessionLra\\" + sce_result_file_Str + ".lra", res_name))
                {
                    insertData(F_SceId, F_RunId);
                    CloseSession();
                    WebRequest req = WebRequest.Create("http://" + (string)context.get("HttpReqAddress") + ":" + (string)context.get("ServerPort") + "/API/SceneDataCollectEnd?keyValue=" + F_SceId + "&F_Type=success&F_Message=");
                    req.Method = "GET";
                    StreamReader reqsr = null;
                    try
                    {
                        reqsr = new StreamReader(req.GetResponse().GetResponseStream(), encoding);
                        if (!reqsr.ReadLine().Equals("success"))
                        {
                            simpleLog.Log("http://" + (string)context.get("HttpReqAddress") + ":" + (string)context.get("ServerPort") + "/API/SceneDataCollectEnd?keyValue=" + F_SceId + "&F_Type=success&F_Message=" + " response is " + reqsr.ReadLine(), "LRSenario");
                        }
                    }
                    catch (Exception e)
                    {
                        simpleLog.Log(e.Message, "MyLRAnalysis");
                    }
                    finally
                    {
                        reqsr.Close();
                    }
                }
                else
                {
                    WebRequest req = WebRequest.Create("http://" + (string)context.get("HttpReqAddress") + ":" + (string)context.get("ServerPort") + "/API/SceneDataCollectEnd?keyValue=" + F_SceId + "&F_Type=error&F_Message=OpenSessionFail");
                    req.Method = "GET";
                    StreamReader reqsr = null;
                    try
                    {
                        reqsr = new StreamReader(req.GetResponse().GetResponseStream(), encoding);
                        if (!reqsr.ReadLine().Equals("success"))
                        {
                            simpleLog.Log("http://" + (string)context.get("HttpReqAddress") + ":" + (string)context.get("ServerPort") + "/API/SceneDataCollectEnd?keyValue=" + F_SceId + "&F_Type=error&F_Message=OpenSessionFail" + " response is " + reqsr.ReadLine(), "LRSenario");
                        }
                    }
                    catch (Exception e)
                    {
                        simpleLog.Log(e.Message, "MyLRAnalysis");
                    }
                    finally
                    {
                        reqsr.Close();
                    }
                }
                
            }
        }
        private bool OpenSession(string file_name)
        {
            bool result = _analysisApi.Session.Open(file_name);
            if (result)
            {
                //get references for easy access
                _currentSession = _analysisApi.Session;
                _currentRun = _currentSession.Runs[_usedRun];
                int Points24 =(int)((_currentRun.LrLocalEndTime - _currentRun.LrLocalStartTime).TotalSeconds/24);
                if (Points24>1800)
                {
                    granularity = 3600;
                }
                else
                {
                    granularity = Points24;
                }                
               // if ((_currentRun.LrLocalEndTime - _currentRun.LrLocalStartTime).TotalHours >= 24)
               // {
               //     granularity = 3600;
               // }   
            }
            else
            {
                //show error message
                simpleLog.Log(_analysisApi.RunTimeErrors.LastErrorMessage, "MyLRAnalysis.Class.OpenSession.Method");
            }
            return result;
        }

        private bool CreateSession(string session_name, string res_name)
        {
            bool result = _analysisApi.Session.Create(session_name, res_name);
            if (result)
            {
                //get references for easy access
                _currentSession = _analysisApi.Session;
                _currentRun = _currentSession.Runs[_usedRun];
            }
            else
            {
                if (_analysisApi.RunTimeErrors.LastErrorMessage.Equals("Failed to apply template."))
                {
                    OpenSession(session_name);
                    result = true;
                }
                else
                {
                    //set all to null
                    _currentSession = null;
                    _currentRun = null;
                    //show error message
                    simpleLog.Log(_analysisApi.RunTimeErrors.LastErrorMessage, "MyLRAnalysis.Class.OpenSession.Method");
                }
            }
            return result;
        }

        private bool CloseSession()
        {
            bool result = _analysisApi.Session.Close();
            _lastGraph = String.Empty;
            _currentSession = null;
            _currentRun = null;
            return result;
        }

        private void ApplyGlobalFilter(double min, double max,bool think_time=false)
        {
            if (!_currentSession.IsOpenedOrCreated)
                return;

            //check if the values are correct
            if (_currentSession.GlobalFilter.ScenarioElapsedTime.AvailableValues.CheckContinuousValue(String.Empty, min, max))
            {
                _currentSession.GlobalFilter.IncludeThinkTime = think_time;
                _currentSession.GlobalFilter.ScenarioElapsedTime.ClearValues();

                _currentSession.GlobalFilter.ScenarioElapsedTime.AddContinuousValue(min, max);

                //apply global filter
                _currentSession.GlobalFilter.ApplyFilter();
            }
            else
                simpleLog.Log("Value out of range", "MyLRAnalysis.Class.ApplyGlobalFilter.Method");
        }

        private void SetGranularity(int outGranularity = 0)
        {
            if (outGranularity > 0)
            {
                granularity = outGranularity;
            }
            else
            {
                simpleLog.Log("Granularity must greater than 0 ", "MyLRAnalysis.Class.SetGranularity.Method");
            }
        }
        public void GetGraphData(string graph_name, string series_name, bool use_iterator)
        {
            if (_currentSession.IsOpenedOrCreated == false)
                return;

            Graph current_graph = null;
            GraphsList g_list = _currentRun.Graphs;
            SeriesList current_serieslist = null;

            
            current_graph = g_list[graph_name];
            current_serieslist = current_graph.Series;

            foreach(Series current_series in current_serieslist)
            {
                foreach (SeriesPoint point in current_series)
                {

                }
            }

        }

        public void InitDataTable()
        {
            datatable = new DataTable("dbo.pt_sce_result");
            datatable.Columns.Add("F_Id", typeof(String));
            datatable.Columns.Add("F_SceId", typeof(String));
            datatable.Columns.Add("F_RunId", typeof(String));
            datatable.Columns.Add("trans_name", typeof(String));
            datatable.Columns.Add("relative_time", typeof(String));
            datatable.Columns.Add("tps", typeof(Decimal));
            datatable.Columns.Add("total_tps", typeof(Decimal));
            datatable.Columns.Add("response_time", typeof(Decimal));
            datatable.Columns.Add("throughput_MB", typeof(Decimal));
            datatable.Columns.Add("summary_success", typeof(Int32));
            datatable.Columns.Add("summary_fail", typeof(Int32));
            datatable.Columns.Add("createdate", typeof(DateTime));
        }
        private string DecodeTime(double d_time)
        {
            int hours, minutes, seconds;
            int time = (int)Math.Ceiling(d_time);

            hours = time / 3600;
            time = time - (hours * 3600);
            minutes = time / 60;
            seconds = time % 60;

            //result string 00:00:00
            return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        public void insertData(String F_SceId, String F_RunId)
        {
            Dictionary<int, string[]> dic = new Dictionary<int, string[]>();
            string[] tmp=new string[12];
            Graph TransactionsPerSecond_graph = _currentRun.Graphs["TransactionsPerSecond"];
            TransactionsPerSecond_graph.Granularity = granularity;
            Graph ResponseTime_graph = _currentRun.Graphs["ResponseTime"];
            ResponseTime_graph.Granularity = granularity;
            //Graph ThroughputMB_graph = _currentRun.Graphs["ThroughputMB"];
            Graph TotalTransactionsPass_graph = _currentRun.Graphs["TotalTransactionsPass"];
            TotalTransactionsPass_graph.Granularity = granularity;
            Graph TransactionSummary_graph = _currentRun.Graphs["TransactionSummary"];
            SeriesList TransactionsPerSecond_serieslist  = TransactionsPerSecond_graph.Series;
            foreach (Series TransactionsPerSecond_series in TransactionsPerSecond_serieslist)
            {
                SeriesPoint[] sp_TransactionsPerSecond = TransactionsPerSecond_series.Points.ToArray();
                Series ResponseTime_Series = ResponseTime_graph.Series[TransactionsPerSecond_series.Name];
                SeriesPoint[] sp_ResponseTime = ResponseTime_Series.Points.ToArray();
                for (int ii = 0; ii < TransactionsPerSecond_series.Points.Count;ii++ )
                {
                    if (ii >= ResponseTime_Series.Points.Count)
                    {
                        datatable.Rows.Add(null, F_SceId, F_RunId, TransactionsPerSecond_series.Name, DecodeTime(sp_TransactionsPerSecond[ii].RelativeTime), sp_TransactionsPerSecond[ii].Value, null, null, null, null, null, null);
                    }
                    else
                    {
                        datatable.Rows.Add(null, F_SceId, F_RunId, TransactionsPerSecond_series.Name, DecodeTime(sp_TransactionsPerSecond[ii].RelativeTime), sp_TransactionsPerSecond[ii].Value, null, sp_ResponseTime[ii].Value, null, null, null, null);
                    }
                }        
            }

            Series TotalTransactionsPass_Series = TotalTransactionsPass_graph.Series["Pass"];
            SeriesPoint[] sp_TotalTransactionsPass = TotalTransactionsPass_Series.Points.ToArray();
            for (int ii = 0; ii < TotalTransactionsPass_Series.Points.Count; ii++)
            {
                datatable.Rows.Add(null, F_SceId, F_RunId, "Pass", DecodeTime(sp_TotalTransactionsPass[ii].RelativeTime), null, sp_TotalTransactionsPass[ii].Value, null, null, null, null, null);
            }

            SeriesList TransactionSummary_serieslist = TransactionSummary_graph.Series;
            foreach (Series TransactionSummary_series in TransactionSummary_serieslist)
            {
                String[] seriesName= TransactionSummary_series.Name.Split(':');
                DataRow[] dr=datatable.Select("trans_name='" + seriesName[0] + "'" );
                SeriesPoint[] sp_TransactionSummary = TransactionSummary_series.Points.ToArray();
                if (dr.Count() == 0)
                    continue;
                switch (seriesName[1])
                {
                    case "Fail":
                        dr[0]["summary_fail"] = sp_TransactionSummary[0].Value;
                        break;
                    case "Pass":
                        dr[0]["summary_success"] = sp_TransactionSummary[0].Value;
                        break;
                    default:
                        break;
                }
            }
            SqlCon sqlcon = new SqlCon();
            sqlcon.DataTableInsert(datatable);
        }
    }
}