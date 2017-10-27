using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Company.Models.Company
{
    public class CompanyNotificationModel
    {
        public int NotificationConfigId { get; set; }

        public string CompanyPublicId { get; set; }

        public string NotificationName { get; set; }

        public DateTime LastModify { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Enable { get; set; }

        public List<CompanyNotificationInfoModel> CompanyNotificationInfo { get; set; }
    }
}
