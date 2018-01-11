using ProveedoresOnLine.Company.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Company.Models.Company
{
    public class CompanyNotificationInfoModel
    {
        public int CompanyNotificationInfoId { get; set; }

        public int CompanyNotificationId { get; set; }

        public CatalogModel ConfigItemType { get; set; }

        public string Value { get; set; }

        public string LargeValue { get; set; }

        public DateTime LastModify { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Enable { get; set; }
    }
}
