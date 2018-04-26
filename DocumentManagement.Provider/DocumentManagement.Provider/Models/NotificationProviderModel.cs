using DocumentManagement.Provider.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Provider.Models.Provider
{
    public class NotificationProviderModel
    {
        public int NotificationProviderId { get; set; }
        public CatalogModel NotificationInfoType { get; set; }
        public string CustomerPublicId { get; set; }
        public string ProviderPublicId { get; set; }
        public string FormPublicId { get; set; }

    }
}