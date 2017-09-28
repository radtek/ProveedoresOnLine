using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProveedoresOnLine.Company.Models.Company
{
    [ElasticsearchType(Name = "Customer_Info")]
    public class CompanyIndexModel
    {
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

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string CompanyPublicId { get; set; }

        public string LogoUrl { get; set; }

        public string CatlificationRating { get; set; }

        [Number]
        public int PrincipalActivityId { get; set; }
        
        [String(Index = FieldIndexOption.Analyzed)]
        public string PrincipalActivity { get; set; }

        [Number]
        public int CountryId { get; set; }

        [String(Index = FieldIndexOption.Analyzed)]
        public string Country { get; set; }

        [Number]
        public int CityId { get; set; }
        [String(Index = FieldIndexOption.Analyzed)]
        public string City { get; set; }

        [String]
        public string CustomerPublicId { get; set; }

        [Boolean]
        public bool InBlackList { get; set; }

        public int ProviderStatusId { get; set; }
        [String(Index = FieldIndexOption.Analyzed)]
        public string ProviderStatus { get; set; }

        public bool CompanyEnable { get; set; }

        [Number]
        public int ICAId { get; set; }
        [String(Index = FieldIndexOption.Analyzed)]
        public string ICA { get; set; }

        [Nested]        
        public List<CustomerProviderIndexModel> oCustomerProviderIndexModel { get; set; }

        [Nested]
        public List<CalificationIndexModel> oCalificationIndexModel { get; set; }

        [Nested]
        public List<CustomFiltersIndexModel> oCustomFiltersIndexModel { get; set; }

        public CompanyIndexModel()
        {

        }

        public CompanyIndexModel(CompanyIndexModel oCompanyIndexModel)
        {
            IdentificationTypeId = oCompanyIndexModel.IdentificationTypeId;
            IdentificationType = oCompanyIndexModel.IdentificationType;

            IdentificationNumber = oCompanyIndexModel.IdentificationNumber;

            CompanyName = oCompanyIndexModel.CompanyName;

            CommercialCompanyName = oCompanyIndexModel.CommercialCompanyName;

            CompanyPublicId = oCompanyIndexModel.CompanyPublicId;

            LogoUrl = oCompanyIndexModel.LogoUrl;

            CatlificationRating = oCompanyIndexModel.CatlificationRating;

            PrincipalActivityId = oCompanyIndexModel.PrincipalActivityId;
            PrincipalActivity = oCompanyIndexModel.PrincipalActivity;

            CountryId = oCompanyIndexModel.CountryId;
            Country = oCompanyIndexModel.Country;

            CityId = oCompanyIndexModel.CityId;
            City = oCompanyIndexModel.City;

            CustomerPublicId = oCompanyIndexModel.CustomerPublicId;

            InBlackList = oCompanyIndexModel.InBlackList;

            ProviderStatus = oCompanyIndexModel.ProviderStatus;

            CompanyEnable = oCompanyIndexModel.CompanyEnable;

            ICAId = oCompanyIndexModel.ICAId;
            ICA = oCompanyIndexModel.ICA;

            oCustomerProviderIndexModel = oCompanyIndexModel.oCustomerProviderIndexModel;
        }
    }
}
