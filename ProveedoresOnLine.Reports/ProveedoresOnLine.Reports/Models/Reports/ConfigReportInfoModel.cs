using ProveedoresOnLine.Reports.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Reports.Models.Reports
{
    public class ConfigReportInfoModel
    {
        public ConfigReportInfoModel(){}
        public string ReportInfoId { get; set; }
        public int? ReportId { get; set; }
        public GenericReportItemInfoModel ReportInfoType { get; set; }
        public int? Parent { get; set; }
        public string Value { get; set; }
        public string LargeValue { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
