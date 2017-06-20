using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess.DAL.Controller
{
    internal class IntegrationPlatformREDEBANDataFactory
    {
        public IntegrationPlatform.REDEBANProcess.Interfaces.IIntegrationPlatformREDEBANProcessData GetREDEBANProcessInstance()
        {
            Type typeoreturn = Type.GetType("IntegrationPlatform.REDEBANProcess.DAL.MySQLDAO.REDEBANProcess_MySqlDao,IntegrationPlatform.REDEBANProcess");
            IntegrationPlatform.REDEBANProcess.Interfaces.IIntegrationPlatformREDEBANProcessData oRetorno = (IntegrationPlatform.REDEBANProcess.Interfaces.IIntegrationPlatformREDEBANProcessData)Activator.CreateInstance(typeoreturn);

            return oRetorno;
        }
    }
}
