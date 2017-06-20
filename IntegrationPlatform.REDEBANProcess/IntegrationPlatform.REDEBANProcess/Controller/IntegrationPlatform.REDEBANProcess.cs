using IntegrationPlatform.REDEBANProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess
{
    public class IntegrationPlatformREDEBANProcess
    {
        public static void StartProcess()
        {
            var providers = GetAllProviders();
            var InfoToExcel = new List<REDEBANInfoModel>();
            if (providers != null)
            {
                providers.All(x =>
                {
                    var prvInfo = GetProviderInfo(REDEBANProcess.Models.Constants.C_REDEBAN_ProviderPublicId,x.CompanyPublicId);
                    if (prvInfo != null)
                    {
                        //TO DO: fill list to put in excel file
                    }
                    
                    return true;
                });
            }
        }

        public static List<CompanyModel> GetAllProviders()
        {
            return DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.GetAllProviders();
        }

        public static REDEBANInfoModel GetProviderInfo(string CustomerPublicId, string ProviderPublicId)
        {
            return DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.GetProviderInfo(CustomerPublicId, ProviderPublicId);
        }
    }
}
