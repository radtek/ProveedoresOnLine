using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProveedoresOnLine.Process.Test
{
    [TestClass]
    public class NotificationsProcessJobTest
    {
        [TestMethod]
        public void NotificationsProcessJob()
        {
            ProveedoresOnLine.Process.Implement.NotificationProcessJob Msj = new Implement.NotificationProcessJob();
            Msj.Execute(null);
        }
    }
}
