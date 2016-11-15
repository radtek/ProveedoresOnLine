using MarketPlace.Models.General;
using MarketPlace.Models.Provider;
using MarketPlace.Models.Survey;
using Microsoft.Reporting.WebForms;
using Nest;
using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.CompanyProvider.Models.Provider;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.SurveyModule.Models;
using ProveedoresOnLine.SurveyModule.Models.Index;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace MarketPlace.Web.Controllers
{
    public partial class SurveyController : BaseController
    {
        public virtual ActionResult Index(string SurveyPublicId, string StepId)
        {
            //Clean the season url saved
            if (MarketPlace.Models.General.SessionModel.CurrentURL != null)
                MarketPlace.Models.General.SessionModel.CurrentURL = null;

            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedSurvey = new SurveyViewModel
                (ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetById(SurveyPublicId));

            //get survey info
            if (oModel.RelatedSurvey.RelatedSurvey.ParentSurveyPublicId == null)
            {
                oModel.RelatedSurvey = new SurveyViewModel(
                    ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetByUser
                    (SurveyPublicId, SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().User));
            }


            List<GenericItemInfoModel> oAssignedAreas = oModel.RelatedSurvey.RelatedSurvey.SurveyInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumSurveyInfoType.CurrentArea).Select(inf => inf).ToList();

            //Get Only Rol Area's
            List<GenericItemModel> Areas = new List<GenericItemModel>();
            List<GenericItemModel> Answers = new List<GenericItemModel>();

            if (oAssignedAreas.Count > 0)
            {
                oAssignedAreas.All(ev =>
                {
                    //Get Areas
                    Areas.AddRange(oModel.RelatedSurvey.RelatedSurvey.RelatedSurveyConfig.RelatedSurveyConfigItem.Where(x => x.ItemId == Convert.ToInt32(ev.Value.Split('_')[0])).Select(x => x).ToList());
                    //Get Areas Iteminfo
                    Areas.AddRange(oModel.RelatedSurvey.RelatedSurvey.RelatedSurveyConfig.RelatedSurveyConfigItem.Where(x => x.ParentItem != null && x.ParentItem.ItemId == Convert.ToInt32(ev.Value.Split('_')[0])).Select(x => x).ToList());
                    return true;
                });
                if (Areas.Count > 0)
                {
                    Areas.All(qs =>
                    {
                        Answers.AddRange(oModel.RelatedSurvey.RelatedSurvey.RelatedSurveyConfig.RelatedSurveyConfigItem.Where(x => x.ItemType.ItemId == (int)enumSurveyConfigItemType.Answer && x.ParentItem.ItemId == qs.ItemId).Select(x => x).ToList());
                        return true;
                    });
                    if (Answers.Count > 0)
                    {
                        Areas.AddRange(Answers);
                    }
                }
                oModel.RelatedSurvey.RelatedSurvey.RelatedSurveyConfig.RelatedSurveyConfigItem = Areas;
            }

            //get basic provider info
            var olstProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.MPProviderSearchById
                (SessionModel.CurrentCompany.CompanyPublicId, oModel.RelatedSurvey.RelatedSurvey.RelatedProvider.RelatedCompany.CompanyPublicId);

            var oProvider = olstProvider.
                Where(x => SessionModel.CurrentCompany.CompanyType.ItemId == (int)enumCompanyType.BuyerProvider ?
                            (x.RelatedCompany.CompanyPublicId == oModel.RelatedSurvey.RelatedSurvey.RelatedProvider.RelatedCompany.CompanyPublicId ||
                            x.RelatedCustomerInfo.Any(y => y.Key == SessionModel.CurrentCompany.CompanyPublicId)) :
                            (SessionModel.CurrentCompany.CompanyType.ItemId == (int)enumCompanyType.Buyer ?
                            x.RelatedCustomerInfo.Any(y => y.Key == SessionModel.CurrentCompany.CompanyPublicId) :
                            x.RelatedCompany.CompanyPublicId == oModel.RelatedSurvey.RelatedSurvey.RelatedProvider.RelatedCompany.CompanyPublicId)).
                FirstOrDefault();

            //validate provider permisions
            if (oProvider == null)
            {
                //return url provider not allowed
            }
            else
            {
                //get provider view model
                oModel.RelatedLiteProvider = new ProviderLiteViewModel(oProvider);

                oModel.RelatedSurvey.RelatedSurvey.RelatedSurveyItem = oModel.RelatedSurvey.RelatedSurvey.RelatedSurveyItem.
                                                Where(x => x.EvaluatorRoleId == SessionManager.SessionController.POLMarketPlace_MarketPlaceUserLogin.RelatedCompany.FirstOrDefault().RelatedUser.
                                                               Where(y => y.RelatedCompanyRole.RoleCompanyId != 0).Select(y => y.UserCompanyId).FirstOrDefault()).Select(x => x).ToList();

                //get current StepId
                oModel.RelatedSurvey.CurrentStepId = string.IsNullOrEmpty(StepId) ? oModel.RelatedSurvey.GetSurveyConfigFirstStep() : Convert.ToInt32(StepId.Trim());

                //get survey action menu
                oModel.RelatedSurvey.CurrentActionMenu = GetSurveyAction(oModel);
            }

            return View(oModel);
        }

        public virtual ActionResult Search(string ProjectPublicId,
            string CompareId,
            string SearchParam,
            string SearchFilter,
            string SearchOrderType,
            string OrderOrientation,
            string PageNumber)
        {
            //Clean the season url saved
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            ProviderSearchViewModel oModel = null;

            if (SessionModel.CurrentCompany != null &&
                !string.IsNullOrEmpty(SessionModel.CurrentCompany.CompanyPublicId))
            {
                Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);

                //get basic search model
                oModel = new ProviderSearchViewModel()
                {
                    ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions(),
                    SearchParam = SearchParam,
                    SearchFilter = SearchFilter == null ? null : (SearchFilter.Trim(new char[] { ',' }).Length > 0 ? SearchFilter.Trim(new char[] { ',' }) : null),
                    SearchOrderType = string.IsNullOrEmpty(SearchOrderType) ? MarketPlace.Models.General.enumSearchOrderType.Relevance : (MarketPlace.Models.General.enumSearchOrderType)Convert.ToInt32(SearchOrderType),
                    OrderOrientation = string.IsNullOrEmpty(OrderOrientation) ? false : ((OrderOrientation.Trim().ToLower() == "1") || (OrderOrientation.Trim().ToLower() == "true")),
                    PageNumber = string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber),
                    ProviderSearchResult = new List<ProviderLiteViewModel>(),
                    CompanyIndexModel = new ProveedoresOnLine.Company.Models.Company.CompanyIndexModel(),
                    CityFilter = new List<ElasticSearchFilter>(),
                    CountryFilter = new List<ElasticSearchFilter>(),
                    IcaFilter = new List<ElasticSearchFilter>(),
                    StatusFilter = new List<ElasticSearchFilter>(),
                    BlackListFilter = new List<ElasticSearchFilter>(),
                    MyProvidersFilter = new List<ElasticSearchFilter>(),
                    OtherProvidersFilter = new List<ElasticSearchFilter>(),
                    SurveyStatus = new List<ElasticSearchFilter>(),
                    SurveyType = new List<ElasticSearchFilter>(),
                };

                #region ElasticSearch

                List<Tuple<string, string, string>> lstSearchFilter = new List<Tuple<string, string, string>>();
                if (!string.IsNullOrEmpty(SearchFilter))
                {
                    lstSearchFilter = oModel.GetlstSearchFilter();
                }

                #region Search Result Company

                settings.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_CompanyIndex].Value);
                settings.DisableDirectStreaming(true);
                ElasticClient client = new ElasticClient(settings);

                oModel.ElasticCompanyModel = client.Search<CompanyIndexModel>(s => s
                .From(string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber) * 20)
                .TrackScores(true)
                .Size(20)
                .Aggregations
                    (agg => agg
                    .Terms("city", aggv => aggv
                        .Field(fi => fi.CityId))
                    .Terms("country", c => c
                        .Field(fi => fi.CountryId)))
                .Query(q => q.
                    Filtered(f => f
                    .Query(q1 => q1.MatchAll() && q.QueryString(qs => qs.Query(SearchParam)))
                    .Filter(f2 =>
                    {
                        QueryContainer qb = null;

                        #region Basic Providers Filters
                        if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.City).Select(y => y).FirstOrDefault() != null)
                        {
                            qb &= q.Term(m => m.CityId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.City).Select(y => y.Item1).FirstOrDefault());
                        }
                        if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.Country).Select(y => y).FirstOrDefault() != null)
                        {
                            qb &= q.Term(m => m.CountryId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.Country).Select(y => y.Item1).FirstOrDefault());
                        }
                        #endregion

                        return qb;
                    })
                    ))
                );

                oModel.TotalRows = (int)oModel.ElasticCompanyModel.Total;
                #endregion

                #region Search Result Survey

                var settingsSurvey = new ConnectionSettings(node);
                settingsSurvey.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value);
                settingsSurvey.DisableDirectStreaming(true);
                ElasticClient srvclient = new ElasticClient(settingsSurvey);

                oModel.ElasticSurveyModel = srvclient.Search<SurveyIndexSearchModel>(s => s
                .From(string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber) * 20)
                .TrackScores(true)
                .Size(20)
                .Aggregations
                    (agg => agg
                    .Terms("srv_status", aggv => aggv
                        .Field(fi => fi.SurveyStatusId))
                    .Terms("srv_type", c => c
                        .Field(fi => fi.SurveyTypeId)))
                .Query(q => q.
                    Filtered(f => f
                     .Query(qi => qi.QueryString(qr => qr.Fields(fds => fds.Field(f1 => f1.CustomerPublicId)).Query(SessionModel.CurrentCompany.CompanyPublicId)))
                        //.Query(q1 => q1.Match(m => m.Field(fld => fld.CustomerPublicId) == "")
                        //MatchAll() && q.Term(po => po.CustomerPublicId, SessionModel.CurrentCompany.CompanyPublicId))
                    .Filter(f2 =>
                    {
                        QueryContainer qb = null;

                        #region Status Srv Filters
                        if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.SurveyStatus).Select(y => y).FirstOrDefault() != null)
                        {
                            qb &= q.Term(m => m.SurveyStatusId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.SurveyStatus).Select(y => y.Item1).FirstOrDefault());
                        }
                        #endregion

                        #region Type Srv Filters
                        if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.SurveyType).Select(y => y).FirstOrDefault() != null)
                        {
                            qb &= q.Term(m => m.SurveyTypeId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.SurveyType).Select(y => y.Item1).FirstOrDefault());
                        }
                        #endregion

                        return qb;
                    })
                    ))
                );

                oModel.TotalRows = (int)oModel.ElasticCompanyModel.Total;
                #endregion

                //parse view model
                if (oModel.ElasticCompanyModel != null && oModel.ElasticCompanyModel.Documents.Count() > 0)
                {
                    oModel.ElasticCompanyModel.Documents.All(prv =>
                    {
                        oModel.ProviderSearchResult.Add
                            (new ProviderLiteViewModel(prv));

                        return true;
                    });
                }

                #region City Aggregation

                oModel.ElasticCompanyModel.Aggs.Terms("city").Buckets.All(x =>
                {
                    oModel.CityFilter.Add(new ElasticSearchFilter
                    {
                        FilterCount = (int)x.DocCount,
                        FilterType = x.Key.Split('.')[0],
                        FilterName = MarketPlace.Models.Company.CompanyUtil.GetCityName(x.Key.Split('.')[0]),
                    });
                    return true;
                });

                #endregion

                #region Country Aggregation

                oModel.ElasticCompanyModel.Aggs.Terms("country").Buckets.All(x =>
                {
                    oModel.CountryFilter.Add(new ElasticSearchFilter
                    {
                        FilterCount = (int)x.DocCount,
                        FilterType = x.Key.Split('.')[0],
                        FilterName = MarketPlace.Models.Company.CompanyUtil.GetCountryNameByCountryId(x.Key.Split('.')[0]),
                    });
                    return true;
                });
                #endregion

                #region Survey Status Aggregation

                oModel.ElasticSurveyModel.Aggs.Terms("srv_status").Buckets.All(x =>
                {
                    oModel.SurveyStatus.Add(new ElasticSearchFilter
                    {
                        FilterCount = (int)x.DocCount,
                        FilterType = x.Key.Split('.')[0],
                        FilterName = MarketPlace.Models.Company.CompanyUtil.GetProviderOptionName(x.Key.Split('.')[0]),
                    });
                    return true;
                });
                #endregion

                #region Survey Type Aggregation

                oModel.ElasticSurveyModel.Aggs.Terms("srv_type").Buckets.All(x =>
                {
                    oModel.SurveyType.Add(new ElasticSearchFilter
                    {
                        FilterCount = (int)x.DocCount,
                        FilterType = x.Key.Split('.')[0],
                        FilterName = MarketPlace.Models.Survey.SurveyUtil.GetSurveyOptionName(x.Key.Split('.')[0]),
                    });
                    return true;
                });
                #endregion

                #endregion

                if (lstSearchFilter != null && lstSearchFilter.Count > 0)
                {
                    string newFilterUrl = "";
                    lstSearchFilter.All(x =>
                    {
                        if (int.Parse(x.Item3) == (int)enumFilterType.City)
                            newFilterUrl += "," + x.Item1 + ";" + oModel.CityFilter.FirstOrDefault().FilterCount + ";" + x.Item3;

                        if (int.Parse(x.Item3) == (int)enumFilterType.Country)
                            newFilterUrl += "," + x.Item1 + ";" + oModel.CountryFilter.FirstOrDefault().FilterCount + ";" + x.Item3 + ",";

                        if (int.Parse(x.Item3) == (int)enumFilterType.SurveyStatus)
                            newFilterUrl += "," + x.Item1 + ";" + oModel.SurveyStatus.FirstOrDefault().FilterCount + ";" + x.Item3;

                        if (int.Parse(x.Item3) == (int)enumFilterType.SurveyType)
                            newFilterUrl += "," + x.Item1 + ";" + oModel.SurveyType.FirstOrDefault().FilterCount + ";" + x.Item3 + ",";

                        return true;
                    });
                    oModel.SearchFilter = newFilterUrl;
                }
            }

            return View(oModel);
        }

        public virtual ActionResult SurveySearchProgram()
        {
            ProviderViewModel oModel = new ProviderViewModel();

            //Clean the season url saved
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            #region Survey Search

            if (!string.IsNullOrEmpty(Request["UpsertAction"]) && Request["UpsertAction"].Trim() == "true")
            {
                oModel.RelatedSurveyProviders = Request["ProviderPublicId"];
            }

            #endregion

            #region Survey Program

            if (!string.IsNullOrEmpty(Request["UpsertSurveySearch"]) && Request["UpsertSurveySearch"].Trim() == "true")
            {
                List<SurveyModel> SurveyListToUpsert = GetSurveySearchUpsertRequest();

                if (SurveyListToUpsert != null &&
                    SurveyListToUpsert.Count > 0)
                {
                    SurveyListToUpsert.All(Survey =>
                    {
                        Survey = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyUpsert(Survey);

                        /*Get Survey Config by surveyconfigid*/
                        SurveyConfigModel oSurveyConfigModel = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyConfigGetById(Survey.RelatedSurveyConfig.ItemId);

                        SurveyIndexSearchModel oModelToIndex = new SurveyIndexSearchModel()
                        {
                            CompanyPublicId = Survey.RelatedProvider.RelatedCompany.CompanyPublicId,
                            CustomerPublicId = SessionModel.CurrentCompany.CompanyPublicId,
                            SurveyPublicId = Survey.SurveyPublicId,
                            SurveyStatusId = Survey.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).DefaultIfEmpty(string.Empty).FirstOrDefault(),
                            SurveyStatus = MarketPlace.Models.Company.CompanyUtil.GetProviderOptionName(Survey.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).DefaultIfEmpty(string.Empty).FirstOrDefault()),
                            SurveyTypeId = oSurveyConfigModel.ItemId.ToString(),
                            SurveyType = oSurveyConfigModel.ItemName,
                        };

                        Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                        var settings = new ConnectionSettings(node);

                        settings.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value);
                        settings.DisableDirectStreaming(true);
                        ElasticClient client = new ElasticClient(settings);

                        ICreateIndexResponse oElasticResponse = client.CreateIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                        .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                            .Tokenizer("whitespace")
                            )).TokenFilters(tf => tf
                                    .EdgeNGram("customEdgeNGram", engrf => engrf
                                    .MinGram(1)
                                    .MaxGram(10)))).NumberOfShards(1)
                        ));

                        var Index = client.Index(oModelToIndex);

                        return true;
                    });
                }
            }

            #endregion

            return View(oModel);
        }

        public virtual ActionResult SurveyUpsert(string SurveyPublicId, string StepId)
        {
            //get survey request model
            ProveedoresOnLine.SurveyModule.Models.SurveyModel oSurveyToUpsert =
                GetSurveyUpsertRequest(SurveyPublicId);

            //upsert survey
            oSurveyToUpsert = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyInfoUpsert(oSurveyToUpsert);
            oSurveyToUpsert = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyItemUpsert(oSurveyToUpsert);

            #region Index Survey

            /*Get Survey Config by surveyconfigid*/
            SurveyModel oSurveyModel = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetById(SurveyPublicId);

            Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);

            settings.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value);
            settings.DisableDirectStreaming(true);
            ElasticClient client = new ElasticClient(settings);

            Nest.ISearchResponse<SurveyIndexSearchModel> oResult = client.Search<SurveyIndexSearchModel>(s => s
                .From(0)
                .Size(1)
                .Query(q => q.QueryString(qs => qs.Query(oSurveyModel.ParentSurveyPublicId))));

            SurveyIndexSearchModel oModelToIndex = oResult.Documents.FirstOrDefault();

            oModelToIndex.SurveyStatusId = oSurveyToUpsert.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).DefaultIfEmpty(string.Empty).FirstOrDefault();
            oModelToIndex.SurveyStatus = MarketPlace.Models.Company.CompanyUtil.GetProviderOptionName(oSurveyToUpsert.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).DefaultIfEmpty(string.Empty).FirstOrDefault());

            ICreateIndexResponse oElasticResponse = client.CreateIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value, c => c
            .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
            .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                .Tokenizer("whitespace")
                )).TokenFilters(tf => tf
                        .EdgeNGram("customEdgeNGram", engrf => engrf
                        .MinGram(1)
                        .MaxGram(10)))).NumberOfShards(1)
            ));

            var Index = client.Index(oModelToIndex);

            #endregion

            //recalculate survey item values
            ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyRecalculate(SurveyPublicId, SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().RelatedCompanyRole.RoleCompanyId
                                , SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().User);

            //redirect
            return RedirectToRoute
                (MarketPlace.Models.General.Constants.C_Routes_Default,
                new
                {
                    controller = MVC.Survey.Name,
                    action = MVC.Survey.ActionNames.Index,
                    SurveyPublicId = SurveyPublicId,
                    StepId = StepId,
                });
        }

        public virtual ActionResult SurveyFinalize(string SurveyPublicId)
        {
            //get survey request model
            ProveedoresOnLine.SurveyModule.Models.SurveyModel oSurveyToUpsert =
                GetSurveyFinalizeRequest(SurveyPublicId);

            //upsert survey
            if (oSurveyToUpsert.SurveyInfo != null && oSurveyToUpsert.SurveyInfo.Count > 0)
            {
                //upsert survey info
                oSurveyToUpsert = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyInfoUpsert(oSurveyToUpsert);

                #region Index Survey

                /*Get Survey Config by surveyconfigid*/
                SurveyModel oSurveyModel = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetById(SurveyPublicId);

                Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);

                settings.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value);
                settings.DisableDirectStreaming(true);
                ElasticClient client = new ElasticClient(settings);

                Nest.ISearchResponse<SurveyIndexSearchModel> oResult = client.Search<SurveyIndexSearchModel>(s => s
                    .From(0)
                    .Size(1)
                    .Query(q => q.QueryString(qs => qs.Query(oSurveyModel.ParentSurveyPublicId))));

                SurveyIndexSearchModel oModelToIndex = oResult.Documents.FirstOrDefault();

                oModelToIndex.SurveyStatusId = oSurveyToUpsert.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).DefaultIfEmpty(string.Empty).FirstOrDefault();
                oModelToIndex.SurveyStatus = MarketPlace.Models.Company.CompanyUtil.GetProviderOptionName(oSurveyToUpsert.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).DefaultIfEmpty(string.Empty).FirstOrDefault());

                ICreateIndexResponse oElasticResponse = client.CreateIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value, c => c
                .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                    .Tokenizer("whitespace")
                    )).TokenFilters(tf => tf
                            .EdgeNGram("customEdgeNGram", engrf => engrf
                            .MinGram(1)
                            .MaxGram(10)))).NumberOfShards(1)
                ));

                var Index = client.Index(oModelToIndex);

                #endregion
            }

            //recalculate survey item values
            ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyRecalculate(SurveyPublicId,
                                        SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().RelatedCompanyRole.RoleCompanyId
                                        , SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().User);

            //redirect
            return RedirectToRoute
                (MarketPlace.Models.General.Constants.C_Routes_Default,
                new
                {
                    controller = MVC.Survey.Name,
                    action = MVC.Survey.ActionNames.Index,
                    SurveyPublicId = SurveyPublicId,
                });
        }

        #region Provider Survey

        public virtual ActionResult SVSurveySearch(
            string ProviderPublicId,
            string SearchOrderType,
            string OrderOrientation,
            string PageNumber,
            string InitDate,
            string EndDate)
        {
            ProviderViewModel oModel = new ProviderViewModel();

            //Clean the season url saved
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            //get basic provider info
            var olstProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.MPProviderSearchById
                (SessionModel.CurrentCompany.CompanyPublicId, ProviderPublicId);

            var oProvider = olstProvider.
                Where(x => SessionModel.CurrentCompany.CompanyType.ItemId == (int)enumCompanyType.BuyerProvider ?
                            (x.RelatedCompany.CompanyPublicId == ProviderPublicId) :
                            x.RelatedCompany.CompanyPublicId == ProviderPublicId).FirstOrDefault();

            //validate provider permisions
            if (oProvider == null)
            {
                //return url provider not allowed
            }
            else
            {
                //get provider view model
                oModel.RelatedLiteProvider = new ProviderLiteViewModel(oProvider);
                oModel.ProviderMenu = GetSurveyMenu(oModel);

                oModel.RelatedSurveySearch = new Models.Survey.SurveySearchViewModel()
                {
                    SearchOrderType = string.IsNullOrEmpty(SearchOrderType) ? enumSurveySearchOrderType.LastModify : (enumSurveySearchOrderType)Convert.ToInt32(SearchOrderType),
                    OrderOrientation = string.IsNullOrEmpty(OrderOrientation) ? false : ((OrderOrientation.Trim().ToLower() == "1") || (OrderOrientation.Trim().ToLower() == "true")),
                    PageNumber = string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber),
                    SurveySearchResult = new List<Models.Survey.SurveyViewModel>(),
                };

                if (SessionModel.CurrentCompany != null &&
                !string.IsNullOrEmpty(SessionModel.CurrentCompany.CompanyPublicId))
                {
                    //search survey
                    int oTotalRowsAux;
                    List<SurveyModel> oSurveyResults = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveySearch
                            (SessionModel.CurrentCompany.CompanyPublicId,
                            oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.CompanyPublicId,
                            (int)oModel.RelatedSurveySearch.SearchOrderType,
                            oModel.RelatedSurveySearch.OrderOrientation,
                            oModel.RelatedSurveySearch.PageNumber,
                            oModel.RelatedSurveySearch.RowCount,
                            out oTotalRowsAux);

                    //Validar q no tenga evaluaciones
                    if (SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().RelatedCompanyRole.ParentRoleCompany == null)
                    {
                        if (oSurveyResults != null)
                        {
                            oSurveyResults = oSurveyResults.Where(x => x.ParentSurveyPublicId == null).Select(x => x).ToList();
                        }
                    }
                    else
                    {
                        if (oSurveyResults != null)
                        {
                            List<SurveyModel> oChildSurvey = new List<SurveyModel>();
                            oSurveyResults.All(x =>
                            {
                                oChildSurvey.Add(ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetByUser(x.SurveyPublicId, SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().User));

                                oChildSurvey.All(y =>
                                {
                                    if (y != null && y.ParentSurveyPublicId == x.SurveyPublicId && y.SurveyInfo.Count == 0)
                                        y.SurveyInfo = x.SurveyInfo;

                                    return true;
                                });
                                return true;
                            });
                            oSurveyResults = oChildSurvey.Where(x => x != null).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(InitDate) && !string.IsNullOrEmpty(EndDate)
                        && oSurveyResults != null && oSurveyResults.Count > 0)
                    {
                        oSurveyResults = oSurveyResults.Where(x =>
                                                        Convert.ToDateTime(x.CreateDate.ToString("yyyy-MM-dd")) >= Convert.ToDateTime(InitDate) &&
                                                        Convert.ToDateTime(x.CreateDate.ToString("yyyy-MM-dd")) <= Convert.ToDateTime(EndDate)).
                                                        Select(x => x).ToList();
                    }
                    oModel.RelatedSurveySearch.TotalRows = oTotalRowsAux;

                    //parse view model
                    if (oSurveyResults != null && oSurveyResults.Count > 0)
                    {
                        //Get the Average
                        decimal Average = 0;
                        //Get ClosedSurve
                        List<SurveyModel> ClosedSurvey = oSurveyResults.Where(x => x.SurveyInfo.
                                                        Where(y => y.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).
                                                        Select(y => y.Value == ((int)enumSurveyStatus.Close).ToString()).FirstOrDefault()).
                                                        Select(x => x).ToList();
                        oSurveyResults.All(srv =>
                        {
                            oModel.RelatedSurveySearch.SurveySearchResult.Add
                                (new MarketPlace.Models.Survey.SurveyViewModel(srv));
                            return true;
                        });

                        if (oModel.RelatedSurveySearch.SurveySearchResult != null && oModel.RelatedSurveySearch.SurveySearchResult.Count > 0)
                        {
                            oModel.RelatedSurveySearch.SurveySearchResult.All(sv =>
                            {
                                if (sv.SurveyStatus == enumSurveyStatus.Close)
                                    Average = (Average += Convert.ToDecimal(sv.SurveyRating.ToString("#,0.##")));
                                return true;
                            });
                            Average = Average != 0 ? Average / oModel.RelatedSurveySearch.SurveySearchResult.Where(x => x.SurveyStatus == enumSurveyStatus.Close).Count() : 0;
                        }

                        //Set the Average
                        oModel.RelatedSurveySearch.SurveySearchResult.FirstOrDefault().Average = Average;
                        if (!string.IsNullOrEmpty(InitDate) && !string.IsNullOrEmpty(EndDate))
                        {
                            oModel.RelatedSurveySearch.SurveySearchResult.FirstOrDefault().FilterDateIni = Convert.ToDateTime(InitDate);
                            oModel.RelatedSurveySearch.SurveySearchResult.FirstOrDefault().FilterEndDate = Convert.ToDateTime(EndDate);
                        }
                    }
                }
            }

            #region report generator

            if (Request["UpsertRequest"] == "true")
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                ProviderModel oToInsert = new ProviderModel()
                {
                    RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                    {
                        CompanyPublicId = ProviderPublicId,
                    },
                    RelatedReports = new List<GenericItemModel>(),
                };
                oToInsert.RelatedReports.Add(this.GetSurveyReportFilterRequest());
                ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.MPReportUpsert(oToInsert);

                parameters.Add(new ReportParameter("currentCompanyName", SessionModel.CurrentCompany.CompanyName));
                parameters.Add(new ReportParameter("currentCompanyTipoDni", SessionModel.CurrentCompany.IdentificationType.ItemName));
                parameters.Add(new ReportParameter("currentCompanyDni", SessionModel.CurrentCompany.IdentificationNumber));
                parameters.Add(new ReportParameter("currentCompanyLogo", SessionModel.CurrentCompany_CompanyLogo));
                parameters.Add(new ReportParameter("providerName", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.CompanyName.ToString()));
                parameters.Add(new ReportParameter("providerTipoDni", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.IdentificationType.ItemName.ToString()));
                parameters.Add(new ReportParameter("providerDni", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.IdentificationNumber.ToString()));
                //order items reports
                if (oToInsert.RelatedReports != null)
                {
                    oToInsert.RelatedReports.All(x =>
                    {
                        parameters.Add(new ReportParameter("remarks",
                            x.ItemInfo.Where(y => y.ItemInfoType.ItemId ==
                            (int)enumSurveyInfoType.RP_Observation).Select(y => y.Value).
                            DefaultIfEmpty("-").
                            FirstOrDefault() + "."));

                        parameters.Add(new ReportParameter("actionPlan",
                            x.ItemInfo.Where(y => y.ItemInfoType.ItemId ==
                            (int)enumSurveyInfoType.RP_ImprovementPlan).Select(y => y.Value).
                            DefaultIfEmpty("-").
                            FirstOrDefault() + "."));

                        parameters.Add(new ReportParameter("dateStart", Convert.ToDateTime(x.ItemInfo.Where(y => y.ItemInfoType.ItemId ==
                                    (int)enumSurveyInfoType.RP_InitDateReport).Select(y => y.Value).
                                    DefaultIfEmpty("-").
                                    FirstOrDefault()).ToString("dd/MM/yyyy")));

                        parameters.Add(new ReportParameter("dateEnd", Convert.ToDateTime(x.ItemInfo.Where(y => y.ItemInfoType.ItemId ==
                            (int)enumSurveyInfoType.RP_EndDateReport).Select(y => y.Value).
                            DefaultIfEmpty("-").
                            FirstOrDefault()).ToString("dd/MM/yyyy")));

                        parameters.Add(new ReportParameter("average",
                            x.ItemInfo.Where(y => y.ItemInfoType.ItemId ==
                            (int)enumSurveyInfoType.RP_ReportAverage).Select(y => y.Value).
                            DefaultIfEmpty("-").
                            FirstOrDefault()));

                        parameters.Add(new ReportParameter("reportDate",
                            x.ItemInfo.Where(y => y.ItemInfoType.ItemId ==
                            (int)enumSurveyInfoType.RP_ReportDate).Select(y => y.Value).
                            DefaultIfEmpty("-").
                            FirstOrDefault()));

                        parameters.Add(new ReportParameter("responsible",
                            x.ItemInfo.Where(y => y.ItemInfoType.ItemId ==
                            (int)enumSurveyInfoType.RP_ReportResponsable).Select(y => y.Value).
                            DefaultIfEmpty("-").
                            FirstOrDefault()));
                        return true;
                    });
                }
                parameters.Add(new ReportParameter("author", SessionModel.CurrentCompanyLoginUser.RelatedUser.Name.ToString() + " " + SessionModel.CurrentCompanyLoginUser.RelatedUser.LastName.ToString()));

                Tuple<byte[], string, string> report = ProveedoresOnLine.Reports.Controller.ReportModule.CP_SurveyReportDetail(
                                                    (int)enumReportType.RP_SurveyReport,
                                                    enumCategoryInfoType.PDF.ToString(),
                                                    parameters,
                                                    Models.General.InternalSettings.Instance[Models.General.Constants.MP_CP_ReportPath].Value.Trim() + "SV_Report_SurveyDetail.rdlc");
                parameters = null;
                return File(report.Item1, report.Item2, report.Item3);
            }

            #endregion report generator

            return View(oModel);
        }

        public virtual ActionResult SVSurveyDetail(string ProviderPublicId, string SurveyPublicId)
        {
            ProviderViewModel oModel = new ProviderViewModel();

            //Clean the season url saved
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            //get basic provider info
            var olstProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.MPProviderSearchById
                (SessionModel.CurrentCompany.CompanyPublicId, ProviderPublicId);

            var oProvider = olstProvider.
                Where(x => SessionModel.CurrentCompany.CompanyType.ItemId == (int)enumCompanyType.BuyerProvider ?
                            (x.RelatedCompany.CompanyPublicId == ProviderPublicId) :
                            x.RelatedCompany.CompanyPublicId == ProviderPublicId).FirstOrDefault();

            //validate provider permisions
            if (oProvider != null)
            {
                //get provider view model
                oModel.RelatedLiteProvider = new ProviderLiteViewModel(oProvider);
                oModel.ProviderMenu = GetSurveyMenu(oModel);
                //get survey info
                oModel.RelatedSurvey = new Models.Survey.SurveyViewModel
                    (ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetById(SurveyPublicId));
            }

            if (Request["DownloadReport"] == "true")
            {
                GenericReportModel SurveyReport = Report_SurveyGeneral(oModel);
                return File(SurveyReport.File, SurveyReport.MimeType, SurveyReport.FileName);
            }

            return View(oModel);
        }

        public virtual ActionResult SVSurveyEvaluatorDetail(string ProviderPublicId, string SurveyPublicId, string User)
        {
            ProviderViewModel oModel = new ProviderViewModel();
            //Clean the season url saved
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            if (Request["DownloadReport"] == "true")
            {
                return File(Convert.FromBase64String(Request["File"]), Request["MimeType"], Request["FileName"]);
            }
            else
            {
                //get basic provider info
                var olstProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.MPProviderSearchById
                    (SessionModel.CurrentCompany.CompanyPublicId, ProviderPublicId);

                var oProvider = olstProvider.
                Where(x => SessionModel.CurrentCompany.CompanyType.ItemId == (int)enumCompanyType.BuyerProvider ?
                            (x.RelatedCompany.CompanyPublicId == ProviderPublicId) :
                            x.RelatedCompany.CompanyPublicId == ProviderPublicId).FirstOrDefault();

                //validate provider permisions
                if (oProvider != null)
                {
                    //get provider view model
                    oModel.RelatedLiteProvider = new ProviderLiteViewModel(oProvider);
                    oModel.ProviderMenu = GetSurveyMenu(oModel);
                    //get survey info
                    oModel.RelatedSurvey = new Models.Survey.SurveyViewModel
                        (ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetByUser(SurveyPublicId, User));
                }
                oModel.SurveytReportModel = new GenericReportModel();
                oModel.SurveytReportModel = Report_SurveyEvaluatorDetail(oModel);

                return View(oModel);
            }
        }

        public virtual ActionResult SVSurveyReport(string ProviderPublicId)
        {
            ProviderViewModel oModel = new ProviderViewModel();

            //Clean the season url saved
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            //get basic provider info
            var olstProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.MPProviderSearchById
                (SessionModel.CurrentCompany.CompanyPublicId, ProviderPublicId);

            var oProvider = olstProvider.
                 Where(x => SessionModel.CurrentCompany.CompanyType.ItemId == (int)enumCompanyType.BuyerProvider ?
                             (x.RelatedCompany.CompanyPublicId == ProviderPublicId) :
                             x.RelatedCompany.CompanyPublicId == ProviderPublicId).FirstOrDefault();

            //validate provider permisions
            if (oProvider == null)
            {
                //return url provider not allowed
            }
            else
            {
                //get provider view model
                oModel.RelatedLiteProvider = new ProviderLiteViewModel(oProvider);

                oModel.RelatedLiteProvider.RelatedProvider.RelatedReports = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.MPReportGetBasicInfo(ProviderPublicId, (int)enumReportType.RP_SurveyReport);
                oModel.RelatedReportInfo = new List<ProviderReportsViewModel>();

                if (oModel.RelatedLiteProvider.RelatedProvider.RelatedReports != null)
                {
                    oModel.RelatedLiteProvider.RelatedProvider.RelatedReports.All(x =>
                    {
                        oModel.RelatedReportInfo.Add(new ProviderReportsViewModel(x));
                        return true;
                    });
                }

                oModel.ProviderMenu = GetSurveyMenu(oModel);
            }

            return View(oModel);
        }

        public virtual ActionResult SVSurveyProgram(string ProviderPublicId, string SurveyPublicId, string ProjectPublicId)
        {
            ProviderViewModel oModel = new ProviderViewModel();

            //Clean the season url saved
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            //get basic provider info
            var olstProvider = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.MPProviderSearchById
                (SessionModel.CurrentCompany.CompanyPublicId, ProviderPublicId);

            var oProvider = olstProvider.
                 Where(x => SessionModel.CurrentCompany.CompanyType.ItemId == (int)enumCompanyType.BuyerProvider ?
                             (x.RelatedCompany.CompanyPublicId == ProviderPublicId) :
                             x.RelatedCompany.CompanyPublicId == ProviderPublicId).FirstOrDefault();

            //validate provider permisions
            if (oProvider == null)
            {
                //return url provider not allowed
            }
            else
            {
                oModel.RelatedLiteProvider = new ProviderLiteViewModel(oProvider);

                if (!string.IsNullOrEmpty(Request["UpsertAction"]) && Request["UpsertAction"].Trim() == "true")
                {
                    SurveyModel SurveyToUpsert = GetSurveyUpsertRequest();
                    SurveyToUpsert = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyUpsert(SurveyToUpsert);

                    #region Index Survey

                    /*Get Survey Config by surveyconfigid*/
                    SurveyConfigModel oSurveyConfigModel = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyConfigGetById(SurveyToUpsert.RelatedSurveyConfig.ItemId);

                    SurveyIndexSearchModel oModelToIndex = new SurveyIndexSearchModel()
                    {
                        CompanyPublicId = SurveyToUpsert.RelatedProvider.RelatedCompany.CompanyPublicId,
                        CustomerPublicId = SessionModel.CurrentCompany.CompanyPublicId,
                        SurveyPublicId = SurveyToUpsert.SurveyPublicId,
                        SurveyStatusId = SurveyToUpsert.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).DefaultIfEmpty(string.Empty).FirstOrDefault(),
                        SurveyStatus = MarketPlace.Models.Company.CompanyUtil.GetProviderOptionName(SurveyToUpsert.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).DefaultIfEmpty(string.Empty).FirstOrDefault()),
                        SurveyTypeId = oSurveyConfigModel.ItemId.ToString(),
                        SurveyType = oSurveyConfigModel.ItemName,
                    };

                    Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                    var settings = new ConnectionSettings(node);

                    settings.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value);
                    settings.DisableDirectStreaming(true);
                    ElasticClient client = new ElasticClient(settings);

                    ICreateIndexResponse oElasticResponse = client.CreateIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_SurveyIndex].Value, c => c
                    .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                    .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                        .Tokenizer("whitespace")
                        )).TokenFilters(tf => tf
                                .EdgeNGram("customEdgeNGram", engrf => engrf
                                .MinGram(1)
                                .MaxGram(10)))).NumberOfShards(1)
                    ));

                    var Index = client.Index(oModelToIndex);

                    #endregion
                }
                if (!string.IsNullOrEmpty(SurveyPublicId))
                {
                    if (oProvider != null)
                    {
                        oModel.RelatedSurvey = new Models.Survey.SurveyViewModel(ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetById(SurveyPublicId));
                        if (oModel.RelatedSurvey != null)
                        {
                            oModel.RelatedSurvey.RelatedSurvey.ChildSurvey = new List<SurveyModel>();
                            List<string> Evaluators = oModel.RelatedSurvey.SurveyEvaluatorList.GroupBy(x => x).Select(grp => grp.First()).ToList();

                            Evaluators.All(evt =>
                            {
                                oModel.RelatedSurvey.RelatedSurvey.ChildSurvey.Add(
                                (ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetByUser(SurveyPublicId, evt)));
                                return true;
                            });
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(ProjectPublicId))
                {
                    oModel.RelatedProject = ProveedoresOnLine.ProjectModule.Controller.ProjectModule.ProjectGetById(ProjectPublicId, SessionModel.CurrentCompany.CompanyPublicId);
                }

                oModel.ProviderMenu = GetSurveyMenu(oModel);
            }

            return View(oModel);
        }

        #endregion

        #region Survey request

        private ProveedoresOnLine.SurveyModule.Models.SurveyModel GetSurveyUpsertRequest(string SurveyPublicId)
        {
            //get current survey values
            ProveedoresOnLine.SurveyModule.Models.SurveyModel oSurvey =
               ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetById(SurveyPublicId);

            ProveedoresOnLine.SurveyModule.Models.SurveyModel oSurveyToUpsert = new ProveedoresOnLine.SurveyModule.Models.SurveyModel()
            {
                SurveyPublicId = oSurvey.SurveyPublicId,
                RelatedSurveyItem = new List<ProveedoresOnLine.SurveyModule.Models.SurveyItemModel>(),
                SurveyInfo = new List<GenericItemInfoModel>(),
            };

            #region get request infos

            Request.Form.AllKeys.Where(req => req.Contains("SurveyInfo_")).All(req =>
            {
                string[] strAux = req.Split('_');

                if (strAux.Length >= 2)
                {
                    int oSurveyInfoTypeId = Convert.ToInt32(strAux[1].Replace(" ", ""));
                    int oSurveyInfoId = strAux.Length >= 3 && !string.IsNullOrEmpty(strAux[2]) ? Convert.ToInt32(strAux[2].Replace(" ", "")) : 0;

                    oSurveyToUpsert.SurveyInfo.Add(new GenericItemInfoModel()
                    {
                        ItemInfoId = oSurveyInfoId,
                        ItemInfoType = new CatalogModel()
                        {
                            ItemId = oSurveyInfoTypeId,
                        },
                        Value = Request[req].Replace(" ", ""),
                        Enable = true,
                    });
                }
                return true;
            });

            #endregion get request infos

            #region get request answers

            //loop request for update answers
            Request.Form.AllKeys.Where(req => req.Contains("SurveyItem_")).All(req =>
            {
                string[] strAux = req.Split('_');

                #region Answers

                if (strAux.Length >= 2)
                {
                    int oSurveyConfigItemId = Convert.ToInt32(strAux[1].Replace(" ", ""));

                    ProveedoresOnLine.SurveyModule.Models.SurveyItemModel oSurveyItem = oSurvey.RelatedSurveyItem.
                        Where(sit => sit.RelatedSurveyConfigItem.ItemId == oSurveyConfigItemId).
                        FirstOrDefault();

                    ProveedoresOnLine.SurveyModule.Models.SurveyItemModel oSurveyItemToUpsert = new ProveedoresOnLine.SurveyModule.Models.SurveyItemModel()
                    {
                        ItemId = oSurveyItem != null ? oSurveyItem.ItemId : 0,
                        RelatedSurveyConfigItem = new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = oSurveyConfigItemId,
                        },
                        Enable = true,
                        EvaluatorRoleId = SessionManager.SessionController.POLMarketPlace_MarketPlaceUserLogin.RelatedCompany.FirstOrDefault().RelatedUser.
                                                               Where(y => y.RelatedCompanyRole.RoleCompanyId != 0).Select(y => y.UserCompanyId).FirstOrDefault(),

                        ItemInfo = new List<GenericItemInfoModel>()
                        {
                            new GenericItemInfoModel()
                            {
                                ItemInfoId =
                                    (oSurveyItem != null &&
                                    oSurveyItem.ItemInfo != null && oSurveyItem.ItemInfo.Any(r => r.LargeValue == SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().UserCompanyId.ToString()) &&
                                    oSurveyItem.ItemInfo.Any(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.Ratting)) ?
                                    oSurveyItem.ItemInfo.
                                        Where(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.Ratting).
                                        Select(sitinf=>sitinf.ItemInfoId).
                                        DefaultIfEmpty(0).
                                        FirstOrDefault() : 0,
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)enumSurveyItemInfoType.Ratting,
                                },
                                Value = oSurvey.RelatedSurveyConfig.RelatedSurveyConfigItem.
                                    Where(scit=> scit.ItemType.ItemId == (int)enumSurveyConfigItemType.Answer &&
                                                 scit.ItemId.ToString() == Request[req].Replace(" ","")).
                                    Select(scit=>scit.ItemInfo.
                                                    Where(scitinf=>scitinf.ItemInfoType.ItemId == (int)enumSurveyConfigItemInfoType.Weight).
                                                    Select(scitinf=>Convert.ToInt32( scitinf.Value)).
                                                    DefaultIfEmpty(0).
                                                    FirstOrDefault()).
                                    DefaultIfEmpty(0).
                                    FirstOrDefault().ToString(),
                                Enable = true,
                            },
                            new GenericItemInfoModel()
                            {
                                ItemInfoId =
                                    (oSurveyItem != null &&
                                    oSurveyItem.ItemInfo != null &&
                                     oSurveyItem.ItemInfo != null && oSurveyItem.ItemInfo.Any(r => r.LargeValue == SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().UserCompanyId.ToString()) &&
                                    oSurveyItem.ItemInfo.Any(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.Answer)) ?
                                    oSurveyItem.ItemInfo.
                                        Where(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.Answer).
                                        Select(sitinf=>sitinf.ItemInfoId).
                                        DefaultIfEmpty(0).
                                        FirstOrDefault() : 0,
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)enumSurveyItemInfoType.Answer,
                                },
                                Value = Request[req],
                                Enable = true,
                            },
                            new GenericItemInfoModel()
                            {
                                ItemInfoId =
                                    (oSurveyItem != null &&
                                    oSurveyItem.ItemInfo != null &&
                                     oSurveyItem.ItemInfo != null && oSurveyItem.ItemInfo.Any(r => r.LargeValue == SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().UserCompanyId.ToString()) &&
                                    oSurveyItem.ItemInfo.Any(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.DescriptionText)) ?
                                    oSurveyItem.ItemInfo.
                                        Where(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.DescriptionText).
                                        Select(sitinf=>sitinf.ItemInfoId).
                                        DefaultIfEmpty(0).
                                        FirstOrDefault() : 0,
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)enumSurveyItemInfoType.DescriptionText,
                                },
                                Value = Request["SurveyItemText_" + oSurveyConfigItemId],
                                Enable = true,
                            },
                             new GenericItemInfoModel()
                            {
                                ItemInfoId =
                                    (oSurveyItem != null &&
                                    oSurveyItem.ItemInfo != null &&
                                     oSurveyItem.ItemInfo != null && oSurveyItem.ItemInfo.Any(r => r.LargeValue == SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().UserCompanyId.ToString()) &&
                                    oSurveyItem.ItemInfo.Any(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.AreaDescription)) ?
                                    oSurveyItem.ItemInfo.
                                        Where(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.AreaDescription).
                                        Select(sitinf=>sitinf.ItemInfoId).
                                        DefaultIfEmpty(0).
                                        FirstOrDefault() : 0,
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)enumSurveyItemInfoType.AreaDescription,
                                },
                                Value = Request[req],
                                Enable = true,
                            },
                             new GenericItemInfoModel()
                            {
                                ItemInfoId =
                                    (oSurveyItem != null &&
                                    oSurveyItem.ItemInfo != null &&
                                     oSurveyItem.ItemInfo != null && oSurveyItem.ItemInfo.Any(r => r.LargeValue == SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().UserCompanyId.ToString()) &&
                                    oSurveyItem.ItemInfo.Any(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.EvaluatorRol)) ?
                                    oSurveyItem.ItemInfo.
                                        Where(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.EvaluatorRol).
                                        Select(sitinf=>sitinf.ItemInfoId).
                                        DefaultIfEmpty(0).
                                        FirstOrDefault() : 0,
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)enumSurveyItemInfoType.EvaluatorRol,
                                },
                                Value = SessionManager.SessionController.POLMarketPlace_MarketPlaceUserLogin.RelatedCompany.FirstOrDefault().RelatedUser.
                                                Where(y => y.RelatedCompanyRole.RoleCompanyId != 0).Select(y => y.RelatedCompanyRole.RoleCompanyId.ToString()).FirstOrDefault(),
                                Enable = true,
                            },
                            new GenericItemInfoModel()
                            {
                                ItemInfoId =
                                    (oSurveyItem != null &&
                                     oSurveyItem.ItemInfo != null && oSurveyItem.ItemInfo.Any(r => r.LargeValue == SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().UserCompanyId.ToString()) &&
                                    oSurveyItem.ItemInfo != null &&
                                    oSurveyItem.ItemInfo.Any(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.EvaluatorName)) ?
                                    oSurveyItem.ItemInfo.
                                        Where(sitinf=>sitinf.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.EvaluatorName).
                                        Select(sitinf=>sitinf.ItemInfoId).
                                        DefaultIfEmpty(0).
                                        FirstOrDefault() : 0,
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)enumSurveyItemInfoType.EvaluatorName,
                                },
                                Value = SessionManager.SessionController.POLMarketPlace_MarketPlaceUserLogin.RelatedCompany.FirstOrDefault().RelatedUser.
                                             Where(y => y.User != null).Select(y => y.User).FirstOrDefault(),
                                Enable = true,
                            },
                        },
                    };
                    //add survey item to survey to upsert
                    oSurveyToUpsert.RelatedSurveyItem.Add(oSurveyItemToUpsert);
                }

                #endregion Answers

                return true;
            });

            #endregion get request answers

            return oSurveyToUpsert;
        }

        private ProveedoresOnLine.SurveyModule.Models.SurveyModel GetSurveyFinalizeRequest(string SurveyPublicId)
        {
            //get current survey values
            ProveedoresOnLine.SurveyModule.Models.SurveyModel oSurvey =
               ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetById(SurveyPublicId);

            ProveedoresOnLine.SurveyModule.Models.SurveyModel oSurveyToUpsert = new ProveedoresOnLine.SurveyModule.Models.SurveyModel()
            {
                SurveyPublicId = oSurvey.SurveyPublicId,
                RelatedSurveyItem = new List<ProveedoresOnLine.SurveyModule.Models.SurveyItemModel>(),
                SurveyInfo = new List<GenericItemInfoModel>(),
            };

            #region get request infos

            Request.Form.AllKeys.Where(req => req.Contains("SurveyInfo_")).All(req =>
            {
                string[] strAux = req.Split('_');

                if (strAux.Length >= 2)
                {
                    int oSurveyInfoTypeId = Convert.ToInt32(strAux[1].Replace(" ", ""));
                    int oSurveyInfoId = strAux.Length >= 3 && !string.IsNullOrEmpty(strAux[2]) ? Convert.ToInt32(strAux[2].Replace(" ", "")) : 0;

                    if (oSurveyInfoTypeId == (int)enumSurveyInfoType.Status)
                    {
                        oSurveyToUpsert.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = oSurveyInfoId,
                            ItemInfoType = new CatalogModel()
                            {
                                ItemId = oSurveyInfoTypeId,
                            },
                            Value = ((int)enumSurveyStatus.Close).ToString(),
                            Enable = true,
                        });
                    }
                }
                return true;
            });

            #endregion get request infos

            return oSurveyToUpsert;
        }

        #endregion Survey request

        #region Private functions

        private SurveyModel GetSurveyUpsertRequest()
        {
            List<Tuple<string, int, int, int>> EvaluatorsRoleObj = new List<Tuple<string, int, int, int>>();
            List<string> EvaluatorsEmail = new List<string>();

            #region Parent Survey

            //Armar el Survey Model Del Papá
            SurveyModel oReturn = new SurveyModel()
            {
                ChildSurvey = new List<SurveyModel>(),
                SurveyPublicId = System.Web.HttpContext.Current.Request["SurveyPublicId"],
                RelatedProvider = new ProviderModel()
                {
                    RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                    {
                        CompanyPublicId = System.Web.HttpContext.Current.Request["ProviderPublicId"],
                    }
                },
                RelatedSurveyConfig = new ProveedoresOnLine.SurveyModule.Models.SurveyConfigModel()
                {
                    ItemId = Convert.ToInt32(System.Web.HttpContext.Current.Request["SurveyConfigId"].Trim()),
                },
                Enable = true,
                User = SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().User, //Responsable
                SurveyInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>()
            };

            System.Web.HttpContext.Current.Request.Form.AllKeys.Where(x => x.Contains("SurveyInfo_")).All(req =>
            {
                string[] strSplit = req.Split('_');

                //Set Parent Survey Info
                if (strSplit.Length >= 3)
                {
                    oReturn.SurveyInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = !string.IsNullOrEmpty(strSplit[2]) ? Convert.ToInt32(strSplit[2].Trim()) : 0,
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = Convert.ToInt32(strSplit[1].Trim())
                        },
                        Value = System.Web.HttpContext.Current.Request[req],
                        Enable = true,
                    });

                    //Get Evaluator Rol info
                    if (Convert.ToInt32(strSplit[1].Trim()) == (int)enumSurveyInfoType.Evaluator)
                        EvaluatorsRoleObj.Add(new Tuple<string, int, int, int>(System.Web.HttpContext.Current.Request[req],
                                                                                Convert.ToInt32(strSplit[4].Trim()),
                                                                                Convert.ToInt32(strSplit[2].Trim()),
                                                                                Convert.ToInt32(strSplit[5].Trim())));
                }
                return true;
            });

            #endregion Parent Survey

            if (EvaluatorsRoleObj != null && EvaluatorsRoleObj.Count > 0)
            {
                EvaluatorsEmail = new List<string>();
                EvaluatorsEmail = EvaluatorsRoleObj.GroupBy(x => x.Item1).Select(grp => grp.First().Item1).ToList();

                #region Child Survey

                //Set survey by evaluators
                EvaluatorsEmail.All(x =>
                {
                    oReturn.ChildSurvey.Add(new SurveyModel()
                    {
                        SurveyPublicId = System.Web.HttpContext.Current.Request["SurveyPublicId"],
                        RelatedProvider = new ProviderModel()
                        {
                            RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                            {
                                CompanyPublicId = System.Web.HttpContext.Current.Request["ProviderPublicId"],
                            }
                        },
                        RelatedSurveyConfig = new ProveedoresOnLine.SurveyModule.Models.SurveyConfigModel()
                        {
                            ItemId = Convert.ToInt32(System.Web.HttpContext.Current.Request["SurveyConfigId"].Trim()),
                        },
                        Enable = true,
                        User = x,//Evaluator,
                        SurveyInfo = new List<GenericItemInfoModel>()
                    });
                    return true;
                });

                //Set SurveyChild Info
                oReturn.ChildSurvey.All(it =>
                {
                    List<Tuple<int, int, int>> AreaIdList = new List<Tuple<int, int, int>>();
                    AreaIdList.AddRange(EvaluatorsRoleObj.Where(y => y.Item1 == it.User).Select(y => new Tuple<int, int, int>(y.Item2, y.Item3, y.Item4)).ToList());
                    AreaIdList = AreaIdList.GroupBy(x => x.Item1).Select(grp => grp.First()).ToList();
                    if (AreaIdList != null)
                    {
                        AreaIdList.All(a =>
                        {
                            it.SurveyInfo.Add(new GenericItemInfoModel()
                            {
                                ItemInfoId = a.Item2 != null ? a.Item2 : 0,
                                ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                {
                                    ItemId = (int)enumSurveyInfoType.CurrentArea
                                },
                                Value = a.Item1.ToString() + "_" + a.Item3.ToString(),
                                Enable = true,
                            });
                            return true;
                        });

                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.Project
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Project).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });

                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.StartDate
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.StartDate).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });

                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.EndDate
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.EndDate).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });

                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.IssueDate
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });
                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.Reminder
                            },
                            Value = "false",
                            Enable = true,
                        });
                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.Contract
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Contract).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });
                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.Status
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });
                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.Comments
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });
                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.Responsible
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });
                        it.SurveyInfo.Add(new GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumSurveyInfoType.ExpirationDate
                            },
                            Value = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(x => x.Value).FirstOrDefault(),
                            Enable = true,
                        });
                        List<GenericItemInfoModel> oEvaluators = new List<GenericItemInfoModel>();
                        oEvaluators = oReturn.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Evaluator).Select(x => x).ToList();
                        oEvaluators = oEvaluators.GroupBy(x => x.Value).Select(grp => grp.First()).ToList();

                        //EvaluatorsRoleObj.GroupBy(x => x.Item1).Select(grp => grp.First().Item1).ToList();
                        if (oEvaluators.Count > 0)
                        {
                            oEvaluators.All(x =>
                            {
                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.Evaluator
                                    },
                                    Value = x.Value,
                                    Enable = true,
                                });
                                return true;
                            });
                        }
                    }
                    return true;
                });

                #endregion Child Survey
            }
            return oReturn;
        }

        private List<SurveyModel> GetSurveySearchUpsertRequest()
        {
            List<Tuple<string, int, int, int>> EvaluatorsRoleObj = new List<Tuple<string, int, int, int>>();
            List<string> EvaluatorsEmail = new List<string>();
            string[] oProviderList = Request["ProviderList"].Split(',');

            List<SurveyModel> oReturn = new List<SurveyModel>();

            if (oProviderList != null &&
                oProviderList.Count() > 0)
            {
                oProviderList.All(prv =>
                {
                    #region Parent Survey

                    //Survey parent model
                    SurveyModel oSurveyModel = new SurveyModel()
                    {
                        ChildSurvey = new List<SurveyModel>(),
                        SurveyPublicId = System.Web.HttpContext.Current.Request["SurveyPublicId"],
                        RelatedProvider = new ProviderModel()
                        {
                            RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                            {
                                CompanyPublicId = prv,
                            }
                        },
                        RelatedSurveyConfig = new ProveedoresOnLine.SurveyModule.Models.SurveyConfigModel()
                        {
                            ItemId = Convert.ToInt32(System.Web.HttpContext.Current.Request["SurveyConfigId"].Trim()),
                        },
                        Enable = true,
                        User = SessionModel.CurrentCompany.RelatedUser.FirstOrDefault().User, //Responsable
                        SurveyInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>()
                    };

                    System.Web.HttpContext.Current.Request.Form.AllKeys.Where(x => x.Contains("SurveyInfo_")).All(req =>
                    {
                        string[] strSplit = req.Split('_');

                        //Set Parent Survey Info
                        if (strSplit.Length >= 3)
                        {
                            oSurveyModel.SurveyInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                            {
                                ItemInfoId = !string.IsNullOrEmpty(strSplit[2]) ? Convert.ToInt32(strSplit[2].Trim()) : 0,
                                ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                {
                                    ItemId = Convert.ToInt32(strSplit[1].Trim())
                                },
                                Value = System.Web.HttpContext.Current.Request[req],
                                Enable = true,
                            });

                            //Get Evaluator Rol info
                            if (Convert.ToInt32(strSplit[1].Trim()) == (int)enumSurveyInfoType.Evaluator)
                                EvaluatorsRoleObj.Add(new Tuple<string, int, int, int>(System.Web.HttpContext.Current.Request[req],
                                                                                        Convert.ToInt32(strSplit[4].Trim()),
                                                                                        Convert.ToInt32(strSplit[2].Trim()),
                                                                                        Convert.ToInt32(strSplit[5].Trim())));
                        }
                        return true;
                    });

                    #endregion Parent Survey

                    if (EvaluatorsRoleObj != null && EvaluatorsRoleObj.Count > 0)
                    {
                        EvaluatorsEmail = new List<string>();
                        EvaluatorsEmail = EvaluatorsRoleObj.GroupBy(x => x.Item1).Select(grp => grp.First().Item1).ToList();

                        #region Child Survey

                        //Set survey by evaluators
                        EvaluatorsEmail.All(x =>
                        {
                            oSurveyModel.ChildSurvey.Add(new SurveyModel()
                            {
                                SurveyPublicId = System.Web.HttpContext.Current.Request["SurveyPublicId"],
                                RelatedProvider = new ProviderModel()
                                {
                                    RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel()
                                    {
                                        CompanyPublicId = prv,
                                    }
                                },
                                RelatedSurveyConfig = new ProveedoresOnLine.SurveyModule.Models.SurveyConfigModel()
                                {
                                    ItemId = Convert.ToInt32(System.Web.HttpContext.Current.Request["SurveyConfigId"].Trim()),
                                },
                                Enable = true,
                                User = x,//Evaluator,
                                SurveyInfo = new List<GenericItemInfoModel>()
                            });
                            return true;
                        });

                        //Set SurveyChild Info
                        oSurveyModel.ChildSurvey.All(it =>
                        {
                            List<Tuple<int, int, int>> AreaIdList = new List<Tuple<int, int, int>>();
                            AreaIdList.AddRange(EvaluatorsRoleObj.Where(y => y.Item1 == it.User).Select(y => new Tuple<int, int, int>(y.Item2, y.Item3, y.Item4)).ToList());
                            AreaIdList = AreaIdList.GroupBy(x => x.Item1).Select(grp => grp.First()).ToList();
                            if (AreaIdList != null)
                            {
                                AreaIdList.All(a =>
                                {
                                    it.SurveyInfo.Add(new GenericItemInfoModel()
                                    {
                                        ItemInfoId = a.Item2 != null ? a.Item2 : 0,
                                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                        {
                                            ItemId = (int)enumSurveyInfoType.CurrentArea
                                        },
                                        Value = a.Item1.ToString() + "_" + a.Item3.ToString(),
                                        Enable = true,
                                    });
                                    return true;
                                });

                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.Project
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Project).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });

                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.StartDate
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.StartDate).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });

                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.EndDate
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.EndDate).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });

                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.IssueDate
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });
                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.Reminder
                                    },
                                    Value = "false",
                                    Enable = true,
                                });
                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.Contract
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Contract).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });
                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.Status
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });
                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.Comments
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });
                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.Responsible
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });
                                it.SurveyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)enumSurveyInfoType.ExpirationDate
                                    },
                                    Value = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(x => x.Value).FirstOrDefault(),
                                    Enable = true,
                                });
                                List<GenericItemInfoModel> oEvaluators = new List<GenericItemInfoModel>();
                                oEvaluators = oSurveyModel.SurveyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumSurveyInfoType.Evaluator).Select(x => x).ToList();
                                oEvaluators = oEvaluators.GroupBy(x => x.Value).Select(grp => grp.First()).ToList();

                                //EvaluatorsRoleObj.GroupBy(x => x.Item1).Select(grp => grp.First().Item1).ToList();
                                if (oEvaluators.Count > 0)
                                {
                                    oEvaluators.All(x =>
                                    {
                                        it.SurveyInfo.Add(new GenericItemInfoModel()
                                        {
                                            ItemInfoId = 0,
                                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                            {
                                                ItemId = (int)enumSurveyInfoType.Evaluator
                                            },
                                            Value = x.Value,
                                            Enable = true,
                                        });
                                        return true;
                                    });
                                }
                            }
                            return true;
                        });

                        #endregion Child Survey
                    }

                    oReturn.Add(oSurveyModel);

                    return true;
                });
            }

            return oReturn;
        }

        private GenericItemModel GetSurveyReportFilterRequest()
        {
            GenericItemModel oReturn = new GenericItemModel()
            {
                ItemId = 0,
                ItemName = "SurveyFilterReport",
                ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                {
                    ItemId = (int)enumReportType.RP_SurveyReport,
                },
                ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),

                Enable = true,
            };

            System.Web.HttpContext.Current.Request.Form.AllKeys.Where(x => x.Contains("SurveyInfo_")).All(req =>
            {
                string[] strSplit = req.Split('_');
                if (strSplit.Length > 0)
                {
                    oReturn.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                    {
                        ItemInfoId = 0,
                        ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                        {
                            ItemId = Convert.ToInt32(strSplit[1].Trim())
                        },
                        Value = System.Web.HttpContext.Current.Request[req],
                        Enable = true,
                    });
                }
                return true;
            });

            return oReturn;
        }

        private GenericReportModel Report_SurveyGeneral(ProviderViewModel oModel)
        {
            List<ReportParameter> parameters = new List<ReportParameter>();
            GenericReportModel oReporModel = new GenericReportModel();

            //CustomerInfo
            parameters.Add(new ReportParameter("CustomerName", SessionModel.CurrentCompany.CompanyName));
            parameters.Add(new ReportParameter("CustomerIdentification", SessionModel.CurrentCompany.IdentificationNumber));
            parameters.Add(new ReportParameter("CustomerIdentificationType", SessionModel.CurrentCompany.IdentificationType.ItemName));
            parameters.Add(new ReportParameter("CustomerImage", SessionModel.CurrentCompany_CompanyLogo));
            //ProviderInfo
            parameters.Add(new ReportParameter("ProviderName", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.CompanyName));
            parameters.Add(new ReportParameter("ProviderIdentificationType", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.IdentificationType.ItemName));
            parameters.Add(new ReportParameter("ProviderIdentificationNumber", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.IdentificationNumber));

            //SurveyInfo
            parameters.Add(new ReportParameter("SurveyConfigName", oModel.RelatedSurvey.SurveyConfigName));
            parameters.Add(new ReportParameter("SurveyRating", oModel.RelatedSurvey.SurveyRating.ToString()));
            parameters.Add(new ReportParameter("SurveyStatusName", oModel.RelatedSurvey.SurveyStatusName));
            parameters.Add(new ReportParameter("SurveyIssueDate", oModel.RelatedSurvey.SurveyIssueDate));
            parameters.Add(new ReportParameter("SurveyLastModify", oModel.RelatedSurvey.SurveyLastModify));
            parameters.Add(new ReportParameter("SurveyResponsible", oModel.RelatedSurvey.SurveyResponsible));
            parameters.Add(new ReportParameter("SurveyAverage", oModel.RelatedSurvey.Average.ToString()));

            // DataSet Evaluators table
            DataTable data = new DataTable();
            data.Columns.Add("SurveyEvaluatorDetail");
            data.Columns.Add("SurveyStatusNameDetail");
            data.Columns.Add("SurveyRatingDetail");
            data.Columns.Add("SurveyProgressDetail");

            List<Models.Survey.SurveyViewModel> EvaluatorDetailList = new List<Models.Survey.SurveyViewModel>();

            foreach (var Evaluator in oModel.RelatedSurvey.SurveyEvaluatorList.Distinct())
            {
                Models.Survey.SurveyViewModel SurveyEvaluatorDetail = new Models.Survey.SurveyViewModel
                        (ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetByUser(oModel.RelatedSurvey.SurveyPublicId, Evaluator));

                EvaluatorDetailList.Add(SurveyEvaluatorDetail);

                DataRow row;
                row = data.NewRow();
                row["SurveyEvaluatorDetail"] = SurveyEvaluatorDetail.SurveyEvaluator;
                row["SurveyStatusNameDetail"] = SurveyEvaluatorDetail.SurveyStatusName;
                row["SurveyRatingDetail"] = SurveyEvaluatorDetail.SurveyRating;
                row["SurveyProgressDetail"] = SurveyEvaluatorDetail.SurveyProgress.ToString() + "%";
                data.Rows.Add(row);
            }

            // DataSet Area's Table
            DataTable data2 = new DataTable();
            data2.Columns.Add("SurveyAreaName");
            data2.Columns.Add("SurveyAreaRating");
            data2.Columns.Add("SurveyAreaWeight");

            foreach (var EvaluationArea in
                        oModel.RelatedSurvey.GetSurveyConfigItem(MarketPlace.Models.General.enumSurveyConfigItemType.EvaluationArea, null))
            {
                int RatingforArea = 0;

                foreach (Models.Survey.SurveyViewModel SurveyDetailInfo in EvaluatorDetailList)
                {
                    var EvaluationAreaInf = SurveyDetailInfo.GetSurveyItem(EvaluationArea.SurveyConfigItemId);

                    if (EvaluationAreaInf != null)
                    {
                        RatingforArea = RatingforArea + (int)EvaluationAreaInf.Ratting;
                    }
                }

                DataRow row;
                row = data2.NewRow();
                row["SurveyAreaName"] = EvaluationArea.Name;
                row["SurveyAreaRating"] = RatingforArea;
                row["SurveyAreaWeight"] = EvaluationArea.Weight.ToString() + "%";
                data2.Rows.Add(row);
            }

            Tuple<byte[], string, string> SurveyGeneralReport = ProveedoresOnLine.Reports.Controller.ReportModule.SV_GeneralReport(
                                                            data,
                                                            data2,
                                                            parameters,
                                                            enumCategoryInfoType.PDF.ToString(),
                                                            Models.General.InternalSettings.Instance[Models.General.Constants.MP_CP_ReportPath].Value.Trim());

            oReporModel.File = SurveyGeneralReport.Item1;
            oReporModel.MimeType = SurveyGeneralReport.Item2;
            oReporModel.FileName = SurveyGeneralReport.Item3;
            return oReporModel;
        }

        private GenericReportModel Report_SurveyEvaluatorDetail(ProviderViewModel oModel)
        {
            List<ReportParameter> parameters = new List<ReportParameter>();
            GenericReportModel oReporModel = new GenericReportModel();

            //CustomerInfo
            parameters.Add(new ReportParameter("CustomerName", SessionModel.CurrentCompany.CompanyName));
            parameters.Add(new ReportParameter("CustomerIdentification", SessionModel.CurrentCompany.IdentificationNumber));
            parameters.Add(new ReportParameter("CustomerIdentificationType", SessionModel.CurrentCompany.IdentificationType.ItemName));
            parameters.Add(new ReportParameter("CustomerImage", SessionModel.CurrentCompany_CompanyLogo));
            //ProviderInfo
            parameters.Add(new ReportParameter("ProviderName", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.CompanyName));
            parameters.Add(new ReportParameter("ProviderIdentificationType", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.IdentificationType.ItemName));
            parameters.Add(new ReportParameter("ProviderIdentificationNumber", oModel.RelatedLiteProvider.RelatedProvider.RelatedCompany.IdentificationNumber));

            //SurveyInfo
            parameters.Add(new ReportParameter("SurveyConfigName", oModel.RelatedSurvey.SurveyConfigName));
            parameters.Add(new ReportParameter("SurveyRating", oModel.RelatedSurvey.SurveyRating.ToString()));
            parameters.Add(new ReportParameter("SurveyStatusName", oModel.RelatedSurvey.SurveyStatusName));
            parameters.Add(new ReportParameter("SurveyIssueDate", oModel.RelatedSurvey.SurveyIssueDate));
            parameters.Add(new ReportParameter("SurveyEvaluator", oModel.RelatedSurvey.SurveyEvaluator));
            parameters.Add(new ReportParameter("SurveyLastModify", oModel.RelatedSurvey.SurveyLastModify));
            parameters.Add(new ReportParameter("SurveyResponsible", oModel.RelatedSurvey.SurveyResponsible));
            parameters.Add(new ReportParameter("SurveyAverage", oModel.RelatedSurvey.Average.ToString()));

            if (oModel.RelatedSurvey.SurveyRelatedProject == null)
            {
                parameters.Add(new ReportParameter("SurveyRelatedProject", "NA"));
            }
            else
            {
                parameters.Add(new ReportParameter("SurveyRelatedProject", oModel.RelatedSurvey.SurveyRelatedProject));
            }

            DataTable data = new DataTable();
            data.Columns.Add("Area");
            data.Columns.Add("Question");
            data.Columns.Add("Answer");
            data.Columns.Add("QuestionRating");
            data.Columns.Add("QuestionWeight");
            data.Columns.Add("QuestionDescription");

            DataRow row;
            foreach (var EvaluationArea in
                        oModel.RelatedSurvey.GetSurveyConfigItem(MarketPlace.Models.General.enumSurveyConfigItemType.EvaluationArea, null))
            {
                var lstQuestion = oModel.RelatedSurvey.GetSurveyConfigItem
                    (MarketPlace.Models.General.enumSurveyConfigItemType.Question, EvaluationArea.SurveyConfigItemId);

                foreach (var Question in lstQuestion)
                {
                    if (Question.QuestionType != "118002")
                    {
                        row = data.NewRow();
                        row["Area"] = EvaluationArea.Name;
                        row["Question"] = Question.Order + " " + Question.Name;

                        var QuestionInfo = oModel.RelatedSurvey.GetSurveyItem(Question.SurveyConfigItemId);
                        var lstAnswer = oModel.RelatedSurvey.GetSurveyConfigItem
                            (MarketPlace.Models.General.enumSurveyConfigItemType.Answer, Question.SurveyConfigItemId);

                        foreach (var Answer in lstAnswer)
                        {
                            if (QuestionInfo != null && QuestionInfo.Answer == Answer.SurveyConfigItemId)
                            {
                                row["Answer"] = Answer.Name;
                            }
                        }

                        if (string.IsNullOrEmpty(row["Answer"].ToString()))
                        {
                            row["Answer"] = "Sin Responder";
                            row["QuestionRating"] = "NA";
                        }
                        else
                        {
                            row["QuestionRating"] = QuestionInfo.Ratting;
                        }

                        row["QuestionWeight"] = Question.Weight;

                        if (QuestionInfo != null && QuestionInfo.DescriptionText != null)
                        {
                            row["QuestionDescription"] = QuestionInfo.DescriptionText;
                        }
                        else
                        {
                            row["QuestionDescription"] = "";
                        }
                        data.Rows.Add(row);
                    }
                }
            }

            Tuple<byte[], string, string> EvaluatorDetailReport = ProveedoresOnLine.Reports.Controller.ReportModule.SV_EvaluatorDetailReport(
                                                                enumCategoryInfoType.PDF.ToString(),
                                                                data,
                                                                parameters,
                                                                Models.General.InternalSettings.Instance[Models.General.Constants.MP_CP_ReportPath].Value.Trim() + "SV_Report_EvaluatorDetail.rdlc");

            oReporModel.File = EvaluatorDetailReport.Item1;
            oReporModel.MimeType = EvaluatorDetailReport.Item2;
            oReporModel.FileName = EvaluatorDetailReport.Item3;

            return oReporModel;
        }

        #endregion

        #region Menu

        private GenericMenu GetSurveyAction(ProviderViewModel vProviderInfo)
        {
            GenericMenu oReturn = new GenericMenu()
            {
                Name = "Guardar",
                Url = Url.RouteUrl(MarketPlace.Models.General.Constants.C_Routes_Default,
                    new
                    {
                        controller = MVC.Survey.Name,
                        action = MVC.Survey.ActionNames.SurveyUpsert,
                        SurveyPublicId = vProviderInfo.RelatedSurvey.SurveyPublicId,
                        StepId = vProviderInfo.RelatedSurvey.CurrentStepId.ToString(),
                    }),
            };

            //eval if has steps
            if (vProviderInfo.RelatedSurvey.SurveyConfigStepEnable)
            {
                Tuple<int?, int?> oStepInfo = vProviderInfo.RelatedSurvey.GetSurveyConfigSteps();

                if (oStepInfo.Item1 != null)
                {
                    oReturn.LastMenu = new GenericMenu()
                    {
                        Name = "Anterior",
                        Url = Url.RouteUrl(MarketPlace.Models.General.Constants.C_Routes_Default,
                            new
                            {
                                controller = MVC.Survey.Name,
                                action = MVC.Survey.ActionNames.SurveyUpsert,
                                SurveyPublicId = vProviderInfo.RelatedSurvey.SurveyPublicId,
                                StepId = oStepInfo.Item1.Value.ToString()
                            }),
                    };
                }
                if (oStepInfo.Item2 != null)
                {
                    oReturn.NextMenu = new GenericMenu()
                    {
                        Name = "Siguiente",
                        Url = Url.RouteUrl(MarketPlace.Models.General.Constants.C_Routes_Default,
                            new
                            {
                                controller = MVC.Survey.Name,
                                action = MVC.Survey.ActionNames.SurveyUpsert,
                                SurveyPublicId = vProviderInfo.RelatedSurvey.SurveyPublicId,
                                StepId = oStepInfo.Item2.Value.ToString()
                            }),
                    };
                }
            }

            return oReturn;
        }

        private List<GenericMenu> GetSurveyMenu(ProviderViewModel vProviderInfo)
        {
            List<GenericMenu> oReturn = new List<GenericMenu>();

            if (vProviderInfo.RelatedLiteProvider != null)
            {
                string oCurrentController = CurrentControllerName;
                string oCurrentAction = CurrentActionName;

                List<int> oCurrentSurveySubMenu = SessionModel.CurrentSurveyOption();

                GenericMenu oMenuAux;

                #region Survey Info

                if (oCurrentSurveySubMenu != null && oCurrentSurveySubMenu.Count > 0)
                {
                    //header
                    oMenuAux = new GenericMenu()
                    {
                        Name = "Evaluación de Desempeño",
                        Position = 6,
                        ChildMenu = new List<GenericMenu>(),
                    };

                    if (oCurrentSurveySubMenu.Any(y => y == (int)enumProviderSubMenu.SurveyList))
                    {
                        //survey list
                        oMenuAux.ChildMenu.Add(new GenericMenu()
                        {
                            Name = "Lista de Evaluaciones",
                            Url = Url.RouteUrl
                                    (Models.General.Constants.C_Routes_Default,
                                    new
                                    {
                                        controller = MVC.Survey.Name,
                                        action = MVC.Survey.ActionNames.SVSurveySearch,
                                        ProviderPublicId = vProviderInfo.RelatedLiteProvider.RelatedProvider.RelatedCompany.CompanyPublicId
                                    }),
                            Position = 0,
                            IsSelected =
                                ((oCurrentAction == MVC.Survey.ActionNames.SVSurveySearch ||
                                oCurrentAction == MVC.Survey.ActionNames.SVSurveyDetail) &&
                                oCurrentController == MVC.Survey.Name),
                        });
                    }

                    if (oCurrentSurveySubMenu.Any(y => y == (int)enumProviderSubMenu.SurveyReports))
                    {
                        //survey reports
                        oMenuAux.ChildMenu.Add(new GenericMenu()
                        {
                            Name = "Reportes de Evaluaciones",
                            Url = Url.RouteUrl
                                    (Models.General.Constants.C_Routes_Default,
                                    new
                                    {
                                        controller = MVC.Survey.Name,
                                        action = MVC.Survey.ActionNames.SVSurveyReport,
                                        ProviderPublicId = vProviderInfo.RelatedLiteProvider.RelatedProvider.RelatedCompany.CompanyPublicId
                                    }),
                            Position = 1,
                            IsSelected =
                                    (oCurrentAction == MVC.Survey.ActionNames.SVSurveyReport &&
                                oCurrentController == MVC.Survey.Name),
                        });
                    }

                    if (oCurrentSurveySubMenu.Any(y => y == (int)enumProviderSubMenu.SurveyProgram))
                    {
                        //survey program
                        oMenuAux.ChildMenu.Add(new GenericMenu()
                        {
                            Name = "Programar Evaluación",
                            Url = Url.RouteUrl
                                    (Models.General.Constants.C_Routes_Default,
                                    new
                                    {
                                        controller = MVC.Survey.Name,
                                        action = MVC.Survey.ActionNames.SVSurveyProgram,
                                        ProviderPublicId = vProviderInfo.RelatedLiteProvider.RelatedProvider.RelatedCompany.CompanyPublicId
                                    }),
                            Position = 2,
                            IsSelected =
                                    (oCurrentAction == MVC.Survey.ActionNames.SVSurveyProgram &&
                                oCurrentController == MVC.Survey.Name),
                        });
                    }

                    //get is selected menu
                    oMenuAux.IsSelected = oMenuAux.ChildMenu.Any(x => x.IsSelected);

                    //add menu
                    oReturn.Add(oMenuAux);
                }

                #endregion
            }
            return oReturn;
        }

        #endregion Menu
    }
}