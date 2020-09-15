using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RemoteTaskManage
{
    public class SqlCon
    {
        public DataTable conSql(String sqlTmp)
        {
            try { 
            SqlConnection sqlCon = new SqlConnection();
            sqlCon.ConnectionString = "server=" + (string)HttpListenerDemo.context.get("SqlServerIP") + ";database=" + (string)HttpListenerDemo.context.get("SqlServerDatabase") +
                                      ";uid=" + (string)HttpListenerDemo.context.get("SqlServerUid") + ";pwd=" + (string)HttpListenerDemo.context.get("SqlServerPwd");
            sqlCon.Open();
            if (sqlCon.State == ConnectionState.Open)
            {
                SqlCommand sqlCom = new SqlCommand();
                sqlCom.Connection = sqlCon;
                sqlCom.CommandType = CommandType.Text;
                sqlCom.CommandText = sqlTmp;
                SqlDataAdapter adr = new SqlDataAdapter(sqlCom);         
                DataTable datatTable = new DataTable();
                adr.Fill(datatTable);
                sqlCon.Close();
                return datatTable;
            }
            else
            {
                return null;
            }
            }catch(Exception e){
                throw new Exception(e.Message);
            }
        }

        public void conSqlUpdata(String sqlTmp)
        {
            try
            {
                SqlConnection sqlCon = new SqlConnection();
                sqlCon.ConnectionString = "server=" + (string)HttpListenerDemo.context.get("SqlServerIP") + ";database=" + (string)HttpListenerDemo.context.get("SqlServerDatabase") +
                                      ";uid=" + (string)HttpListenerDemo.context.get("SqlServerUid") + ";pwd=" + (string)HttpListenerDemo.context.get("SqlServerPwd");
                sqlCon.Open();
                if (sqlCon.State == ConnectionState.Open)
                {
                    SqlCommand sqlCom = new SqlCommand();
                    sqlCom.Connection = sqlCon;
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.CommandText = sqlTmp;
                    sqlCom.ExecuteNonQuery();
                    sqlCon.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void DataTableInsert(DataTable datatable)
        {
            try
            {
                SqlBulkCopy ss = new SqlBulkCopy("server=" + (string)HttpListenerDemo.context.get("SqlServerIP") + ";database=" + (string)HttpListenerDemo.context.get("SqlServerDatabase") +
                                      ";uid=" + (string)HttpListenerDemo.context.get("SqlServerUid") + ";pwd=" + (string)HttpListenerDemo.context.get("SqlServerPwd"));
                ss.DestinationTableName = "dbo.pt_sce_result";
                ss.NotifyAfter = datatable.Rows.Count;
                ss.WriteToServer(datatable);
                ss.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}