using ProveedoresOnLine.Notification.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Notification.DAL.MySQLDAO
{
    internal class Notification_MySqlDao : INotificationData
    {
        private ADO.Interfaces.IADO DataInstance;
        
        public Notification_MySqlDao()
        {
            DataInstance = new ADO.MYSQL.MySqlImplement(Models.Constants.C_POL_NotificationConnectionName);
        }
    }
}
