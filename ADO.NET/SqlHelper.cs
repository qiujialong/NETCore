using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ReCheckWebSite.Models;
using TuniVision.SurfaceDetection.Data.DB;

namespace ReCheckWebSite.ADO.NET
{
    /// <summary> 
    /// 针对SQL Server数据库操作的通用类 
    /// </summary> 
    public class SqlDbHelper
    {
        private string connectionString;
        /// <summary> 
        /// 设置数据库连接字符串 
        /// </summary> 
        public string ConnectionString
        {
            set { connectionString = value; }
        }

        public SqlDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="connectionString">数据库连接字符串</param> 
        public SqlDbHelper()
        {
        }

        #region MYSQL search by DataAdapter return datatable

        /// <summary> 
        /// 执行一个查询，并返回结果集 
        /// </summary> 
        /// <param name="sql">要执行的查询SQL文本命令</param> 
        /// <returns>返回查询结果集</returns> 
        public DataTable MySQLExecuteDataTable(string sql)
        {
            return MySQLExecuteDataTable(sql, CommandType.Text, null);
        }

        /// <summary> 
        /// 执行一个查询,并返回查询结果 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <returns>返回查询结果集</returns> 
        public DataTable MySQLExecuteDataTable(string sql, CommandType commandType)
        {
            return MySQLExecuteDataTable(sql, commandType, null);
        }

        /// <summary> 
        /// 执行一个查询,并返回查询结果 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <returns></returns> 
        public DataTable MySQLExecuteDataTable(string sql, CommandType commandType, MySqlParameter[] parameters)
        {
            DataTable data = new DataTable();//实例化DataTable，用于装载查询结果集 
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType 
                                                      //如果同时传入了参数，则添加这些参数 
                    if (parameters != null)
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    //通过包含查询SQL的SqlCommand实例来实例化SqlDataAdapter 
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                    adapter.Fill(data);//填充DataTable 
                }
            }
            return data;
        }

        public IEnumerable<T> ExecuteDataTableByKey<T>(string id) where T : class, new()
        {
            T t = CreateSQLStr.CreateEntityByKey<T>(id);
            string sql = CreateSQLStr.SelectSQLStr<T>(t);
            return ExecuteDataTable<T>(sql, CommandType.Text, null);
        }

        /// <summary> 
        /// 执行一个查询,并返回查询结果 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <returns></returns> 
        public IEnumerable<T> ExecuteDataTable<T>(string sql, CommandType commandType, MySqlParameter[] parameters) where T : class, new()
        {
            DataTable data = new DataTable();//实例化DataTable，用于装载查询结果集 
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType 
                                                      //如果同时传入了参数，则添加这些参数 
                    if (parameters != null)
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    //通过包含查询SQL的SqlCommand实例来实例化SqlDataAdapter 
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                    adapter.Fill(data);//填充DataTable 
                }
            }
            return data.ToEnumerable<T>();
        }

        public bool AddOrUpdate<T>(T entity) where T : class
        {
            if (typeof(T) == typeof(FlawInfoFeedBack))
            {
                try
                {
                    FlawInfoFeedBack record = entity as FlawInfoFeedBack;

                    Task.Run(() =>
                    {
                        lock (entity)
                        {
                            bool exist = ExecuteDataTableByKey<FlawInfoFeedBack>(record.ID).Count() > 0;
                            if (exist)
                            {
                                var sql = CreateSQLStr.UpdateSQLStr(entity);
                                return ExecuteNonQuery(sql) > 0;
                            }
                            else
                            {
                                var sql = CreateSQLStr.SaveSQLStr(entity);
                                return ExecuteNonQuery(sql) > 0;
                            }
                        }
                    });
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }

        /// <summary> 
        /// 对数据库执行增删改操作 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string sql, CommandType commandType)
        {
            return ExecuteNonQuery(sql, commandType, null);
        }

        /// <summary> 
        /// 对数据库执行增删改操作 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string sql, CommandType commandType, MySqlParameter[] parameters)
        {
            int count = 0;
            MySqlTransaction mySqlTransaction = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;//设置command的CommandType为指定的CommandType 
                                                          //如果同时传入了参数，则添加这些参数 
                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        connection.Open();//打开数据库连接 
                        mySqlTransaction = connection.BeginTransaction();
                        count = command.ExecuteNonQuery();
                        mySqlTransaction.Commit();
                    }
                }
                catch (MySqlException ex)
                {
                    mySqlTransaction.Rollback();
                }
            }

            return count;//返回执行增删改操作之后，数据库中受影响的行数 
        }
        #endregion
    }
}