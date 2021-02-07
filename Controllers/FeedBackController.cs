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
using System.Text;

namespace ReCheckWebSite.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FeedBackController : ControllerBase
    {
        private string strMySqlConn = @"server=10.73.111.78;port=3306;database=Tunibusinesss_FeedBack;uid=root;password=Tunicorn@hf#2017;
                                        Persist Security Info=True;";

        [HttpPost]
        public bool InsertFeedBack()
        {
            var request = HttpContext.Request;

            byte[] requestData = new byte[(int)request.ContentLength];
            request.Body.Read(requestData, 0, requestData.Length);

            var input = Encoding.UTF8.GetString(requestData);

            SqlDbHelper sqlDbHelper = new SqlDbHelper();
            sqlDbHelper.ConnectionString = strMySqlConn;

            var flaws = JsonConvert.DeserializeObject<List<FlawInfoFeedBack>>(input);
            if (flaws.Count > 0)
            {
                foreach (var f in flaws)
                    sqlDbHelper.AddOrUpdate<FlawInfoFeedBack>(f);
            }

            return true;
        }

    }
}