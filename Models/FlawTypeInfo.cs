using ReCheckWebSite.ADO.NET;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace ReCheckWebSite.Models
{
    /// <summary>
    /// 缺陷类型信息【缺陷类型字典项】
    /// </summary>
    public class FlawTypeInfo
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Name_En { get; set; }
    }

    public class FlawTypeInfoList
    {
        public List<FlawTypeInfo> FlawTypeInfos { get; set; } = new List<FlawTypeInfo>();
    }
}
