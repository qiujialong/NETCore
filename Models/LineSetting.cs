using System;
using System.Collections.Generic;

namespace ReCheckWebSite.Models
{
    [Serializable]
    public class LineSetting
    {
        /// <summary>
        /// 产线编号
        /// </summary>
        public string LineNo { get; set; }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType DBType { get; set; }
    }

    public class LineSettingModel
    { 
        public List<LineSetting> LineSettings { get; set; }
    }

    public enum DBType
    {
        SQLSERVER = 0,
        MYSQL = 1,
        //SQLite = 2
    }
}
