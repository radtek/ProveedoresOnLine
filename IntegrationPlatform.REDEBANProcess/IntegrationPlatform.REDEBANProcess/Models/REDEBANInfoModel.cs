using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess.Models
{
    public class REDEBANInfoModel
    {
        public CompanyModel ProviderBasicInfo { get; set; }

        public CompanyModel ProviderFullInfo { get; set; }

        public List<GenericItemModel> LegalInfo_ChaimberOfCommerce { get; set; }

        public List<GenericItemModel> LegalInfo_RUT{ get; set; }

        public List<GenericItemModel> FinancialInfo_FinStats { get; set; }

        public List<GenericItemModel> FinancialInfo_BankCert { get; set; }

        public List<GenericItemModel> FinancialInfo_IncomeStatement { get; set; }

        public List<GenericItemModel> Commercial_CertExp { get; set; }

        public List<GenericItemModel> HSEQ_Cert { get; set; }

        public List<GenericItemModel> HSEQ_Health { get; set; }

        public List<GenericItemModel> HSEQ_Riskes { get; set; }

        public List<GenericItemModel> HSEQ_Acc { get; set; }

        public List<GenericItemModel> AditionalDocuments { get; set; }
    }
}
