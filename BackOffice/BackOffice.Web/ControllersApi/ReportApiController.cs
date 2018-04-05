using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProveedoresOnLine.Reports.Models.Reports;

namespace BackOffice.Web.ControllersApi
{
    public class ReportApiController : ApiController
    {
        [HttpPost]
        [HttpGet]
        public List<ConfigReportModel> CC_Report()
        {
            List<ConfigReportModel> oReturn = new List<ConfigReportModel>();

            oReturn = ProveedoresOnLine.Reports.Controller.ReportModule.CC_Report_GetReportPublicId(null);


            return oReturn;
        }
    }
}
