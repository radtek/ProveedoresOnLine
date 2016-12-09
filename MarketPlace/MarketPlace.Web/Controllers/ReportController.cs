using MarketPlace.Models.Compare;
using MarketPlace.Models.General;
using MarketPlace.Models.Provider;
using MarketPlace.Models.Survey;
using ProveedoresOnLine.Company.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MarketPlace.Web.Controllers
{
    public partial class ReportController : BaseController
    {
        public virtual ActionResult Index()
        {
            //Clean the season url saved
            if (MarketPlace.Models.General.SessionModel.CurrentURL != null)
            {
                MarketPlace.Models.General.SessionModel.CurrentURL = null;
            }
            return View();
        }

        public virtual ActionResult PRGeneral()
        {
            ProviderViewModel oModel = new ProviderViewModel();
            oModel.ProviderMenu = GetReportControllerMenu();

            //Clean the season url saved
            if (MarketPlace.Models.General.SessionModel.CurrentURL != null)
                MarketPlace.Models.General.SessionModel.CurrentURL = null;

            return View(oModel);
        }

        public virtual ActionResult RP_SV_SurveyGeneralInfoReport()
        {
            ProviderViewModel oModel = new ProviderViewModel();
            oModel.ProviderMenu = GetReportControllerMenu();

            //Clean the season url saved
            if (MarketPlace.Models.General.SessionModel.CurrentURL != null)
                MarketPlace.Models.General.SessionModel.CurrentURL = null;

            if (Request["SurveyGeneralInfoReport"] == "True")
            {
                #region GeneralSurveyReport
                byte[] buffer = null;

                StringBuilder data = new StringBuilder();
                var surveyByProvider = ProveedoresOnLine.Reports.Controller.ReportModule.ReportSurveyAllbyCustomer(SessionModel.CurrentCompany.CompanyPublicId);
                string strSep = ";";
                data.AppendLine
                        (
                        "\"" + "TIPO EVALUACION" + "\"" + strSep +
                        "\"" + "RESPONSABLE" + "\"" + strSep +
                        "\"" + "PROVEEDOR" + "\"" + strSep +
                        "\"" + "NIT" + "\"" + strSep +
                        "\"" + "PROYECTO" + "\"" + strSep +
                        "\"" + "OBSERVACIONES" + "\"" + strSep +
                        "\"" + "ESTADO" + "\"" + strSep +
                        "\"" + "FECHA DE ENVIO" + "\"" + strSep +
                        "\"" + "FECHA DE CADUCIDAD" + "\"" + strSep +
                        "\"" + "EVALUADORES" + "\"" + strSep +
                        "\"" + "CALIFICACION" + "\"" + strSep +
                        "\"" + "ULTIMA MODIFICACION" + "\"" + strSep +
                        "\"" + "AREA" + "\"" + strSep +
                        "\"" + "COMENTARIO AREA" + "\"" + strSep +
                        "\"" + "PREGUNTAS" + "\"" + strSep +
                        "\"" + "RESPUESTAS" + "\"" + strSep +
                        "\"" + "COMENTARIO RESPUESTA" + "\"");

                if (surveyByProvider != null && surveyByProvider.Count > 0)
                {
                    surveyByProvider.All(x =>
                    {
                        //Info Parent
                        if (x != null && x.Item1.SurveyInfo.Count > 0)
                        {
                            data.AppendLine
                            (
                                "\"" + x.Item1.RelatedSurveyConfig.ItemName.ToString() + "\"" + strSep +
                                "\"" + x.Item1.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                "\"" + x.Item1.RelatedProvider.RelatedCompany.CompanyName + "\"" + "" + strSep +
                                "\"" + x.Item1.RelatedProvider.RelatedCompany.IdentificationNumber + "\"" + "" + strSep +
                                "\"" + "N/D" + "\"" + "" + strSep +
                                "\"" + x.Item1.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                "\"" + x.Item1.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(a => a.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                "\"" + x.Item1.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                "\"" + x.Item1.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                "\"" + "N/D" + "\"" + "" + strSep +
                                "\"" + x.Item1.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Rating).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                "\"" + x.Item1.LastModify.ToString("dd/MM/yyyy") + "\"" + "" + strSep +
                                "\"" + "N/D" + "\"" + "" + strSep +
                                "\"" + "N/D" + "\"" + "" + strSep +
                                "\"" + "N/D" + "\"" + "" + strSep +
                                "\"" + "N/D" + "\"" + "" + strSep +
                                "\"" + "N/D" + "\""
                            );
                            /* survey */
                            if (x.Item1.ChildSurvey != null)
                            {
                                x.Item3.All(
                                    q => {
                                        string QuestionName = x.Item4 != null ? x.Item4.Where(re => re != null && re.ParentItem.ItemId == q.Item2.ItemId).Select(re => re.ItemName).DefaultIfEmpty("").FirstOrDefault() : "N/D";
                                        string QuestionDesc = "N/D";
                                        data.AppendLine
                                        (
                                            "\"" + x.Item1.RelatedSurveyConfig.ItemName.ToString() + "\"" + strSep +
                                            "\"" + x.Item1.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                            "\"" + x.Item1.RelatedProvider.RelatedCompany.CompanyName + "\"" + "" + strSep +
                                            "\"" + x.Item1.ChildSurvey.FirstOrDefault().RelatedProvider.RelatedCompany.IdentificationNumber + "\"" + "" + strSep +
                                            "\"" + q.Item1.SurveyInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumSurveyInfoType.Contract).Select(inf => inf.Value).DefaultIfEmpty("").FirstOrDefault() + "\"" + "" + strSep +
                                            "\"" + q.Item1.SurveyInfo.Where(k => k.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(k => k.Value).DefaultIfEmpty("").FirstOrDefault() + "\"" + "" + strSep +
                                            "\"" + q.Item1.SurveyInfo.Where(k => k.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(k => k.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                            "\"" + q.Item1.SurveyInfo.Where(j => j.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(j => j.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                            "\"" + q.Item1.SurveyInfo.Where(j => j.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(j => j.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                            /*get evaluator*/
                                            "\"" + q.Item1.User + "\"" + "" + strSep +
                                            "\"" + q.Item1.SurveyInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumSurveyInfoType.Rating).Select(inf => inf.Value).FirstOrDefault() + "\"" + "" + strSep +
                                            "\"" + x.Item1.ChildSurvey.FirstOrDefault().LastModify.ToString("dd/MM/yyyy") + "\"" + "" + strSep +
                                            /*get area name*/
                                            "\"" + x.Item2.Select(z => z.ItemName).DefaultIfEmpty("").FirstOrDefault() + "\"" + "" + strSep +
                                            "\"" + q.Item1.SurveyInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(inf => inf.Value).FirstOrDefault() + "\"" + "" + strSep +
                                            "\"" + q.Item2.ItemName + "\"" + "" + strSep +
                                            "\"" + QuestionName + "\"" + "" + strSep +
                                            "\"" + QuestionDesc + "\""
                                        );
                                        return true;
                                    });
                            }
                        }
                        return true;
                    });
                    buffer = Encoding.Default.GetBytes(data.ToString().ToCharArray());
                }
                return File(buffer, "application/csv", "InformacionGeneral_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");
                #endregion

            }
            return View(oModel);
        }

        #region Menu

        private List<GenericMenu> GetReportControllerMenu()
        {
            List<GenericMenu> oReturn = new List<GenericMenu>();

            string oCurrentController = MarketPlace.Web.Controllers.BaseController.CurrentControllerName;
            string oCurrentAction = MarketPlace.Web.Controllers.BaseController.CurrentActionName;

            #region Reports

            List<int> oCurrentModule = MarketPlace.Models.General.SessionModel.CurrentUserModules();
            List<int> oCurrentMenu = MarketPlace.Models.General.SessionModel.CurrentProviderMenu();

            if (oCurrentModule.Any(x => x == (int)MarketPlace.Models.General.enumModule.ReportsInfo) &&
                oCurrentMenu.Any(x => x == (int)MarketPlace.Models.General.enumProviderMenu.Survey))
            {
                MarketPlace.Models.General.GenericMenu oMenuAux = new GenericMenu();

                #region Survey Report
                //header
                oMenuAux = new Models.General.GenericMenu()
                {
                    Name = "Evaluación de Desempeño",
                    Position = 0,
                    ChildMenu = new List<Models.General.GenericMenu>(),
                };

                //Información General
                oMenuAux.ChildMenu.Add(new Models.General.GenericMenu()
                {
                    Name = "Información General",
                    Url = Url.RouteUrl
                            (MarketPlace.Models.General.Constants.C_Routes_Default,
                            new
                            {
                                controller = MVC.Report.Name,
                                action = MVC.Report.ActionNames.RP_SV_SurveyGeneralInfoReport,
                            }),
                    Position = 0,
                    IsSelected =
                        (oCurrentAction == MVC.Report.ActionNames.RP_SV_SurveyGeneralInfoReport &&
                        oCurrentController == MVC.Report.Name),
                });
                //get is selected menu
                oMenuAux.IsSelected = oMenuAux.ChildMenu.Any(x => x.IsSelected);

                //add menu
                oReturn.Add(oMenuAux);
                #endregion
            }

            #endregion

            return oReturn;
        }

        #endregion
    }
}