using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TuniVision.SurfaceDetection.Data.DB
{

    public static class DataTableExtensions
    {

        //public static IEnumerable<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        //{
        //    DataTable dt = SqlQuery(facade, sql, parameters);
        //    return dt.ToEnumerable<T>();
        //}

        public static IEnumerable<T> ToEnumerable<T>(this DataTable dt) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    string name = p.Name;
                    if (CreateSQLStr.ExcuteSpecialColumn(p, ref name))
                        continue;

                    if (dt.Columns.IndexOf(name) != -1 && row[name] != DBNull.Value)
                    {
                        if (p.PropertyType == typeof(DateTime))
                            p.SetValue(t, DateTime.Parse(row[name].ToString()), null);
                        else if (dt.Columns[name].DataType == typeof(Boolean))
                            p.SetValue(t, bool.Parse(row[name].ToString()), null);
                        else
                            p.SetValue(t, row[name], null);
                    }

                }
                ts[i] = t;
                i++;
            }
            return ts;
        }

        //public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        //{
        //    DbCommand cmd = CreateCommand(facade, sql, out DbConnection conn, parameters);
        //    DbDataReader reader = cmd.ExecuteReader();
        //    DataTable dt = new DataTable();
        //    dt.Load(reader);
        //    reader.Close();
        //    conn.Close();
        //    return dt;
        //}

        //private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection dbConn, params object[] parameters)
        //{
        //    DbConnection conn = facade.GetDbConnection();
        //    dbConn = conn;
        //    conn.Open();
        //    DbCommand cmd = conn.CreateCommand();
        //    if (facade.IsMySql())
        //    {
        //        cmd.CommandText = sql;
        //        CombineParams(ref cmd, parameters);
        //    }
        //    return cmd;
        //}

        //private static void CombineParams(ref DbCommand command, params object[] parameters)
        //{
        //    if (parameters != null)
        //    {
        //        foreach (MySqlParameter parameter in parameters)
        //        {
        //            if (!parameter.ParameterName.Contains("@"))
        //                parameter.ParameterName = $"@{parameter.ParameterName}";
        //            command.Parameters.Add(parameter);
        //        }
        //    }
        //}
    }
}
