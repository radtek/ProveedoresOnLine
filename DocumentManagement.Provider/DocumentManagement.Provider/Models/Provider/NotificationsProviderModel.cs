using DocumentManagement.Provider.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Provider.Models.Provider
{
    public class NotificationsProviderModel
    {
        public int NotificationProviderId { get; set; }
        public CatalogModel NotificationInfoType { get; set; }
        public string Customer { get; set; }
        public string Provider { get; set; }
        public string FormId { get; set; }

    }
}
