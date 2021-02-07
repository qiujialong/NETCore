using System;

namespace ReCheckWebSite.Models
{
    public class InspectDisplayInfo
    {
        public string ID { get; set; }

        /// <summary>
        /// 对应玻璃的检测对象ID
        /// </summary>
        //[XmlIgnore]
        public string InspectID { get; set; }

        /// <summary>
        /// 横向part索引编号——Y
        /// </summary>
        public int PartH { get; set; }

        /// <summary>
        /// 纵向part索引编号——T
        /// </summary>
        public int PartV { get; set; }

        /// <summary>
        /// 显示图缩放比例
        /// </summary>
        public double DisplayImageRatio { get; set; }

        /// <summary>
        /// 显示图片中玻璃的四个顶点位置
        /// 左上，左下，右下，右上
        /// </summary>
        public int[] FourCorners { get; set; }

        /// <summary>
        /// 顶点数据库存储
        /// </summary>
        public string FourCornerString
        {
            get
            {
                string[] arrStr = new string[FourCorners.Length];

                for (int i = 0; i < FourCorners.Length; i++)
                {
                    arrStr[i] = FourCorners[i].ToString();
                }

                return string.Join(",", arrStr);
            }
            set
            {
                if (value != null)
                {
                    var cournerlist = value.Split(',');
                    FourCorners = new int[8];
                    for (int i = 0; i < cournerlist.Length && i < 8; i++)
                    {
                        int corner = 0;
                        Int32.TryParse(cournerlist[i], out corner);
                        FourCorners[0] = corner;
                    }
                }
            }
        }
    }
}
