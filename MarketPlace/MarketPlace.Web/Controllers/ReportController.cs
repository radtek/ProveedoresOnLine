﻿using MarketPlace.Models.Compare;
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
                byte[] buffer = null;
                StringBuilder data = new StringBuilder();
                List<ProveedoresOnLine.SurveyModule.Models.SurveyModel> surveyByProvider = ProveedoresOnLine.Reports.Controller.ReportModule.SurveyGetAllByCustomer(SessionModel.CurrentCompany.CompanyPublicId);
                string strSep = ";";
                data.AppendLine
                        (
                        "\"" + "TIPO EVALUACION" + "\"" + strSep +
                        "\"" + "RESPONSABLE" + "\"" + strSep +
                        "\"" + "PROVEEDOR" + "\"" + strSep +
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
                if(surveyByProvider != null && surveyByProvider.Count > 0)
                {
                    surveyByProvider.All(x =>
                    {
                        oModel.RelatedSurvey = new SurveyViewModel(ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetById(x.SurveyPublicId));

                        if (x != null && x.SurveyInfo.Count > 0)
                        {
                            data.AppendLine
                            (
                            "\"" + x.RelatedSurveyConfig.ItemName.ToString() + "\"" + strSep +
                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                            "\"" + x.RelatedProvider.RelatedCompany.CompanyName + "\"" + "" + strSep +
                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Project).Select(a => a.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(a => a.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                            "\"" + "N/D" + "\"" + "" + strSep +
                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Rating).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                            "\"" + x.LastModify.ToString("dd/MM/yyyy") + "\"" + "" + strSep +
                            "\"" + "N/D" + "\"" + "" + strSep +
                            "\"" + "N/D" + "\"" + "" + strSep +
                            "\"" + "N/D" + "\"" + "" + strSep +
                            "\"" + "N/D" + "\"" + "" + strSep +
                            "\"" + "N/D" + "\""

                            );
                            string area = "";
                            string detallearea = "";
                            string preguntas = "";
                            string respuestas = "";
                            string detallerespuesta = "";
                            if (x.ChildSurvey != null && x.ChildSurvey.Count > 0)
                            {
                                //RECORRO POR EVALUADORES
                                x.ChildSurvey.All(y =>
                                {
                                    if (y != null && y.ParentSurveyPublicId == x.SurveyPublicId)
                                    {
                                        /* obtenga las areas asignadas al evaluador*/
                                        y.RelatedSurveyConfig.RelatedSurveyConfigItem.All(o =>
                                        {
                                            if (o.ItemType.ItemId == (int)enumSurveyConfigItemType.EvaluationArea) {
                                                /*obtengo el area*/
                                                        area =
                                                          "\"" + x.RelatedSurveyConfig.ItemName.ToString() + "\"" + strSep +
                                                          "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                          "\"" + x.RelatedProvider.RelatedCompany.CompanyName + "\"" + "" + strSep +
                                                          "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Project).Select(a => a.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                          "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(b => b.Value).DefaultIfEmpty("").FirstOrDefault() + "\"" + "" + strSep +
                                                          "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(b => b.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                          "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                          "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                          /*obtengo los evaluadores*/
                                                          "\"" + y.User.ToString() + "\"" + strSep +
                                                          "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Rating).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                          "\"" + x.LastModify.ToString("dd/MM/yyyy") + "\"" + "" + strSep +
                                                          /*obtengo el area*/
                                                          "\"" + o.ItemName.ToString() + "\"" + "" + strSep +
                                                          "\"" + "N/D" + "\"" + "" + strSep +
                                                          "\"" + "N/D" + "\"" + "" + strSep +
                                                          "\"" + "N/D" + "\"" + "" + strSep +
                                                          "\"" + "N/D" + "\"";
                                                data.AppendLine(area);
                                                /*Obtengo la descripcion del area*/
                                                y.RelatedSurveyItem.Where(r => r.RelatedSurveyConfigItem.ItemId == o.ItemId).All(z=> {
                                                    z.ItemInfo.All(m=> {
                                                        if (m.ItemInfoType!= null && m.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.Answer) {
                                                            /*obtengo la descripcion del area*/
                                                            detallearea =
                                                                "\"" + x.RelatedSurveyConfig.ItemName.ToString() + "\"" + strSep +
                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                "\"" + x.RelatedProvider.RelatedCompany.CompanyName + "\"" + "" + strSep +
                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Project).Select(a => a.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(b => b.Value).DefaultIfEmpty("").FirstOrDefault() + "\"" + "" + strSep +
                                                                "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(b => b.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                /*obtengo los evaluadores*/
                                                                "\"" + y.User.ToString() + "\"" + strSep +
                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Rating).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                "\"" + x.LastModify.ToString("dd/MM/yyyy") + "\"" + "" + strSep +
                                                                /*obtengo el area*/
                                                                "\"" + o.ItemName.ToString() + "\"" + "" + strSep +
                                                                "\"" + m.Value + "\"" + "" + strSep +
                                                                "\"" + "N/D" + "\"" + "" + strSep +
                                                                "\"" + "N/D" + "\"" + "" + strSep +
                                                                "\"" + "N/D" + "\"";
                                                                data.AppendLine(detallearea);

                                                        }
                                                        return true;
                                                    });
                                                    return true;
                                                });
                                                /* obtengo las preguntas del area */
                                                y.RelatedSurveyConfig.RelatedSurveyConfigItem.All(p => {
                                                    if (p.ParentItem != null && p.ParentItem.ItemId == o.ItemId && p.ItemType.ItemId == (int)enumSurveyConfigItemType.Question)
                                                    {
                                                        /*preguntas del area*/
                                                        preguntas =
                                                            "\"" + x.RelatedSurveyConfig.ItemName.ToString() + "\"" + strSep +
                                                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                            "\"" + x.RelatedProvider.RelatedCompany.CompanyName + "\"" + "" + strSep +
                                                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Project).Select(a => a.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                            "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(b => b.Value).DefaultIfEmpty("").FirstOrDefault() + "\"" + "" + strSep +
                                                            "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(b => b.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                            /*obtengo los evaluadores*/
                                                            "\"" + y.User.ToString() + "\"" + strSep +
                                                            "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Rating).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                            "\"" + x.LastModify.ToString("dd/MM/yyyy") + "\"" + "" + strSep +
                                                            /*obtengo el area*/
                                                            "\"" + o.ItemName.ToString() + "\"" + "" + strSep +
                                                            "\"" + "N/D" + "\"" + "" + strSep +
                                                            "\"" + p.ItemName.ToString() + "\"" + "" + strSep +
                                                            "\"" + "N/D" + "\"" + "" + strSep +
                                                            "\"" + "N/D" + "\"";
                                                            data.AppendLine(preguntas);

                                                        /* obtengo las respuestas de las preguntas del area */
                                                        y.RelatedSurveyConfig.RelatedSurveyConfigItem.All(q => {
                                                            if (q.ParentItem != null && q.ParentItem.ItemId == p.ItemId && q.ItemType.ItemId == (int)enumSurveyConfigItemType.Answer)
                                                            {
                                                                /*respuestas de la pregunta*/
                                                                respuestas =
                                                                    "\"" + x.RelatedSurveyConfig.ItemName.ToString() + "\"" + strSep +
                                                                    "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                    "\"" + x.RelatedProvider.RelatedCompany.CompanyName + "\"" + "" + strSep +
                                                                    "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Project).Select(a => a.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                    "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(b => b.Value).DefaultIfEmpty("").FirstOrDefault() + "\"" + "" + strSep +
                                                                    "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(b => b.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                    "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                    "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                    /*obtengo los evaluadores*/
                                                                    "\"" + y.User.ToString() + "\"" + strSep +
                                                                    "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Rating).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                    "\"" + x.LastModify.ToString("dd/MM/yyyy") + "\"" + "" + strSep +
                                                                    /*obtengo el area*/
                                                                    "\"" + o.ItemName.ToString() + "\"" + "" + strSep +
                                                                    "\"" + "N/D" + "\"" + "" + strSep +
                                                                    "\"" + p.ItemName.ToString() + "\"" + "" + strSep +
                                                                    "\"" + q.ItemName.ToString() + "\"" + "" + strSep +
                                                                    "\"" + "N/D" + "\"";
                                                                    data.AppendLine(respuestas);
                                                                /*Obtengo la descripcion de la respuesta*/
                                                                y.RelatedSurveyItem.Where(r => r.RelatedSurveyConfigItem.ItemId == p.ItemId).All(z => {

                                                                    z.ItemInfo.All(m => {
                                                                        if (m.ItemInfoType != null && m.ItemInfoType.ItemId == (int)enumSurveyItemInfoType.DescriptionText)
                                                                        {
                                                                            /*obtengo la descripcion del area*/
                                                                            detallerespuesta =
                                                                                "\"" + x.RelatedSurveyConfig.ItemName.ToString() + "\"" + strSep +
                                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Responsible).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                                "\"" + x.RelatedProvider.RelatedCompany.CompanyName + "\"" + "" + strSep +
                                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Project).Select(a => a.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                                "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Comments).Select(b => b.Value).DefaultIfEmpty("").FirstOrDefault() + "\"" + "" + strSep +
                                                                                "\"" + y.SurveyInfo.Where(b => b.ItemInfoType.ItemId == (int)enumSurveyInfoType.Status).Select(b => b.ValueName).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.IssueDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.ExpirationDate).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                                /*obtengo los evaluadores*/
                                                                                "\"" + y.User.ToString() + "\"" + strSep +
                                                                                "\"" + x.SurveyInfo.Where(a => a.ItemInfoType.ItemId == (int)enumSurveyInfoType.Rating).Select(a => a.Value).DefaultIfEmpty("").FirstOrDefault().ToString() + "\"" + "" + strSep +
                                                                                "\"" + x.LastModify.ToString("dd/MM/yyyy") + "\"" + "" + strSep +
                                                                                /*obtengo el area*/
                                                                                "\"" + o.ItemName.ToString() + "\"" + "" + strSep +
                                                                                "\"" + "N/D" + "\"" + "" + strSep +
                                                                                "\"" + "N/D" + "\"" + "" + strSep +
                                                                                "\"" + "N/D" + "\"" + "" + strSep +
                                                                                "\"" + m.Value + "\"";
                                                                            data.AppendLine(detallerespuesta);

                                                                        }
                                                                        return true;
                                                                    });

                                                                    return true;
                                                                });
                                                                }
                                                            return true;
                                                        });
                                                    }
                                                    return true;
                                                });
                                            }
                                            return true;
                                        });
                                    }
                                    return true;
                                });
                            }
                        }
                        return true;
                    });
                }
                



                buffer = Encoding.ASCII.GetBytes(data.ToString().ToCharArray());
                return File(buffer, "application/csv", "InformacionGeneral_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");
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

            foreach (var module in MarketPlace.Models.General.SessionModel.CurrentUserModules())
            {
                MarketPlace.Models.General.GenericMenu oMenuAux = new GenericMenu();
                /*
                if (module == (int)MarketPlace.Models.General.enumMarketPlaceCustomerModules.ProviderDetail)
                {
                    #region Provider Report Menu
                    
                    //header
                    oMenuAux = new GenericMenu()
                    {
                        Name = "Proveedores",
                        Position = 0,
                        ChildMenu = new List<GenericMenu>(),
                    };

                    //Gerencial
                    oMenuAux.ChildMenu.Add(new GenericMenu()
                    {
                        Name = "Informe Gerencial",
                        Url = Url.RouteUrl
                                (MarketPlace.Models.General.Constants.C_Routes_Default,
                                new
                                {
                                    controller = MVC.Report.Name,
                                    action = MVC.Report.ActionNames.PRGeneral
                                }),
                        Position = 0,
                        IsSelected =
                            (oCurrentAction == MVC.Report.ActionNames.PRGeneral &&
                            oCurrentController == MVC.Report.Name)
                    });

                    oMenuAux.IsSelected = oMenuAux.ChildMenu.Any(x => x.IsSelected);

                    oReturn.Add(oMenuAux);
                    
                    #endregion
                }*/
                if (module == (int)MarketPlace.Models.General.enumMarketPlaceCustomerModules.ProviderRatingCreate)
                {
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
            }

            #endregion

            return oReturn;
        }

        #endregion
    }
}