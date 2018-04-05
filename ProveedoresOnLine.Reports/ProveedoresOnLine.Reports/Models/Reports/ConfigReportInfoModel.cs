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

        public int ReportInfoId { get; set; }
        public int ReportId { get; set; }
        public GenericReportItemInfoModel ReportInfoType { get; set; }
        public ConfigReportInfoModel Parent { get; set; }
        public string Value { get; set; }
        public string LargeValue { get; set; }
        public string Enable { get; set; }
        public string LastModify { get; set; }
        public string CreateDate { get; set; }
    }
}
