using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.Reports.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Reports.Models.Reports
{
    public class ConfigReportModel
    {
        public string ReportId { get; set; }
        public string ReportPublic { get; set; }
        public GenericItemModel ReportType { get; set; }
        public string ReportName { get; set; }
        public string User { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }
        public List<ConfigReportInfoModel> configReportInfo { get; set; }

    }
}
