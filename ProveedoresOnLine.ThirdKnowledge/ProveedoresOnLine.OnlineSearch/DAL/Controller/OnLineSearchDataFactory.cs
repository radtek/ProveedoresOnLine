using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.DAL.Controller
{
    internal class OnLineSearchDataFactory
    {
        public ProveedoresOnLine.OnlineSearch.Interfaces.IOnLineSearchData GetThirdKnowledgeInstance()
        {
            Type typetoreturn = Type.GetType("ProveedoresOnLine.OnlineSearch.DAL.MySQLDAO.OnLinSearch_MySqlDao,ProveedoresOnLine.OnlineSearch");
            ProveedoresOnLine.OnlineSearch.Interfaces.IOnLineSearchData oRetorno = (ProveedoresOnLine.OnlineSearch.Interfaces.IOnLineSearchData)Activator.CreateInstance(typetoreturn);
            return oRetorno;
        }
    }
}
