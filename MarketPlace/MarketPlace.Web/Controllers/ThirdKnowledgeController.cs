using MarketPlace.Models.Company;
using MarketPlace.Models.Compare;
using MarketPlace.Models.General;
using MarketPlace.Models.Provider;
using MarketPlace.Models.ThirdKnowledge;
using Microsoft.Reporting.WebForms;
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
        public virtual ActionResult TKDetailSingleSearch(string QueryBasicPublicId, string ElasticId, string ReturnUrl)
        {
            ProviderViewModel oModel = new ProviderViewModel();

            TDQueryInfoModel QueryDetailInfo = new TDQueryInfoModel();
            try
            {
                oModel.ProviderMenu = GetThirdKnowledgeControllerMenu();
                //Clean the season url saved
                if (SessionModel.CurrentURL != null)
                    SessionModel.CurrentURL = null;

                int oTotalRows = 0;

                //Get The Active Plan By Customer 
                QueryDetailInfo = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetQueryInfoByQueryPublicIdAndElasticId(QueryBasicPublicId, Convert.ToInt32(ElasticId));

                oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel(QueryDetailInfo);

                if (ReturnUrl == "null")
                    oModel.RelatedThidKnowledgeSearch.ReturnUrl = ReturnUrl;

                oModel.RelatedThidKnowledgeSearch.QueryBasicPublicId = QueryBasicPublicId;               

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

        public virtual ActionResult TKThirdKnowledgeSearch(string PageNumber, string InitDate, string EndDate, string SearchType, string User)
        {
            if (SessionModel.CurrentURL != null)
                SessionModel.CurrentURL = null;
            string RelatedUser = null;

            if (SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().RelatedCompanyRole.ParentRoleCompany != null)
            {
                RelatedUser = SessionModel.CurrentCompanyLoginUser.RelatedUser.Email;
            }
            else
            {
                RelatedUser = null;
            }
            if (User !=null)           
                RelatedUser = User;
            

            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();
            List<ProveedoresOnLine.ThirdKnowledge.Models.TDQueryModel> oQueryModel = new List<TDQueryModel>();

            List<PlanModel> oCurrentPeriodList = new List<PlanModel>();
            oCurrentPeriodList = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetCurrenPeriod(SessionModel.CurrentCompany.CompanyPublicId, true);
            if (oCurrentPeriodList != null && oCurrentPeriodList.Count > 0)
            {
                oModel.RelatedThidKnowledgeSearch.CurrentPlanModel = oCurrentPeriodList.OrderByDescending(x => x.CreateDate).First();
            }

            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager = new Models.ThirdKnowledge.ThirdKnowledgeSearchViewModel()
            {
                PageNumber = !string.IsNullOrEmpty(PageNumber) ? Convert.ToInt32(PageNumber) : 0,
            };
            int TotalRows = 0;
            oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.PageNumber = !string.IsNullOrEmpty(PageNumber) ? Convert.ToInt32(PageNumber) : 0;

            oQueryModel = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.ThirdKnowledgeSearch(
                SessionModel.CurrentCompany.CompanyPublicId,
                RelatedUser,
                !string.IsNullOrEmpty(InitDate) ? InitDate : "",
                !string.IsNullOrEmpty(EndDate) ? EndDate : "",
                oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.PageNumber,
                Convert.ToInt32(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_Grid_RowCountDefault].Value.Trim()),
                SearchType,
                null,
                out TotalRows);
            List<TDQueryInfoModel> objQueryInfo = new List<TDQueryInfoModel>();
            
            oQueryModel.All(x =>
            {
                objQueryInfo.Add(ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetQueryInfoByInfoPublicId(x.QueryPublicId));
                x.RelatedQueryInfoModel = new List<TDQueryInfoModel>(objQueryInfo);
                return true;
            });

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

            return View(oModel);
        }

        public virtual ActionResult TKThirdKnowledgeDetail(string QueryPublicId,  string PageNumber, string InitDate, string EndDate, string Enable, string IsSuccess)
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
            
            List<ProveedoresOnLine.ThirdKnowledge.Models.TDQueryModel> oQueryResult = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.ThirdKnowledgeSearchByPublicId
                (SessionModel.CurrentCompany.CompanyPublicId
                , QueryPublicId
                , Enable == "1" ? true : false
                , oModel.RelatedThidKnowledgeSearch.RelatedThidKnowledgePager.PageNumber
                , oTotalRowsAux
                , out TotalRows);

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
                                                                Models.General.InternalSettings.Instance[Models.General.Constants.MP_CP_ReportPath].Value.Trim() + "TK_Report_ThirdKnowledgeQuery.rdlc");
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
    }
}