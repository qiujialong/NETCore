using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TuniVision.SurfaceDetection.Data.DB
{
    public class CreateSQLStr
    {
        /// <summary>
        /// 通用实体类存储新数据到数据库的方法 
        /// 调用此方法可获得SQL Insert语句
        /// </summary>
        /// <typeparam name="T">模板T</typeparam>
        /// <param name="model">实体对象</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public static string SaveSQLStr<T>(T model)
        {
            string tablename = GetTableName<T>();
            return SaveSQLStr(model, tablename);
        }

        public static string SaveSQLStr<T>(T model, string tablename)
        {
            string fieldsName = "INSERT IGNORE INTO " + tablename + "(";
            string fieldsValue = "VALUES(";

            PropertyInfo[] propertys = model.GetType().GetProperties();
            //遍历该对象的所有属性
            foreach (PropertyInfo pi in propertys)
            {
                string name = pi.Name;
                object value = pi.GetValue(model, null);

                if (ExcuteSpecialColumn(pi, ref name))
                    continue;

                if (value != null)
                {
                    if (pi.PropertyType.Name.Equals("Int32"))
                    {
                        fieldsName = fieldsName + "`" + name + "`,";
                        fieldsValue = fieldsValue + Int32.Parse(value.ToString()) + ',';
                    }
                    else if (pi.PropertyType.Name.Equals("DateTime"))
                    {
                        if (value.Equals("0001/1/1 0:00:000") || value.Equals("0001/1/1 0:00:00") || (DateTime)value == DateTime.MinValue)
                        {
                            continue;
                        }
                        else
                        {
                            fieldsName = fieldsName + "`" + name + "`,";
                            fieldsValue = fieldsValue + "'" + value.ToString() + '\'' + ',';
                        }
                    }
                    else if (pi.PropertyType.Name.Equals("String") || pi.PropertyType.Name.Equals("Nullable`1"))
                    {
                        fieldsName = fieldsName + "`" + name + "`,";

                        fieldsValue = fieldsValue + "'" + value.ToString() + '\'' + ',';
                    }
                    else if (pi.PropertyType.Name.Equals("Boolean"))
                    {
                        fieldsName = fieldsName + "`" + name + "`,";

                        fieldsValue = fieldsValue + "'" + (bool.Parse(value.ToString()) ? "1" : "0") + '\'' + ',';
                    }
                    else
                    {
                        fieldsName = fieldsName + "`" + name + "`,";

                        fieldsValue = fieldsValue + "'" + value.ToString() + '\'' + ',';
                    }
                }

            }
            fieldsName = fieldsName.Substring(0, fieldsName.Length - 1) + ')' + ' ';

            //确保该语句返回值为主键ID
            fieldsValue = fieldsValue.Substring(0, fieldsValue.Length - 1) + ')';

            return (fieldsName + fieldsValue).Replace("\\", "\\\\");
        }


        /// <summary>
        /// 通用实体类更新数据库表数据的方法 
        /// 调用此方法可获得SQL Update语句
        /// </summary>
        /// <typeparam name="T">模板T</typeparam>
        /// <param name="model">实体对象</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public static string UpdateSQLStr<T>(T model)
        {
            bool flag = false;
            string tablename = GetTableName<T>();

            string fields = "Update " + tablename + " set ";
            string where = " where ";

            PropertyInfo[] propertys = model.GetType().GetProperties();
            //遍历该对象的所有属性
            foreach (PropertyInfo pi in propertys)
            {
                string name = pi.Name;
                object value = pi.GetValue(model, null);

                if (ExcuteSpecialColumn(pi, ref name))
                    continue;

                if (value != null)
                {
                    var keyAttr = pi.GetCustomAttribute(typeof(KeyAttribute), false);
                    if (keyAttr == null)
                    {
                        if (pi.PropertyType.Name.Equals("Int32"))
                        {
                            fields = fields + "`" + name + "`" + '=' + Int32.Parse(value.ToString()) + ',';
                        }
                        else if (pi.PropertyType.Name.Equals("DateTime"))
                        {
                            if (value.Equals("0001/1/1 0:00:000") || value.Equals("0001/1/1 0:00:00") || (DateTime)value == DateTime.MinValue)
                            {
                                continue;
                            }
                            else
                            {
                                fields = fields + "`" + name + "`" + '=' + "'" + value.ToString() + '\'' + ',';
                            }
                        }
                        else if (pi.PropertyType.Name.Equals("String"))
                        {
                            fields = fields + "`" + name + "`" + '=' + "'" + value.ToString() + '\'' + ',';
                        }
                        else if (pi.PropertyType.Name.Equals("Boolean"))
                        {
                            fields = fields + "`" + name + "`" + '=' + "'" + (bool.Parse(value.ToString()) ? "1" : "0") + '\'' + ',';
                        }
                        else
                        {
                            fields = fields + "`" + name + "`" + '=' + "'" + value.ToString() + '\'' + ',';
                        }
                    }
                    else
                    {
                        where = where + "`" + name + "`" + "='" + value.ToString() + "'";
                        flag = true;
                    }
                }

            }
            fields = fields.Substring(0, fields.Length - 1) + ' ';

            if (flag)
            {
                return (fields + where).Replace("\\", "\\\\"); ;
            }
            else
            {
                return "false";
            }

        }

        /// <summary>
        /// 通用实体类更新数据库表数据的方法 
        /// 调用此方法可获得SQL Update语句
        /// </summary>
        /// <typeparam name="T">模板T</typeparam>
        /// <param name="model">实体对象</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public static string UpdateSqlStrForDelete<T>(T model)
        {
            bool flag = false;
            string tablename = GetTableName<T>();

            string fields = "Update " + tablename + " set IsDelete = 1 ";
            string where = " where ";

            PropertyInfo[] propertys = model.GetType().GetProperties();
            //遍历该对象的所有属性
            foreach (PropertyInfo pi in propertys)
            {
                string name = pi.Name;
                object value = pi.GetValue(model, null);

                if (ExcuteSpecialColumn(pi, ref name))
                    continue;

                var keyAttr = pi.GetCustomAttribute(typeof(KeyAttribute), false);
                if (keyAttr == null)
                    continue;

                if (value != null)
                {
                    where = where + name + "='" + value.ToString() + "'";
                    flag = true;
                }

            }
            fields = fields.Substring(0, fields.Length - 1) + ' ';

            if (flag)
            {
                return fields + where;
            }
            else
            {
                return "false";
            }
        }

        /// <summary>
        /// 通用实体类更新数据库表数据的方法 
        /// 调用此方法可获得SQL Update语句
        /// </summary>
        /// <typeparam name="T">模板T</typeparam>
        /// <param name="model">实体对象</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public static string UpdateSqlStrForBatchDelete<T>(List<string> ids)
        {
            bool flag = false;
            string tablename = GetTableName<T>();

            string fields = "Update " + tablename + " set IsDelete = 1 ";
            string where = " where ";
            Type type = typeof(T);
            PropertyInfo[] propertys = type.GetProperties();
            //遍历该对象的所有属性
            foreach (PropertyInfo pi in propertys)
            {
                string name = pi.Name;

                if (ExcuteSpecialColumn(pi, ref name))
                    continue;

                var keyAttr = pi.GetCustomAttribute(typeof(KeyAttribute), false);
                if (keyAttr == null)
                    continue;

                where = where + "`" + name + "`" + " in ('" + string.Join("','", ids.ToArray()) + "')";
                flag = true;
            }
            fields = fields.Substring(0, fields.Length - 1) + ' ';

            if (flag)
            {
                return fields + where;
            }
            else
            {
                return "false";
            }
        }


        /// <summary>
        /// 通用实体类删除数据库表数据的方法
        /// 调用此方法可获得SQL Delete语句
        /// </summary>
        /// <typeparam name="T">模板T</typeparam>
        /// <param name="model">实体对象</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public static string DeleteSQLStr<T>(T model)
        {
            bool flag = false;
            string tablename = GetTableName<T>();
            string fields = "Delete from  " + tablename;
            string where = " where ";

            PropertyInfo[] propertys = model.GetType().GetProperties();
            //遍历该对象的所有属性
            foreach (PropertyInfo pi in propertys)
            {
                string name = pi.Name;
                object value = pi.GetValue(model, null);
                if (name.Equals("ID") && Int32.Parse(value.ToString()) != 0)
                {
                    where = where + pi.Name + "=" + Int32.Parse(value.ToString());
                    flag = true;
                }
            }

            if (flag)
            {
                return fields + where;
            }
            else
            {
                return "false";
            }

        }


        /// <summary>
        /// 通用实体类查询数据库表数据的方法
        /// 调用此方法可获得SQL Select语句
        /// </summary>
        /// <typeparam name="T">模板T</typeparam>
        /// <param name="model">实体对象</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public static string SelectSQLStr<T>(T model)
        {
            string tablename = GetTableName<T>();
            string where = " where ";
            PropertyInfo[] propertys = model.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                string name = pi.Name;
                object value = pi.GetValue(model, null);

                if (ExcuteSpecialColumn(pi, ref name))
                    continue;

                var keyAttr = pi.GetCustomAttribute(typeof(KeyAttribute), false);
                if (keyAttr == null)
                    continue;

                if (value != null)
                    where = where + "`" + name + "`" + "='" + value.ToString() + "'";
            }

            return "SELECT * FROM " + tablename + where + " and IsDelete=0";
        }

        public static string SelectSQLStr<T>(T model, List<string> queryFields)
        {
            string tablename = GetTableName<T>();
            string where = " where ";
            bool flag = false;
            PropertyInfo[] propertys = model.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                string name = pi.Name;

                if (!queryFields.Contains(name))
                    continue;

                object value = pi.GetValue(model, null);

                if (ExcuteSpecialColumn(pi, ref name))
                    continue;

                if (value != null)
                {
                    if (pi.PropertyType.Name.Equals("Int32"))
                    {
                        where = where + name + "=" + Int32.Parse(value.ToString()) + " and ";
                        flag = true;
                    }
                    else if (pi.PropertyType.Name.Equals("DateTime"))
                    {
                        where = where + name + '=' + "'" + value.ToString() + '\'' + " and ";
                        flag = true;
                    }
                    else if (pi.PropertyType.Name.Equals("String"))
                    {
                        where = where + name + '=' + "'" + value.ToString() + '\'' + " and ";
                        flag = true;
                    }
                    else if (pi.PropertyType.Name.Equals("Boolean"))
                    {
                        where = where + name + '=' + "'" + (bool.Parse(value.ToString()) ? "1" : "0") + '\'' + " and ";
                        flag = true;
                    }
                    else
                    {
                        where = where + name + '=' + "'" + value.ToString() + '\'' + " and ";
                        flag = true;
                    }
                }
            }

            where = where.Substring(0, where.Length - 4);

            if (flag)
            {
                return "SELECT * FROM " + tablename + where;
            }
            else
            {
                return "SELECT * FROM " + tablename;
            }
        }

        public static string QueryAll<T>()
        {
            string tableName = GetTableName<T>();

            return "SELECT * FROM " + tableName + " where IsDelete=0";

        }

        public static T CreateEntityByKey<T>(string id) where T : class, new()
        {
            T t = new T();
            PropertyInfo[] propertys = t.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                string name = pi.Name;
                object value = pi.GetValue(t, null);

                var keyAttr = pi.GetCustomAttribute(typeof(KeyAttribute), false);
                if (keyAttr == null)
                    continue;

                pi.SetValue(t, id);
            }

            return t;
        }

        public static bool ExcuteSpecialColumn(PropertyInfo pi, ref string name)
        {
            bool result = false;

            var notMappedAttr = pi.GetCustomAttribute(typeof(NotMapAttribute), false);
            if (notMappedAttr != null)
                return true;

            var columnAttr = pi.GetCustomAttribute(typeof(ColumnAttribute), false);
            if (columnAttr != null)
                name = (columnAttr as ColumnAttribute).Name;

            return result;
        }

        private static string GetTableName<T>()
        {
            Type type = typeof(T);
            string tablename = (type.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute).Name;

            return tablename;
        }

    }
}
