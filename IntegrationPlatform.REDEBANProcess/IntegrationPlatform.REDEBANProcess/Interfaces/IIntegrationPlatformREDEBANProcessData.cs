using IntegrationPlatform.REDEBANProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess.Interfaces
{
    internal interface IIntegrationPlatformREDEBANProcessData
    {
        List<CompanyModel> GetAllProviders();

        REDEBANInfoModel GetProviderInfo(string CompanyPublicId, string ProviderPublicId);        

        int RedebanProcessLogUpsert(int RedebanProcessLogId,string ProceesName, string FileName, bool SendStatus, bool IsSucces, bool Enable);

        RedebanLogModel GetLogBySendStatus(bool SendStatus);        
    }
}
