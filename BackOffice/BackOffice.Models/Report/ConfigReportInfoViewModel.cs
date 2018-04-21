using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Report
{
    public class ConfigReportInfoViewModel
    {
        public string Parent { get; set; }
        public int? ReportId { get; set; }
        public string ReportInfoId { get; set; }
        public string ReportInfoType { get; set; }
        public string ReportInfoFieldId { get; set; }
        public string Field { get; set; }
        public string ReportInfoEnableId { get; set; }
        public string Enable { get; set; }

        


    }
}
