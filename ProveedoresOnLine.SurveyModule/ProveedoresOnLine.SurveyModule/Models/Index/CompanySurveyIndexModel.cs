using Nest;
using ProveedoresOnLine.Company.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.SurveyModule.Models.Index
{
    [ElasticsearchType(Name = "CompanySurvey_Info")]
    public class CompanySurveyIndexModel
    {
        public CompanySurveyIndexModel()
        {

        }
        public string Id { get { return CompanyPublicId; } }
        [Number]
        public int IdentificationTypeId { get; set; }
        public string IdentificationType { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string IdentificationNumber { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string CompanyName { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string CommercialCompanyName { get; set; }

        public string CompanyPublicId { get; set; }

        public string LogoUrl { get; set; }

        public string CatlificationRating { get; set; }

        [Number]
        public int PrincipalActivityId { get; set; }
        [String]
        public string PrincipalActivity { get; set; }

        [Number]
        public int CountryId { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string Country { get; set; }

        [Number]
        public int CityId { get; set; }
        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string City { get; set; }

        [String]
        public string CustomerPublicId { get; set; }

        [Boolean]
        public bool InBlackList { get; set; }

        public int ProviderStatusId { get; set; }
        public string ProviderStatus { get; set; }

        public bool CompanyEnable { get; set; }

        [Number]
        public int ICAId { get; set; }
        [String]
        public string ICA { get; set; }

        [Nested]
        public List<CustomerProviderIndexModel> oCustomerProviderIndexModel { get; set; }

        [Nested]
        public List<SurveyIndexModel> oSurveyIndexModel { get; set; }

        public CompanySurveyIndexModel(CompanySurveyIndexModel oCompanySurveyIndexModel)
        {
            IdentificationTypeId = oCompanySurveyIndexModel.IdentificationTypeId;
            IdentificationType = oCompanySurveyIndexModel.IdentificationType;
           
            IdentificationNumber = oCompanySurveyIndexModel.IdentificationNumber;
           
            CompanyName = oCompanySurveyIndexModel.CompanyName;
            
            CommercialCompanyName = oCompanySurveyIndexModel.CommercialCompanyName;
            
            CompanyPublicId = oCompanySurveyIndexModel.CompanyPublicId;
           
            LogoUrl = oCompanySurveyIndexModel.LogoUrl;
            
            CatlificationRating = oCompanySurveyIndexModel.CatlificationRating;
            
            PrincipalActivityId = oCompanySurveyIndexModel.PrincipalActivityId;
            PrincipalActivity = oCompanySurveyIndexModel.PrincipalActivity;
            
            CountryId = oCompanySurveyIndexModel.CountryId;
            Country = oCompanySurveyIndexModel.Country;
            
            CityId = oCompanySurveyIndexModel.CityId;
            City = oCompanySurveyIndexModel.City;
            
            CustomerPublicId = oCompanySurveyIndexModel.CustomerPublicId;
            
            InBlackList = oCompanySurveyIndexModel.InBlackList;
            
            ProviderStatusId = oCompanySurveyIndexModel.ProviderStatusId;
            ProviderStatus = oCompanySurveyIndexModel.ProviderStatus;
            
            CompanyEnable = oCompanySurveyIndexModel.CompanyEnable;
            
            ICAId = oCompanySurveyIndexModel.ICAId;
            ICA = oCompanySurveyIndexModel.ICA;
            
            oCustomerProviderIndexModel = oCompanySurveyIndexModel.oCustomerProviderIndexModel;
            
            oSurveyIndexModel = oCompanySurveyIndexModel.oSurveyIndexModel;
        }
    }
}
