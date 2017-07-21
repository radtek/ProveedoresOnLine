using Register_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Register_Core.DAL.Controller
{
    internal class RegisterDataController : IRegisterData
    {
        #region singleton instance

        private static Interfaces.IRegisterData oInstance;
        internal static Interfaces.IRegisterData Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new RegisterDataController();
                return oInstance;
            }
        }

        private Interfaces.IRegisterData DataFactory;

        #endregion

        #region Constructor

        public RegisterDataController()
        {
            RegisterDataFactory factory = new RegisterDataFactory();
            DataFactory = factory.GetRegisterInstance();
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
