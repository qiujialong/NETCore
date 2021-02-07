using System;

namespace ReCheckWebSite.Models
{
    public class FlawInfo
    {
        public int SortIndex { get; set; }

        private string _ID = Guid.NewGuid().ToString();

        /// <summary>
        /// 对应玻璃的检测对象ID
        /// </summary>
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string InspectID { get; set; }

        private int _flawTypeCode;
        /// <summary>
        /// 缺陷类型编码
        /// </summary>
        public int FlawTypeCode
        {
            get { return _flawTypeCode; }
            set
            {
                _flawTypeCode = value;
            }
        }

        public string FlawTypeName { get; set; }

        #region 缺陷信息 单位mm —— 相对于原图[采集图]
        /// <summary>
        /// 缺陷中心点位置 —— X
        /// mm
        /// </summary>
        public double FlawCenterX { get; set; }
        /// <summary>
        /// 缺陷中心点位置 —— Y
        /// mm
        /// </summary>
        public double FlawCenterY { get; set; }
        /// <summary>
        /// 缺陷宽度
        /// mm
        /// </summary>
        public double FlawWidth { get; set; }
        /// <summary>
        /// 缺陷高度
        /// mm
        /// </summary>
        public double FlawHeight { get; set; }
        #endregion

        private int _PartH = 0;
        /// <summary>
        /// 横向part索引编号——Y
        /// </summary>
        public int PartH
        {
            get { return _PartH; }
            set { _PartH = value; }
        }
        private int _PartV = 0;
        /// <summary>
        /// 纵向part索引编号——T
        /// </summary>
        public int PartV
        {
            get { return _PartV; }
            set { _PartV = value; }
        }

        /// <summary>
        /// 缺陷面积大小【mm²】
        /// </summary>
        public double FlawAreaSize { get; set; }

        /// <summary>
        /// 像素转实际尺寸（mm）比例【数据存储均转换为mm，仅当需要显示时，使用此值转换为像素】
        /// 横向比例
        /// </summary>
        public double PixelsRatio { get; set; }

        /// <summary>
        /// 缺陷所在区域
        /// </summary>
        public int regionIndex { get; set; }
    }
}