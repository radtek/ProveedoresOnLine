using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudicialProcess_Core.DAL
{
   internal class JudicialProcessDataFactory
    {
        public Interfaces.IJudicialProcessData GetJudicialProcessInstance()
        {
            Type typetoreturn = Type.GetType("JudicialProcess_Core.DAL.MySQLDAO.JudicialProcess_MySqlDao,JudicialProcess_Core");
            Interfaces.IJudicialProcessData oRetorno = (Interfaces.IJudicialProcessData)Activator.CreateInstance(typetoreturn);
            return oRetorno;
        }
    }
}
