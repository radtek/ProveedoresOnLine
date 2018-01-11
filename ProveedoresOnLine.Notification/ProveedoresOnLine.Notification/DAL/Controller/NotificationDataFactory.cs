using ProveedoresOnLine.Notification.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Notification.DAL.Controller
{
    internal class NotificationDataFactory
    {
        public INotificationData GetNotificationInstance()
        {
            Type typetoreturn = Type.GetType("ProveedoresOnLine.Notification.DAL.MySQLDAO.Notification_MySqlDao,ProveedoresOnLine.Notification");
            INotificationData oRetorno = (INotificationData)Activator.CreateInstance(typetoreturn);
            return oRetorno;
        }
    }
}
