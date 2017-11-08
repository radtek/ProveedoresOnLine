using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProveedoresOnLine.Notification.Models.Enumerations;

namespace ProveedoresOnLine.Notification.Interfaces
{
    internal interface INotificationCore
    {

        bool ManageNotification(string CompanyPublicId, List<ProveedoresOnLine.Company.Models.Company.CompanyNotificationInfoModel> NotificationConfigInfoModel);
    }
}
