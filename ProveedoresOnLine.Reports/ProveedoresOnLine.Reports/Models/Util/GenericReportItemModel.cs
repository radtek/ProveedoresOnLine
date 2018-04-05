using ProveedoresOnLine.Company.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Reports.Models.Util
{
    public class GenericReportItemModel
    {
        public GenericReportItemModel() {}

        public int ItemId { get; set; }
        public CatalogModel ItemType { get; set; }
        public string ItemName { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }
        public List<GenericReportItemInfoModel> ItemInfo { get; set; }
        public GenericReportItemModel ParentItem { get; set; }
    }
}
