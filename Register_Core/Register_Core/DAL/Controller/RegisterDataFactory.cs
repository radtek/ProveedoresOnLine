using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Register_Core.DAL.Controller
{
    internal class RegisterDataFactory
    {
        public Interfaces.IRegisterData GetRegisterInstance()
        {
            Type typetoreturn = Type.GetType("Register_Core.DAL.MySQLDAO.Register_MySqlDao,Register_Core");
            Interfaces.IRegisterData oRetorno = (Interfaces.IRegisterData)Activator.CreateInstance(typetoreturn);
            return oRetorno;
        }
    }
}
