using ProveedoresOnLine.Company.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Reports.Models.Util
{
    public class GenericReportItemInfoModel
    {
        public GenericReportItemInfoModel(){}

        public int ItemInfoId { get; set; }
        public CatalogModel ItemInfoType { get; set; }
        public string Value { get; set; }
        public string ValueName { get; set; }
        public string LargeValue { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
