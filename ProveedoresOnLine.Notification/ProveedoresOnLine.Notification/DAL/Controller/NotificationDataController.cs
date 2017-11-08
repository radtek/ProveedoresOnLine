using ProveedoresOnLine.Notification.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Notification.DAL.Controller
{
    internal class NotificationDataController : INotificationData
    {
        private static INotificationData oInstance;

        internal static Interfaces.INotificationData Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new NotificationDataController();
                return oInstance;
            }
        }

        private INotificationData DataFactory;

        #region Contructor

        public NotificationDataController()
        {
            NotificationDataFactory factory = new NotificationDataFactory();
            DataFactory = factory.GetNotificationInstance();
        } 
        #endregion
    }
}
