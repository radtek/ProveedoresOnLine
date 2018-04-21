using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Report
{
    public class ConfigReportViewModel
    {        
        public DateTime CreateDate { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public string ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportPublic { get; set; }
        public string ReportTypeId { get; set; }
        public string User { get; set; }
    }
}
