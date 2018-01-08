using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Process.Implement
{
    [DisallowConcurrentExecution]
    public class ThirdKnowledgeProcessJob : Quartz.IJob
    {
        public async void Execute(Quartz.IJobExecutionContext context)
        {            
           await ProveedoresOnLine.ThirdKnowledgeBatch.ThirdKnowledgeFTPProcess.StartProcess();            
        }
    }
}
