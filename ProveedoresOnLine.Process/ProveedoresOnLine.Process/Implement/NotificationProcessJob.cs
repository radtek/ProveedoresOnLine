using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace ProveedoresOnLine.Process.Implement
{
    public class NotificationProcessJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ProveedoresOnLine.Notification.Controller.NotificationModule.StartProcess();
        }
    }
}
