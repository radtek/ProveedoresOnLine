﻿using MarketPlace.Models.Company;
using MarketPlace.Models.Compare;
using MarketPlace.Models.General;
using MarketPlace.Models.Provider;
using MarketPlace.Models.ThirdKnowledge;
using Microsoft.Reporting.WebForms;
using Nest;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarketPlace.Web.Controllers
{
    public partial class ThirdKnowledgeController : BaseController
    {
        public virtual ActionResult Index()
        {
            //Clean the season url saved
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;


            return View();
        }

        public virtual ActionResult TKSingleSearch(string Name, string IdentificationNumber, string ThirdKnowledgeIdType)
        {
            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThirdKnowledge = new ThirdKnowledgeViewModel();
            List<PlanModel> oCurrentPeriodList = new List<PlanModel>();

            try
            {
                oModel.ProviderMenu = GetThirdKnowledgeControllerMenu();

                //Clean the season url saved
                if (SessionModel.CurrentURL != null)
                    SessionModel.CurrentURL = null;

                //Get The Active Plan By Customer 
                oCurrentPeriodList = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetCurrenPeriod(SessionModel.CurrentCompany.CompanyPublicId, true);

                if (oCurrentPeriodList != null && oCurrentPeriodList.Count > 0)
                {
                    oModel.RelatedThirdKnowledge.HasPlan = true;
                    if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(IdentificationNumber))
                    {
                        oModel.RelatedThirdKnowledge.SearchNameParam = Name;
                        oModel.RelatedThirdKnowledge.SearchIdNumberParam = IdentificationNumber;
                        oModel.RelatedThirdKnowledge.IdType = ThirdKnowledgeIdType;
                        oModel.RelatedThirdKnowledge.ReSearch = true;
                    }
                    else
                        oModel.RelatedThirdKnowledge.ReSearch = false;

                    //Get The Most Recently Period When Plan is More Than One
                    oModel.RelatedThirdKnowledge.CurrentPlanModel = oCurrentPeriodList.OrderByDescending(x => x.CreateDate).First();
                }
                else
                {
                    oModel.RelatedThirdKnowledge.HasPlan = false;
                }

                //Get Provider Options
                oModel.ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions();

                return View(oModel);
            }
            catch (Exception ex)
            {

                throw ex.InnerException;
            }
        }

        public virtual ActionResult TKMasiveSearch()
        {
            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThirdKnowledge = new ThirdKnowledgeViewModel();
            List<PlanModel> oCurrentPeriodList = new List<PlanModel>();

            try
            {
                oModel.ProviderMenu = GetThirdKnowledgeControllerMenu();

                //Clean the season url saved
                if (SessionModel.CurrentURL != null)
                    SessionModel.CurrentURL = null;

                //Get The Active Plan By Customer 
                oCurrentPeriodList = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetCurrenPeriod(SessionModel.CurrentCompany.CompanyPublicId, true);

                if (oCurrentPeriodList != null && oCurrentPeriodList.Count > 0)
                {
                    oModel.RelatedThirdKnowledge.HasPlan = true;

                    //Get The Most Recently Period When Plan is More Than One
                    oModel.RelatedThirdKnowledge.CurrentPlanModel = oCurrentPeriodList.OrderByDescending(x => x.CreateDate).First();
                }
                else
                {
                    oModel.RelatedThirdKnowledge.HasPlan = false;
                }
                return View(oModel);
            }
            catch (Exception ex)
            {

                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Controller detail
        /// </summary>
        /// <param name="QueryBasicPublicId">This is the QueryInfo</param>
        /// <param name="ReturnUrl">URL to go back</param>
        /// <returns>Detail View</returns>
        /// 

        public virtual ActionResult TKDetailSingleSearch(string QueryPublicId, string QueryInfoPublicId, string ElasticId, string ReturnUrl)
        {
            ProviderViewModel oModel = new ProviderViewModel();

            TDQueryInfoModel QueryDetailInfo = new TDQueryInfoModel();
            try
            {
                oModel.ProviderMenu = GetThirdKnowledgeControllerMenu();
                //Clean the season url saved
                if (SessionModel.CurrentURL != null)
                    SessionModel.CurrentURL = null;

                //Get The Active Plan By Customer 
                if (!string.IsNullOrEmpty(QueryInfoPublicId))
                    QueryDetailInfo = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetQueryInfoByInfoPublicId(QueryInfoPublicId);
                else
                    QueryDetailInfo = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetQueryInfoByQueryPublicIdAndElasticId(QueryPublicId, Convert.ToInt32(ElasticId));

                int TotalRows = 0;

                List<TDQueryModel> oQueryResult = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.ThirdKnowledgeSearchByPublicId(QueryPublicId,
                    0, 20, out TotalRows);
                if (QueryDetailInfo != null)
                    oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel(QueryDetailInfo);
                else
                    oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();


                if (ReturnUrl == "null")
                    oModel.RelatedThidKnowledgeSearch.ReturnUrl = ReturnUrl;

                oModel.RelatedThidKnowledgeSearch.QueryBasicPublicId = QueryInfoPublicId;
                oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult = oQueryResult;
                //Get report generator
                if (Request["DownloadReport"] == "true")
                {
                    #region Set Parameters
                    List<ReportParameter> parameters = new List<ReportParameter>();
                    //Customer Info
                    parameters.Add(new ReportParameter("CustomerName", SessionModel.CurrentCompany.CompanyName));
                    parameters.Add(new ReportParameter("CustomerIdentification", SessionModel.CurrentCompany.IdentificationNumber));
                    parameters.Add(new ReportParameter("CustomerIdentificationType", SessionModel.CurrentCompany.IdentificationType.ItemName));
                    parameters.Add(new ReportParameter("CustomerImage", SessionModel.CurrentCompany_CompanyLogo));
                    parameters.Add(new ReportParameter("SearchName", oModel.RelatedThidKnowledgeSearch.RequestName));
                    parameters.Add(new ReportParameter("SearchIdentification", oModel.RelatedThidKnowledgeSearch.IdNumberRequest));
                    //Query Detail Info
                    parameters.Add(new ReportParameter("ThirdKnowledgeText", MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.MP_TK_TextImage].Value));
                    parameters.Add(new ReportParameter("NameResult", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.NameResult) ? oModel.RelatedThidKnowledgeSearch.NameResult : "--"));
                    parameters.Add(new ReportParameter("IdentificationType", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.TypeDocument) ? oModel.RelatedThidKnowledgeSearch.TypeDocument : "--"));
                    parameters.Add(new ReportParameter("IdentificationNumber", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.IdentificationNumberResult) ? oModel.RelatedThidKnowledgeSearch.IdentificationNumberResult : "--"));
                    parameters.Add(new ReportParameter("Zone", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.Zone) ? oModel.RelatedThidKnowledgeSearch.Zone : "--"));
                    parameters.Add(new ReportParameter("Priority", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.Priority) ? oModel.RelatedThidKnowledgeSearch.Priority : "--"));
                    parameters.Add(new ReportParameter("Offence", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.Offense) ? oModel.RelatedThidKnowledgeSearch.Offense : "--"));
                    parameters.Add(new ReportParameter("Peps", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.Peps) ? oModel.RelatedThidKnowledgeSearch.Peps : "--"));
                    parameters.Add(new ReportParameter("ListName", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.ListName) ? oModel.RelatedThidKnowledgeSearch.ListName : "--"));
                    parameters.Add(new ReportParameter("Alias", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.Alias) ? oModel.RelatedThidKnowledgeSearch.Alias : "--"));
                    parameters.Add(new ReportParameter("LastUpdate", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.LastModifyDate) ? oModel.RelatedThidKnowledgeSearch.LastModifyDate : "--"));
                    parameters.Add(new ReportParameter("QueryCreateDate", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.RegisterDate) ? oModel.RelatedThidKnowledgeSearch.RegisterDate : "--"));
                    parameters.Add(new ReportParameter("Link", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.Link) ? oModel.RelatedThidKnowledgeSearch.Link : "--"));
                    parameters.Add(new ReportParameter("MoreInformation", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.MoreInfo) ? oModel.RelatedThidKnowledgeSearch.MoreInfo : "--"));
                    parameters.Add(new ReportParameter("User", SessionModel.CurrentLoginUser.Name != null ? SessionModel.CurrentLoginUser.Name.ToString() : "" + " " + SessionModel.CurrentLoginUser.LastName != null ? SessionModel.CurrentLoginUser.LastName.ToString() : ""));
                    parameters.Add(new ReportParameter("ReportCreateDate", DateTime.Now.ToString()));
                    parameters.Add(new ReportParameter("Group", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.GroupName) ? oModel.RelatedThidKnowledgeSearch.GroupName : "--"));
                    parameters.Add(new ReportParameter("Status", !string.IsNullOrEmpty(oModel.RelatedThidKnowledgeSearch.Status) ? oModel.RelatedThidKnowledgeSearch.Status : "--"));
                    string fileFormat = Request["ThirdKnowledge_cmbFormat"] != null ? Request["ThirdKnowledge_cmbFormat"].ToString() : "pdf";
                    Tuple<byte[], string, string> ThirdKnowledgeReport = ProveedoresOnLine.Reports.Controller.ReportModule.TK_QueryDetailReport(
                                                                    fileFormat,
                                                                    parameters,
                                                                    Models.General.InternalSettings.Instance[Models.General.Constants.MP_CP_ReportPath].Value.Trim() + "TK_Report_ThirdKnowledgeQueryDetail.rdlc");
                    parameters = null;
                    return File(ThirdKnowledgeReport.Item1, ThirdKnowledgeReport.Item2, ThirdKnowledgeReport.Item3);

                    #endregion
                }

                return View(oModel);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public virtual ActionResult TKThirdKnowledgeSearch(string PageNumber, string InitDate, string EndDate, string SearchType, string Status, string User, string Domain)
        {
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            string RelatedUser = null;
            var ParentRole = SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().RelatedCompanyRole.ParentRoleCompany;

            if (ParentRole != null)
            {
                RelatedUser = SessionModel.CurrentCompanyLoginUser.RelatedUser.Email;
            }
            else
            {
                RelatedUser = null;
            }
            if (!string.IsNullOrEmpty(User))
            {
                RelatedUser = User;
            }

            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();
            List<ProveedoresOnLine.ThirdKnowledge.Models.TDQueryModel> oQueryModel = new List<TDQueryModel>();

            List<PlanModel> oCurrentPeriodList = new List<PlanModel>();
            oCurrentPeriodList = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetCurrenPeriod(SessionModel.CurrentCompany.CompanyPublicId, true);
            if (oCurrentPeriodList != null && oCurrentPeriodList.Count > 0)
                oModel.RelatedThidKnowledgeSearch.CurrentPlanModel = oCurrentPeriodList.OrderByDescending(x => x.CreateDate).First();

            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager = new Models.ThirdKnowledge.ThirdKnowledgeSearchViewModel()
            {
                PageNumber = !string.IsNullOrEmpty(PageNumber) ? Convert.ToInt32(PageNumber) : 0,
            };
            int TotalRows = 0;
            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.PageNumber = !string.IsNullOrEmpty(PageNumber) ? Convert.ToInt32(PageNumber) : 0;


            oQueryModel = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.ThirdKnowledgeSearch(
                SessionModel.CurrentCompany.CompanyPublicId,
                RelatedUser,
                Domain,
                !string.IsNullOrEmpty(InitDate) ? InitDate : "",
                !string.IsNullOrEmpty(EndDate) ? EndDate : "",
                oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.PageNumber,
                Convert.ToInt32(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_Grid_RowCountDefault].Value.Trim()),
                SearchType,
                Status,
                out TotalRows);

            oModel.RelatedThidKnowledgeSearch.FilterList = new Dictionary<string, int>();
            if (!string.IsNullOrEmpty(InitDate))
                oModel.RelatedThidKnowledgeSearch.FilterList.Add(InitDate, (int)enumTKFilter.DateFromFilter);
            if (!string.IsNullOrEmpty(EndDate))
            {
                oModel.RelatedThidKnowledgeSearch.FilterList.Add(EndDate, (int)enumTKFilter.DateToFilter);
            }

            #region ElasticSearch

            Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);

            #region Search 

            settings.DefaultIndex(Models.General.InternalSettings.Instance[Models.General.Constants.C_Settings_QueryModelIndex].Value);
            settings.DisableDirectStreaming(true);
            ElasticClient client = new ElasticClient(settings);

            oModel.RelatedThidKnowledgeSearch.ElasticQueryModel = client.Search<TK_QueryIndexModel>(s => s
            .From(string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber) * 20)
            .TrackScores(true)
            .Size(20)
            .Aggregations
                (agg => agg
                .Terms("status", aggv => aggv
                    .Field(fi => fi.QueryStatus))
                .Terms("date", aggv => aggv
                    .Field(fi => fi.LastModify))
                .Terms("searchtype", c => c
                    .Field(fi => fi.SearchType))
                .Terms("domain", c => c
                    .Field(fi => fi.Domain))
                .Terms("useremail", bl => bl
                    .Field(fi => fi.User)))
                .Query(q => q.Bool(f => f.
                    Filter(f2 =>
                        {
                            QueryContainer qb = null;

                            qb &= f2.Terms(tms => tms
                            .Field(fi => fi.CustomerPublicId.ToLower())
                             .Terms<string>(SessionModel.CurrentCompany.CompanyPublicId.ToLower())
                            );

                            if (!string.IsNullOrEmpty(Status))
                            {
                                qb &= q.Term(m => m.QueryStatus, Status);
                                oModel.RelatedThidKnowledgeSearch.FilterList.Add(Status, (int)enumTKFilter.StatusFilter);
                            }
                            if (!string.IsNullOrEmpty(SearchType))
                            {
                                qb &= q.Term(m => m.SearchType, SearchType);
                                oModel.RelatedThidKnowledgeSearch.FilterList.Add(SearchType, (int)enumTKFilter.QueryTypeFilter);
                            }
                            if (!string.IsNullOrEmpty(RelatedUser))
                            {
                                qb &= q.Term(m => m.User, RelatedUser);
                                oModel.RelatedThidKnowledgeSearch.FilterList.Add(RelatedUser, (int)enumTKFilter.UserFilter);
                            }
                            if (!string.IsNullOrEmpty(Domain))
                            {
                                qb &= q.Term(m => m.Domain, Domain);
                                oModel.RelatedThidKnowledgeSearch.FilterList.Add(Domain, (int)enumTKFilter.DomainFilter);
                            }
                            if (!string.IsNullOrEmpty(InitDate) && !string.IsNullOrEmpty(EndDate))
                            {
                                qb &= q.DateRange(dr => dr
                                        .Field(t2 => t2.CreateDate)
                                        .GreaterThanOrEquals(InitDate).LessThan(EndDate)
                                    );
                            }


                            return qb;
                        }
                    )
                )
                )
            );
            #region Status Aggregation
            oModel.RelatedThidKnowledgeSearch.QueryStatusFilter = new List<ElasticSearchFilter>();
            oModel.RelatedThidKnowledgeSearch.ElasticQueryModel.Aggs.Terms("status").Buckets.All(x =>
            {
                oModel.RelatedThidKnowledgeSearch.QueryStatusFilter.Add(new ElasticSearchFilter
                {
                    FilterCount = (int)x.DocCount,
                    FilterType = x.Key,
                    FilterName = x.Key == ((int)enumThirdKnowledgeQueryStatus.Finalized).ToString() ? "Finalizado" : x.Key.Split('.')[0] == ((int)enumThirdKnowledgeQueryStatus.InProcess).ToString() ? "En Progreso" : "N/E",
                });
                return true;
            });

            #endregion

            #region Search type Aggregation
            oModel.RelatedThidKnowledgeSearch.QueryTypeFilter = new List<ElasticSearchFilter>();
            oModel.RelatedThidKnowledgeSearch.ElasticQueryModel.Aggs.Terms("searchtype").Buckets.All(x =>
            {
                oModel.RelatedThidKnowledgeSearch.QueryTypeFilter.Add(new ElasticSearchFilter
                {
                    FilterCount = (int)x.DocCount,
                    FilterType = x.Key,
                    FilterName = x.Key == ((int)enumThirdKnowledgeQueryType.Masive).ToString() ? "Masiva" : x.Key.Split('.')[0] == ((int)enumThirdKnowledgeQueryType.Simple).ToString() ? "Individual" : "N/E",
                });
                return true;
            });

            #endregion

            #region User Aggregation
            oModel.RelatedThidKnowledgeSearch.UserFilter = new List<ElasticSearchFilter>();

            oModel.RelatedThidKnowledgeSearch.ElasticQueryModel.Aggs.Terms("useremail").Buckets.All(x =>
            {
                oModel.RelatedThidKnowledgeSearch.UserFilter.Add(new ElasticSearchFilter
                {
                    FilterCount = (int)x.DocCount,
                    FilterType = x.Key,
                    FilterName = x.Key,
                });
                return true;
            });

            #endregion

            #region Domain Aggregation
            oModel.RelatedThidKnowledgeSearch.DomainFilter = new List<ElasticSearchFilter>();
            oModel.RelatedThidKnowledgeSearch.ElasticQueryModel.Aggs.Terms("domain").Buckets.All(x =>
            {
                oModel.RelatedThidKnowledgeSearch.DomainFilter.Add(new ElasticSearchFilter
                {
                    FilterCount = (int)x.DocCount,
                    FilterType = x.Key,
                    FilterName = x.Key,
                });
                return true;
            });

            #endregion

            #endregion

            #endregion ElasticSearch          

            List<TDQueryInfoModel> objQueryInfo = new List<TDQueryInfoModel>();
            oModel.RelatedThirdKnowledge = new ThirdKnowledgeViewModel();

            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.TotalRows = TotalRows;

            if (oQueryModel != null && oQueryModel.Count > 0)
            {
                oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult = oQueryModel.OrderByDescending(x => x.CreateDate).ToList();
            }
            else
            {
                oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult = new List<TDQueryModel>();
            }

            oModel.ProviderMenu = GetThirdKnowledgeControllerMenu();

            #region Report

            // Get report generator
            if (Request["DownloadReport"] == "true")
            {
                #region ElasticSearch
                oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();
                oModel.RelatedThidKnowledgeSearch.FilterList = new Dictionary<string, int>();
                if (!string.IsNullOrEmpty(InitDate))
                    oModel.RelatedThidKnowledgeSearch.FilterList.Add(InitDate, (int)enumTKFilter.DateFromFilter);
                if (!string.IsNullOrEmpty(EndDate))
                {
                    oModel.RelatedThidKnowledgeSearch.FilterList.Add(EndDate, (int)enumTKFilter.DateToFilter);
                }

                Uri node2 = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings2 = new ConnectionSettings(node2);


                settings2.DefaultIndex(Models.General.InternalSettings.Instance[Models.General.Constants.C_Settings_QueryModelIndex].Value);
                settings2.DisableDirectStreaming(true);
                ElasticClient client2 = new ElasticClient(settings2);

                oModel.RelatedThidKnowledgeSearch.ElasticQueryModel = client2.Search<TK_QueryIndexModel>(s => s
                .From(string.IsNullOrEmpty(PageNumber) ? 0 : Convert.ToInt32(PageNumber) * 20)
                .TrackScores(true)
                .Size(2000000)
                .Aggregations
                    (agg => agg
                    .Terms("status", aggv => aggv
                        .Field(fi => fi.QueryStatus))
                    .Terms("date", aggv => aggv
                        .Field(fi => fi.LastModify))
                    .Terms("searchtype", c => c
                        .Field(fi => fi.SearchType))
                    .Terms("domain", c => c
                        .Field(fi => fi.Domain))
                    .Terms("useremail", bl => bl
                        .Field(fi => fi.User)))
                    .Query(q => q.Bool(f => f.
                        Filter(f2 =>
                        {
                            QueryContainer qb = null;

                            qb &= f2.Terms(tms => tms
                        .Field(fi => fi.CustomerPublicId.ToLower())
                         .Terms<string>(SessionModel.CurrentCompany.CompanyPublicId.ToLower())
                        );

                            if (!string.IsNullOrEmpty(Status))
                            {
                                qb &= q.Term(m => m.QueryStatus, Status);
                                oModel.RelatedThidKnowledgeSearch.FilterList.Add(Status, (int)enumTKFilter.StatusFilter);
                            }
                            if (!string.IsNullOrEmpty(SearchType))
                            {
                                qb &= q.Term(m => m.SearchType, SearchType);
                                oModel.RelatedThidKnowledgeSearch.FilterList.Add(SearchType, (int)enumTKFilter.QueryTypeFilter);
                            }
                            if (!string.IsNullOrEmpty(User))
                            {
                                qb &= q.Term(m => m.User, User);
                                oModel.RelatedThidKnowledgeSearch.FilterList.Add(User, (int)enumTKFilter.UserFilter);
                            }
                            if (!string.IsNullOrEmpty(Domain))
                            {
                                qb &= q.Term(m => m.Domain, Domain);
                                oModel.RelatedThidKnowledgeSearch.FilterList.Add(Domain, (int)enumTKFilter.DomainFilter);
                            }
                            if (!string.IsNullOrEmpty(InitDate) && !string.IsNullOrEmpty(EndDate))
                            {
                                qb &= q.DateRange(dr => dr
                                    .Field(t2 => t2.CreateDate)
                                    .GreaterThanOrEquals(InitDate).LessThan(EndDate)
                                );
                            }


                            return qb;
                        }
                        )
                    )
                    )
                );
                #endregion

                #region SetParameters

                List<ReportParameter> parameters = new List<ReportParameter>();

                //Customer Info Parameters
                parameters.Add(new ReportParameter("CustomerName", SessionModel.CurrentCompany.CompanyName));
                parameters.Add(new ReportParameter("CustomerIdentification", SessionModel.CurrentCompany.IdentificationNumber));
                parameters.Add(new ReportParameter("CustomerIdentificationType", SessionModel.CurrentCompany.IdentificationType.ItemName));
                parameters.Add(new ReportParameter("CustomerImage", SessionModel.CurrentCompany_CompanyLogo));

                parameters.Add(new ReportParameter("ThirdKnowledgeText", MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.MP_TK_TextImage].Value));
                parameters.Add(new ReportParameter("User", SessionModel.CurrentLoginUser.Email));
                parameters.Add(new ReportParameter("FirstName", SessionModel.CurrentLoginUser.Name));
                parameters.Add(new ReportParameter("LastName", SessionModel.CurrentLoginUser.LastName));


                #endregion

                var data_Query = new DataTable();
                data_Query.Columns.Add("User");
                data_Query.Columns.Add("QueryDate");
                data_Query.Columns.Add("Status");
                data_Query.Columns.Add("QueryType");
                DataRow row_Query;

                oModel.RelatedThidKnowledgeSearch.ElasticQueryModel.Documents.All(x =>
                {
                    row_Query = data_Query.NewRow();
                    row_Query["User"] = x.User;
                    row_Query["QueryDate"] = x.CreateDate;
                    row_Query["Status"] = x.QueryStatus == Convert.ToString((int)MarketPlace.Models.General.enumThirdKnowledgeQueryStatus.Finalized) ? "Finalizada" : "En Progreso";
                    row_Query["QueryType"] = x.SearchType == Convert.ToString((int)MarketPlace.Models.General.enumThirdKnowledgeQueryType.Simple) ? "Individual" : "Masiva";

                    data_Query.Rows.Add(row_Query);
                    return true;
                });

                string fileFormat = Request["ThirdKnowledge_cmbFormat"] != null ? Request["ThirdKnowledge_cmbFormat"].ToString() : "pdf";
                Tuple<byte[], string, string> ThirdKnowledgeReport = ProveedoresOnLine.Reports.Controller.ReportModule.TK_MyQueriesReport(
                                                                fileFormat, parameters,
                                                                data_Query,
                                                                Models.General.InternalSettings.Instance[Models.General.Constants.MP_CP_ReportPath].Value.Trim() + "TK_Report_ThirdKnowledgeMyQueries.rdlc");
                parameters = null;
                return File(ThirdKnowledgeReport.Item1, ThirdKnowledgeReport.Item2, ThirdKnowledgeReport.Item3);

            }

            #endregion

            return View(oModel);
        }

        public virtual ActionResult TKThirdKnowledgeDetail(string QueryPublicId, string PageNumber, string InitDate, string EndDate, string Enable, string IsSuccess)
        {
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            int oTotalRowsAux = Convert.ToInt32(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_Grid_RowCountDefault].Value.Trim());

            if (Request["DownloadReport"] == "true")
                oTotalRowsAux = 10000;

            if (!string.IsNullOrEmpty(Request["ThirdKnowledge_FormQueryPublicId"]))
                QueryPublicId = Request["ThirdKnowledge_FormQueryPublicId"];

            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();
            oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult = new List<TDQueryModel>();
            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager = new Models.ThirdKnowledge.ThirdKnowledgeSearchViewModel()
            {
                PageNumber = !string.IsNullOrEmpty(PageNumber) ? Convert.ToInt32(PageNumber) : 0,
            };
            int TotalRows = 0;

            List<TDQueryModel> oQueryResult = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.ThirdKnowledgeSearchByPublicId(QueryPublicId, oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.PageNumber, oTotalRowsAux, out TotalRows);

            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.TotalRows = TotalRows;

            if (oQueryResult != null && oQueryResult.Count > 0)
                oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult = oQueryResult;
            else if (IsSuccess == "Finalizado")
                oModel.RelatedThidKnowledgeSearch.Message = "La búsqueda no arrojó resultados.";

            if (!string.IsNullOrEmpty(InitDate) && !string.IsNullOrEmpty(EndDate))
            {
                oModel.RelatedThidKnowledgeSearch.InitDate = Convert.ToDateTime(InitDate);
                oModel.RelatedThidKnowledgeSearch.EndDate = Convert.ToDateTime(EndDate);
            }

            oModel.ProviderMenu = GetThirdKnowledgeControllerMenu();


            if (oModel != null)
            {
                List<Tuple<string, List<ThirdKnowledgeViewModel>>> oGroup = new List<Tuple<string, List<ThirdKnowledgeViewModel>>>();
                List<string> Item1 = new List<string>();

                oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.All(
                item =>
                {
                    item.RelatedQueryInfoModel.All(x =>
                    {
                        Item1.Add(x.GroupName);
                        return true;
                    });
                    Item1 = Item1.GroupBy(x => x).Select(grp => grp.First()).ToList();

                    List<ThirdKnowledgeViewModel> oItem2 = new List<ThirdKnowledgeViewModel>();
                    Tuple<string, List<ThirdKnowledgeViewModel>> oTupleItem = new Tuple<string, List<ThirdKnowledgeViewModel>>("", new List<ThirdKnowledgeViewModel>());

                    Item1.All(x =>
                    {
                        oItem2 = new List<ThirdKnowledgeViewModel>();
                        if (item.RelatedQueryInfoModel.Where(td => td.GroupName == x) != null)
                        {
                            item.RelatedQueryInfoModel.Where(td => td.GroupName == x).
                            Select(td => td).ToList().All(d =>
                            {
                                oItem2.Add(new ThirdKnowledgeViewModel(d));
                                return true;
                            });
                            oTupleItem = new Tuple<string, List<ThirdKnowledgeViewModel>>(x, oItem2);
                            oGroup.Add(oTupleItem);
                        }
                        return true;
                    });
                    return true;
                });

                List<Tuple<string, List<ThirdKnowledgeViewModel>>> oGroupOrder = new List<Tuple<string, List<ThirdKnowledgeViewModel>>>();

                oGroupOrder.AddRange(oGroup.Where(x => x.Item1.Contains("Criticidad Alta")));
                oGroupOrder.AddRange(oGroup.Where(x => x.Item1.Contains("Criticidad Media")));
                oGroupOrder.AddRange(oGroup.Where(x => x.Item1.Contains("Criticidad Baja")));
                oGroupOrder.AddRange(oGroup.Where(x => x.Item1.Contains("SIN COINCIDENCIAS")));
                oModel.Group = oGroupOrder;
            }


            //Get report generator
            if (Request["DownloadReport"] == "true")
            {
                #region Set Parameters
                //Get Request

                var objRelatedQueryBasicInfo = oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.RelatedQueryInfoModel != null).FirstOrDefault().RelatedQueryInfoModel.FirstOrDefault();
                string searchName = "";
                string searchIdentification = "";
                if (!string.IsNullOrEmpty(objRelatedQueryBasicInfo.QueryName))
                    searchName = objRelatedQueryBasicInfo.QueryName;

                if (!string.IsNullOrEmpty(objRelatedQueryBasicInfo.QueryIdentification))
                {
                    searchIdentification += objRelatedQueryBasicInfo.QueryIdentification;
                }

                List<ReportParameter> parameters = new List<ReportParameter>();

                //Customer Info Parameters
                parameters.Add(new ReportParameter("CustomerName", SessionModel.CurrentCompany.CompanyName));
                parameters.Add(new ReportParameter("CustomerIdentification", SessionModel.CurrentCompany.IdentificationNumber));
                parameters.Add(new ReportParameter("CustomerIdentificationType", SessionModel.CurrentCompany.IdentificationType.ItemName));
                parameters.Add(new ReportParameter("CustomerImage", SessionModel.CurrentCompany_CompanyLogo));

                //Query Info Parameters
                parameters.Add(new ReportParameter("ThirdKnowledgeText", MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.MP_TK_TextImage].Value));
                parameters.Add(new ReportParameter("User", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.User != null).Select(x => x.User).DefaultIfEmpty("No hay campo").FirstOrDefault()));
                parameters.Add(new ReportParameter("CreateDate", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.CreateDate != null).Select(x => x.CreateDate.AddHours(-5).ToString().ToString()).DefaultIfEmpty("No hay campo").FirstOrDefault()));
                parameters.Add(new ReportParameter("QueryType", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.SearchType != null).Select(x => x.SearchType.ItemName).DefaultIfEmpty("No hay campo").FirstOrDefault()));
                parameters.Add(new ReportParameter("Status", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.QueryStatus != null).Select(x => x.QueryStatus.ItemName).DefaultIfEmpty("No hay campo").FirstOrDefault()));
                parameters.Add(new ReportParameter("searchName", searchName));
                parameters.Add(new ReportParameter("searchIdentification", searchIdentification));
                parameters.Add(new ReportParameter("IsSuccess", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x != null).Select(x => x.IsSuccess).FirstOrDefault().ToString()));

                /*data for Matches with High Critical*/
                DataTable data_HighCritical = new DataTable();
                data_HighCritical.Columns.Add("IdentificationResult");
                data_HighCritical.Columns.Add("NameResult");
                data_HighCritical.Columns.Add("Offense");
                data_HighCritical.Columns.Add("Peps");
                data_HighCritical.Columns.Add("Priority");
                data_HighCritical.Columns.Add("Status");
                data_HighCritical.Columns.Add("ListName");
                data_HighCritical.Columns.Add("IdentificationSearch");
                data_HighCritical.Columns.Add("NameSearch");
                DataRow row_HighCrit;
                var lrs = new List<ThirdKnowledgeViewModel>();
                oModel.Group.All(x =>
                {
                    if (x.Item1.Contains("Criticidad Alta"))
                    {
                        lrs.AddRange(x.Item2);
                    }
                    return true;
                });

                if (lrs != null)
                    lrs.All(y =>
                    {
                        row_HighCrit = data_HighCritical.NewRow();
                        row_HighCrit["IdentificationResult"] = y.IdentificationNumberResult;
                        row_HighCrit["NameResult"] = y.NameResult;
                        row_HighCrit["Offense"] = y.Offense;
                        row_HighCrit["Peps"] = y.Peps;
                        row_HighCrit["Priority"] = y.Priority;
                        row_HighCrit["Status"] = y.Status.ToLower() == "true" ? "Activo" : "Inactivo";
                        row_HighCrit["ListName"] = y.ListName;
                        row_HighCrit["IdentificationSearch"] = y.IdNumberRequest; // SearchId Param
                        row_HighCrit["NameSearch"] = y.RequestName; // SearchName Param
                        data_HighCritical.Rows.Add(row_HighCrit);
                        return true;
                    });

                /*data for Matches with Medium Critical*/
                DataTable data_MediumCritical = new DataTable();
                data_MediumCritical.Columns.Add("IdentificationResult");
                data_MediumCritical.Columns.Add("NameResult");
                data_MediumCritical.Columns.Add("Offense");
                data_MediumCritical.Columns.Add("Peps");
                data_MediumCritical.Columns.Add("Priority");
                data_MediumCritical.Columns.Add("Status");
                data_MediumCritical.Columns.Add("ListName");
                data_MediumCritical.Columns.Add("IdentificationSearch");
                data_MediumCritical.Columns.Add("NameSearch");
                DataRow row_MediumCrit;
                var dce = new List<ThirdKnowledgeViewModel>();
                oModel.Group.All(x =>
                {
                    if (x.Item1.Contains("Criticidad Media"))
                    {
                        dce.AddRange(x.Item2);
                    }
                    return true;
                });
                if (dce != null)
                    dce.All(y =>
                    {
                        row_MediumCrit = data_MediumCritical.NewRow();
                        row_MediumCrit["IdentificationResult"] = y.IdentificationNumberResult;
                        parameters.Add(new ReportParameter("GroupNameDce", y.GroupName));
                        row_MediumCrit["NameResult"] = y.NameResult;
                        row_MediumCrit["Offense"] = y.Offense;
                        row_MediumCrit["Peps"] = y.Peps;
                        row_MediumCrit["Priority"] = y.Priority;
                        row_MediumCrit["Status"] = y.Status.ToLower() == "true" ? "Activo" : "Inactivo";
                        row_MediumCrit["ListName"] = y.ListName;
                        row_MediumCrit["IdentificationSearch"] = y.IdNumberRequest; // SearchId Param
                        row_MediumCrit["NameSearch"] = y.RequestName; // SearchName Param
                        data_MediumCritical.Rows.Add(row_MediumCrit);
                        return true;
                    });

                /*data for Matches with Low Critical*/
                DataTable data_LowCritical = new DataTable();
                data_LowCritical.Columns.Add("IdentificationResult");
                data_LowCritical.Columns.Add("NameResult");
                data_LowCritical.Columns.Add("Offense");
                data_LowCritical.Columns.Add("Peps");
                data_LowCritical.Columns.Add("Priority");
                data_LowCritical.Columns.Add("Status");
                data_LowCritical.Columns.Add("ListName");
                data_LowCritical.Columns.Add("IdentificationSearch");
                data_LowCritical.Columns.Add("NameSearch");
                DataRow row_LowCrit;
                var psp = new List<ThirdKnowledgeViewModel>();
                oModel.Group.All(x =>
                {
                    if (x.Item1.Contains("Criticidad Baja"))
                    {
                        psp.AddRange(x.Item2);
                    }
                    return true;
                });
                if (psp != null)
                    psp.All(y =>
                    {
                        row_LowCrit = data_LowCritical.NewRow();
                        row_LowCrit["IdentificationResult"] = y.IdentificationNumberResult;
                        row_LowCrit["NameResult"] = y.NameResult;
                        row_LowCrit["Offense"] = y.Offense;
                        row_LowCrit["Peps"] = y.Peps;
                        row_LowCrit["Priority"] = y.Priority;
                        row_LowCrit["Status"] = y.Status.ToLower() == "true" ? "Activo" : "Inactivo";
                        row_LowCrit["ListName"] = y.ListName;
                        row_LowCrit["IdentificationSearch"] = y.IdNumberRequest; // SearchId Param
                        row_LowCrit["NameSearch"] = y.RequestName; // SearchName Param
                        data_LowCritical.Rows.Add(row_LowCrit);
                        return true;
                    });


                /*data for No Match Results*/
                DataTable data_NoMatch = new DataTable();
                data_NoMatch.Columns.Add("IdentificationResult");
                data_NoMatch.Columns.Add("NameResult");
                data_NoMatch.Columns.Add("IdentificationSearch");
                data_NoMatch.Columns.Add("NameSearch");
                DataRow row_NoMatch;
                List<ThirdKnowledgeViewModel> snc = oModel.Group.Where(x => x.Item1.Contains("SIN COINCIDENCIAS")).Select(x => x.Item2).FirstOrDefault();
                if (snc != null)
                    snc.All(y =>
                    {
                        row_NoMatch = data_NoMatch.NewRow();
                        row_NoMatch["IdentificationSearch"] = y.IdNumberRequest; // SearchId Param
                        row_NoMatch["NameSearch"] = y.NameResult; // SearchName Param
                        row_NoMatch["IdentificationResult"] = y.IdNumberRequest;
                        row_NoMatch["NameResult"] = y.RequestName;

                        data_NoMatch.Rows.Add(row_NoMatch);
                        return true;
                    });
                string fileFormat = Request["ThirdKnowledge_cmbFormat"] != null ? Request["ThirdKnowledge_cmbFormat"].ToString() : "pdf";
                Tuple<byte[], string, string> ThirdKnowledgeReport = ProveedoresOnLine.Reports.Controller.ReportModule.TK_QueryReport(
                                                                fileFormat,
                                                                data_HighCritical,
                                                                data_MediumCritical,
                                                                data_LowCritical,
                                                                data_NoMatch,
                                                                parameters,
                                                                Models.General.InternalSettings.Instance[Models.General.Constants.MP_CP_ReportPath].Value.Trim() + "TK_Report_ThirdKnowledgeQueryNew.rdlc");
                parameters = null;
                return File(ThirdKnowledgeReport.Item1, ThirdKnowledgeReport.Item2, ThirdKnowledgeReport.Item3);

                #endregion
            }

            return View(oModel);
        }

        public virtual ActionResult TKThirdKnowledgeDetailNew(string QueryPublicId, string PageNumber, string InitDate, string EndDate, string Enable, string IsSuccess)
        {
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;

            int oTotalRowsAux = Convert.ToInt32(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_Grid_RowCountDefault].Value.Trim());

            if (Request["DownloadReport"] == "true")
                oTotalRowsAux = 10000;

            if (!string.IsNullOrEmpty(Request["ThirdKnowledge_FormQueryPublicId"]))
                QueryPublicId = Request["ThirdKnowledge_FormQueryPublicId"];

            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();
            oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult = new List<TDQueryModel>();
            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager = new Models.ThirdKnowledge.ThirdKnowledgeSearchViewModel()
            {
                PageNumber = !string.IsNullOrEmpty(PageNumber) ? Convert.ToInt32(PageNumber) : 0,
            };
            int TotalRows = 0;

            //TODO: Dejar El nuevo Objeto según  el cambio de Listas Restrictivas para poderlo enviar al reporte
            List<TDQueryModel> oQueryResult = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.ThirdKnowledgeSearchByPublicId(QueryPublicId, oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.PageNumber, oTotalRowsAux, out TotalRows);

            //Call Function to build object

            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.TotalRows = TotalRows;

            if (oQueryResult != null && oQueryResult.Count > 0)
                oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult = oQueryResult;
            else if (IsSuccess == "Finalizado")
                oModel.RelatedThidKnowledgeSearch.Message = "La búsqueda no arrojó resultados.";

            if (!string.IsNullOrEmpty(InitDate) && !string.IsNullOrEmpty(EndDate))
            {
                oModel.RelatedThidKnowledgeSearch.InitDate = Convert.ToDateTime(InitDate);
                oModel.RelatedThidKnowledgeSearch.EndDate = Convert.ToDateTime(EndDate);
            }

            oModel.ProviderMenu = GetThirdKnowledgeControllerMenu();


            if (oModel != null)
            {
                List<Tuple<string, List<ThirdKnowledgeViewModel>>> oGroup = new List<Tuple<string, List<ThirdKnowledgeViewModel>>>();
                List<string> Item1 = new List<string>();

                oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.All(
                item =>
                {
                    item.RelatedQueryInfoModel.All(x =>
                    {
                        Item1.Add(x.GroupName);
                        return true;
                    });
                    Item1 = Item1.GroupBy(x => x).Select(grp => grp.First()).ToList();

                    List<ThirdKnowledgeViewModel> oItem2 = new List<ThirdKnowledgeViewModel>();
                    Tuple<string, List<ThirdKnowledgeViewModel>> oTupleItem = new Tuple<string, List<ThirdKnowledgeViewModel>>("", new List<ThirdKnowledgeViewModel>());

                    Item1.All(x =>
                    {
                        oItem2 = new List<ThirdKnowledgeViewModel>();
                        if (item.RelatedQueryInfoModel.Where(td => td.GroupName == x) != null)
                        {
                            item.RelatedQueryInfoModel.Where(td => td.GroupName == x).
                            Select(td => td).ToList().All(d =>
                            {
                                oItem2.Add(new ThirdKnowledgeViewModel(d));
                                return true;
                            });
                            oTupleItem = new Tuple<string, List<ThirdKnowledgeViewModel>>(x, oItem2);
                            oGroup.Add(oTupleItem);
                        }
                        return true;
                    });
                    return true;
                });

                List<Tuple<string, List<ThirdKnowledgeViewModel>>> oGroupOrder = new List<Tuple<string, List<ThirdKnowledgeViewModel>>>();

                oGroupOrder.AddRange(oGroup.Where(x => x.Item1.Contains("Criticidad Alta")));
                oGroupOrder.AddRange(oGroup.Where(x => x.Item1.Contains("Criticidad Media")));
                oGroupOrder.AddRange(oGroup.Where(x => x.Item1.Contains("Criticidad Baja")));
                oGroupOrder.AddRange(oGroup.Where(x => x.Item1.Contains("SIN COINCIDENCIAS")));
                oModel.Group = oGroupOrder;
            }


            //Get report generator
            if (Request["DownloadReport"] == "true")
            {
                #region Set Parameters
                //Get Request

                var objRelatedQueryBasicInfo = oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.RelatedQueryInfoModel != null).FirstOrDefault().RelatedQueryInfoModel.FirstOrDefault();
                string searchName = "";
                string searchIdentification = "";
                if (!string.IsNullOrEmpty(objRelatedQueryBasicInfo.QueryName))
                    searchName = objRelatedQueryBasicInfo.QueryName;

                if (!string.IsNullOrEmpty(objRelatedQueryBasicInfo.QueryIdentification))
                {
                    searchIdentification += objRelatedQueryBasicInfo.QueryIdentification;
                }

                List<ReportParameter> parameters = new List<ReportParameter>();

                //Customer Info Parameters
                parameters.Add(new ReportParameter("CustomerName", SessionModel.CurrentCompany.CompanyName));
                parameters.Add(new ReportParameter("CustomerIdentification", SessionModel.CurrentCompany.IdentificationNumber));
                parameters.Add(new ReportParameter("CustomerIdentificationType", SessionModel.CurrentCompany.IdentificationType.ItemName));
                parameters.Add(new ReportParameter("CustomerImage", SessionModel.CurrentCompany_CompanyLogo));

                //Query Info Parameters
                parameters.Add(new ReportParameter("ThirdKnowledgeText", MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.MP_TK_TextImage].Value));
                parameters.Add(new ReportParameter("User", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.User != null).Select(x => x.User).DefaultIfEmpty("No hay campo").FirstOrDefault()));
                parameters.Add(new ReportParameter("CreateDate", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.CreateDate != null).Select(x => x.CreateDate.AddHours(-5).ToString().ToString()).DefaultIfEmpty("No hay campo").FirstOrDefault()));
                parameters.Add(new ReportParameter("QueryType", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.SearchType != null).Select(x => x.SearchType.ItemName).DefaultIfEmpty("No hay campo").FirstOrDefault()));
                parameters.Add(new ReportParameter("Status", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x.QueryStatus != null).Select(x => x.QueryStatus.ItemName).DefaultIfEmpty("No hay campo").FirstOrDefault()));
                parameters.Add(new ReportParameter("searchName", searchName));
                parameters.Add(new ReportParameter("searchIdentification", searchIdentification));
                parameters.Add(new ReportParameter("IsSuccess", oModel.RelatedThidKnowledgeSearch.ThirdKnowledgeResult.Where(x => x != null).Select(x => x.IsSuccess).FirstOrDefault().ToString()));

                /*data for Matches with High Critical*/
                DataTable data_HighCritical = new DataTable();
                data_HighCritical.Columns.Add("IdentificationResult");
                data_HighCritical.Columns.Add("NameResult");
                data_HighCritical.Columns.Add("Offense");
                data_HighCritical.Columns.Add("Peps");
                data_HighCritical.Columns.Add("Priority");
                data_HighCritical.Columns.Add("Status");
                data_HighCritical.Columns.Add("ListName");
                data_HighCritical.Columns.Add("IdentificationSearch");
                data_HighCritical.Columns.Add("NameSearch");
                DataRow row_HighCrit;
                var lrs = new List<ThirdKnowledgeViewModel>();
                oModel.Group.All(x =>
                {
                    if (x.Item1.Contains("Criticidad Alta"))
                    {
                        lrs.AddRange(x.Item2);
                    }
                    return true;
                });

                if (lrs != null)
                    lrs.All(y =>
                    {
                        row_HighCrit = data_HighCritical.NewRow();
                        row_HighCrit["IdentificationResult"] = y.IdentificationNumberResult;
                        row_HighCrit["NameResult"] = y.NameResult;
                        row_HighCrit["Offense"] = y.Offense;
                        row_HighCrit["Peps"] = y.Peps;
                        row_HighCrit["Priority"] = y.Priority;
                        row_HighCrit["Status"] = y.Status.ToLower() == "true" ? "Activo" : "Inactivo";
                        row_HighCrit["ListName"] = y.ListName;
                        row_HighCrit["IdentificationSearch"] = y.IdNumberRequest; // SearchId Param
                        row_HighCrit["NameSearch"] = y.RequestName; // SearchName Param
                        data_HighCritical.Rows.Add(row_HighCrit);
                        return true;
                    });

                /*data for Matches with Medium Critical*/
                DataTable data_MediumCritical = new DataTable();
                data_MediumCritical.Columns.Add("IdentificationResult");
                data_MediumCritical.Columns.Add("NameResult");
                data_MediumCritical.Columns.Add("Offense");
                data_MediumCritical.Columns.Add("Peps");
                data_MediumCritical.Columns.Add("Priority");
                data_MediumCritical.Columns.Add("Status");
                data_MediumCritical.Columns.Add("ListName");
                data_MediumCritical.Columns.Add("IdentificationSearch");
                data_MediumCritical.Columns.Add("NameSearch");
                DataRow row_MediumCrit;
                var dce = new List<ThirdKnowledgeViewModel>();
                oModel.Group.All(x =>
                {
                    if (x.Item1.Contains("Criticidad Media"))
                    {
                        dce.AddRange(x.Item2);
                    }
                    return true;
                });
                if (dce != null)
                    dce.All(y =>
                    {
                        row_MediumCrit = data_MediumCritical.NewRow();
                        row_MediumCrit["IdentificationResult"] = y.IdentificationNumberResult;
                        parameters.Add(new ReportParameter("GroupNameDce", y.GroupName));
                        row_MediumCrit["NameResult"] = y.NameResult;
                        row_MediumCrit["Offense"] = y.Offense;
                        row_MediumCrit["Peps"] = y.Peps;
                        row_MediumCrit["Priority"] = y.Priority;
                        row_MediumCrit["Status"] = y.Status.ToLower() == "true" ? "Activo" : "Inactivo";
                        row_MediumCrit["ListName"] = y.ListName;
                        row_MediumCrit["IdentificationSearch"] = y.IdNumberRequest; // SearchId Param
                        row_MediumCrit["NameSearch"] = y.RequestName; // SearchName Param
                        data_MediumCritical.Rows.Add(row_MediumCrit);
                        return true;
                    });

                /*data for Matches with Low Critical*/
                DataTable data_LowCritical = new DataTable();
                data_LowCritical.Columns.Add("IdentificationResult");
                data_LowCritical.Columns.Add("NameResult");
                data_LowCritical.Columns.Add("Offense");
                data_LowCritical.Columns.Add("Peps");
                data_LowCritical.Columns.Add("Priority");
                data_LowCritical.Columns.Add("Status");
                data_LowCritical.Columns.Add("ListName");
                data_LowCritical.Columns.Add("IdentificationSearch");
                data_LowCritical.Columns.Add("NameSearch");
                DataRow row_LowCrit;
                var psp = new List<ThirdKnowledgeViewModel>();
                oModel.Group.All(x =>
                {
                    if (x.Item1.Contains("Criticidad Baja"))
                    {
                        psp.AddRange(x.Item2);
                    }
                    return true;
                });
                if (psp != null)
                    psp.All(y =>
                    {
                        row_LowCrit = data_LowCritical.NewRow();
                        row_LowCrit["IdentificationResult"] = y.IdentificationNumberResult;
                        row_LowCrit["NameResult"] = y.NameResult;
                        row_LowCrit["Offense"] = y.Offense;
                        row_LowCrit["Peps"] = y.Peps;
                        row_LowCrit["Priority"] = y.Priority;
                        row_LowCrit["Status"] = y.Status.ToLower() == "true" ? "Activo" : "Inactivo";
                        row_LowCrit["ListName"] = y.ListName;
                        row_LowCrit["IdentificationSearch"] = y.IdNumberRequest; // SearchId Param
                        row_LowCrit["NameSearch"] = y.RequestName; // SearchName Param
                        data_LowCritical.Rows.Add(row_LowCrit);
                        return true;
                    });


                /*data for No Match Results*/
                DataTable data_NoMatch = new DataTable();
                data_NoMatch.Columns.Add("IdentificationResult");
                data_NoMatch.Columns.Add("NameResult");
                data_NoMatch.Columns.Add("IdentificationSearch");
                data_NoMatch.Columns.Add("NameSearch");
                DataRow row_NoMatch;
                List<ThirdKnowledgeViewModel> snc = oModel.Group.Where(x => x.Item1.Contains("SIN COINCIDENCIAS")).Select(x => x.Item2).FirstOrDefault();
                if (snc != null)
                    snc.All(y =>
                    {
                        row_NoMatch = data_NoMatch.NewRow();
                        row_NoMatch["IdentificationSearch"] = y.IdNumberRequest; // SearchId Param
                        row_NoMatch["NameSearch"] = y.NameResult; // SearchName Param
                        row_NoMatch["IdentificationResult"] = y.IdNumberRequest;
                        row_NoMatch["NameResult"] = y.RequestName;

                        data_NoMatch.Rows.Add(row_NoMatch);
                        return true;
                    });
                string fileFormat = Request["ThirdKnowledge_cmbFormat"] != null ? Request["ThirdKnowledge_cmbFormat"].ToString() : "pdf";
                Tuple<byte[], string, string> ThirdKnowledgeReport = ProveedoresOnLine.Reports.Controller.ReportModule.TK_QueryReport(
                                                                fileFormat,
                                                                data_HighCritical,
                                                                data_MediumCritical,
                                                                data_LowCritical,
                                                                data_NoMatch,
                                                                parameters,
                                                                Models.General.InternalSettings.Instance[Models.General.Constants.MP_CP_ReportPath].Value.Trim() + "TK_Report_ThirdKnowledgeQueryNew.rdlc");
                parameters = null;
                return File(ThirdKnowledgeReport.Item1, ThirdKnowledgeReport.Item2, ThirdKnowledgeReport.Item3);

                #endregion
            }

            return View(oModel);
        }

        #region Menu

        private List<GenericMenu> GetThirdKnowledgeControllerMenu()
        {
            List<GenericMenu> oReturn = new List<GenericMenu>();

            string oCurrentController = MarketPlace.Web.Controllers.BaseController.CurrentControllerName;
            string oCurrentAction = MarketPlace.Web.Controllers.BaseController.CurrentActionName;
            List<int> oCurrentSurveySubMenu = SessionModel.CurrentThirdknowledgeOption();

            #region Menu Usuario

            MarketPlace.Models.General.GenericMenu oMenuAux = new GenericMenu();
            if (oCurrentSurveySubMenu != null && oCurrentSurveySubMenu.Count > 0)
            {
                //header
                oMenuAux = new GenericMenu()
                {
                    Name = "Menú Usuario",
                    Position = 0,
                    ChildMenu = new List<GenericMenu>(),
                };
                if (oCurrentSurveySubMenu.Any(y => y == (int)enumProviderSubMenu.IndividualQuery))
                {


                    //Consulta individual
                    oMenuAux.ChildMenu.Add(new GenericMenu()
                    {
                        Name = "Consulta Individual",
                        Url = Url.RouteUrl
                                (MarketPlace.Models.General.Constants.C_Routes_Default,
                                new
                                {
                                    controller = MVC.ThirdKnowledge.Name,
                                    action = MVC.ThirdKnowledge.ActionNames.TKSingleSearch
                                }),
                        Position = 0,
                        IsSelected =
                            (oCurrentAction == MVC.ThirdKnowledge.ActionNames.TKSingleSearch &&
                            oCurrentController == MVC.ThirdKnowledge.Name)
                    });
                }

                if (oCurrentSurveySubMenu.Any(y => y == (int)enumProviderSubMenu.MasiveQuery))
                {
                    //Consulta masiva
                    oMenuAux.ChildMenu.Add(new GenericMenu()
                    {
                        Name = "Consulta Masiva",
                        Url = Url.RouteUrl
                            (MarketPlace.Models.General.Constants.C_Routes_Default,
                            new
                            {
                                controller = MVC.ThirdKnowledge.Name,
                                action = MVC.ThirdKnowledge.ActionNames.TKMasiveSearch
                            }),
                        Position = 1,
                        IsSelected =
                        (oCurrentAction == MVC.ThirdKnowledge.ActionNames.TKMasiveSearch &&
                        oCurrentController == MVC.ThirdKnowledge.Name)
                    });
                }

                if (oCurrentSurveySubMenu.Any(y => y == (int)enumProviderSubMenu.MyQueries))
                {
                    //Mis Consultas
                    oMenuAux.ChildMenu.Add(new GenericMenu()
                    {
                        Name = "Mis Consultas",
                        Url = Url.RouteUrl
                            (MarketPlace.Models.General.Constants.C_Routes_Default,
                            new
                            {
                                controller = MVC.ThirdKnowledge.Name,
                                action = MVC.ThirdKnowledge.ActionNames.TKThirdKnowledgeSearch
                            }),
                        Position = 2,
                        IsSelected =
                        (oCurrentAction == MVC.ThirdKnowledge.ActionNames.TKThirdKnowledgeSearch &&
                        oCurrentController == MVC.ThirdKnowledge.Name)
                    });
                }
                #endregion

                //get is selected menu
                oMenuAux.IsSelected = oMenuAux.ChildMenu.Any(x => x.IsSelected);

                //add menu
                oReturn.Add(oMenuAux);
            }
            return oReturn;
        }

        #endregion

        #region Private functions

        public List<Tuple<string, string, string, List<string>, bool>> TK_CreateObjectReport(List<TDQueryModel> oQueryResult)
        {
            List<string> SancionedList = MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.TK_ListToValidateSancioned].Value.Split(';').ToList();
            List<string> PepList = MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.TK_ListToValidatePEP].Value.Split(';').ToList();
            List<string> GeneralList = MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.TK_ListToValidateGeneralInfo].Value.Split(';').ToList();

            List<Tuple<string, string, string, List<string>, bool>> oReturn = new List<Tuple<string, string, string, List<string>, bool>>();
            if (oQueryResult != null)
            {
                oQueryResult.All(x =>
                {
                    #region GeneralInfo                                
                    List<string> oDetails = new List<string>();
                    if (x.RelatedQueryInfoModel != null && !string.IsNullOrEmpty(x.RelatedQueryInfoModel.Where(y => y.DocumentType != null).Select(y => y.DocumentType).FirstOrDefault()))
                    {
                        int IdType = int.Parse(x.RelatedQueryInfoModel.Where(y => y.DocumentType != null).Select(y => y.DocumentType).FirstOrDefault());
                        //GetName
                        if (IdType == 2 && x.RelatedQueryInfoModel.Where(p => p.ListName == GeneralList[2]).Select(p => p.NameResult).FirstOrDefault() != null)
                        {
                            oDetails.Add(x.RelatedQueryInfoModel.Where(p => p.ListName == GeneralList[2]).Select(p => p.NameResult).FirstOrDefault());
                            oDetails.Add(x.RelatedQueryInfoModel.Where(p => p.ListName == GeneralList[2]).Select(p => p.ElasticId.ToString()).FirstOrDefault());
                        }

                        else if (IdType == 1 && x.RelatedQueryInfoModel.Where(p => p.ListName == GeneralList[0]).Select(p => p.NameResult).FirstOrDefault() != null)
                        {
                            oDetails.Add(x.RelatedQueryInfoModel.Where(p => p.ListName == GeneralList[0]).Select(p => p.NameResult).FirstOrDefault());
                            oDetails.Add(x.RelatedQueryInfoModel.Where(p => p.ListName == GeneralList[0]).Select(p => p.ElasticId.ToString()).FirstOrDefault());
                        }

                        oDetails.Add(x.QueryPublicId);
                        Tuple<string, string, string, List<string>, bool> oDetail = new
                                    Tuple<string, string, string, List<string>, bool>("INFORMACIÓN BÁSICA",
                                        IdType == 2 ? GeneralList[2] : GeneralList[1],
                                        IdType == 2 ? x.RelatedQueryInfoModel.Where(p => p.ListName == GeneralList[2]).Select(p => p.Link).FirstOrDefault() :
                                        IdType == 1 ? x.RelatedQueryInfoModel.Where(p => p.ListName == GeneralList[1]).Select(p => p.Link).FirstOrDefault() : "", oDetails,
                                                    oDetails.Count > 0 ? true : false);
                        oReturn.Add(oDetail);
                    }

                    #endregion

                    #region Sancioned Group List
                    SancionedList.All(sl =>
                    {
                        oDetails = new List<string>();
                        bool exist = false;
                        if (x.RelatedQueryInfoModel.Where(y => y.ListName == sl).Select(y => y).FirstOrDefault() != null)
                        {
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == sl).Select(y => y).FirstOrDefault().NameResult);
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == sl).Select(y => y).FirstOrDefault().IdentificationResult);
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == sl).Select(y => y).FirstOrDefault().QueryInfoPublicId);
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == sl).Select(y => y).FirstOrDefault().QueryPublicId);
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == sl).Select(y => y).FirstOrDefault().ElasticId.ToString());
                            exist = true;
                        }
                        else
                            exist = false;
                        oDetails.Add(x.QueryPublicId);

                        Tuple<string, string, string, List<string>, bool> oDetail = new
                               Tuple<string, string, string, List<string>, bool>("LISTAS RESTRICTIVAS, SANCIONES NACIONALES E INTERNACIONALES",
                                   sl, "", oDetails, exist);
                        oReturn.Add(oDetail);
                        return true;
                    });
                    #endregion

                    #region Peps
                    PepList.All(pep =>
                    {
                        oDetails = new List<string>();
                        bool exist = false;
                        if (x.RelatedQueryInfoModel.Where(y => y.ListName == pep).Select(y => y).FirstOrDefault() != null)
                        {
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == pep).Select(y => y).FirstOrDefault().NameResult);
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == pep).Select(y => y).FirstOrDefault().IdentificationResult);
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == pep).Select(y => y).FirstOrDefault().QueryInfoPublicId);
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == pep).Select(y => y).FirstOrDefault().QueryPublicId);
                            oDetails.Add(x.RelatedQueryInfoModel.Where(y => y.ListName == pep).Select(y => y).FirstOrDefault().ElasticId.ToString());
                            exist = true;
                        }
                        else
                            exist = false;
                        oDetails.Add(x.QueryPublicId);                        
                        Tuple<string, string, string, List<string>, bool> oDetail = new
                               Tuple<string, string, string, List<string>, bool>("PEPS -  PERSONAS POLITICAMENTE Y PUBLICAMENTE EXPUESTAS",
                                   pep, "", oDetails, exist);
                        oReturn.Add(oDetail);
                        return true;
                    });
                    #endregion
                    return true;
                });
            }

            return null;
        }

        #endregion
    }
}