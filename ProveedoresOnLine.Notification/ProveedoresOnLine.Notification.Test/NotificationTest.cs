using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProveedoresOnLine.Notification.Test
{
    [TestClass]
    public class NotificationTest
    {
        [TestMethod]
        public void ManageNotification()
        {
            Controller.NotificationModule.StartProcess();
        }
    }
}
