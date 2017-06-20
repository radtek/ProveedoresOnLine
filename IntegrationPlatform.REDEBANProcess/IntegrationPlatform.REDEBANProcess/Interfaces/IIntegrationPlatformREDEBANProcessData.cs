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

        int RedebanProcessLogInsert(string CompanyPublicId, string ProceesName, string FileName, bool SendStatus, bool IsSucces, bool Enable);

        int RedebanProcessUpdateLog(int RedebanProcessLogId,string CompanyPublicId, string ProceesName, string FileName, bool SendStatus, bool IsSucces, bool Enable);
    }
}
