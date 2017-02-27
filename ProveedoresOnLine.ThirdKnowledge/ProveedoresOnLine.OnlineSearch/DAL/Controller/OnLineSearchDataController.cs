using ProveedoresOnLine.OnlineSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.DAL.Controller
{
    internal class OnLineSearchDataController : IOnLineSearchData
    {
        private static ProveedoresOnLine.OnlineSearch.Interfaces.IOnLineSearchData oInstance;

        internal static ProveedoresOnLine.OnlineSearch.Interfaces.IOnLineSearchData Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new OnLineSearchDataController();                    
                return oInstance;
            }
        }

        private ProveedoresOnLine.OnlineSearch.Interfaces.IOnLineSearchData DataFactory;

        public Models.TreeModel GetAnswerByTreeidAndQuestion(int TreeType, string Question)
        {
            return DataFactory.GetAnswerByTreeidAndQuestion(TreeType, Question);
        }

        public OnLineSearchDataController()
        {
            OnLineSearchDataFactory factory = new OnLineSearchDataFactory();
            DataFactory = factory.GetThirdKnowledgeInstance();
        }
    }
}
