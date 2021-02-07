using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Xml.Serialization;

namespace ReCheckWebSite.Models
{
    [XmlRoot("FlawInfoFeedBack", Namespace = "")]
    [XmlType("FlawInfoFeedBack", Namespace = "")]
    [Table("FlawInfoFeedBack")]
    public class FlawInfoFeedBack
    {
        [Key]
        [StringLength(80)]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 对应玻璃的检测对象ID
        /// </summary>
        [Required]
        [StringLength(80)]
        public string InspectID { get; set; }

        /// <summary>
        /// 缺陷类型编码
        /// </summary>
        public int FlawTypeCode { get; set; }

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

        #region 缺陷矩形框选信息 单位：像素
        /// <summary>
        /// 缺陷外部矩形框topleft坐标——x
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// 缺陷外部矩形框topleft坐标——y
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// 缺陷外部矩形框宽度
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 缺陷外部矩形框高度
        /// </summary>
        public int height { get; set; }
        #endregion

        /// <summary>
        /// 缺陷编号索引
        /// </summary>
        public int flawIndex { get; set; }
        /// <summary>
        /// 缺陷所在区域
        /// </summary>
        public int regionIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RowColor { get; set; }

        #region 缺陷大小位置信息(单位mm)——相对于裁剪后的检测区域【可用于点密集性和线间距检测】
        /// <summary>
        /// 检测区域中的缺陷中心位置
        /// </summary>
        public double RotatedRectCenterX { get; set; }
        /// <summary>
        /// 检测区域中的缺陷中心位置
        /// </summary>
        public double RotatedRectCenterY { get; set; }
        /// <summary>
        /// 检测区域中的缺陷旋转框宽度
        /// </summary>
        public double RotatedRectWidth { get; set; }
        /// <summary>
        /// 检测区域中缺陷旋转框高度
        /// </summary>
        public double RotatedRectHeight { get; set; }
        /// <summary>
        /// 缺陷角度
        /// </summary>
        public double angle { get; set; }
        #endregion

        /// <summary>
        /// 节点编号
        /// todo PartH更有意义
        /// </summary>
        [Obsolete("逐步去除此字段，可以通过Y值获取到相应的节点编号")]
        public int NodeItemNo { get; set; } = 0;

        /// <summary>
        /// 横向part索引编号——Y
        /// </summary>
        public int PartH { get; set; } = 0;
        /// <summary>
        /// 纵向part索引编号——T
        /// </summary>
        public int PartV { get; set; } = 0;

        /// <summary>
        /// 缺陷面积大小【mm²】
        /// </summary>
        public double FlawAreaSize { get; set; }

        /// <summary>
        /// 像素转实际尺寸（mm）比例【数据存储均转换为mm，仅当需要显示时，使用此值转换为像素】
        /// 横向比例
        /// </summary>
        public double PixelsRatio { get; set; }

        ///// <summary>
        ///// TODO 像素尺寸缩放比
        ///// 纵向比例
        ///// </summary>
        //[ProtoMember(26)]
        //public double VertPixelsRatio { get; set; }

        ///// <summary>
        ///// TODO 检出工位
        ///// </summary>
        //[ProtoMember(27)]
        //public int Station { get; set; }

        /// <summary>
        /// 是否是检出的缺陷【即按规则筛选出的缺陷】
        /// </summary>
        public bool IsDetected { get; set; } = false;

        /// <summary>
        /// 是否删除 false：使用中 true：删除
        /// </summary>
        public bool IsDelete { get; set; } = false;

        [StringLength(30)]
        public string CaptureDateTime { get; set; }

        [StringLength(50)]
        public string ServerIP { get; set; }

        [StringLength(50)]
        public string QRCodeAllInfo { get; set; }

        [StringLength(200)]
        public string ImagePath { get; set; }

        public int Line { get; set; }

        public int FeedBackCode { get; set; }
    }
}