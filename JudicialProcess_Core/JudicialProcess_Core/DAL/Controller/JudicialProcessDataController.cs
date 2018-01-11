using JudicialProcess_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudicialProcess_Core.DAL
{
    internal class JudicialProcessDataController : IJudicialProcessData
    {
        #region singleton instance

        private static Interfaces.IJudicialProcessData oInstance;
        internal static Interfaces.IJudicialProcessData Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new JudicialProcessDataController();
                return oInstance;
            }
        }

        private Interfaces.IJudicialProcessData DataFactory;

        #endregion

        #region Constructor

        public JudicialProcessDataController()
        {
            JudicialProcessDataFactory factory = new JudicialProcessDataFactory();
            DataFactory = factory.GetJudicialProcessInstance();
        }

        public bool IsAuthorized(string Token)
        {
            return DataFactory.IsAuthorized(Token);
        }

        public void SaveTransaction(string Token, string Message, string Query, int ServiceType, bool IsSuccess)
        {
            DataFactory.SaveTransaction(Token, Message, Query, ServiceType, IsSuccess);
        }

        #endregion
    }
}
