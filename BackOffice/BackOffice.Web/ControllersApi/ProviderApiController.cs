using BackOffice.Models.General;
using BackOffice.Models.Provider;
using Microsoft.Reporting.WebForms;
using Nest;
using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.CompanyCustomer.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackOffice.Web.ControllersApi
{
    public class ProviderApiController : BaseApiController
    {
        #region Search Methods

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderSearchViewModel> SMProviderSearch
            (string SMProviderSearch,
            string SearchParam,
            string SearchFilter,
            string PageNumber,
            string RowCount)
        {
            string oSearchFilter = string.Join(",", (SearchFilter ?? string.Empty).Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            oSearchFilter = string.IsNullOrEmpty(oSearchFilter) ? null : oSearchFilter;

            List<ProviderSearchViewModel> oReturn = new List<ProviderSearchViewModel>();

            string oCompanyType =
                    ((int)(BackOffice.Models.General.enumCompanyType.Provider)).ToString() + "," +
                    ((int)(BackOffice.Models.General.enumCompanyType.BuyerProvider)).ToString();

            int oPageNumber = string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber.Trim());

            int oRowCount = Convert.ToInt32(string.IsNullOrEmpty(RowCount) ?
                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_Grid_RowCountDefault].Value :
                RowCount.Trim());



            List<Tuple<string, string, string>> lstSearchFilter = new List<Tuple<string, string, string>>();


            #region Search Result Company
            Uri node = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);

            settings.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value);
            settings.DisableDirectStreaming(true);
            ElasticClient client = new ElasticClient(settings);

            Nest.ISearchResponse<CompanyIndexModel> ElasticResult = client.Search<CompanyIndexModel>(s => s
            .From(string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber) * 20)
            .TrackScores(true)
            .Size(20)
            .Aggregations
                (agg => agg
                    .Nested("status_avg", x => x.
                        Path(p => p.oCustomerProviderIndexModel).
                            Aggregations(aggs => aggs.Terms("status", term => term.Field(fi => fi.oCustomerProviderIndexModel.First().StatusId)
                            )
                        )
                    )
                .Terms("ica", aggv => aggv
                    .Field(fi => fi.ICAId))
                .Terms("city", aggv => aggv
                    .Field(fi => fi.CityId))
                .Terms("country", c => c
                    .Field(fi => fi.CountryId))
                .Terms("blacklist", bl => bl
                    .Field(fi => fi.InBlackList)))
            .Query(q => q.
                Filtered(f => f
                .Query(q1 => q1.MatchAll() && q.QueryString(qs => qs.Query(SearchParam)))
                .Filter(f2 =>
                {
                    QueryContainer qb = null;

                    #region Basic Providers Filters

                    if (!string.IsNullOrEmpty(SearchFilter))
                    {
                        List<Tuple<string, string>> oFilterList = new List<Tuple<string, string>>();
                        ProviderSearchViewModel oFilterModel = new ProviderSearchViewModel();
                        oFilterList = oFilterModel.GetlstSearchFilter(SearchFilter);

                        if (oFilterList.Count > 0)
                        {
                            oFilterList.All(i =>
                            {
                                if (i.Item1 == ((int)enumFilterType.ProviderStatus).ToString())
                                {
                                    qb &= q.Nested(n => n
                                   .Path(p => p.oCustomerProviderIndexModel)
                                  .Query(fq => fq
                                      .Match(match => match
                                      .Field(field => field.oCustomerProviderIndexModel.First().StatusId)
                                      .Query(i.Item2)
                                      )
                                    )
                                 );
                                }
                                if (i.Item1 == ((int)enumFilterType.City).ToString())
                                {
                                    qb &= q.Term(m => m.CityId, Convert.ToDouble(SearchFilter.Split(';')[1]));
                                }
                                if (i.Item1 == ((int)enumFilterType.Country).ToString())
                                {
                                    qb &= q.Term(m => m.CountryId, Convert.ToDouble(SearchFilter.Split(';')[1]));
                                }

                                return true;
                            });
                        }

                    }
                    #endregion

                    return qb;
                }
        )
        )));

            #endregion

            if (ElasticResult != null)
            {
                ElasticResult.Documents.All(sr =>
                {
                    oReturn.Add(new Models.Provider.ProviderSearchViewModel(sr, (int)ElasticResult.Total));
                    return true;
                });
            }

            return oReturn;
        }

        #endregion

        #region Generic Info

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderContactViewModel> GIContactGetByType
            (string GIContactGetByType,
            string ProviderPublicId,
            string ContactType,
            string ViewEnable)
        {
            int oTotalRows;
            List<BackOffice.Models.Provider.ProviderContactViewModel> oReturn = new List<Models.Provider.ProviderContactViewModel>();

            if (GIContactGetByType == "true")
            {
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oContact = ProveedoresOnLine.Company.Controller.Company.ContactGetBasicInfo
                    (ProviderPublicId,
                    string.IsNullOrEmpty(ContactType) ? null : (int?)Convert.ToInt32(ContactType.Trim()), Convert.ToBoolean(ViewEnable));

                List<ProveedoresOnLine.Company.Models.Util.GeographyModel> oCities = null;

                if (oContact != null)
                {
                    if (ContactType == ((int)BackOffice.Models.General.enumContactType.Brach).ToString() ||
                        ContactType == ((int)BackOffice.Models.General.enumContactType.Distributor).ToString())
                    {
                        oCities = ProveedoresOnLine.Company.Controller.Company.CategorySearchByGeography(null, null, 0, 0, out oTotalRows);
                    }

                    oContact.All(x =>
                    {
                        oReturn.Add(new BackOffice.Models.Provider.ProviderContactViewModel(x, oCities));
                        return true;
                    });
                }
            }

            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public BackOffice.Models.Provider.ProviderContactViewModel GIContactUpsert
            (string GIContactUpsert,
            string ProviderPublicId,
            string ContactType)
        {
            int oTotalCount;
            BackOffice.Models.Provider.ProviderContactViewModel oReturn = null;

            if (GIContactUpsert == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]) &&
                !string.IsNullOrEmpty(ContactType))
            {
                List<string> lstUsedFiles = new List<string>();

                BackOffice.Models.Provider.ProviderContactViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.ProviderContactViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.ProviderContactViewModel));

                ProveedoresOnLine.Company.Models.Company.CompanyModel oCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                {
                    CompanyPublicId = ProviderPublicId,
                    RelatedContact = new List<ProveedoresOnLine.Company.Models.Util.GenericItemModel>()
                    {
                        new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = string.IsNullOrEmpty(oDataToUpsert.ContactId) ? 0 : Convert.ToInt32(oDataToUpsert.ContactId.Trim()),
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = Convert.ToInt32(ContactType.Trim()),
                            },
                            ItemName = oDataToUpsert.ContactName,
                            Enable = oDataToUpsert.Enable,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        },
                    }
                };

                #region Contact
                if (oCompany.RelatedContact.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumContactType.CompanyContact)
                {
                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CC_ValueId) ? 0 : Convert.ToInt32(oDataToUpsert.CC_ValueId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.CC_Value
                        },
                        Value = oDataToUpsert.CC_Value,
                        Enable = true,
                    });
                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CC_CompanyContactTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.CC_CompanyContactTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.CC_CompanyContactType
                        },
                        Value = oDataToUpsert.CC_CompanyContactType,
                        Enable = true,
                    });
                }
                #endregion

                #region Person Contact
                else if (oCompany.RelatedContact.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumContactType.PersonContact)
                {
                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CP_PersonContactTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.CP_PersonContactTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.CP_PersonContactType
                        },
                        Value = oDataToUpsert.CP_PersonContactType,
                        Enable = true,
                    });



                    lstUsedFiles.Add(oDataToUpsert.CP_IdentificationFile);

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CP_PhoneId) ? 0 : Convert.ToInt32(oDataToUpsert.CP_PhoneId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.CP_Phone
                        },
                        Value = oDataToUpsert.CP_Phone,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CP_EmailId) ? 0 : Convert.ToInt32(oDataToUpsert.CP_EmailId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.CP_Email
                        },
                        Value = oDataToUpsert.CP_Email,
                        Enable = true,
                    });

                }
                #endregion

                #region Branch
                else if (oCompany.RelatedContact.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumContactType.Brach)
                {

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.BR_AddressId) ? 0 : Convert.ToInt32(oDataToUpsert.BR_AddressId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.BR_Address
                        },
                        Value = oDataToUpsert.BR_Address,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.BR_CityId) ? 0 : Convert.ToInt32(oDataToUpsert.BR_CityId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.BR_City
                        },
                        Value = oDataToUpsert.BR_City,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.BR_PhoneId) ? 0 : Convert.ToInt32(oDataToUpsert.BR_PhoneId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.BR_Phone
                        },
                        Value = oDataToUpsert.BR_Phone,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.BR_CellphoneId) ? 0 : Convert.ToInt32(oDataToUpsert.BR_CellphoneId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.BR_Cellphone
                        },
                        Value = oDataToUpsert.BR_Cellphone,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.BR_EmailId) ? 0 : Convert.ToInt32(oDataToUpsert.BR_EmailId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.BR_Email
                        },
                        Value = oDataToUpsert.BR_Email,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.BR_WebsiteId) ? 0 : Convert.ToInt32(oDataToUpsert.BR_WebsiteId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.BR_Website
                        },
                        Value = oDataToUpsert.BR_Website,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.BR_IsPrincipalId) ? 0 : Convert.ToInt32(oDataToUpsert.BR_IsPrincipalId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.BR_IsPrincipal
                        },
                        Value = Convert.ToBoolean(oDataToUpsert.BR_IsPrincipal) == true ? "1" : "0",
                        Enable = true,
                    });


                }
                #endregion

                #region Distributor
                else if (oCompany.RelatedContact.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumContactType.Distributor)
                {
                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.DT_DistributorTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.DT_DistributorTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.DT_DistributorType
                        },
                        Value = oDataToUpsert.DT_DistributorType,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.DT_RepresentativeId) ? 0 : Convert.ToInt32(oDataToUpsert.DT_RepresentativeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.DT_Representative
                        },
                        Value = oDataToUpsert.DT_Representative,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.DT_EmailId) ? 0 : Convert.ToInt32(oDataToUpsert.DT_EmailId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.DT_Email
                        },
                        Value = oDataToUpsert.DT_Email,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.DT_PhoneId) ? 0 : Convert.ToInt32(oDataToUpsert.DT_PhoneId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.DT_Phone
                        },
                        Value = oDataToUpsert.DT_Phone,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.DT_CityId) ? 0 : Convert.ToInt32(oDataToUpsert.DT_CityId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.DT_City
                        },
                        Value = oDataToUpsert.DT_City,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.BR_EmailId) ? 0 : Convert.ToInt32(oDataToUpsert.BR_EmailId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.BR_Email
                        },
                        Value = oDataToUpsert.BR_Email,
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.DT_DateIssueId) ? 0 : Convert.ToInt32(oDataToUpsert.DT_DateIssueId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.DT_DateIssue
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.DT_DateIssue) ?
                            string.Empty :
                            oDataToUpsert.DT_DateIssue.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.DT_DateIssue :
                            DateTime.ParseExact(
                                oDataToUpsert.DT_DateIssue,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = true,
                    });

                    oCompany.RelatedContact.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.DT_DistributorFileId) ? 0 : Convert.ToInt32(oDataToUpsert.DT_DistributorFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumContactInfoType.DT_DistributorFile
                        },
                        Value = oDataToUpsert.DT_DistributorFile,
                        Enable = true,
                    });

                    lstUsedFiles.Add(oDataToUpsert.DT_DistributorFile);
                }
                #endregion

                oCompany = ProveedoresOnLine.Company.Controller.Company.ContactUpsert(oCompany);

                //eval company partial index
                List<int> InfoTypeModified = new List<int>();

                oCompany.RelatedContact.All(x =>
                {
                    InfoTypeModified.AddRange(x.ItemInfo.Select(y => y.ItemInfoType.ItemId));
                    return true;
                });

                //ProveedoresOnLine.Company.Controller.Company.CompanyPartialIndex(oCompany.CompanyPublicId, InfoTypeModified);


                List<ProveedoresOnLine.Company.Models.Util.GeographyModel> oCities = null;

                if (ContactType == ((int)BackOffice.Models.General.enumContactType.Brach).ToString() ||
                    ContactType == ((int)BackOffice.Models.General.enumContactType.Distributor).ToString())
                {
                    oCities = ProveedoresOnLine.Company.Controller.Company.CategorySearchByGeography(null, null, 0, 0, out oTotalCount);
                }

                oReturn = new Models.Provider.ProviderContactViewModel(oCompany.RelatedContact.FirstOrDefault(), oCities);

                //register used files
                LogManager.ClientLog.FileUsedCreate(lstUsedFiles);

                #region Index


                #region CompanyIndex


                Uri node = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);

                settings.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value);
                settings.DisableDirectStreaming(true);
                ElasticClient client = new ElasticClient(settings);

                //Getting Model from index
                Nest.ISearchResponse<CompanyIndexModel> oResult = client.Search<CompanyIndexModel>(s => s
                    .From(0)
                    .Size(1)
                    .Query(q => q.QueryString(qs => qs.Query(ProviderPublicId))));

                //Model to index 
                #region Model

                CompanyIndexModel oCompanyIndexModel = new CompanyIndexModel()
                {
                    CatlificationRating = oResult.Documents.FirstOrDefault().CatlificationRating,

                    City = oResult.Documents.FirstOrDefault().City,
                    CityId = oResult.Documents.FirstOrDefault().CityId,

                    CommercialCompanyName = oResult.Documents.FirstOrDefault().CommercialCompanyName,

                    CompanyEnable = oResult.Documents.FirstOrDefault().CompanyEnable,

                    CompanyName = oResult.Documents.FirstOrDefault().CompanyName,

                    CompanyPublicId = oResult.Documents.FirstOrDefault().CompanyPublicId,

                    Country = oResult.Documents.FirstOrDefault().Country,
                    CountryId = oResult.Documents.FirstOrDefault().CountryId,
                    CustomerPublicId = oResult.Documents.FirstOrDefault().CustomerPublicId,

                    ICA = oResult.Documents.FirstOrDefault().ICA,
                    ICAId = oResult.Documents.FirstOrDefault().ICAId,

                    IdentificationNumber = oResult.Documents.FirstOrDefault().IdentificationNumber,


                    IdentificationType = oResult.Documents.FirstOrDefault().IdentificationType,
                    IdentificationTypeId = oResult.Documents.FirstOrDefault().IdentificationTypeId,

                    InBlackList = oResult.Documents.FirstOrDefault().InBlackList,

                    LogoUrl = oResult.Documents.FirstOrDefault().LogoUrl,

                    oCustomerProviderIndexModel = oResult.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                    PrincipalActivity = oResult.Documents.FirstOrDefault().PrincipalActivity,
                    PrincipalActivityId = oResult.Documents.FirstOrDefault().PrincipalActivityId,

                    ProviderStatus = oResult.Documents.FirstOrDefault().ProviderStatus,
                    ProviderStatusId = oResult.Documents.FirstOrDefault().ProviderStatusId,
                };

                #endregion

                if (oDataToUpsert.BR_IsPrincipal == true && oDataToUpsert.Enable == true)
                {
                    oCompanyIndexModel.CityId = Convert.ToInt32(oDataToUpsert.BR_City);
                    oCompanyIndexModel.City = oDataToUpsert.BR_CityName;
                    oCompanyIndexModel.CountryId = oCities.Where(x => x.City.ItemId == Convert.ToInt32(oDataToUpsert.BR_City)).Select(y => y.Country.ItemId).DefaultIfEmpty(0).FirstOrDefault();
                    oCompanyIndexModel.Country = oCities.Where(x => x.City.ItemId == Convert.ToInt32(oDataToUpsert.BR_City)).Select(y => y.Country.ItemName).FirstOrDefault();

                    ICreateIndexResponse oElasticResponse = client.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                        .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                            .Tokenizer("whitespace")
                            )).TokenFilters(tf => tf
                                    .EdgeNGram("customEdgeNGram", engrf => engrf
                                    .MinGram(1)
                                    .MaxGram(10)))).NumberOfShards(1)
                        ));

                    var Index = client.Index(oCompanyIndexModel);
                }
                else
                {
                    oCompanyIndexModel.CityId = 0;
                    oCompanyIndexModel.City = string.Empty;
                    oCompanyIndexModel.CountryId = 0;
                    oCompanyIndexModel.Country = string.Empty;

                    ICreateIndexResponse oElasticResponse = client.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                        .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                            .Tokenizer("whitespace")
                            )).TokenFilters(tf => tf
                                    .EdgeNGram("customEdgeNGram", engrf => engrf
                                    .MinGram(1)
                                    .MaxGram(10)))).NumberOfShards(1)
                        ));

                    var Index = client.Index(oCompanyIndexModel);
                }


                #endregion

                #region SurveyIndex
                Uri node2 = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings2 = new ConnectionSettings(node2);

                settings2.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanySurveyIndex].Value);
                settings2.DisableDirectStreaming(true);
                ElasticClient client2 = new ElasticClient(settings2);

                //Getting Model from index
                Nest.ISearchResponse<ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel> oResult2 = client2.Search<ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel>(s => s
                  .From(0)
                  .Size(1)
                  .Query(q => q.QueryString(qs => qs.Query(ProviderPublicId))));

                if (oResult2.Documents != null)
                {
                    if (oResult2.Documents.Count() > 0)
                    {
                        //Model to index 
                        #region Model

                        ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel oCompanySurveyIndexModel = new ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel()
                        {
                            CatlificationRating = oResult2.Documents.FirstOrDefault().CatlificationRating,

                            City = oResult2.Documents.FirstOrDefault().City,
                            CityId = oResult2.Documents.FirstOrDefault().CityId,

                            CommercialCompanyName = oResult2.Documents.FirstOrDefault().CommercialCompanyName,

                            CompanyEnable = oResult2.Documents.FirstOrDefault().CompanyEnable,

                            CompanyName = oResult2.Documents.FirstOrDefault().CompanyName,

                            CompanyPublicId = oResult2.Documents.FirstOrDefault().CompanyPublicId,

                            Country = oResult2.Documents.FirstOrDefault().Country,
                            CountryId = oResult2.Documents.FirstOrDefault().CountryId,
                            CustomerPublicId = oResult2.Documents.FirstOrDefault().CustomerPublicId,

                            ICA = oResult2.Documents.FirstOrDefault().ICA,
                            ICAId = oResult2.Documents.FirstOrDefault().ICAId,

                            IdentificationNumber = oResult2.Documents.FirstOrDefault().IdentificationNumber,


                            IdentificationType = oResult2.Documents.FirstOrDefault().IdentificationType,
                            IdentificationTypeId = oResult2.Documents.FirstOrDefault().IdentificationTypeId,

                            InBlackList = oResult2.Documents.FirstOrDefault().InBlackList,

                            LogoUrl = oResult2.Documents.FirstOrDefault().LogoUrl,

                            oCustomerProviderIndexModel = oResult2.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                            PrincipalActivity = oResult2.Documents.FirstOrDefault().PrincipalActivity,
                            PrincipalActivityId = oResult2.Documents.FirstOrDefault().PrincipalActivityId,

                            ProviderStatus = oResult2.Documents.FirstOrDefault().ProviderStatus,
                            ProviderStatusId = oResult2.Documents.FirstOrDefault().ProviderStatusId,
                        };

                        #endregion

                        if (oDataToUpsert.BR_IsPrincipal == true && oDataToUpsert.Enable == true)
                        {
                            oCompanySurveyIndexModel.CityId = Convert.ToInt32(oDataToUpsert.BR_City);
                            oCompanySurveyIndexModel.City = oDataToUpsert.BR_CityName;
                            oCompanySurveyIndexModel.CountryId = oCities.Where(x => x.City.ItemId == Convert.ToInt32(oDataToUpsert.BR_City)).Select(y => y.Country.ItemId).DefaultIfEmpty(0).FirstOrDefault();
                            oCompanySurveyIndexModel.Country = oCities.Where(x => x.City.ItemId == Convert.ToInt32(oDataToUpsert.BR_City)).Select(y => y.Country.ItemName).FirstOrDefault();

                            ICreateIndexResponse oElasticResponse2 = client2.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                                .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                                .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                                    .Tokenizer("whitespace")
                                    )).TokenFilters(tf => tf
                                            .EdgeNGram("customEdgeNGram", engrf => engrf
                                            .MinGram(1)
                                            .MaxGram(10)))).NumberOfShards(1)
                                ));

                            var Index = client2.Index(oCompanySurveyIndexModel);
                        }
                        else
                        {
                            oCompanySurveyIndexModel.CityId = 0;
                            oCompanySurveyIndexModel.City = string.Empty;
                            oCompanySurveyIndexModel.CountryId = 0;
                            oCompanySurveyIndexModel.Country = string.Empty;

                            ICreateIndexResponse oElasticResponse2 = client2.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                                .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                                .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                                    .Tokenizer("whitespace")
                                    )).TokenFilters(tf => tf
                                            .EdgeNGram("customEdgeNGram", engrf => engrf
                                            .MinGram(1)
                                            .MaxGram(10)))).NumberOfShards(1)
                                ));

                            var Index = client2.Index(oCompanySurveyIndexModel);
                        }

                    }
                }
                #endregion


                #endregion
            }
            return oReturn;
        }

        #endregion

        #region Commercial Info

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderCommercialViewModel> CICommercialGetByType
            (string CICommercialGetByType,
            string ProviderPublicId,
            string CommercialType,
            string ViewEnable)
        {
            List<BackOffice.Models.Provider.ProviderCommercialViewModel> oReturn = new List<Models.Provider.ProviderCommercialViewModel>();

            if (CICommercialGetByType == "true")
            {
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oCommercial = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CommercialGetBasicInfo
                    (ProviderPublicId,
                    string.IsNullOrEmpty(CommercialType) ? null : (int?)Convert.ToInt32(CommercialType.Trim()), Convert.ToBoolean(ViewEnable));

                if (oCommercial != null)
                {
                    oCommercial.All(x =>
                    {
                        oReturn.Add(new BackOffice.Models.Provider.ProviderCommercialViewModel(x));
                        return true;
                    });
                }
            }

            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public BackOffice.Models.Provider.ProviderCommercialViewModel CICommercialUpsert
            (string CICommercialUpsert,
            string ProviderPublicId,
            string CommercialType)
        {
            BackOffice.Models.Provider.ProviderCommercialViewModel oReturn = null;

            if (CICommercialUpsert == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]) &&
                !string.IsNullOrEmpty(CommercialType))
            {
                List<string> lstUsedFiles = new List<string>();

                BackOffice.Models.Provider.ProviderCommercialViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.ProviderCommercialViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.ProviderCommercialViewModel));


                ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel oProvider = new ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel()
                {
                    RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                    {
                        CompanyPublicId = ProviderPublicId,
                    },

                    RelatedCommercial = new List<ProveedoresOnLine.Company.Models.Util.GenericItemModel>()
                    {
                        new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = string.IsNullOrEmpty(oDataToUpsert.CommercialId) ? 0 : Convert.ToInt32(oDataToUpsert.CommercialId.Trim()),
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = Convert.ToInt32(CommercialType.Trim()),
                            },
                            ItemName = oDataToUpsert.CommercialName,
                            Enable = oDataToUpsert.Enable,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        },
                    },
                };

                if (oProvider.RelatedCommercial.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumCommercialType.Experience)
                {
                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_ContractTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_ContractTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_ContractType
                        },
                        Value = oDataToUpsert.EX_ContractType,
                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_DateIssueId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_DateIssueId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_DateIssue
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.EX_DateIssue) ?
                            string.Empty :
                            oDataToUpsert.EX_DateIssue.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.EX_DateIssue :
                            DateTime.ParseExact(
                                oDataToUpsert.EX_DateIssue,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),

                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_DueDateId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_DueDateId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_DueDate
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.EX_DueDate) ?
                            string.Empty :
                            oDataToUpsert.EX_DueDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.EX_DueDate :
                            DateTime.ParseExact(
                                oDataToUpsert.EX_DueDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),

                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_ClientId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_ClientId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_Client
                        },
                        Value = oDataToUpsert.EX_Client,
                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_ContractNumberId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_ContractNumberId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_ContractNumber
                        },
                        Value = oDataToUpsert.EX_ContractNumber,
                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_CurrencyId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_CurrencyId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_Currency
                        },
                        Value = oDataToUpsert.EX_Currency,
                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_ContractValueId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_ContractValueId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_ContractValue
                        },
                        Value = !string.IsNullOrEmpty(oDataToUpsert.EX_ContractValue) ? (Convert.ToDecimal(oDataToUpsert.EX_ContractValue, System.Globalization.CultureInfo.InvariantCulture)).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) : string.Empty,
                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_PhoneId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_PhoneId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_Phone
                        },
                        Value = oDataToUpsert.EX_Phone,
                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_ExperienceFileId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_ExperienceFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_ExperienceFile
                        },
                        Value = oDataToUpsert.EX_ExperienceFile,
                        Enable = true,
                    });

                    lstUsedFiles.Add(oDataToUpsert.EX_ExperienceFile);

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_ContractSubjectId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_ContractSubjectId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_ContractSubject
                        },
                        LargeValue = oDataToUpsert.EX_ContractSubject,
                        Enable = true,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_EconomicActivityId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_EconomicActivityId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_EconomicActivity
                        },
                        LargeValue = oDataToUpsert.EX_EconomicActivity != null ? string.Join(",", oDataToUpsert.EX_EconomicActivity.Select(x => x.EconomicActivityId.Trim()).Distinct().ToList()) : string.Empty,
                        Enable = true,

                        ValueName = oDataToUpsert.EX_EconomicActivity != null ? string.Join(";", oDataToUpsert.EX_EconomicActivity.Select(x => x.EconomicActivityId.Trim() + "," + x.ActivityName.Trim()).Distinct().ToList()) : string.Empty,
                    });

                    oProvider.RelatedCommercial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.EX_CustomEconomicActivityId) ? 0 : Convert.ToInt32(oDataToUpsert.EX_CustomEconomicActivityId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumCommercialInfoType.EX_CustomEconomicActivity
                        },
                        LargeValue = oDataToUpsert.EX_CustomEconomicActivity != null ? string.Join(",", oDataToUpsert.EX_CustomEconomicActivity.Select(x => x.EconomicActivityId.Trim()).Distinct().ToList()) : string.Empty,
                        Enable = true,

                        ValueName = oDataToUpsert.EX_CustomEconomicActivity != null ? string.Join(";", oDataToUpsert.EX_CustomEconomicActivity.Select(x => x.EconomicActivityId.Trim() + "," + x.ActivityName.Trim()).Distinct().ToList()) : string.Empty,
                    });
                }

                oProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CommercialUpsert(oProvider);

                //eval company partial index
                List<int> InfoTypeModified = new List<int>() { 3 };

                oProvider.RelatedCommercial.All(x =>
                {
                    InfoTypeModified.AddRange(x.ItemInfo.Select(y => y.ItemInfoType.ItemId));
                    return true;
                });

                //ProveedoresOnLine.Company.Controller.Company.CompanyPartialIndex(oProvider.RelatedCompany.CompanyPublicId, InfoTypeModified);

                oReturn = new Models.Provider.ProviderCommercialViewModel
                    (oProvider.RelatedCommercial.FirstOrDefault());

                //register used files
                LogManager.ClientLog.FileUsedCreate(lstUsedFiles);
            }
            return oReturn;
        }

        #endregion

        #region HSEQ
        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderHSEQViewModel> HIHSEQGetByType
            (string HIHSEQGetByType,
            string ProviderPublicId,
            string HSEQType,
            string ViewEnable)
        {
            List<BackOffice.Models.Provider.ProviderHSEQViewModel> oReturn = new List<Models.Provider.ProviderHSEQViewModel>();

            if (HIHSEQGetByType == "true")
            {
                var oViewEnable = ViewEnable;
                if (HSEQType == ((int)BackOffice.Models.General.enumHSEQType.CompanyHealtyPolitic).ToString())
                {
                    ViewEnable = "true";
                }

                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oCertification = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CertficationGetBasicInfo
                    (ProviderPublicId,
                    string.IsNullOrEmpty(HSEQType) ? null : (int?)Convert.ToInt32(HSEQType.Trim()), Convert.ToBoolean(ViewEnable));

                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oRule = null;
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oCompanyRule = null;
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oARL = null;

                if (oCertification != null)
                {
                    List<int> ArrayDocuments = new List<int>();
                    if (HSEQType == ((int)BackOffice.Models.General.enumHSEQType.Certifications).ToString())
                    {
                        oRule = ProveedoresOnLine.Company.Controller.Company.CategorySearchByRules(null, 0, 0);
                        oCompanyRule = ProveedoresOnLine.Company.Controller.Company.CategorySearchByCompanyRules(null, 0, 0);
                        oCertification.All(x =>
                        {
                            oReturn.Add(new BackOffice.Models.Provider.ProviderHSEQViewModel(x, oRule, oCompanyRule, oARL, 0));
                            return true;
                        });
                    }
                    else if (HSEQType == ((int)BackOffice.Models.General.enumHSEQType.CompanyRiskPolicies).ToString())
                    {
                        oARL = ProveedoresOnLine.Company.Controller.Company.CategorySearchByARLCompany(null, 0, 0);
                        oCertification.All(x =>
                        {
                            oReturn.Add(new BackOffice.Models.Provider.ProviderHSEQViewModel(x, oRule, oCompanyRule, oARL, 0));
                            return true;
                        });
                    }
                    else if (HSEQType == ((int)BackOffice.Models.General.enumHSEQType.CompanyHealtyPolitic).ToString())
                    {


                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_PoliticsSecurity);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_PoliticsNoAlcohol);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_ProgramOccupationalHealth);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_RuleIndustrialSecurity);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_MatrixRiskControl);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_CorporateSocialResponsability);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_ProgramEnterpriseSecurity);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_PoliticsRecruiment);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_CertificationsForm);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_PoliticIntegral);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_Other);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_EnvironmentalManagement);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_PoliticsSalary);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_ImplementationDangerPsicosocial);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_CertificationBeneficialExtralegal);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_SupportOfHoursExtras);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_SupportOfHoursRecreation);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_PoliticCompensation);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_PoliticDDHH);
                        ArrayDocuments.Add((int)BackOffice.Models.General.enumHSEQInfoType.CH_ReportSustainableAudit);

                        oCertification.All(x =>
                        {
                            x.ItemInfo.
                                Where(y => ArrayDocuments.Count(z => z == y.ItemInfoType.ItemId) > 0).All(z =>
                                {
                                    if (z.Enable.ToString().ToUpper() == oViewEnable.ToUpper())
                                    {
                                        oReturn.Add(new BackOffice.Models.Provider.ProviderHSEQViewModel(x, oRule, oCompanyRule, oARL, z.ItemInfoType.ItemId));
                                    }
                                    return true;
                                });

                            return true;
                        });
                    }
                    else
                    {
                        oCertification.All(x =>
                        {
                            oReturn.Add(new BackOffice.Models.Provider.ProviderHSEQViewModel(x, oRule, oCompanyRule, oARL, 0));
                            return true;
                        });
                    }

                }
            }

            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public BackOffice.Models.Provider.ProviderHSEQViewModel HIHSEQUpsert
            (string HIHSEQUpsert,
            string ProviderPublicId,
            string HSEQType)
        {
            BackOffice.Models.Provider.ProviderHSEQViewModel oReturn = null;

            int TypeDocumentId = 0;

            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oRule = null;
            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oCompanyRule = null;
            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oARL = null;


            if (HSEQType == ((int)BackOffice.Models.General.enumHSEQType.Certifications).ToString())
            {
                oRule = ProveedoresOnLine.Company.Controller.Company.CategorySearchByRules(null, 0, 0);
                oCompanyRule = ProveedoresOnLine.Company.Controller.Company.CategorySearchByCompanyRules(null, 0, 0);
            }
            else if (HSEQType == ((int)BackOffice.Models.General.enumHSEQType.CompanyRiskPolicies).ToString())
            {
                oARL = ProveedoresOnLine.Company.Controller.Company.CategorySearchByARLCompany(null, 0, 0);
            }



            if (HIHSEQUpsert == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]) &&
                !string.IsNullOrEmpty(HSEQType))
            {
                List<string> lstUsedFiles = new List<string>();

                BackOffice.Models.Provider.ProviderHSEQViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.ProviderHSEQViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.ProviderHSEQViewModel));

                TypeDocumentId = string.IsNullOrEmpty(oDataToUpsert.CH_TypeDocument) ? 0 : Convert.ToInt32(oDataToUpsert.CH_TypeDocument);

                ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel oProvider = new ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel()
                {
                    RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                    {
                        CompanyPublicId = ProviderPublicId,
                    },
                    RelatedCertification = new List<ProveedoresOnLine.Company.Models.Util.GenericItemModel>()
                    {
                        new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = string.IsNullOrEmpty(oDataToUpsert.CertificationId) ? 0 : Convert.ToInt32(oDataToUpsert.CertificationId.Trim()),
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = Convert.ToInt32(HSEQType.Trim()),
                            },
                            ItemName = oDataToUpsert.CertificationName,
                            Enable = oDataToUpsert.Enable,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        },
                    },
                };

                if (oProvider.RelatedCertification.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumHSEQType.Certifications)
                {
                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.C_CertificationCompanyId) ? 0 : Convert.ToInt32(oDataToUpsert.C_CertificationCompanyId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.C_CertificationCompany
                        },
                        Value = oDataToUpsert.C_CertificationCompany,
                        Enable = true,
                    });
                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.C_RuleId) ? 0 : Convert.ToInt32(oDataToUpsert.C_RuleId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.C_Rule
                        },
                        Value = oDataToUpsert.C_Rule,
                        Enable = true,
                    });
                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.C_StartDateCertificationId) ? 0 : Convert.ToInt32(oDataToUpsert.C_StartDateCertificationId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.C_StartDateCertification
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.C_StartDateCertification) ?
                            string.Empty :
                            oDataToUpsert.C_StartDateCertification.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.C_StartDateCertification :
                            DateTime.ParseExact(oDataToUpsert.C_StartDateCertification, BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value, System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = true,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.C_EndDateCertificationId) ? 0 : Convert.ToInt32(oDataToUpsert.C_EndDateCertificationId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.C_EndDateCertification
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.C_EndDateCertification) ?
                            string.Empty :
                            oDataToUpsert.C_EndDateCertification.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.C_EndDateCertification :
                            DateTime.ParseExact(oDataToUpsert.C_EndDateCertification, BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value, System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = true,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.C_CertificationFileId) ? 0 : Convert.ToInt32(oDataToUpsert.C_CertificationFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.C_CertificationFile
                        },
                        Value = oDataToUpsert.C_CertificationFile,
                        Enable = true,
                    });

                    lstUsedFiles.Add(oDataToUpsert.C_CertificationFile);

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.C_ScopeId) ? 0 : Convert.ToInt32(oDataToUpsert.C_ScopeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.C_Scope
                        },
                        LargeValue = oDataToUpsert.C_Scope,
                        Enable = true,
                    });
                }
                else if (oProvider.RelatedCertification.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumHSEQType.CompanyHealtyPolitic)
                {
                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CH_YearId) ? 0 : Convert.ToInt32(oDataToUpsert.CH_YearId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CH_Year
                        },
                        Value = oDataToUpsert.CH_Year,
                        Enable = oDataToUpsert.Enable,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CH_DocumentId) ? 0 : Convert.ToInt32(oDataToUpsert.CH_DocumentId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = Convert.ToInt32(oDataToUpsert.CH_TypeDocument)
                        },
                        Value = oDataToUpsert.CH_Document,
                        LargeValue = oDataToUpsert.CH_Other,
                        Enable = oDataToUpsert.Enable,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().Enable = true;
                }
                else if (oProvider.RelatedCertification.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumHSEQType.CompanyRiskPolicies)
                {
                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CR_SystemOccupationalHazardsId) ? 0 : Convert.ToInt32(oDataToUpsert.CR_SystemOccupationalHazardsId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CR_SystemOccupationalHazards
                        },
                        Value = oDataToUpsert.CR_SystemOccupationalHazards,
                        Enable = true,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CR_RateARLId) ? 0 : Convert.ToInt32(oDataToUpsert.CR_RateARLId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CR_RateARL
                        },
                        Value = oDataToUpsert.CR_RateARL,
                        Enable = true,
                    });

                }
                else if (oProvider.RelatedCertification.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumHSEQType.CertificatesAccident)
                {

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CA_CertificateAccidentARLId) ? 0 : Convert.ToInt32(oDataToUpsert.CA_CertificateAccidentARLId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CA_CertificateAccidentARL
                        },
                        Value = oDataToUpsert.CA_CertificateAccidentARL,
                        Enable = true,
                    });

                    lstUsedFiles.Add(oDataToUpsert.CA_CertificateAccidentARL);

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CA_YearId) ? 0 : Convert.ToInt32(oDataToUpsert.CA_YearId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CA_Year
                        },
                        Value = oDataToUpsert.CA_Year,
                        Enable = true,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CA_ManHoursWorkedId) ? 0 : Convert.ToInt32(oDataToUpsert.CA_ManHoursWorkedId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CA_ManHoursWorked
                        },
                        Value = oDataToUpsert.CA_ManHoursWorked,
                        Enable = true,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CA_FatalitiesId) ? 0 : Convert.ToInt32(oDataToUpsert.CA_FatalitiesId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CA_Fatalities
                        },
                        Value = oDataToUpsert.CA_Fatalities,
                        Enable = true,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CA_NumberAccidentId) ? 0 : Convert.ToInt32(oDataToUpsert.CA_NumberAccidentId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CA_NumberAccident
                        },
                        Value = oDataToUpsert.CA_NumberAccident,
                        Enable = true,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CA_NumberAccidentDisablingId) ? 0 : Convert.ToInt32(oDataToUpsert.CA_NumberAccidentDisablingId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CA_NumberAccidentDisabling
                        },
                        Value = oDataToUpsert.CA_NumberAccidentDisabling,
                        Enable = true,
                    });

                    oProvider.RelatedCertification.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CA_DaysIncapacityId) ? 0 : Convert.ToInt32(oDataToUpsert.CA_DaysIncapacityId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumHSEQInfoType.CA_DaysIncapacity
                        },
                        Value = oDataToUpsert.CA_DaysIncapacity,
                        Enable = true,
                    });
                }

                oProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CertificationUpsert(oProvider);

                //eval company partial index
                List<int> InfoTypeModified = new List<int>() { 7 };

                oProvider.RelatedCertification.All(x =>
                {
                    InfoTypeModified.AddRange(x.ItemInfo.Select(y => y.ItemInfoType.ItemId));
                    return true;
                });

                //ProveedoresOnLine.Company.Controller.Company.CompanyPartialIndex(oProvider.RelatedCompany.CompanyPublicId, InfoTypeModified);

                //register used files
                LogManager.ClientLog.FileUsedCreate(lstUsedFiles);

                oReturn = new Models.Provider.ProviderHSEQViewModel(oProvider.RelatedCertification.FirstOrDefault(), oRule, oCompanyRule, oARL, TypeDocumentId);
            }


            return oReturn;
        }

        #endregion

        #region Finantial Info

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderFinancialViewModel> FIFinancialGetByType
            (string FIFinancialGetByType,
            string ProviderPublicId,
            string FinancialType,
            string ViewEnable)
        {
            List<BackOffice.Models.Provider.ProviderFinancialViewModel> oReturn = new List<Models.Provider.ProviderFinancialViewModel>();

            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oBank = null;

            if (FIFinancialGetByType == "true")
            {
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oFinancial =
                ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.FinancialGetBasicInfo
                    (ProviderPublicId, string.IsNullOrEmpty(FinancialType) ? null : (int?)Convert.ToInt32(FinancialType.Trim()), Convert.ToBoolean(ViewEnable));

                if (FinancialType == ((int)BackOffice.Models.General.enumFinancialType.BankInfoType).ToString())
                {
                    oBank = ProveedoresOnLine.Company.Controller.Company.CategorySearchByBank(null, 0, 0);
                }

                if (oFinancial != null)
                {
                    oFinancial.All(x =>
                    {
                        oReturn.Add(new BackOffice.Models.Provider.ProviderFinancialViewModel(x, oBank));
                        return true;
                    });
                }
            }

            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderBalanceSheetViewModel> FIBalanceSheetGetByFinancial
            (string FIBalanceSheetGetByFinancial,
            string FinancialId)
        {
            List<BackOffice.Models.Provider.ProviderBalanceSheetViewModel> oReturn = new List<Models.Provider.ProviderBalanceSheetViewModel>();

            if (FIBalanceSheetGetByFinancial == "true")
            {
                //get account info
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> olstAccount =
                    ProveedoresOnLine.Company.Controller.Company.CategoryGetFinantialAccounts();

                List<ProveedoresOnLine.CompanyProvider.Models.Provider.BalanceSheetDetailModel> olstBalanceSheetDetail = null;

                if (!string.IsNullOrEmpty(FinancialId))
                    olstBalanceSheetDetail = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.BalanceSheetGetByFinancial(Convert.ToInt32(FinancialId));

                if (olstBalanceSheetDetail == null)
                    olstBalanceSheetDetail = new List<ProveedoresOnLine.CompanyProvider.Models.Provider.BalanceSheetDetailModel>();

                if (olstAccount != null && olstAccount.Count > 0)
                {
                    oReturn = GetBalanceSheetViewModel(null, olstAccount, olstBalanceSheetDetail);
                }
            }

            return oReturn;
        }

        //recursive hierarchy get account
        private List<BackOffice.Models.Provider.ProviderBalanceSheetViewModel> GetBalanceSheetViewModel
            (ProveedoresOnLine.Company.Models.Util.GenericItemModel RelatedAccount,
            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> lstAccount,
            List<ProveedoresOnLine.CompanyProvider.Models.Provider.BalanceSheetDetailModel> lstBalanceSheetDetail)
        {
            List<BackOffice.Models.Provider.ProviderBalanceSheetViewModel> oReturn = new List<Models.Provider.ProviderBalanceSheetViewModel>();

            lstAccount.
                Where(ac => RelatedAccount != null ?
                        (ac.ParentItem != null && ac.ParentItem.ItemId == RelatedAccount.ItemId) :
                        (ac.ParentItem == null)).
                OrderBy(ac => ac.ItemInfo.
                    Where(aci => aci.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumCategoryInfoType.AI_Order).
                    Select(aci => Convert.ToInt32(aci.Value)).
                    DefaultIfEmpty(0).
                    FirstOrDefault()).
                All(ac =>
                {
                    BackOffice.Models.Provider.ProviderBalanceSheetViewModel oItemToAdd =
                        new Models.Provider.ProviderBalanceSheetViewModel
                            (ac,
                            lstBalanceSheetDetail.Where(bsd => bsd.RelatedAccount.ItemId == ac.ItemId).FirstOrDefault());

                    oItemToAdd.ChildBalanceSheet = GetBalanceSheetViewModel(ac, lstAccount, lstBalanceSheetDetail);
                    oReturn.Add(oItemToAdd);

                    return true;
                });

            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public BackOffice.Models.Provider.ProviderFinancialViewModel FIFinancialUpsert
            (string FIFinancialUpsert,
            string ProviderPublicId,
            string FinancialType)
        {
            BackOffice.Models.Provider.ProviderFinancialViewModel oReturn = null;
            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oBank = null;
            if (FIFinancialUpsert == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]) &&
                !string.IsNullOrEmpty(FinancialType))
            {
                List<string> lstUsedFiles = new List<string>();

                BackOffice.Models.Provider.ProviderFinancialViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.ProviderFinancialViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.ProviderFinancialViewModel));

                ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel oProvider = new ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel()
                {
                    RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                    {
                        CompanyPublicId = ProviderPublicId,
                    },
                    RelatedFinantial = new List<ProveedoresOnLine.Company.Models.Util.GenericItemModel>()
                    {
                        new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = string.IsNullOrEmpty(oDataToUpsert.FinancialId) ? 0 : Convert.ToInt32(oDataToUpsert.FinancialId.Trim()),
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = Convert.ToInt32(FinancialType.Trim()),
                            },
                            ItemName = oDataToUpsert.FinancialName,
                            Enable = oDataToUpsert.Enable,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        },
                    },
                };

                if (oProvider.RelatedFinantial.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumFinancialType.IncomeStatementInfoType)
                {
                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IS_YearId) ? 0 : Convert.ToInt32(oDataToUpsert.IS_YearId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IS_Year,
                        },
                        Value = oDataToUpsert.IS_Year,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IS_GrossIncomeId) ? 0 : Convert.ToInt32(oDataToUpsert.IS_GrossIncomeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IS_GrossIncome,
                        },
                        Value = oDataToUpsert.IS_GrossIncome,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IS_NetIncomeId) ? 0 : Convert.ToInt32(oDataToUpsert.IS_NetIncomeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IS_NetIncome,
                        },
                        Value = oDataToUpsert.IS_NetIncome,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IS_GrossEstateId) ? 0 : Convert.ToInt32(oDataToUpsert.IS_GrossEstateId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IS_GrossEstate,
                        },
                        Value = oDataToUpsert.IS_GrossEstate,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IS_LiquidHeritageId) ? 0 : Convert.ToInt32(oDataToUpsert.IS_LiquidHeritageId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IS_LiquidHeritage,
                        },
                        Value = oDataToUpsert.IS_LiquidHeritage,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IS_FileIncomeStatementId) ? 0 : Convert.ToInt32(oDataToUpsert.IS_FileIncomeStatementId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IS_FileIncomeStatement,
                        },
                        Value = oDataToUpsert.IS_FileIncomeStatement,
                        Enable = true,
                    });

                    lstUsedFiles.Add(oDataToUpsert.IS_FileIncomeStatement);
                }
                else if (oProvider.RelatedFinantial.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumFinancialType.TaxInfoType)
                {
                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.TX_YearId) ? 0 : Convert.ToInt32(oDataToUpsert.TX_YearId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.TX_Year,
                        },
                        Value = oDataToUpsert.TX_Year,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.TX_TaxFileId) ? 0 : Convert.ToInt32(oDataToUpsert.TX_TaxFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.TX_TaxFile,
                        },
                        Value = oDataToUpsert.TX_TaxFile,
                        Enable = true,
                    });
                    lstUsedFiles.Add(oDataToUpsert.TX_TaxFile);
                }
                else if (oProvider.RelatedFinantial.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumFinancialType.BankInfoType)
                {

                    oBank = ProveedoresOnLine.Company.Controller.Company.CategorySearchByBank(null, 0, 0);

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_BankId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_BankId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_Bank,
                        },
                        Value = oDataToUpsert.IB_Bank,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_AccountTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_AccountTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_AccountType,
                        },
                        Value = oDataToUpsert.IB_AccountType,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_AccountNumberId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_AccountNumberId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_AccountNumber,
                        },
                        Value = oDataToUpsert.IB_AccountNumber,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_AccountHolderId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_AccountHolderId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_AccountHolder,
                        },
                        Value = oDataToUpsert.IB_AccountHolder,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_ABAId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_ABAId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_ABA,
                        },
                        Value = oDataToUpsert.IB_ABA,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_SwiftId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_SwiftId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_Swift,
                        },
                        Value = oDataToUpsert.IB_Swift,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_IBANId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_IBANId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_IBAN,
                        },
                        Value = oDataToUpsert.IB_IBAN,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_CustomerId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_CustomerId),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_Customer,
                        },
                        Value = oDataToUpsert.IB_Customer,
                        Enable = true,
                    });

                    oProvider.RelatedFinantial.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.IB_AccountFileId) ? 0 : Convert.ToInt32(oDataToUpsert.IB_AccountFileId),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumFinancialInfoType.IB_AccountFile,
                        },
                        Value = oDataToUpsert.IB_AccountFile,
                        Enable = true,
                    });

                    lstUsedFiles.Add(oDataToUpsert.IB_AccountFile);
                }

                oProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.FinancialUpsert(oProvider);

                //eval company partial index
                List<int> InfoTypeModified = new List<int>() { 5 };

                oProvider.RelatedFinantial.All(x =>
                {
                    InfoTypeModified.AddRange(x.ItemInfo.Select(y => y.ItemInfoType.ItemId));
                    return true;
                });

                //ProveedoresOnLine.Company.Controller.Company.CompanyPartialIndex(oProvider.RelatedCompany.CompanyPublicId, InfoTypeModified);

                oReturn = new Models.Provider.ProviderFinancialViewModel(oProvider.RelatedFinantial.FirstOrDefault(), oBank);

                //register used files
                LogManager.ClientLog.FileUsedCreate(lstUsedFiles);
            }

            return oReturn;
        }

        #endregion

        #region Legal Info

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderLegalViewModel> LILegalInfoGetByType
            (string LILegalInfoGetByType,
            string ProviderPublicId,
            string LegalInfoType
            , string ViewEnable)
        {
            List<BackOffice.Models.Provider.ProviderLegalViewModel> oReturn = new List<Models.Provider.ProviderLegalViewModel>();
            int TotalRows = 0;
            if (LILegalInfoGetByType == "true")
            {
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oLegalInfo = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.LegalGetBasicInfo
                    (ProviderPublicId,
                    string.IsNullOrEmpty(LegalInfoType) ? null : (int?)Convert.ToInt32(LegalInfoType.Trim()), Convert.ToBoolean(ViewEnable));

                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oICA = null;

                if (LegalInfoType == ((int)BackOffice.Models.General.enumLegalType.RUT).ToString())
                {
                    oICA = ProveedoresOnLine.Company.Controller.Company.CategorySearchByICA(null, 0, 0, out TotalRows);
                }
                if (oLegalInfo != null)
                {
                    oLegalInfo.All(x =>
                    {
                        oReturn.Add(new BackOffice.Models.Provider.ProviderLegalViewModel(x, oICA));
                        return true;
                    });

                    if (oReturn.Any(x => x.RelatedLegal.ItemType.ItemId == (int)BackOffice.Models.General.enumLegalType.SARLAFT))
                    {
                        oReturn = oReturn.OrderByDescending(x => string.IsNullOrEmpty(x.SF_ProcessDate) ? DateTime.Now :
                            DateTime.ParseExact(
                                x.SF_ProcessDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value,
                                System.Globalization.CultureInfo.InvariantCulture)).ToList();
                    }
                }
            }
            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public BackOffice.Models.Provider.ProviderLegalViewModel LILegalUpsert
            (string LILegalInfoUpsert,
            string ProviderPublicId,
            string LegalInfoType, string LegalId)
        {
            BackOffice.Models.Provider.ProviderLegalViewModel oReturn = null;

            if (LILegalInfoUpsert == "true" &&
               !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]) &&
               !string.IsNullOrEmpty(LegalInfoType))
            {
                BackOffice.Models.Provider.ProviderLegalViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.ProviderLegalViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.ProviderLegalViewModel));

                ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel oProvider = new ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel()
                {
                    RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                    {
                        CompanyPublicId = ProviderPublicId,
                    },
                    RelatedLegal = new List<ProveedoresOnLine.Company.Models.Util.GenericItemModel>()
                    {
                        new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = string.IsNullOrEmpty(oDataToUpsert.LegalId) ? 0 : Convert.ToInt32(oDataToUpsert.LegalId.Trim()),
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = Convert.ToInt32(LegalInfoType.Trim()),
                            },
                            ItemName = oDataToUpsert.LegalName,
                            Enable = oDataToUpsert.Enable,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        },
                    },
                };

                #region Designations

                if (oProvider.RelatedLegal.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumLegalType.Designations)
                {
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CD_PartnerNameId) ? 0 : Convert.ToInt32(oDataToUpsert.CD_PartnerNameId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.CD_PartnerName
                        },
                        Value = oDataToUpsert.CD_PartnerName,
                        Enable = oDataToUpsert.Enable,
                    });

                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CD_PartnerIdentificationNumberId) ? 0 : Convert.ToInt32(oDataToUpsert.CD_PartnerIdentificationNumberId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.CD_PartnerIdentificationNumber
                        },
                        Value = oDataToUpsert.CD_PartnerIdentificationNumber,
                        Enable = oDataToUpsert.Enable,

                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CD_PartnerRankId) ? 0 : Convert.ToInt32(oDataToUpsert.CD_PartnerRankId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.CD_PartnerRank
                        },
                        Value = oDataToUpsert.CD_PartnerRank,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CD_PartnerdocumentId) ? 0 : Convert.ToInt32(oDataToUpsert.CD_PartnerdocumentId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.CD_Partnerdocument
                        },
                        Value = oDataToUpsert.CD_Partnerdocument,
                        Enable = oDataToUpsert.Enable,
                    });
                }
                #endregion

                #region RUT
                if (oProvider.RelatedLegal.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumLegalType.RUT)
                {
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_PersonTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.R_PersonTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_PersonType
                        },
                        Value = oDataToUpsert.R_PersonType,
                        Enable = oDataToUpsert.Enable,
                    });

                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_LargeContributorId) ? 0 : Convert.ToInt32(oDataToUpsert.R_LargeContributorId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_LargeContributor
                        },
                        Value = oDataToUpsert.R_LargeContributor.ToString(),
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_LargeContributorReceiptId) ? 0 : Convert.ToInt32(oDataToUpsert.R_LargeContributorReceiptId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_LargeContributorReceipt
                        },
                        Value = oDataToUpsert.R_LargeContributorReceipt,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_LargeContributorDateId) ? 0 : Convert.ToInt32(oDataToUpsert.R_LargeContributorDateId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_LargeContributorDate
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.R_LargeContributorDate) ?
                            string.Empty :
                            oDataToUpsert.R_LargeContributorDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.R_LargeContributorDate :
                            DateTime.ParseExact(
                                oDataToUpsert.R_LargeContributorDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_SelfRetainerId) ? 0 : Convert.ToInt32(oDataToUpsert.R_SelfRetainerId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_SelfRetainer
                        },
                        Value = oDataToUpsert.R_SelfRetainer.ToString(),
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_SelfRetainerRecieptId) ? 0 : Convert.ToInt32(oDataToUpsert.R_SelfRetainerRecieptId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_SelfRetainerReciept
                        },
                        Value = oDataToUpsert.R_SelfRetainerReciept,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_SelfRetainerDateId) ? 0 : Convert.ToInt32(oDataToUpsert.R_SelfRetainerDateId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_SelfRetainerDate
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.R_SelfRetainerDate) ?
                            string.Empty :
                            oDataToUpsert.R_SelfRetainerDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.R_SelfRetainerDate :
                            DateTime.ParseExact(
                                oDataToUpsert.R_SelfRetainerDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_EntityTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.R_EntityTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_EntityType
                        },
                        Value = oDataToUpsert.R_EntityType,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_IVAId) ? 0 : Convert.ToInt32(oDataToUpsert.R_IVAId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_IVA
                        },
                        Value = oDataToUpsert.R_IVA.ToString(),
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_TaxPayerTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.R_TaxPayerTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_TaxPayerType
                        },
                        Value = oDataToUpsert.R_TaxPayerType,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_ICAId) ? 0 : Convert.ToInt32(oDataToUpsert.R_ICAId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_ICA
                        },
                        Value = oDataToUpsert.R_ICA,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_RUTFileId) ? 0 : Convert.ToInt32(oDataToUpsert.R_RUTFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_RUTFile
                        },
                        Value = oDataToUpsert.R_RUTFile,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_LargeContributorFileId) ? 0 : Convert.ToInt32(oDataToUpsert.R_LargeContributorFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_LargeContributorFile
                        },
                        Value = oDataToUpsert.R_LargeContributorFile,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_SelfRetainerFileId) ? 0 : Convert.ToInt32(oDataToUpsert.R_SelfRetainerFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_SelfRetainerFile
                        },
                        Value = oDataToUpsert.R_SelfRetainerFile,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.R_ClassTaxId) ? 0 : Convert.ToInt32(oDataToUpsert.R_ClassTaxId.Trim()),
                        ItemInfoType = new CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.R_ClassTax,
                        },
                        Value = oDataToUpsert.R_ClassTax,
                        Enable = oDataToUpsert.Enable,
                    });
                };

                #endregion

                #region CIFIN

                if (oProvider.RelatedLegal.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumLegalType.CIFIN)
                {
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CF_QueryDateId) ? 0 : Convert.ToInt32(oDataToUpsert.CF_QueryDateId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.CF_QueryDate
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.CF_QueryDate) ?
                            string.Empty :
                            oDataToUpsert.CF_QueryDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.CF_QueryDate :
                            DateTime.ParseExact(
                                oDataToUpsert.CF_QueryDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = oDataToUpsert.Enable,
                    });

                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CF_ResultQueryId) ? 0 : Convert.ToInt32(oDataToUpsert.CF_ResultQueryId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.CF_ResultQuery
                        },
                        Value = oDataToUpsert.CF_ResultQuery,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.CF_AutorizationFileId) ? 0 : Convert.ToInt32(oDataToUpsert.CF_AutorizationFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.CF_AutorizationFile
                        },
                        Value = oDataToUpsert.CF_AutorizationFile,
                        Enable = oDataToUpsert.Enable,
                    });
                };

                #endregion

                #region SARLAFT

                if (oProvider.RelatedLegal.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumLegalType.SARLAFT)
                {
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.SF_ProcessDateId) ? 0 : Convert.ToInt32(oDataToUpsert.SF_ProcessDateId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.SF_ProcessDate
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.SF_ProcessDate) ?
                            string.Empty :
                            oDataToUpsert.SF_ProcessDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.SF_ProcessDate :
                            DateTime.ParseExact(
                                oDataToUpsert.SF_ProcessDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = oDataToUpsert.Enable,
                    });

                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.SF_PersonTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.SF_PersonTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.SF_PersonType
                        },
                        Value = oDataToUpsert.SF_PersonType,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.SF_SARLAFTFileId) ? 0 : Convert.ToInt32(oDataToUpsert.SF_SARLAFTFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.SF_SARLAFTFile
                        },
                        Value = oDataToUpsert.SF_SARLAFTFile,
                        Enable = oDataToUpsert.Enable,
                    });
                };

                #endregion

                #region Resolutions

                if (oProvider.RelatedLegal.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumLegalType.Resoluciones)
                {
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.RS_EntityTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.RS_EntityTypeId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.RS_EntityType
                        },
                        Value = oDataToUpsert.RS_EntityType,
                        Enable = oDataToUpsert.Enable,
                    });

                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.RS_ResolutionFileId) ? 0 : Convert.ToInt32(oDataToUpsert.RS_ResolutionFileId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.RS_ResolutionFile
                        },
                        Value = oDataToUpsert.RS_ResolutionFile,
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.RS_StartDateId) ? 0 : Convert.ToInt32(oDataToUpsert.RS_StartDateId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.RS_StartDate
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.RS_StartDate) ?
                            string.Empty :
                            oDataToUpsert.RS_StartDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.RS_StartDate :
                            DateTime.ParseExact(
                                oDataToUpsert.RS_StartDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.RS_EndDateId) ? 0 : Convert.ToInt32(oDataToUpsert.RS_EndDateId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.RS_EndDate
                        },
                        Value = string.IsNullOrEmpty(oDataToUpsert.RS_EndDate) ?
                            string.Empty :
                            oDataToUpsert.RS_EndDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.RS_EndDate :
                            DateTime.ParseExact(
                                oDataToUpsert.RS_EndDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                        Enable = oDataToUpsert.Enable,
                    });
                    oProvider.RelatedLegal.FirstOrDefault().ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.RS_DescriptionId) ? 0 : Convert.ToInt32(oDataToUpsert.RS_DescriptionId.Trim()),
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumLegalInfoType.RS_Description
                        },
                        Value = oDataToUpsert.RS_Description,
                        Enable = oDataToUpsert.Enable,
                    });
                };

                #endregion

                #region Index

                Uri node = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);

                settings.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value);
                settings.DisableDirectStreaming(true);
                ElasticClient client = new ElasticClient(settings);

                //Getting Model from index
                Nest.ISearchResponse<CompanyIndexModel> oResult = client.Search<CompanyIndexModel>(s => s
                    .From(0)
                    .Size(1)
                    .Query(q => q.QueryString(qs => qs.Query(ProviderPublicId))));

                //Model to index 
                #region Model

                CompanyIndexModel oCompanyIndexModel = new CompanyIndexModel()
                {
                    CatlificationRating = oResult.Documents.FirstOrDefault().CatlificationRating,

                    City = oResult.Documents.FirstOrDefault().City,
                    CityId = oResult.Documents.FirstOrDefault().CityId,

                    CommercialCompanyName = oResult.Documents.FirstOrDefault().CommercialCompanyName,

                    CompanyEnable = oResult.Documents.FirstOrDefault().CompanyEnable,

                    CompanyName = oResult.Documents.FirstOrDefault().CompanyName,

                    CompanyPublicId = oResult.Documents.FirstOrDefault().CompanyPublicId,

                    Country = oResult.Documents.FirstOrDefault().Country,
                    CountryId = oResult.Documents.FirstOrDefault().CountryId,
                    CustomerPublicId = oResult.Documents.FirstOrDefault().CustomerPublicId,

                    ICA = oResult.Documents.FirstOrDefault().ICA,
                    ICAId = oResult.Documents.FirstOrDefault().ICAId,

                    IdentificationNumber = oResult.Documents.FirstOrDefault().IdentificationNumber,


                    IdentificationType = oResult.Documents.FirstOrDefault().IdentificationType,
                    IdentificationTypeId = oResult.Documents.FirstOrDefault().IdentificationTypeId,

                    InBlackList = oResult.Documents.FirstOrDefault().InBlackList,

                    LogoUrl = oResult.Documents.FirstOrDefault().LogoUrl,

                    oCustomerProviderIndexModel = oResult.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                    PrincipalActivity = oResult.Documents.FirstOrDefault().PrincipalActivity,
                    PrincipalActivityId = oResult.Documents.FirstOrDefault().PrincipalActivityId,

                    ProviderStatus = oResult.Documents.FirstOrDefault().ProviderStatus,
                    ProviderStatusId = oResult.Documents.FirstOrDefault().ProviderStatusId,
                };

                #endregion

                if (oDataToUpsert.Enable == true)
                {

                    oCompanyIndexModel.ICAId = !string.IsNullOrEmpty(oDataToUpsert.R_ICA) ? Convert.ToInt32(oDataToUpsert.R_ICA) : 0;
                    oCompanyIndexModel.ICA = !string.IsNullOrEmpty(oDataToUpsert.R_ICAName) ? oDataToUpsert.R_ICAName : "";


                    ICreateIndexResponse oElasticResponse = client.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                        .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                            .Tokenizer("whitespace")
                            )).TokenFilters(tf => tf
                                    .EdgeNGram("customEdgeNGram", engrf => engrf
                                    .MinGram(1)
                                    .MaxGram(10)))).NumberOfShards(1)
                        ));

                    var Index = client.Index(oCompanyIndexModel);
                }
                else
                {
                    oCompanyIndexModel.ICAId = 0;
                    oCompanyIndexModel.ICA = "";


                    ICreateIndexResponse oElasticResponse = client.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                        .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                            .Tokenizer("whitespace")
                            )).TokenFilters(tf => tf
                                    .EdgeNGram("customEdgeNGram", engrf => engrf
                                    .MinGram(1)
                                    .MaxGram(10)))).NumberOfShards(1)
                        ));

                    var Index = client.Index(oCompanyIndexModel);


                }



                #endregion

                oProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.LegalUpsert(oProvider);

                //eval company partial index
                List<int> InfoTypeModified = new List<int>() { 5 };

                oProvider.RelatedLegal.All(x =>
                {
                    InfoTypeModified.AddRange(x.ItemInfo.Select(y => y.ItemInfoType.ItemId));
                    return true;
                });

                //ProveedoresOnLine.Company.Controller.Company.CompanyPartialIndex(oProvider.RelatedCompany.CompanyPublicId, InfoTypeModified);

                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> osICA = null;
                int ototal;
                osICA = ProveedoresOnLine.Company.Controller.Company.CategorySearchByICA(null, 0, 0, out ototal);

                oReturn = new Models.Provider.ProviderLegalViewModel(oProvider.RelatedLegal.FirstOrDefault(), osICA);


            }

            return oReturn;
        }

        #endregion

        #region Aditional Documents

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderAditionalDocumentViewModel> ADGetAditionalDocument
            (string ADGetAditionalDocument,
            string ProviderPublicId,
            string AditionalDataType,
            string ViewEnable)
        {
            List<BackOffice.Models.Provider.ProviderAditionalDocumentViewModel> oReturn = new List<ProviderAditionalDocumentViewModel>();

            if (ADGetAditionalDocument == "true")
            {
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oAditionalDocumentInfo = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.AditionalDocumentGetByType
                    (ProviderPublicId,
                    Convert.ToInt32(AditionalDataType),
                    ViewEnable == "true" ? true : false);

                ProveedoresOnLine.CompanyCustomer.Models.Customer.CustomerModel oCustomerByProvider =
                    ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.GetCustomerByProvider(ProviderPublicId, null);

                if (oAditionalDocumentInfo != null)
                {
                    oAditionalDocumentInfo.All(ad =>
                    {
                        oReturn.Add(new ProviderAditionalDocumentViewModel(ad, oCustomerByProvider));
                        return true;
                    });
                }
            }
            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public BackOffice.Models.Provider.ProviderAditionalDocumentViewModel ADAditionalDocumentUpsert
            (string ADAditionalDocumentUpsert,
            string ProviderPublicId,
            string AditionalDataType)
        {
            BackOffice.Models.Provider.ProviderAditionalDocumentViewModel oReturn = null;

            if (ADAditionalDocumentUpsert == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]))
            {
                List<string> lstUsedFiles = new List<string>();

                ProveedoresOnLine.CompanyCustomer.Models.Customer.CustomerModel oCustomerByProvider =
                    ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.GetCustomerByProvider(ProviderPublicId, null);

                BackOffice.Models.Provider.ProviderAditionalDocumentViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.ProviderAditionalDocumentViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.ProviderAditionalDocumentViewModel));

                ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel oProvider = new ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel()
                {
                    RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                    {
                        CompanyPublicId = ProviderPublicId,
                    },
                    RelatedAditionalDocuments = new List<GenericItemModel>()
                    {
                        new GenericItemModel()
                        {
                            ItemId = string.IsNullOrEmpty(oDataToUpsert.AditionalDocumentId) ? 0 : Convert.ToInt32(oDataToUpsert.AditionalDocumentId.Trim()),
                            ItemType = new CatalogModel()
                            {
                                ItemId = Convert.ToInt32(AditionalDataType.Trim()),
                            },
                            ItemName = Convert.ToInt32(AditionalDataType.Trim()) == (int)BackOffice.Models.General.enumAditionalDocumentType.AditionalDocument ? oDataToUpsert.AD_Title : oDataToUpsert.ADT_Title,
                            Enable = oDataToUpsert.Enable,
                            ItemInfo = new List<GenericItemInfoModel>(),
                        },
                    },
                };

                if (oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentType.AditionalDocument)
                {
                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                    new GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.AD_RelatedUserId) ? 0 : Convert.ToInt32(oDataToUpsert.AD_RelatedUserId.Trim()),
                        ItemInfoType = new CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedUser,
                        },
                        Value = BackOffice.Models.General.SessionModel.CurrentLoginUser.Email.ToString(),
                        Enable = true,
                    });

                    if (oDataToUpsert.AditionalDocumentId != null)
                    {
                        List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oAditionalDocumentInfo = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.AditionalDocumentGetByType
                           (ProviderPublicId, Convert.ToInt32(AditionalDataType), true);

                        oAditionalDocumentInfo.Where(x => x.ItemId == Convert.ToInt32(oDataToUpsert.AditionalDocumentId.Trim()))
                            .All(x => {
                                x.ItemInfo.Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedCustomer)
                                    .All(z => {
                                        if (oDataToUpsert.AD_RelatedCustomerList.Where(a => a.CP_CustomerPublicId == z.Value).SingleOrDefault() == null)
                                        {
                                            z.Enable = false;
                                        }
                                        else
                                        {
                                            oDataToUpsert.AD_RelatedCustomerList.RemoveAll(a => a.CP_CustomerPublicId == z.Value);
                                            z.Enable = true;
                                        }
                                        return true;
                                    });
                                return true;
                            });
                        if (oAditionalDocumentInfo.Count(x => x.ItemId == Convert.ToInt32(oDataToUpsert.AditionalDocumentId.Trim())) > 0)
                        {
                            oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.AddRange(oAditionalDocumentInfo.Where(x => x.ItemId == Convert.ToInt32(oDataToUpsert.AditionalDocumentId.Trim())).SingleOrDefault().ItemInfo.Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedCustomer));
                        }

                    }


                    oDataToUpsert.AD_RelatedCustomerList.All(x => {
                        oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                        new GenericItemInfoModel()
                        {
                            ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.AD_RelatedCustomerId) ? 0 : Convert.ToInt32(oDataToUpsert.AD_RelatedCustomerId.Trim()),
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedCustomer,
                            },
                            Value = x.CP_CustomerPublicId,
                            Enable = true,
                        });
                        return true;
                    });

                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                        new GenericItemInfoModel()
                        {
                            ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.AD_FileId) ? 0 : Convert.ToInt32(oDataToUpsert.AD_FileId.Trim()),
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_File,
                            },
                            Value = oDataToUpsert.AD_File,
                            Enable = true,
                        });
                    //New Data for the grid
                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                        new GenericItemInfoModel()
                        {
                            ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.AD_ValueId) ? 0 : Convert.ToInt32(oDataToUpsert.AD_ValueId.Trim()),
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Value,
                            },
                            Value = oDataToUpsert.AD_Value,
                            Enable = true,
                        });
                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                        new GenericItemInfoModel()
                        {
                            ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.AD_InitialDateId) ? 0 : Convert.ToInt32(oDataToUpsert.AD_InitialDateId.Trim()),
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_InitialDate,
                            },
                            Value = string.IsNullOrEmpty(oDataToUpsert.AD_InitialDate) ?
                            string.Empty :
                            oDataToUpsert.AD_InitialDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.AD_InitialDate :
                            DateTime.ParseExact(
                                oDataToUpsert.AD_InitialDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                            Enable = true,
                        });
                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                       new GenericItemInfoModel()
                       {
                           ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.AD_EndDateId) ? 0 : Convert.ToInt32(oDataToUpsert.AD_EndDateId.Trim()),
                           ItemInfoType = new CatalogModel()
                           {
                               ItemId = (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_EndDate,
                           },
                           Value = string.IsNullOrEmpty(oDataToUpsert.AD_EndDate) ?
                            string.Empty :
                            oDataToUpsert.AD_EndDate.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.AD_EndDate :
                            DateTime.ParseExact(
                                oDataToUpsert.AD_EndDate,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                           Enable = true,
                       });
                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                       new GenericItemInfoModel()
                       {
                           ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.AD_VigencyId) ? 0 : Convert.ToInt32(oDataToUpsert.AD_VigencyId.Trim()),
                           ItemInfoType = new CatalogModel()
                           {
                               ItemId = (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Vigency,
                           },
                           Value = string.IsNullOrEmpty(oDataToUpsert.AD_Vigency) ?
                            string.Empty :
                            oDataToUpsert.AD_Vigency.Replace(" ", "").Length == BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value.Replace(" ", "").Length ?
                            oDataToUpsert.AD_Vigency :
                            DateTime.ParseExact(
                                oDataToUpsert.AD_Vigency,
                                BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_KendoToServer].Value,
                                System.Globalization.CultureInfo.InvariantCulture).
                            ToString(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_DateFormat_Server].Value),
                           Enable = true,
                       });
                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                       new GenericItemInfoModel()
                       {
                           ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.AD_DescriptionId) ? 0 : Convert.ToInt32(oDataToUpsert.AD_DescriptionId.Trim()),
                           ItemInfoType = new CatalogModel()
                           {
                               ItemId = (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Description,
                           },
                           LargeValue = oDataToUpsert.AD_Description,
                           Enable = true,
                       });
                    lstUsedFiles.Add(oDataToUpsert.AD_File);
                }
                else if (oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentType.AditionalData)
                {
                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                    new GenericItemInfoModel()
                    {
                        ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.ADT_RelatedUserId) ? 0 : Convert.ToInt32(oDataToUpsert.ADT_RelatedUserId.Trim()),
                        ItemInfoType = new CatalogModel()
                        {
                            ItemId = (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_RelatedUser,
                        },
                        Value = BackOffice.Models.General.SessionModel.CurrentLoginUser.Email.ToString(),
                        Enable = true,
                    });

                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                        new GenericItemInfoModel()
                        {
                            ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.ADT_RelatedCustomerId) ? 0 : Convert.ToInt32(oDataToUpsert.ADT_RelatedCustomerId.Trim()),
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_RelatedCustomer,
                            },
                            Value = oDataToUpsert.ADT_RelatedCustomer,
                            Enable = true,
                        });

                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                        new GenericItemInfoModel()
                        {
                            ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.ADT_DataTypeId) ? 0 : Convert.ToInt32(oDataToUpsert.ADT_DataTypeId.Trim()),
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_DataType,
                            },
                            Value = oDataToUpsert.ADT_DataType,
                            Enable = true,
                        });

                    oProvider.RelatedAditionalDocuments.FirstOrDefault().ItemInfo.Add(
                        new GenericItemInfoModel()
                        {
                            ItemInfoId = string.IsNullOrEmpty(oDataToUpsert.ADT_DataValueId) ? 0 : Convert.ToInt32(oDataToUpsert.ADT_DataValueId.Trim()),
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_Value,
                            },
                            Value = oDataToUpsert.ADT_DataValue,
                            Enable = true,
                        });
                }

                oProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.AditionalDocumentsUpsert(oProvider);

                LogManager.ClientLog.FileUsedCreate(lstUsedFiles);

                oReturn = new ProviderAditionalDocumentViewModel(oProvider.RelatedAditionalDocuments.FirstOrDefault(), oCustomerByProvider);
            }

            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderAditionalDocumentViewModel> ADGetAditionalData
            (string ADGetAditionalData,
            string ProviderPublicId,
            string AditionalDataType,
            string ViewEnable)
        {
            List<BackOffice.Models.Provider.ProviderAditionalDocumentViewModel> oReturn = new List<ProviderAditionalDocumentViewModel>();

            if (ADGetAditionalData == "true")
            {
                List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oAditionalDocumentInfo = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.AditionalDocumentGetByType
                    (ProviderPublicId,
                    Convert.ToInt32(AditionalDataType),
                    ViewEnable == "true" ? true : false);

                ProveedoresOnLine.CompanyCustomer.Models.Customer.CustomerModel oCustomerByProvider =
                    ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.GetCustomerByProvider(ProviderPublicId, null);

                if (oAditionalDocumentInfo != null)
                {
                    oAditionalDocumentInfo.All(ad =>
                    {
                        oReturn.Add(new ProviderAditionalDocumentViewModel(ad, oCustomerByProvider));
                        return true;
                    });
                }
            }
            return oReturn;
        }

        #endregion

        #region Customer Provider

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderCustomerViewModel> CPCustomerProvider
        (string CPCustomerProvider,
            string ProviderPublicId,
            string CustomerRelated,
            int AddCustomer,
            string ViewEnable)
        {
            List<BackOffice.Models.Provider.ProviderCustomerViewModel> oReturn = new List<Models.Provider.ProviderCustomerViewModel>();

            if (CPCustomerProvider == "true")
            {
                ProveedoresOnLine.CompanyCustomer.Models.Customer.CustomerModel oCustomerByProvider = new CustomerModel();

                if (Convert.ToBoolean(ViewEnable) == true)
                {
                    oCustomerByProvider =
                    ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.GetCustomerByProvider(ProviderPublicId, CustomerRelated == "2" ? null : CustomerRelated);
                }
                else
                {
                    oCustomerByProvider =
                    ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.GetCustomerByProvider(ProviderPublicId, "0");
                }

                List<CustomerProviderModel> oCustomerProvider = new List<CustomerProviderModel>();

                if (oCustomerByProvider != null && oCustomerByProvider.RelatedProvider != null && oCustomerByProvider.RelatedProvider.Count >= 1)
                {
                    if (AddCustomer == 1)
                    {
                        oCustomerByProvider.RelatedProvider.Where(x => x.RelatedProvider.CompanyPublicId != BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_PublicarPublicId].Value.ToString()).All(x =>
                        {
                            oReturn.Add(new ProviderCustomerViewModel(x));
                            return true;
                        });
                    }
                    else
                    {
                        oCustomerByProvider.RelatedProvider.All(x =>
                        {
                            oReturn.Add(new ProviderCustomerViewModel(x));
                            return true;
                        });
                    }
                }
            }


            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.TrackingViewModel> CPCustomerProviderInfo
        (string CPCustomerProviderInfo,
            int CustomerProviderId,
            string ViewEnable,
            string PageNumber,
            string RowCount)
        {
            List<BackOffice.Models.Provider.TrackingViewModel> oReturn = new List<Models.Provider.TrackingViewModel>();

            if (CPCustomerProviderInfo == "true")
            {
                int oPageNumber = string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber.Trim());

                int oRowCount = Convert.ToInt32(string.IsNullOrEmpty(RowCount) ?
                    BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_Grid_RowCountDefault].Value :
                    RowCount.Trim());

                int oTotalRows;

                ProveedoresOnLine.CompanyCustomer.Models.Customer.CustomerModel oCustomerProviderInfo =
                    ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.GetCustomerInfoByProvider
                        (CustomerProviderId,
                        Convert.ToBoolean(ViewEnable),
                        oPageNumber,
                        oRowCount,
                        out oTotalRows);

                if (oCustomerProviderInfo != null && oCustomerProviderInfo.RelatedProvider.Count > 0)
                {
                    oCustomerProviderInfo.RelatedProvider.First().CustomerProviderInfo
                        .Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumProviderCustomerType.CustomerMonitoring ||
                                    x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumProviderCustomerType.InternalMonitoring)
                                     .All(x =>
                                     {
                                         oReturn.Add(new TrackingViewModel(x, oTotalRows));
                                         return true;
                                     });
                }
            }

            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public void CPCustomerProviderUpsert
            (string UpsertCustomerByProvider,
            string oProviderPublicId,
            string oCompanyPublicList,
            int oEnable)
        {

            if (UpsertCustomerByProvider == "true")
            {
                string[] oCustomerPublicId = oCompanyPublicList.Split(new char[] { ',' });

                BackOffice.Models.Provider.ProviderViewModel oModel = new Models.Provider.ProviderViewModel()
                {
                    ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions(),
                };

                foreach (var item in oCustomerPublicId)
                {
                    ProveedoresOnLine.Company.Models.Company.CompanyModel oCompanyModel = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(item);

                    CustomerModel oCustomerModel = new CustomerModel();
                    oCustomerModel.RelatedProvider = new List<CustomerProviderModel>();

                    oCustomerModel.RelatedProvider.Add(new CustomerProviderModel()
                    {
                        RelatedProvider = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                        {
                            CompanyPublicId = oProviderPublicId,
                        },
                        Status = new CatalogModel()
                        {
                            ItemId = Convert.ToInt32(BackOffice.Models.General.enumProviderCustomerStatus.Creation),
                        },
                        Enable = oEnable == 1 ? true : false,
                    });

                    oCustomerModel.RelatedCompany = oCompanyModel;

                    oCustomerModel = ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.CustomerProviderUpsert(oCustomerModel);

                    #region Index

                    #region CompanyIndex
                    Uri node = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                    var settings = new ConnectionSettings(node);

                    settings.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value);
                    settings.DisableDirectStreaming(true);
                    ElasticClient client = new ElasticClient(settings);

                    //Getting Model from index
                    Nest.ISearchResponse<CompanyIndexModel> oResult = client.Search<CompanyIndexModel>(s => s
                        .From(0)
                        .Size(1)
                        .Query(q => q.QueryString(qs => qs.Query(oProviderPublicId))));

                    //Model to index 
                    #region Model

                    CompanyIndexModel oCompanyIndexModel = new CompanyIndexModel()
                    {
                        CatlificationRating = oResult.Documents.FirstOrDefault().CatlificationRating,

                        City = oResult.Documents.FirstOrDefault().City,
                        CityId = oResult.Documents.FirstOrDefault().CityId,

                        CommercialCompanyName = oResult.Documents.FirstOrDefault().CommercialCompanyName,

                        CompanyEnable = oResult.Documents.FirstOrDefault().CompanyEnable,

                        CompanyName = oResult.Documents.FirstOrDefault().CompanyName,

                        CompanyPublicId = oResult.Documents.FirstOrDefault().CompanyPublicId,

                        Country = oResult.Documents.FirstOrDefault().Country,
                        CountryId = oResult.Documents.FirstOrDefault().CountryId,
                        CustomerPublicId = oResult.Documents.FirstOrDefault().CustomerPublicId,

                        ICA = oResult.Documents.FirstOrDefault().ICA,
                        ICAId = oResult.Documents.FirstOrDefault().ICAId,

                        IdentificationNumber = oResult.Documents.FirstOrDefault().IdentificationNumber,


                        IdentificationType = oResult.Documents.FirstOrDefault().IdentificationType,
                        IdentificationTypeId = oResult.Documents.FirstOrDefault().IdentificationTypeId,

                        InBlackList = oResult.Documents.FirstOrDefault().InBlackList,

                        LogoUrl = oResult.Documents.FirstOrDefault().LogoUrl,

                        oCustomerProviderIndexModel = oResult.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                        PrincipalActivity = oResult.Documents.FirstOrDefault().PrincipalActivity,
                        PrincipalActivityId = oResult.Documents.FirstOrDefault().PrincipalActivityId,

                        ProviderStatus = oResult.Documents.FirstOrDefault().ProviderStatus,
                        ProviderStatusId = oResult.Documents.FirstOrDefault().ProviderStatusId,
                    };

                    #endregion

                    if (!oCompanyIndexModel.oCustomerProviderIndexModel.Any(x => x.CustomerPublicId == item))
                    {
                        oCompanyIndexModel.oCustomerProviderIndexModel.Add(new CustomerProviderIndexModel()
                        {
                            ProviderPublicId = oProviderPublicId,
                            StatusId = Convert.ToInt32(BackOffice.Models.General.enumProviderCustomerStatus.Creation),
                            Status = oModel.ProviderOptions.Where(y => y.ItemId == Convert.ToInt32(BackOffice.Models.General.enumProviderCustomerStatus.Creation)).Select(y => y.ItemName).DefaultIfEmpty(string.Empty).FirstOrDefault(),
                            CustomerPublicId = item,
                            CustomerProviderEnable = true,
                            CustomerProviderId = oCustomerModel.RelatedProvider.Where(y => y.RelatedProvider.CompanyPublicId == oProviderPublicId).Select(y => y.CustomerProviderId).DefaultIfEmpty(0).FirstOrDefault(),

                        });

                        ICreateIndexResponse oElasticResponse = client.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                            .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                            .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                                .Tokenizer("whitespace")
                                )).TokenFilters(tf => tf
                                        .EdgeNGram("customEdgeNGram", engrf => engrf
                                        .MinGram(1)
                                        .MaxGram(10)))).NumberOfShards(1)
                            ));

                        var Index = client.Index(oCompanyIndexModel);
                    }
                    #endregion

                    #region CustomerProviderIndex

                    Uri node3 = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                    var settings3 = new ConnectionSettings(node3);
                    settings3.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CustomerProviderIndex].Value);
                    ElasticClient client3 = new ElasticClient(settings3);

                    ICreateIndexResponse oElasticResponse3 = client3.
                            CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CustomerProviderIndex].Value, c => c
                            .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                            .Analysis(a => a.
                                Analyzers(an => an.
                                    Custom("customWhiteSpace", anc => anc.
                                        Filters("asciifolding", "lowercase").
                                        Tokenizer("whitespace")
                                            )
                                        ).TokenFilters(tf => tf
                                        .EdgeNGram("customEdgeNGram", engrf => engrf
                                        .MinGram(1)
                                        .MaxGram(10))
                                    )
                                ).NumberOfShards(1)
                            )
                        );
                    client3.Map<CustomerProviderIndexModel>(m => m.AutoMap());

                    var Index3 = client3.IndexMany(oCompanyIndexModel.oCustomerProviderIndexModel, BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CustomerProviderIndex].Value);

                    #endregion

                    #region SurveyIndex
                    Uri node2 = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                    var settings2 = new ConnectionSettings(node2);

                    settings2.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanySurveyIndex].Value);
                    settings2.DisableDirectStreaming(true);
                    ElasticClient client2 = new ElasticClient(settings2);

                    //Getting Model from index
                    Nest.ISearchResponse<ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel> oResult2 = client2.Search<ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel>(s => s
                      .From(0)
                      .Size(1)
                      .Query(q => q.QueryString(qs => qs.Query(oProviderPublicId))));

                    //Model to index 
                    #region Model

                    ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel oCompanySurveyIndexModel = new ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel()
                    {
                        CatlificationRating = oResult2.Documents.FirstOrDefault().CatlificationRating,

                        City = oResult2.Documents.FirstOrDefault().City,
                        CityId = oResult2.Documents.FirstOrDefault().CityId,

                        CommercialCompanyName = oResult2.Documents.FirstOrDefault().CommercialCompanyName,

                        CompanyEnable = oResult2.Documents.FirstOrDefault().CompanyEnable,

                        CompanyName = oResult2.Documents.FirstOrDefault().CompanyName,

                        CompanyPublicId = oResult2.Documents.FirstOrDefault().CompanyPublicId,

                        Country = oResult2.Documents.FirstOrDefault().Country,
                        CountryId = oResult2.Documents.FirstOrDefault().CountryId,
                        CustomerPublicId = oResult2.Documents.FirstOrDefault().CustomerPublicId,

                        ICA = oResult2.Documents.FirstOrDefault().ICA,
                        ICAId = oResult2.Documents.FirstOrDefault().ICAId,

                        IdentificationNumber = oResult2.Documents.FirstOrDefault().IdentificationNumber,


                        IdentificationType = oResult2.Documents.FirstOrDefault().IdentificationType,
                        IdentificationTypeId = oResult2.Documents.FirstOrDefault().IdentificationTypeId,

                        InBlackList = oResult2.Documents.FirstOrDefault().InBlackList,

                        LogoUrl = oResult2.Documents.FirstOrDefault().LogoUrl,

                        oCustomerProviderIndexModel = oResult2.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                        PrincipalActivity = oResult2.Documents.FirstOrDefault().PrincipalActivity,
                        PrincipalActivityId = oResult2.Documents.FirstOrDefault().PrincipalActivityId,

                        ProviderStatus = oResult2.Documents.FirstOrDefault().ProviderStatus,
                        ProviderStatusId = oResult2.Documents.FirstOrDefault().ProviderStatusId,
                    };

                    #endregion

                    if (!oCompanySurveyIndexModel.oCustomerProviderIndexModel.Any(x => x.CustomerPublicId == item))
                    {
                        oCompanySurveyIndexModel.oCustomerProviderIndexModel.Add(new CustomerProviderIndexModel()
                        {
                            ProviderPublicId = oProviderPublicId,
                            StatusId = Convert.ToInt32(BackOffice.Models.General.enumProviderCustomerStatus.Creation),
                            Status = oModel.ProviderOptions.Where(y => y.ItemId == Convert.ToInt32(BackOffice.Models.General.enumProviderCustomerStatus.Creation)).Select(y => y.ItemName).DefaultIfEmpty(string.Empty).FirstOrDefault(),
                            CustomerPublicId = item,
                            CustomerProviderEnable = true,
                            CustomerProviderId = oCustomerModel.RelatedProvider.Where(y => y.RelatedProvider.CompanyPublicId == oProviderPublicId).Select(y => y.CustomerProviderId).DefaultIfEmpty(0).FirstOrDefault(),

                        });

                        ICreateIndexResponse oElasticResponse2 = client2.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                            .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                            .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                                .Tokenizer("whitespace")
                                )).TokenFilters(tf => tf
                                        .EdgeNGram("customEdgeNGram", engrf => engrf
                                        .MinGram(1)
                                        .MaxGram(10)))).NumberOfShards(1)
                            ));

                        var Index = client2.Index(oCompanySurveyIndexModel);
                    }

                    #endregion

                    #endregion
                }
            }
        }

        [HttpPost]
        [HttpGet]
        public void CPCustomerProvierInfoUpsert
            (string UpsertCustomerInfoByProvider,
            string ProviderPublicId,
            string CompanyList)
        {
            if (UpsertCustomerInfoByProvider == "true")
            {
                string[] CustomerPublicId = CompanyList.Split(new char[] { ',' });

                BackOffice.Models.Provider.ProviderViewModel oModel = new Models.Provider.ProviderViewModel()
                {
                    ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions(),
                };

                foreach (var customer in CustomerPublicId)
                {
                    ProveedoresOnLine.Company.Models.Company.CompanyModel oCompanyModel = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(customer);


                    List<GenericItemInfoModel> oInfoModel = new List<GenericItemInfoModel>();

                    if (System.Web.HttpContext.Current.Request.Form["SH_InternalTracking"] != null && System.Web.HttpContext.Current.Request.Form["SH_InternalTracking"].Length > 0)
                    {
                        TrackingDetailViewModel oDetail = new TrackingDetailViewModel()
                        {
                            User = BackOffice.Models.General.SessionModel.CurrentLoginUser.Email,
                            Description = System.Web.HttpContext.Current.Request.Form["SH_InternalTracking"],
                        };

                        oInfoModel.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = Convert.ToInt32(BackOffice.Models.General.enumProviderCustomerType.InternalMonitoring),
                            },
                            LargeValue = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(oDetail),
                            Enable = true,
                        });
                    }
                    if (System.Web.HttpContext.Current.Request.Form["SH_CustomerTracking"] != null && System.Web.HttpContext.Current.Request.Form["SH_CustomerTracking"].Length > 0)
                    {
                        TrackingDetailViewModel oDetail = new TrackingDetailViewModel()
                        {
                            User = BackOffice.Models.General.SessionModel.CurrentLoginUser.Email,
                            Description = System.Web.HttpContext.Current.Request.Form["SH_CustomerTracking"],
                        };

                        oInfoModel.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = Convert.ToInt32(BackOffice.Models.General.enumProviderCustomerType.CustomerMonitoring),
                            },
                            LargeValue = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(oDetail),
                            Enable = true,
                        });
                    }

                    CustomerModel oCustomerModel = new CustomerModel()
                    {
                        RelatedCompany = oCompanyModel,
                        RelatedProvider = new List<CustomerProviderModel>(){
                            new CustomerProviderModel(){
                                RelatedProvider = new ProveedoresOnLine.Company.Models.Company.CompanyModel(){
                                    CompanyPublicId = ProviderPublicId,
                                },
                                Status = new CatalogModel(){
                                    ItemId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Currency"]),
                                },
                                CustomerProviderInfo = oInfoModel,
                                Enable = true,
                            },
                        },
                    };

                    CustomerModel uCustomerModel = new CustomerModel()
                    {
                        RelatedCompany = oCompanyModel,
                        RelatedProvider = new List<CustomerProviderModel>(){
                            new CustomerProviderModel(){
                                RelatedProvider = new ProveedoresOnLine.Company.Models.Company.CompanyModel(){
                                    CompanyPublicId = ProviderPublicId,
                                },
                                Status = new CatalogModel(){
                                    ItemId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Voiced"]),
                                },
                                CustomerProviderInfo = oInfoModel,
                                Enable = true,
                            },
                        },
                    };



                    ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.CustomerProviderUpsert(oCustomerModel);

                    #region Index

                    #region CompanyIndex

                    Uri node = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                    var settings = new ConnectionSettings(node);

                    settings.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value);
                    settings.DisableDirectStreaming(true);
                    ElasticClient client = new ElasticClient(settings);

                    //Getting Model from index
                    Nest.ISearchResponse<CompanyIndexModel> oResult = client.Search<CompanyIndexModel>(s => s
                        .From(0)
                        .Size(1)
                        .Query(q => q.QueryString(qs => qs.Query(ProviderPublicId))));

                    //Model to index 
                    #region Model

                    CompanyIndexModel oCompanyIndexModel = new CompanyIndexModel()
                    {
                        CatlificationRating = oResult.Documents.FirstOrDefault().CatlificationRating,

                        City = oResult.Documents.FirstOrDefault().City,
                        CityId = oResult.Documents.FirstOrDefault().CityId,

                        CommercialCompanyName = oResult.Documents.FirstOrDefault().CommercialCompanyName,

                        CompanyEnable = oResult.Documents.FirstOrDefault().CompanyEnable,

                        CompanyName = oResult.Documents.FirstOrDefault().CompanyName,

                        CompanyPublicId = oResult.Documents.FirstOrDefault().CompanyPublicId,

                        Country = oResult.Documents.FirstOrDefault().Country,
                        CountryId = oResult.Documents.FirstOrDefault().CountryId,
                        CustomerPublicId = oResult.Documents.FirstOrDefault().CustomerPublicId,

                        ICA = oResult.Documents.FirstOrDefault().ICA,
                        ICAId = oResult.Documents.FirstOrDefault().ICAId,

                        IdentificationNumber = oResult.Documents.FirstOrDefault().IdentificationNumber,


                        IdentificationType = oResult.Documents.FirstOrDefault().IdentificationType,
                        IdentificationTypeId = oResult.Documents.FirstOrDefault().IdentificationTypeId,

                        InBlackList = oResult.Documents.FirstOrDefault().InBlackList,

                        LogoUrl = oResult.Documents.FirstOrDefault().LogoUrl,

                        oCustomerProviderIndexModel = oResult.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                        PrincipalActivity = oResult.Documents.FirstOrDefault().PrincipalActivity,
                        PrincipalActivityId = oResult.Documents.FirstOrDefault().PrincipalActivityId,

                        ProviderStatus = oResult.Documents.FirstOrDefault().ProviderStatus,
                        ProviderStatusId = oResult.Documents.FirstOrDefault().ProviderStatusId,
                    };

                    #endregion

                    if (oCompanyIndexModel.oCustomerProviderIndexModel.Any(x => x.CustomerPublicId == customer))
                    {
                        oCompanyIndexModel.oCustomerProviderIndexModel.Where(x => x.CustomerPublicId == customer).All(x =>
                        {
                            x.ProviderPublicId = ProviderPublicId;
                            x.StatusId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Currency"]);
                            x.Status = oModel.ProviderOptions.Where(y => y.ItemId == Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Currency"])).Select(y => y.ItemName).DefaultIfEmpty(string.Empty).FirstOrDefault();
                            return true;
                        });
                    }

                    if (oCompanyIndexModel.oCustomerProviderIndexModel.Any(x => x.CustomerPublicId == customer))
                    {
                        oCompanyIndexModel.oCustomerProviderIndexModel.Where(x => x.CustomerPublicId == customer).All(x =>
                        {
                            x.ProviderPublicId = ProviderPublicId;
                            x.StatusId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Voiced"]);
                            x.Status = oModel.ProviderOptions.Where(y => y.ItemId == Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Voiced"])).Select(y => y.ItemName).DefaultIfEmpty(string.Empty).FirstOrDefault();
                            return true;
                        });
                    }

                    ICreateIndexResponse oElasticResponse = client.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                            .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                            .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                                .Tokenizer("whitespace")
                                )).TokenFilters(tf => tf
                                        .EdgeNGram("customEdgeNGram", engrf => engrf
                                        .MinGram(1)
                                        .MaxGram(10)))).NumberOfShards(1)
                            ));

                    var Index = client.Index(oCompanyIndexModel);

                    #endregion

                    #region SurveyIndex

                    Uri node2 = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                    var settings2 = new ConnectionSettings(node2);

                    settings2.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanySurveyIndex].Value);
                    settings2.DisableDirectStreaming(true);
                    ElasticClient client2 = new ElasticClient(settings2);

                    //Getting Model from index
                    Nest.ISearchResponse<ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel> oResult2 = client2.Search<ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel>(s => s
                        .From(0)
                        .Size(1)
                        .Query(q => q.QueryString(qs => qs.Query(ProviderPublicId))));

                    if (oResult2.Documents != null)
                    {
                        if (oResult2.Documents.Count() > 0)
                        {
                            //Model to index 
                            #region Model

                            ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel oCompanySurveyIndexModel = new ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel()
                            {
                                CatlificationRating = oResult2.Documents.FirstOrDefault().CatlificationRating,

                                City = oResult2.Documents.FirstOrDefault().City,
                                CityId = oResult2.Documents.FirstOrDefault().CityId,

                                CommercialCompanyName = oResult2.Documents.FirstOrDefault().CommercialCompanyName,

                                CompanyEnable = oResult2.Documents.FirstOrDefault().CompanyEnable,

                                CompanyName = oResult2.Documents.FirstOrDefault().CompanyName,

                                CompanyPublicId = oResult2.Documents.FirstOrDefault().CompanyPublicId,

                                Country = oResult2.Documents.FirstOrDefault().Country,
                                CountryId = oResult2.Documents.FirstOrDefault().CountryId,
                                CustomerPublicId = oResult2.Documents.FirstOrDefault().CustomerPublicId,

                                ICA = oResult2.Documents.FirstOrDefault().ICA,
                                ICAId = oResult2.Documents.FirstOrDefault().ICAId,

                                IdentificationNumber = oResult2.Documents.FirstOrDefault().IdentificationNumber,


                                IdentificationType = oResult2.Documents.FirstOrDefault().IdentificationType,
                                IdentificationTypeId = oResult2.Documents.FirstOrDefault().IdentificationTypeId,

                                InBlackList = oResult2.Documents.FirstOrDefault().InBlackList,

                                LogoUrl = oResult2.Documents.FirstOrDefault().LogoUrl,

                                oCustomerProviderIndexModel = oResult2.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                                PrincipalActivity = oResult2.Documents.FirstOrDefault().PrincipalActivity,
                                PrincipalActivityId = oResult2.Documents.FirstOrDefault().PrincipalActivityId,

                                ProviderStatus = oResult2.Documents.FirstOrDefault().ProviderStatus,
                                ProviderStatusId = oResult2.Documents.FirstOrDefault().ProviderStatusId,
                            };

                            #endregion

                            if (oCompanySurveyIndexModel.oCustomerProviderIndexModel.Any(x => x.CustomerPublicId == customer))
                            {
                                oCompanySurveyIndexModel.oCustomerProviderIndexModel.Where(x => x.CustomerPublicId == customer).All(x =>
                                {
                                    x.ProviderPublicId = ProviderPublicId;
                                    x.StatusId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Currency"]);
                                    x.Status = oModel.ProviderOptions.Where(y => y.ItemId == Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Currency"])).Select(y => y.ItemName).DefaultIfEmpty(string.Empty).FirstOrDefault();
                                    return true;
                                });
                            }

                            if (oCompanySurveyIndexModel.oCustomerProviderIndexModel.Any(x => x.CustomerPublicId == customer))
                            {
                                oCompanySurveyIndexModel.oCustomerProviderIndexModel.Where(x => x.CustomerPublicId == customer).All(x =>
                                {
                                    x.ProviderPublicId = ProviderPublicId;
                                    x.StatusId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Voiced"]);
                                    x.Status = oModel.ProviderOptions.Where(y => y.ItemId == Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["SH_Voiced"])).Select(y => y.ItemName).DefaultIfEmpty(string.Empty).FirstOrDefault();
                                    return true;
                                });
                            }

                            ICreateIndexResponse oElasticResponse2 = client2.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                                    .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                                    .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                                        .Tokenizer("whitespace")
                                        )).TokenFilters(tf => tf
                                                .EdgeNGram("customEdgeNGram", engrf => engrf
                                                .MinGram(1)
                                                .MaxGram(10)))).NumberOfShards(1)
                                    ));

                            var Index2 = client2.Index(oCompanySurveyIndexModel);
                        }
                    }

                    #endregion

                    #endregion
                }


            }
        }

        [HttpPost]
        [HttpGet]
        public BackOffice.Models.Provider.ProviderCustomerViewModel CPCustomerProviderUpdate
            (string CPCustomerProviderUpdate,
            string ProviderPublicId)
        {
            BackOffice.Models.Provider.ProviderCustomerViewModel oReturn = new ProviderCustomerViewModel();

            if (CPCustomerProviderUpdate == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]))
            {
                BackOffice.Models.Provider.ProviderCustomerViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.ProviderCustomerViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.ProviderCustomerViewModel));

                ProveedoresOnLine.Company.Models.Company.CompanyModel oProvider = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(ProviderPublicId);
                ProveedoresOnLine.Company.Models.Company.CompanyModel oCustomer = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(oDataToUpsert.CP_CustomerPublicId);

                CustomerModel oCustomerProvider = new CustomerModel()
                {
                    RelatedCompany = oCustomer,
                    RelatedProvider = new List<CustomerProviderModel>(){
                        new CustomerProviderModel ()
                        {
                            CustomerProviderId = Convert.ToInt32(oDataToUpsert.CP_CustomerProviderId),
                            RelatedProvider = oProvider,
                            Status = new CatalogModel(){
                                ItemId = Convert.ToInt32(oDataToUpsert.CP_StatusId),
                            },
                            Enable = Convert.ToBoolean(oDataToUpsert.CP_Enable),
                        },
                    },
                };

                ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.CustomerProviderUpsert(oCustomerProvider);

                /*Asociate related customer provider to Document Management*/
                ProveedoresOnLine.AsociateProvider.Client.Controller.AsociateProviderClient.AP_AsociateRelatedCustomerProvider(oCustomer.CompanyPublicId, ProviderPublicId, oCustomerProvider.RelatedProvider.FirstOrDefault().Enable);

                #region Index
                #region CompanyIndex

                Uri node = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);

                settings.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value);
                settings.DisableDirectStreaming(true);
                ElasticClient client = new ElasticClient(settings);

                //Getting Model from index
                Nest.ISearchResponse<CompanyIndexModel> oResult = client.Search<CompanyIndexModel>(s => s
                    .From(0)
                    .Size(1)
                    .Query(q => q.QueryString(qs => qs.Query(ProviderPublicId))));

                //Model to index 
                #region Model

                CompanyIndexModel oCompanyIndexModel = new CompanyIndexModel()
                {
                    CatlificationRating = oResult.Documents.FirstOrDefault().CatlificationRating,

                    City = oResult.Documents.FirstOrDefault().City,
                    CityId = oResult.Documents.FirstOrDefault().CityId,

                    CommercialCompanyName = oResult.Documents.FirstOrDefault().CommercialCompanyName,

                    CompanyEnable = oResult.Documents.FirstOrDefault().CompanyEnable,

                    CompanyName = oResult.Documents.FirstOrDefault().CompanyName,

                    CompanyPublicId = oResult.Documents.FirstOrDefault().CompanyPublicId,

                    Country = oResult.Documents.FirstOrDefault().Country,
                    CountryId = oResult.Documents.FirstOrDefault().CountryId,
                    CustomerPublicId = oResult.Documents.FirstOrDefault().CustomerPublicId,

                    ICA = oResult.Documents.FirstOrDefault().ICA,
                    ICAId = oResult.Documents.FirstOrDefault().ICAId,

                    IdentificationNumber = oResult.Documents.FirstOrDefault().IdentificationNumber,


                    IdentificationType = oResult.Documents.FirstOrDefault().IdentificationType,
                    IdentificationTypeId = oResult.Documents.FirstOrDefault().IdentificationTypeId,

                    InBlackList = oResult.Documents.FirstOrDefault().InBlackList,

                    LogoUrl = oResult.Documents.FirstOrDefault().LogoUrl,

                    oCustomerProviderIndexModel = oResult.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                    PrincipalActivity = oResult.Documents.FirstOrDefault().PrincipalActivity,
                    PrincipalActivityId = oResult.Documents.FirstOrDefault().PrincipalActivityId,

                    ProviderStatus = oResult.Documents.FirstOrDefault().ProviderStatus,
                    ProviderStatusId = oResult.Documents.FirstOrDefault().ProviderStatusId,
                };
                #endregion




                if (oCompanyIndexModel.oCustomerProviderIndexModel.Any(x => x.CustomerPublicId == oDataToUpsert.CP_CustomerPublicId))
                {
                    oCompanyIndexModel.oCustomerProviderIndexModel.Where(x => x.CustomerPublicId == oDataToUpsert.CP_CustomerPublicId).All(x =>
                    {
                        x.ProviderPublicId = ProviderPublicId;
                        x.CustomerProviderEnable = Convert.ToBoolean(oDataToUpsert.CP_Enable);
                        return true;
                    });

                    ICreateIndexResponse oElasticResponse = client.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                        .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                            .Tokenizer("whitespace")
                            )).TokenFilters(tf => tf
                                    .EdgeNGram("customEdgeNGram", engrf => engrf
                                    .MinGram(1)
                                    .MaxGram(10)))).NumberOfShards(1)
                        ));

                    var Index = client.Index(oCompanyIndexModel);
                }
                #endregion

                #region SurveyIndex

                Uri node2 = new Uri(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings2 = new ConnectionSettings(node2);

                settings2.DefaultIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanySurveyIndex].Value);
                settings2.DisableDirectStreaming(true);
                ElasticClient client2 = new ElasticClient(settings2);

                //Getting Model from index
                Nest.ISearchResponse<ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel> oResult2 = client2.Search<ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel>(s => s
                    .From(0)
                    .Size(1)
                    .Query(q => q.QueryString(qs => qs.Query(ProviderPublicId))));

                //Model to index 
                #region Model

                ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel oCompanySurveyIndexModel = new ProveedoresOnLine.SurveyModule.Models.Index.CompanySurveyIndexModel()
                {
                    CatlificationRating = oResult2.Documents.FirstOrDefault().CatlificationRating,

                    City = oResult2.Documents.FirstOrDefault().City,
                    CityId = oResult2.Documents.FirstOrDefault().CityId,

                    CommercialCompanyName = oResult2.Documents.FirstOrDefault().CommercialCompanyName,

                    CompanyEnable = oResult2.Documents.FirstOrDefault().CompanyEnable,

                    CompanyName = oResult2.Documents.FirstOrDefault().CompanyName,

                    CompanyPublicId = oResult2.Documents.FirstOrDefault().CompanyPublicId,

                    Country = oResult2.Documents.FirstOrDefault().Country,
                    CountryId = oResult2.Documents.FirstOrDefault().CountryId,
                    CustomerPublicId = oResult2.Documents.FirstOrDefault().CustomerPublicId,

                    ICA = oResult2.Documents.FirstOrDefault().ICA,
                    ICAId = oResult2.Documents.FirstOrDefault().ICAId,

                    IdentificationNumber = oResult2.Documents.FirstOrDefault().IdentificationNumber,


                    IdentificationType = oResult2.Documents.FirstOrDefault().IdentificationType,
                    IdentificationTypeId = oResult2.Documents.FirstOrDefault().IdentificationTypeId,

                    InBlackList = oResult2.Documents.FirstOrDefault().InBlackList,

                    LogoUrl = oResult2.Documents.FirstOrDefault().LogoUrl,

                    oCustomerProviderIndexModel = oResult2.Documents.FirstOrDefault().oCustomerProviderIndexModel,

                    PrincipalActivity = oResult2.Documents.FirstOrDefault().PrincipalActivity,
                    PrincipalActivityId = oResult2.Documents.FirstOrDefault().PrincipalActivityId,

                    ProviderStatus = oResult2.Documents.FirstOrDefault().ProviderStatus,
                    ProviderStatusId = oResult2.Documents.FirstOrDefault().ProviderStatusId,
                };
                #endregion

                if (oCompanySurveyIndexModel.oCustomerProviderIndexModel.Any(x => x.CustomerPublicId == oDataToUpsert.CP_CustomerPublicId))
                {
                    oCompanySurveyIndexModel.oCustomerProviderIndexModel.Where(x => x.CustomerPublicId == oDataToUpsert.CP_CustomerPublicId).All(x =>
                    {
                        x.ProviderPublicId = ProviderPublicId;
                        x.CustomerProviderEnable = Convert.ToBoolean(oDataToUpsert.CP_Enable);
                        return true;
                    });

                    ICreateIndexResponse oElasticResponse2 = client2.CreateIndex(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_CompanyIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                        .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                            .Tokenizer("whitespace")
                            )).TokenFilters(tf => tf
                                    .EdgeNGram("customEdgeNGram", engrf => engrf
                                    .MinGram(1)
                                    .MaxGram(10)))).NumberOfShards(1)
                        ));

                    var Index = client2.Index(oCompanySurveyIndexModel);
                }

                #endregion

                #endregion

            }

            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public BackOffice.Models.Provider.TrackingViewModel CPTrackingUpsert
            (string CPTrackingUpsert,
            string CustomerProviderId,
            string ProviderPublicId)
        {
            BackOffice.Models.Provider.TrackingViewModel oReturn = null;

            if (CustomerProviderId != string.Empty && CustomerProviderId != null &&
                CPTrackingUpsert == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]))
            {
                BackOffice.Models.Provider.TrackingViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.TrackingViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.TrackingViewModel));

                ProveedoresOnLine.Company.Models.Company.CompanyModel oCompanyModel = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(ProviderPublicId);

                CustomerProviderModel oCustomerProvider = new CustomerProviderModel()
                {
                    CustomerProviderId = Convert.ToInt32(CustomerProviderId),
                    CustomerProviderInfo = new List<GenericItemInfoModel>(){
                        new GenericItemInfoModel(){
                            ItemInfoId = Convert.ToInt32(oDataToUpsert.CPI_CustomerProviderInfoId),
                            ItemInfoType = new CatalogModel(){
                                ItemId = Convert.ToInt32(oDataToUpsert.RelatedCustomerProviderInfo.ItemInfoType.ItemId),
                            },
                            LargeValue = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(oDataToUpsert.CPI_Tracking),
                            Enable = oDataToUpsert.CPI_Enable,
                        },
                    }
                };

                ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.CustomerProviderInfoUpsert(oCustomerProvider);
            }
            return oReturn;
        }


        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.ProviderCustomerViewModel> GetAllCustomers
        (string GetAllCustomers,
            string ProviderPublicId, string SearchParam)
        {
            string[] oSearchParam = SearchParam.Split(',');
            List<BackOffice.Models.Provider.ProviderCustomerViewModel> oReturn = new List<Models.Provider.ProviderCustomerViewModel>();

            if (GetAllCustomers == "true")
            {
                ProveedoresOnLine.CompanyCustomer.Models.Customer.CustomerModel oCustomerByProvider =
                    ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.GetCustomerByProvider(ProviderPublicId, null);
                List<CustomerProviderModel> oCustomerProvider = new List<CustomerProviderModel>();

                if (oCustomerByProvider != null && oCustomerByProvider.RelatedProvider != null && oCustomerByProvider.RelatedProvider.Count > 0)
                {

                    oCustomerByProvider.RelatedProvider.All(x =>
                    {
                        oReturn.Add(new ProviderCustomerViewModel(
                                x.CustomerProviderId.ToString(),
                                x.RelatedProvider,
                                x.Enable
                            ));
                        return true;
                    });
                    if (!string.IsNullOrEmpty(oSearchParam[0]))
                        oReturn = oReturn.Where(x => x.CP_Customer.ToLower().Contains(oSearchParam[0])).Select(x => x).ToList();

                    if (oSearchParam[1] == "True")
                    {
                        oReturn.Add(new ProviderCustomerViewModel
                        {
                            CP_Customer = "A Quien Interese",
                            CP_CustomerPublicId = "00000000",
                        });
                    }
                }
            }
            return oReturn.OrderBy(x => x.CP_CustomerPublicId).Select(x => x).ToList();
        }

        [HttpPost]
        [HttpGet]
        public void CPUpsertCustomerByProviderStatus
        (string UpsertCustomerByProviderStatus,
         bool oIsCreate)
        {
            if (UpsertCustomerByProviderStatus == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]) &&
                oIsCreate != null)
            {
                ProviderCustomerViewModel oDataToUpsert =
                    (ProviderCustomerViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(ProviderCustomerViewModel));
            }
        }

        #endregion

        #region CalificationProjectConfigInfo

        [HttpPost]
        [HttpGet]
        public List<BackOffice.Models.Provider.CalificationProjectConfigInfoViewModel> CPCCalificationProjectConfigInfoProviderGetbyProvider
            (
                string CPCCalificationProjectConfigInfoProviderGetbyProvider,
                string ProviderPublicId,
                string Enable
            )
        {
            var oReturn = new List<CalificationProjectConfigInfoViewModel>();

            if (CPCCalificationProjectConfigInfoProviderGetbyProvider == "true")
            {
                var oConfigInfo = ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigInfoGetByProvider(ProviderPublicId, Enable == "true" ? true : false);

                if (oConfigInfo.Count > 0)
                {
                    oConfigInfo.All
                        (x =>
                        {
                            oReturn.Add(new CalificationProjectConfigInfoViewModel(x));
                            return true;
                        });
                }
            }
            return oReturn;
        }


        [HttpPost]
        [HttpGet]
        public void CPCCalificationProjectConfigInfoProviderUpsert(string CPCCalificationProjectConfigInfoProviderUpsert, string ProviderPublicId)
        {
            if (CPCCalificationProjectConfigInfoProviderUpsert == "true" &&
               !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]) &&
               !string.IsNullOrEmpty(ProviderPublicId))
            {
                BackOffice.Models.Provider.CalificationProjectConfigInfoViewModel oDataToUpsert =
                    (BackOffice.Models.Provider.CalificationProjectConfigInfoViewModel)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                                typeof(BackOffice.Models.Provider.CalificationProjectConfigInfoViewModel));

                ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigInfoUpsert(new ProveedoresOnLine.CalificationProject.Models.CalificationProject.ConfigInfoModel()
                {
                    CalificationProjectConfigInfoId = oDataToUpsert.CalificationProjectConfigInfoId != null ? int.Parse(oDataToUpsert.CalificationProjectConfigInfoId) : 0,
                    RelatedCalificationProjectConfig = new ProveedoresOnLine.CalificationProject.Models.CalificationProject.CalificationProjectConfigModel()
                    {
                        CalificationProjectConfigId = oDataToUpsert.CalificationProjectConfigId,
                        Company = new CompanyModel()
                        {
                            CompanyPublicId = ProviderPublicId
                        }
                    },
                    Enable = oDataToUpsert.Enable,
                });

            }
        }

        [HttpPost]
        [HttpGet]
        public void CPCStartProcessByProviderAndCustomer(string CPCStartProcessByProviderAndCustomer, string ProviderPublicId, string CalificationId)
        {
            var CalificationCompanyInfo = ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfig_GetByCalificationProjectConfigId(int.Parse(CalificationId));
            ProveedoresOnLine.CalificationProject.Controller.CalificationProject.StartProcessByProviderAndCustomer(ProviderPublicId, int.Parse(CalificationId));
            var Provider = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(ProviderPublicId);
            var Customer = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(CalificationCompanyInfo.Company.CompanyPublicId);
            MessageModule.Client.Models.ClientMessageModel oMessage = new MessageModule.Client.Models.ClientMessageModel()
            {
                Agent = Models.General.InternalSettings.Instance[Models.General.Constants.C_Settings_CalificationProcessEnd_Mail].Value,
                User = SessionModel.CurrentLoginUser.Email.ToString(),
                ProgramTime = DateTime.Now,
                MessageQueueInfo = new System.Collections.Generic.List<Tuple<string, string>>()
                                {
                                    new Tuple<string,string>("To", SessionModel.CurrentLoginUser.Email),
                                    new Tuple<string,string>("ProviderLogo", Provider.CompanyInfo.Any(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo) ? Provider.CompanyInfo.Where(y=> y.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo).Select(y => y.Value).FirstOrDefault() : Models.General.InternalSettings.Instance[Models.General.Constants.C_Settings_DefaultImage].Value),
                                    new Tuple<string,string>("ProviderName", Provider.CompanyName),
                                    new Tuple<string,string>("ProviderPublicId", ProviderPublicId),
                                    new Tuple<string,string>("ProviderIdentification",Provider.IdentificationNumber),
                                    new Tuple<string,string>("CustomerName",Customer.CompanyName),
                                    new Tuple<string,string>("CustomerIdentification",Customer.CompanyName),
                                    new Tuple<string,string>("CustomerLogo",Customer.CompanyInfo.Where(y=> y.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo).Select(y => y.Value).FirstOrDefault()),
                                    new Tuple<string,string>("CalificationProcessName",CalificationCompanyInfo.CalificationProjectConfigName),
                                },
            };
            MessageModule.Client.Controller.ClientController.CreateMessage(oMessage);            
        }

        #endregion

        #region Notifications

        [HttpPost]
        [HttpGet]
        public Boolean SendCertificationStatusProviderByCustomer(string SendCertificationStatusProviderByCustomer)
        {
            Boolean oRestul = false;
            
            string ProviderPublicId =
                    (string)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["ProviderPublicId"],
                                typeof(string));

            string CustomerProviderId =
                    (string)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["CustomerProviderId"],
                                typeof(string));

            string CustomerPublicId =
                    (string)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["CustomerPublicId"],
                                typeof(string));

            string[] Responsables =
                    (string[])
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["Responsables"],
                                typeof(string[]));

            string bodyEmail =
                    (string)
                    (new System.Web.Script.Serialization.JavaScriptSerializer()).
                    Deserialize(System.Web.HttpContext.Current.Request["bodyEmail"],
                                typeof(string));
            //Notification Certification

            if (SendCertificationStatusProviderByCustomer == "true")
            {
                if (!string.IsNullOrWhiteSpace(CustomerProviderId) && !string.IsNullOrWhiteSpace(CustomerPublicId))
                {
                    try
                    {
                        //get Provider Info
                        BackOffice.Models.Provider.ProviderViewModel oModel = new Models.Provider.ProviderViewModel();
                        oModel.RelatedProvider = new ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel()
                        {
                            RelatedCompany = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(ProviderPublicId),
                        };

                        //get Customer info
                        BackOffice.Models.Provider.ProviderViewModel oCustomer = new Models.Provider.ProviderViewModel();
                        oCustomer.RelatedProvider = new ProveedoresOnLine.CompanyProvider.Models.Provider.ProviderModel()
                        {
                            RelatedCompany = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(CustomerPublicId),
                        };

                        //Create Certification
                        List<ReportParameter> parameters = new List<ReportParameter>();

                        parameters.Add(new ReportParameter("ProviderName", oModel.RelatedProvider.RelatedCompany.CompanyName.ToString()));

                        parameters.Add(new ReportParameter("DocumentType", oModel.RelatedProvider.RelatedCompany.IdentificationType.ItemName.ToString()));

                        parameters.Add(new ReportParameter("IdentificationProvider", oModel.RelatedProvider.RelatedCompany.IdentificationNumber.ToString()));

                        parameters.Add(new ReportParameter("CustomerLogo", oCustomer.RelatedProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo).Select(x => x.Value).FirstOrDefault()));

                        parameters.Add(new ReportParameter("ExpeditionDate", DateTime.Now.ToString()));
                        
                        string stringCalification = oModel.RelatedProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.CalificationProcess).Select(x => x.Value).FirstOrDefault();

                        if (stringCalification != null)
                        {
                            string[] split = stringCalification.Split('_');

                            if (split[0] == CustomerPublicId)
                            {
                                parameters.Add(new ReportParameter("CalificationProviderValue", split[1].ToUpper()));

                                parameters.Add(new ReportParameter("CalificationProviderName", split[2]));
                            }
                            else
                            {
                                parameters.Add(new ReportParameter("CalificationProviderValue", "NA"));

                                parameters.Add(new ReportParameter("CalificationProviderName", "NA"));
                            }
                        }
                        else
                        {
                            parameters.Add(new ReportParameter("CalificationProviderValue", "NA"));

                            parameters.Add(new ReportParameter("CalificationProviderName", "NA"));
                        }

                        Tuple<byte[], string, string> report = ProveedoresOnLine.Reports.Controller.ReportModule.GetProviderValitaed(
                                                            parameters,
                                                            enumCategoryInfoType.PDF.ToString(),
                                                            Models.General.InternalSettings.Instance[Models.General.Constants.C_Settings_BO_CP_ReportPath].Value.Trim() + "C_ReportProviderValidated.rdlc");

                        //Save Certifications S3
                        //get folder
                        string strFolder = System.Web.HttpContext.Current.Server.MapPath
                            (BackOffice.Models.General.InternalSettings.Instance
                            [BackOffice.Models.General.Constants.C_Settings_File_TempDirectory].Value);

                        if (!System.IO.Directory.Exists(strFolder))
                            System.IO.Directory.CreateDirectory(strFolder);

                        string strFile = strFolder.TrimEnd('\\') +
                                "\\CompanyFile_" +
                                oModel.RelatedProvider.RelatedCompany.CompanyPublicId + "_" +
                                DateTime.Now.ToString("yyyyMMddHHmmss") + "." +
                                "pdf";

                        System.IO.File.WriteAllBytes(strFile, report.Item1);

                        //load file to s3
                        string strRemoteFile = ProveedoresOnLine.FileManager.FileController.LoadFile
                            (strFile,
                            BackOffice.Models.General.InternalSettings.Instance
                                [BackOffice.Models.General.Constants.C_Settings_File_RemoteDirectory].Value.TrimEnd('\\') +
                                "\\CompanyFile\\" + oModel.RelatedProvider.RelatedCompany.CompanyPublicId + "\\");

                        //remove temporal file
                        if (System.IO.File.Exists(strFile))
                            System.IO.File.Delete(strFile);

                        //Send Emails
                        if (Responsables.Length > 0)
                        {
                            Responsables.Where(x => !string.IsNullOrEmpty(x)).All(x =>
                            {
                                MessageModule.Client.Models.ClientMessageModel oMessage = new MessageModule.Client.Models.ClientMessageModel()
                                {
                                    Agent = Models.General.InternalSettings.Instance[Models.General.Constants.C_Settings_N_Provider_Certification_Mail].Value,
                                    User = SessionModel.CurrentLoginUser.Email.ToString(),
                                    ProgramTime = DateTime.Now,
                                    MessageQueueInfo = new System.Collections.Generic.List<Tuple<string, string>>()
                                {
                                    new Tuple<string,string>("To",x.Replace("Emails_","")),
                                    new Tuple<string,string>("CertificationUrl", strRemoteFile),
                                    new Tuple<string,string>("ProviderName",oModel.RelatedProvider.RelatedCompany.CompanyName.ToString()),
                                    new Tuple<string,string>("IdentificationProvider",oModel.RelatedProvider.RelatedCompany.IdentificationNumber.ToString()),
                                    new Tuple<string,string>("CustomerName",oCustomer.RelatedProvider.RelatedCompany.CompanyName),
                                    new Tuple<string,string>("CustomerLogo",oCustomer.RelatedProvider.RelatedCompany.CompanyInfo.Where(y=> y.ItemInfoType.ItemId == (int)enumCompanyInfoType.CompanyLogo).Select(y => y.Value).FirstOrDefault()),
                                    new Tuple<string,string>("BodyMessage", bodyEmail == null ? ""  : bodyEmail),
                                },
                                };

                                MessageModule.Client.Controller.ClientController.CreateMessage(oMessage);

                                return true;
                            });
                        }

                        //Save Certification
                        CustomerProviderModel oCustomerProvider = new CustomerProviderModel()
                        {
                            CustomerProviderId = Convert.ToInt32(CustomerProviderId),
                            CustomerProviderInfo = new List<GenericItemInfoModel>() {
                            new GenericItemInfoModel(){
                                ItemInfoType = new CatalogModel(){
                                    ItemId = (int)enumProviderCustomerType.CertificationUrl
                                },
                                Value = "",
                                LargeValue = strRemoteFile,
                                Enable = true
                            }
                        }
                        };

                        ProveedoresOnLine.CompanyCustomer.Controller.CompanyCustomer.CustomerProviderInfoUpsert(oCustomerProvider);
                    }
                    catch (Exception ex)
                    {
                        oRestul = false;
                    }
                }
            }
            return oRestul;
        }

        #endregion
    }
}
