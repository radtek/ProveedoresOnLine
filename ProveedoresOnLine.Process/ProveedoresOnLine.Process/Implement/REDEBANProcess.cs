using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace ProveedoresOnLine.Process.Implement
{
    public class REDEBANProcess : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            IntegrationPlatform.REDEBANProcess.IntegrationPlatformREDEBANProcess.StartProcess();
        }
    }
}
