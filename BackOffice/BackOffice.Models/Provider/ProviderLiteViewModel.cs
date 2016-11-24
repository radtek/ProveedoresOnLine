using BackOffice.Models.General;
using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.SurveyModule.Models.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Provider
{
    public class ProviderLiteViewModel
    {
        public ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel RelatedProvider { get; private set; }

        public CompanyIndexModel ElasticRealtedProvider { get; private set; }

        /// <summary>
        /// provider is related for session customer
        /// </summary>
        public bool IsProviderCustomer
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Provider status id for session customer
        /// </summary>
        public int ProviderStatusId
        {
            get
            {
                if (ElasticRealtedProvider != null)
                {
                    return ElasticRealtedProvider.oCustomerProviderIndexModel.Where(x => x.CustomerPublicId == SessionModel.CurrentCompany.CompanyPublicId).Select(x => x.StatusId).FirstOrDefault();
                }
                else
                {
                    if (SessionModel.CurrentCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.OtherProviders).Select(x => x).FirstOrDefault() != null)
                    {
                        return IsProviderCustomer &&
                         RelatedProvider != null &&
                         RelatedProvider.RelatedCustomerInfo != null ?
                             RelatedProvider.
                             RelatedCustomerInfo[Models.General.InternalSettings.Instance[Models.General.Constants.CC_CompanyPublicId_Publicar].Value].
                             ItemType.
                             ItemId :
                             0;
                    }
                    else
                    {
                        return IsProviderCustomer &&
                         RelatedProvider != null &&
                         RelatedProvider.RelatedCustomerInfo != null ?
                             RelatedProvider.
                             RelatedCustomerInfo[BackOffice.Models.General.SessionModel.CurrentCompany.CompanyPublicId].
                             ItemType.
                             ItemId :
                             0;
                    }
                }
            }
        }


        /// <summary>
        /// Is certified
        /// </summary>
        public bool ProviderIsCertified
        {
            get
            {
                if (ProviderStatusId.ToString() ==
                    BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ProviderStatus_Certified].Value ||
                    ProviderStatusId.ToString() == "902004" ||
                    ProviderStatusId.ToString() == "902008")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Provider logo url
        /// </summary>
        public string ProviderLogoUrl
        {
            get
            {
                if (RelatedProvider != null && RelatedProvider.RelatedCompany != null && RelatedProvider.RelatedCompany.CompanyInfo.Any(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo))
                {
                    return !string.IsNullOrEmpty(RelatedProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo).Select(x => x.Value).FirstOrDefault()) ? RelatedProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo).Select(x => x.Value).FirstOrDefault() : BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_Company_DefaultLogoUrl].Value;
                }

                string pic = ElasticRealtedProvider != null ? ElasticRealtedProvider.LogoUrl :
                            RelatedProvider != null && RelatedProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo).Select(x => x.Value).FirstOrDefault() != null
                            ? RelatedProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo).Select(x => x.Value).FirstOrDefault() :
                            BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_Company_DefaultLogoUrl].Value;

                if (!string.IsNullOrEmpty(pic))
                {
                    return pic;
                }
                else
                {
                    return BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_Company_DefaultLogoUrl].Value;
                }
            }
        }

        /// <summary>
        /// Provider rate for session customer
        /// </summary>
        public decimal ProviderRate
        {
            get
            {
                if (ElasticRealtedProvider != null)
                {
                    return (5 * (ElasticRealtedProvider.CatlificationRating != null ?
                            int.Parse(ElasticRealtedProvider.CatlificationRating) :
                            0) / 100);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Provider rate count for session customer
        /// </summary>
        public int ProviderRateCount
        {
            get
            {
                return 0;
            }
        }


        /// <summary>
        /// Provider alert risk
        /// </summary>
        public BackOffice.Models.General.enumBlackListStatus ProviderAlertRisk
        {
            get
            {
                return ElasticRealtedProvider != null && ElasticRealtedProvider.InBlackList ?
                            BackOffice.Models.General.enumBlackListStatus.ShowAlert :
                            BackOffice.Models.General.enumBlackListStatus.DontShowAlert;
            }
        }

        public BackOffice.Models.General.enumProviderLiteType ProviderLiteType { get; set; }

        public bool ProviderAddRemoveEnable { get; set; }

        public ProviderLiteViewModel(ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel oRelatedProvider)
        {
            RelatedProvider = oRelatedProvider;
        }

        public ProviderLiteViewModel(CompanyIndexModel oElasticSearchModel)
        {
            ElasticRealtedProvider = oElasticSearchModel;
        }

        public ProviderLiteViewModel(CompanySurveyIndexModel oElasticSurveySearchModel)
        {
            ElasticRealtedProvider = new CompanyIndexModel()
            {
                City = oElasticSurveySearchModel.City,
                CityId = oElasticSurveySearchModel.CityId,
                CommercialCompanyName = oElasticSurveySearchModel.CommercialCompanyName,
                CompanyEnable = oElasticSurveySearchModel.CompanyEnable,
                CompanyName = oElasticSurveySearchModel.CompanyName,
                CompanyPublicId = oElasticSurveySearchModel.CompanyPublicId,
                Country = oElasticSurveySearchModel.Country,
                CountryId = oElasticSurveySearchModel.CountryId,
                CustomerPublicId = oElasticSurveySearchModel.CustomerPublicId,
                ICA = oElasticSurveySearchModel.ICA,
                ICAId = oElasticSurveySearchModel.ICAId,
                IdentificationNumber = oElasticSurveySearchModel.IdentificationNumber,
                IdentificationType = oElasticSurveySearchModel.IdentificationType,
                IdentificationTypeId = oElasticSurveySearchModel.IdentificationTypeId,
                InBlackList = oElasticSurveySearchModel.InBlackList,
                LogoUrl = oElasticSurveySearchModel.LogoUrl,
                oCustomerProviderIndexModel = oElasticSurveySearchModel.oCustomerProviderIndexModel,
                PrincipalActivity = oElasticSurveySearchModel.PrincipalActivity,
                PrincipalActivityId = oElasticSurveySearchModel.PrincipalActivityId,
                ProviderStatus = oElasticSurveySearchModel.ProviderStatus,
                ProviderStatusId = oElasticSurveySearchModel.ProviderStatusId,
            };
        }
    }
}
