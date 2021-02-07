using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReCheckDemo_Core.Models;
using ReCheckWebSite.ADO.NET;
using ReCheckWebSite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Xml;
using System.Linq;
using System.Threading.Tasks;

namespace ReCheckWebSite.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReCheckController : ControllerBase
    {
        private static FlawTypeInfoList flawTypeInfoList = null;

        private static List<LineSetting> lineSettings = null;

        private string strSqlServerConn = @"data source={0}\SQLEXPRESS;initial catalog={1};User Id=sa;Password=sa;MultipleActiveResultSets=True;App=EntityFramework";

        private string strMySqlConn = @"server={0};port=3306;database={1};uid=root;password=Tunicorn@hf#2017;
                                        Persist Security Info=True;";


        /// <summary>
        /// HttpGet接口，根据条形码查询缺陷信息
        /// </summary>
        /// <param name="qrcode"></param>
        /// <returns></returns>
        [HttpGet]
        public string Search(string qrcode)
        {
            if (string.IsNullOrWhiteSpace(qrcode))
                return "[]";

            FlawTypeInit();
            LineSettingInit();

            List<InspectRecord> records = new List<InspectRecord>();

            //var sql = "select * from inspectrecord limit 5";
            var sql = string.Format("select ID,ISOK,QRCODE,QRCODEALLINFO,NGREASON,CAPTUREDATETIME,DETECTDATETIME,FlawImagePath from InspectRecord where QRCodeAllInfo = '{0}'", qrcode);

            //Parallel.ForEach(lineSettings, setting =>
            //{
            //    SqlDbHelper sqlDbHelper = new SqlDbHelper();
            //    string conn = string.Format(strMySqlConn, setting.ServerIP, setting.DBName);
            //    sqlDbHelper.ConnectionString = conn;


            //    DataTable dt = sqlDbHelper.MySQLExecuteDataTable(sql);

            //    var ret = AnalyzeInspectRecord(dt, setting.LineNo, setting.ServerIP);

            //    if (ret != null && ret.Count > 0)
            //    {
            //        foreach (var item in ret)
            //        {
            //            var id = item.ID;

            //            DateTime capturedatetime = DateTime.Parse(item.CaptureDateTime);
            //            string tablename = "flawinfo_" + capturedatetime.Month.ToString().PadLeft(2, '0');
            //            sql = string.Format("select * from {1} where inspectid = '{0}'", id, tablename);

            //            dt = sqlDbHelper.MySQLExecuteDataTable(sql);
            //            item.FlawInfoList.AddRange(AnalyzeFlawInfo(dt));

            //            var sqlDisp = string.Format("select * from InspectDisplayInfo where inspectid = '{0}'", id);
            //            dt = sqlDbHelper.MySQLExecuteDataTable(sqlDisp);
            //            item.DisplayInfoList.AddRange(AnalyzeDisplayInfo(dt));
            //        }

            //        records.AddRange(ret);
            //    }
            //});
            foreach (var setting in lineSettings)
            {
                SqlDbHelper sqlDbHelper = new SqlDbHelper();
                string conn = string.Format(strMySqlConn, setting.ServerIP, setting.DBName);
                sqlDbHelper.ConnectionString = conn;

                DataTable dt = sqlDbHelper.MySQLExecuteDataTable(sql);

                var ret = AnalyzeInspectRecord(dt, setting.LineNo, setting.ServerIP);

                if (ret != null && ret.Count > 0)
                {
                    foreach (var item in ret)
                    {
                        var id = item.ID;

                        DateTime capturedatetime = DateTime.Parse(item.CaptureDateTime);
                        string tablename = "flawinfo_" + capturedatetime.Month.ToString().PadLeft(2, '0');
                        sql = string.Format("select * from {1} where inspectid = '{0}'", id, tablename);

                        dt = sqlDbHelper.MySQLExecuteDataTable(sql);
                        item.FlawInfoList.AddRange(AnalyzeFlawInfo(dt));

                        var sqlDisp = string.Format("select * from InspectDisplayInfo where inspectid = '{0}'", id);
                        dt = sqlDbHelper.MySQLExecuteDataTable(sqlDisp);
                        item.DisplayInfoList.AddRange(AnalyzeDisplayInfo(dt));
                    }

                    records.AddRange(ret);
                }
            }

            return JsonConvert.SerializeObject(records);
        }

        /// <summary>
        /// 产线配置初始化,解析LineSetting.xml
        /// </summary>
        private void LineSettingInit()
        {
            if (lineSettings == null || lineSettings.Count == 0)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "LineSetting.xml");

                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        lineSettings = new List<LineSetting>();
                        XmlDocument doc = new XmlDocument();
                        doc.Load(path);

                        XmlNode node = doc.SelectSingleNode("LineSettings");

                        foreach (XmlNode xmlNode in node)
                        {
                            LineSetting lineSetting = new LineSetting();
                            lineSetting.LineNo = xmlNode.Attributes["LineNo"].Value.ToString();
                            lineSetting.ServerIP = xmlNode.Attributes["ServerIP"].Value.ToString();
                            lineSetting.DBName = xmlNode.Attributes["DBName"].Value.ToString();
                            lineSetting.DBType = (DBType)int.Parse(xmlNode.Attributes["DBType"].Value);

                            lineSettings.Add(lineSetting);
                        }
                    }
                    catch (Exception ex)
                    {
                        lineSettings = new List<LineSetting>();
                    }
                }
            }
        }

        /// <summary>
        /// 缺陷类型初始化,解析FlawTypeInfo.xml
        /// </summary>
        private void FlawTypeInit()
        {
            if (flawTypeInfoList == null || flawTypeInfoList.FlawTypeInfos.Count == 0)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "FlawTypeInfo.xml");

                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        flawTypeInfoList = new FlawTypeInfoList();
                        XmlDocument doc = new XmlDocument();
                        doc.Load(path);

                        XmlNode node = doc.SelectSingleNode("FlawTypeInfos");

                        foreach (XmlNode xmlNode in node)
                        {
                            FlawTypeInfo flawType = new FlawTypeInfo();
                            flawType.Code = xmlNode.Attributes["Code"].Value.ToString();
                            flawType.Name = xmlNode.Attributes["Name"].Value.ToString();
                            flawType.Name_En = xmlNode.Attributes["Name_En"].Value.ToString();

                            flawTypeInfoList.FlawTypeInfos.Add(flawType);
                        }
                    }
                    catch (Exception ex)
                    {
                        flawTypeInfoList = new FlawTypeInfoList();
                    }
                }
            }
        }

        /// <summary>
        /// 解析InspectRecord实体
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="lineNo"></param>
        /// <param name="serverIP"></param>
        private List<InspectRecord> AnalyzeInspectRecord(DataTable dt, string lineNo, string serverIP)
        {
            List<InspectRecord> inspectRecords = new List<InspectRecord>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    InspectRecord record = new InspectRecord();
                    record.ID = row["ID"].ToString();
                    if (dt.Columns["ISOK"].DataType == typeof(sbyte))
                        record.ISOK = int.Parse(row["ISOK"].ToString()) == 0 ? "OK" : "NG";
                    else if (dt.Columns["ISOK"].DataType == typeof(bool))
                        record.ISOK = bool.Parse(row["ISOK"].ToString()) ? "OK" : "NG";
                    record.QRCode = row["QRCode"].ToString();
                    record.QRCodeAllInfo = row["QRCodeAllInfo"].ToString();
                    record.NGReason = int.Parse(row["NGReason"].ToString());
                    record.CaptureDateTime = row["CaptureDateTime"].ToString();
                    record.DetectDateTime = row["DetectDateTime"].ToString();
                    record.FlawImagePath = row["FlawImagePath"].ToString();
                    record.LineNo = lineNo;
                    record.ServerIP = serverIP;

                    inspectRecords.Add(record);
                }
            }

            return inspectRecords;
        }

        /// <summary>
        /// 解析FlawInfo实体
        /// </summary>
        /// <param name="dt"></param>
        private List<FlawInfo> AnalyzeFlawInfo(DataTable dt)
        {
            List<FlawInfo> flawInfos = new List<FlawInfo>();
            if (dt != null && dt.Rows.Count > 0)
            {
                int index = 0;
                foreach (DataRow row in dt.Rows)
                {
                    FlawInfo flawInfo = new FlawInfo();
                    flawInfo.ID = row["ID"].ToString();
                    flawInfo.InspectID = row["InspectID"].ToString();
                    flawInfo.FlawTypeCode = int.Parse(row["FlawTypeCode"].ToString());

                    //显示内容只要贴合异物、贴合脏污、划伤三项缺陷
                    if (flawInfo.FlawTypeCode == 1 || flawInfo.FlawTypeCode == 45 || flawInfo.FlawTypeCode == 46)
                    {
                        flawInfo.FlawTypeName = flawTypeInfoList.FlawTypeInfos.Where(t => t.Code == row["FlawTypeCode"].ToString()).Select(t => t.Name).FirstOrDefault();
                        flawInfo.FlawCenterX = double.Parse(row["FlawCenterX"].ToString());
                        flawInfo.FlawCenterY = double.Parse(row["FlawCenterY"].ToString());
                        flawInfo.FlawWidth = double.Parse(row["FlawWidth"].ToString());
                        flawInfo.FlawHeight = double.Parse(row["FlawHeight"].ToString());

                        flawInfo.PartH = int.Parse(row["PartH"].ToString());
                        flawInfo.PartV = int.Parse(row["PartV"].ToString());
                        flawInfo.FlawAreaSize = double.Parse(row["FlawAreaSize"].ToString());
                        flawInfo.PixelsRatio = double.Parse(row["PixelsRatio"].ToString());
                        flawInfo.regionIndex = int.Parse(row["regionIndex"].ToString());

                        flawInfo.SortIndex = index++;

                        flawInfos.Add(flawInfo);
                    }
                }
            }

            return flawInfos;
        }

        /// <summary>
        /// 解析DisplayInfo实体
        /// </summary>
        /// <param name="dt"></param>
        private List<InspectDisplayInfo> AnalyzeDisplayInfo(DataTable dt)
        {
            List<InspectDisplayInfo> inspectDisplayInfos = new List<InspectDisplayInfo>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    InspectDisplayInfo displayInfo = new InspectDisplayInfo();
                    displayInfo.ID = row["ID"].ToString();
                    displayInfo.InspectID = row["InspectID"].ToString();
                    displayInfo.PartH = int.Parse(row["PartH"].ToString());
                    displayInfo.PartV = int.Parse(row["PartV"].ToString());
                    displayInfo.DisplayImageRatio = double.Parse(row["DisplayImageRatio"].ToString());
                    displayInfo.FourCornerString = row["FourCorners"].ToString();

                    inspectDisplayInfos.Add(displayInfo);
                }
            }

            return inspectDisplayInfos;
        }
    }
}