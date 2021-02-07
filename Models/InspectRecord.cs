using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ReCheckWebSite.Models
{
    /// <summary>
    /// 检测记录
    /// or 检测目标的信息
    /// </summary>
    public class InspectRecord
    {
        public string ISOK { get; set; }

        public string LineNo { get; set; }

        public string QRCode { get; set; }

        public string QRCodeAllInfo { get; set; }

        public string NGReasonString { get; private set; }

        public string CaptureDateTime { get; set; }

        public string DetectDateTime { get; set; }

        public string FlawImagePath { get; set; }

        public string ID { get; set; }

        public string ServerIP { get; set; }

        private int _NGReason;
        public int NGReason
        {
            get { return _NGReason; }
            set
            {
                _NGReason = value;
                NGReasonString = GetDescription<NGReasons>(value);
            }
        }

        /// <summary>
        /// 检测缺陷结果列表
        /// 与record一起保存 —— 因为xml保存时，缺陷保存路径在record中存储
        /// </summary>
        public List<FlawInfo> FlawInfoList { get; set; } = new List<FlawInfo>();

        /// <summary>
        /// 检测显示信息
        /// </summary>
        public List<InspectDisplayInfo> DisplayInfoList { get; set; } = new List<InspectDisplayInfo>();

        /// <summary>
        /// 获取枚举Description Attribute的值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        public string GetDescription<T>(int? value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return GetDescription((T)System.Enum.Parse(typeof(T), value.Value.ToString()));
        }

        /// <summary>
        /// 获取枚举Description Attribute的值
        /// </summary>
        /// <param name="value">枚举</param>
        /// <returns></returns>
        private string GetDescription(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var field = value.GetType().GetField(value.ToString());


            if (field == null)
            {
                return string.Empty;
            }

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }

    /// <summary>
    /// 采集检测记录列表结构
    /// </summary>
    public class InspectRecordList
    {
        public List<InspectRecord> InspectRecords { get; set; } = new List<InspectRecord>();
    }

    /// <summary>
    /// ng原因类型[一片玻璃可以有多个原因导致ng-如不同节点状态]
    /// </summary>
    [Flags]
    public enum NGReasons : int
    {
        /// <summary>
        /// 无NG原因 or ok情况 or 未设置
        /// </summary>
        [Description("")]
        None = 0,

        [Description("规则过滤")]
        Rule = 1,

        [Description("拍照超时")]
        CaptureTimeout = 2,

        [Description("检测超时")]
        DetectTimeout = 4,

        [Description("未撕膜")]
        DetectFilm = 8,

        [Description("节点超时")]
        NodeTimeout = 16,

        [Description("对象超时")]
        DetectObjectTimeout = 32,

        [Description("检测异常")]
        DetectError = 64,
    }
}