using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProveedoresOnLine.Reports.Models.Reports;

namespace BackOffice.Web.Controllers
{
    public partial class ReportController : Controller
    {
        // GET: Report
        public virtual ActionResult Index()
        {
            BackOffice.Models.Provider.ProviderViewModel oModel = new Models.Provider.ProviderViewModel()
            {
                ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions(),
            };

            //get provider menu
            oModel.ProviderMenu = GetReportMenu(oModel);

            return View(oModel);
        }

        public virtual ActionResult Template()
        {
            BackOffice.Models.Provider.ProviderViewModel oModel = new Models.Provider.ProviderViewModel()
            {
                ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions(),
            };

            //get provider menu
            oModel.ProviderMenu = GetReportMenu(oModel);

            return View(oModel);
        }

        public virtual ActionResult GeneredReport()
        {
            BackOffice.Models.Provider.ProviderViewModel oModel = new Models.Provider.ProviderViewModel()
            {
                ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions(),
            };

            //get provider menu
            oModel.ProviderMenu = GetReportMenu(oModel);

            return View(oModel);
        }


        #region Menu

        private List<BackOffice.Models.General.GenericMenu> GetReportMenu
            (BackOffice.Models.Provider.ProviderViewModel vAdminInfo)
        {
            List<BackOffice.Models.General.GenericMenu> oReturn = new List<Models.General.GenericMenu>();

            //get current controller action
            string oCurrentController = BackOffice.Web.Controllers.BaseController.CurrentControllerName;
            string oCurrentAction = BackOffice.Web.Controllers.BaseController.CurrentActionName;

            #region Administration

            //header
            BackOffice.Models.General.GenericMenu oMenuAux = new Models.General.GenericMenu()
            {
                Name = "Reporte",
                Position = 0,
                ChildMenu = new List<Models.General.GenericMenu>(),
            };

            //My Templates
            oMenuAux.ChildMenu.Add(new Models.General.GenericMenu()
            {
                Name = "Mis Plantillas",
                Url = Url.Action
                    (MVC.Report.ActionNames.Index,
                    MVC.Report.Name),
                Position = 1,
                IsSelected =
                    (oCurrentAction == MVC.Report.ActionNames.Index &&
                    oCurrentController == MVC.Report.Name),
            });

            //Create Templates
            oMenuAux.ChildMenu.Add(new Models.General.GenericMenu()
            {
                Name = "Crear Plantilla",
                Url = Url.Action
                    (MVC.Report.ActionNames.Template,
                    MVC.Report.Name),
                Position = 2,
                IsSelected =
                    (oCurrentAction == MVC.Report.ActionNames.Template &&
                    oCurrentController == MVC.Report.Name),
            });

            //Generic Report
            oMenuAux.ChildMenu.Add(new Models.General.GenericMenu()
            {
                Name = "Generar Reporte",
                Url = Url.Action
                    (MVC.Report.ActionNames.GeneredReport,
                    MVC.Report.Name),
                Position = 3,
                IsSelected =
                    (oCurrentAction == MVC.Report.ActionNames.GeneredReport &&
                    oCurrentController == MVC.Report.Name),
            });

            oReturn.Add(oMenuAux);


            #endregion Administration

            BackOffice.Models.General.GenericMenu MenuAux = null;

            oReturn.OrderBy(x => x.Position).All(pm =>
            {
                pm.ChildMenu.OrderBy(x => x.Position).All(sm =>
                {
                    if (MenuAux != null)
                    {
                        MenuAux.NextMenu = sm;
                    }
                    sm.LastMenu = MenuAux;
                    MenuAux = sm;

                    return true;
                });

                return true;
            });

            return oReturn;
        }

        #endregion Menu

        public virtual ActionResult CC_Report_Upsert(string ReportId)
        {
            
            BackOffice.Models.Provider.ProviderViewModel oModel = new Models.Provider.ProviderViewModel()
            {
                ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions(),
            };

            //get provider menu
            oModel.ProviderMenu = GetReportMenu(oModel);

            return View(oModel);
        }

        public virtual ActionResult CC_ReportInfo_Upsert(string ReportInfoId, string ReportInfoFieldId, string ReportInfoEnableId)
        {

            BackOffice.Models.Provider.ProviderViewModel oModel = new Models.Provider.ProviderViewModel()
            {
                ProviderOptions = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions(),
            };

            //get provider menu
            oModel.ProviderMenu = GetReportMenu(oModel);

            return View(oModel);
        }
    }
}