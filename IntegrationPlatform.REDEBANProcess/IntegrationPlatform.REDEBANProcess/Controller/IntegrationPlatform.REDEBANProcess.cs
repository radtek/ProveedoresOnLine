using IntegrationPlatform.REDEBANProcess.Models;
using OfficeOpenXml;
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
            var oRowExcel = new REDEBANInfoModel();
            var oFileExcel = new StringBuilder();
            //oFileExcel.AppendLine
            //    ("CompanyName"+
            //    );
            if (providers != null)
            {
                providers.All(x =>
                {
                    var prvInfo = GetProviderInfo(REDEBANProcess.Models.Constants.C_REDEBAN_ProviderPublicId,x.CompanyPublicId);
                    if (prvInfo != null)
                    {
                        oRowExcel.Provider.CompanyName = prvInfo.Provider.CompanyName;
                        

                    }
                    
                    return true;
                });
            }
        }

        //private static ExcelWorksheet CreateExcelFile(ExcelPackage p, string SheetName)
        //{
        //    p.Workbook.Worksheets.Add
        //}

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
