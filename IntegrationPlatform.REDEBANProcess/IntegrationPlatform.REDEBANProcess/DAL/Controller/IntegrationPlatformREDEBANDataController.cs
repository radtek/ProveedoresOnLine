using IntegrationPlatform.REDEBANProcess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationPlatform.REDEBANProcess.Models;

namespace IntegrationPlatform.REDEBANProcess.DAL.Controller
{
    internal class IntegrationPlatformREDEBANDataController: IIntegrationPlatformREDEBANProcessData
    {
        #region singleton instance

        private static IntegrationPlatform.REDEBANProcess.Interfaces.IIntegrationPlatformREDEBANProcessData oInstance;

        internal static IntegrationPlatform.REDEBANProcess.Interfaces.IIntegrationPlatformREDEBANProcessData Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new IntegrationPlatformREDEBANDataController();
                return oInstance;
            }            
        }

        private IntegrationPlatform.REDEBANProcess.Interfaces.IIntegrationPlatformREDEBANProcessData DataFactory;

        #endregion

        #region constructor

        public IntegrationPlatformREDEBANDataController()
        {
            IntegrationPlatform.REDEBANProcess.DAL.Controller.IntegrationPlatformREDEBANDataFactory factory = new IntegrationPlatformREDEBANDataFactory();
            DataFactory = factory.GetREDEBANProcessInstance();
        }

        #endregion

        public List<CompanyModel> GetAllProviders()
        {
            return DataFactory.GetAllProviders();
        }

        public REDEBANInfoModel GetProviderInfo(string CustomerPublicId, string ProviderPublicId)
        {
            return DataFactory.GetProviderInfo(CustomerPublicId, ProviderPublicId);
        }

        public int RedebanProcessLogUpsert(int RedebanProcessLogId, string ProceesName, string FileName, bool SendStatus, bool IsSucces, bool Enable)
        {
            return DataFactory.RedebanProcessLogUpsert(RedebanProcessLogId, ProceesName, FileName, SendStatus, IsSucces, Enable);
        }
    }
}
