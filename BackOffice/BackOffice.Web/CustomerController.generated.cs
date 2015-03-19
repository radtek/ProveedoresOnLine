// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
#pragma warning disable 1591, 3008, 3009
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace BackOffice.Web.Controllers
{
    public partial class CustomerController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public CustomerController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected CustomerController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult GICustomerUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GICustomerUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ROCustomerUserUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ROCustomerUserUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult SCSurveyConfigUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SCSurveyConfigUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult SCSurveyConfigItemUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SCSurveyConfigItemUpsert);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public CustomerController Actions { get { return MVC.Customer; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Customer";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Customer";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string GICustomerUpsert = "GICustomerUpsert";
            public readonly string ROCustomerUserUpsert = "ROCustomerUserUpsert";
            public readonly string SCSurveyConfigUpsert = "SCSurveyConfigUpsert";
            public readonly string SCSurveyConfigItemUpsert = "SCSurveyConfigItemUpsert";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string GICustomerUpsert = "GICustomerUpsert";
            public const string ROCustomerUserUpsert = "ROCustomerUserUpsert";
            public const string SCSurveyConfigUpsert = "SCSurveyConfigUpsert";
            public const string SCSurveyConfigItemUpsert = "SCSurveyConfigItemUpsert";
        }


        static readonly ActionParamsClass_GICustomerUpsert s_params_GICustomerUpsert = new ActionParamsClass_GICustomerUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GICustomerUpsert GICustomerUpsertParams { get { return s_params_GICustomerUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GICustomerUpsert
        {
            public readonly string CustomerPublicId = "CustomerPublicId";
        }
        static readonly ActionParamsClass_ROCustomerUserUpsert s_params_ROCustomerUserUpsert = new ActionParamsClass_ROCustomerUserUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ROCustomerUserUpsert ROCustomerUserUpsertParams { get { return s_params_ROCustomerUserUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ROCustomerUserUpsert
        {
            public readonly string CustomerPublicId = "CustomerPublicId";
        }
        static readonly ActionParamsClass_SCSurveyConfigUpsert s_params_SCSurveyConfigUpsert = new ActionParamsClass_SCSurveyConfigUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SCSurveyConfigUpsert SCSurveyConfigUpsertParams { get { return s_params_SCSurveyConfigUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SCSurveyConfigUpsert
        {
            public readonly string CustomerPublicId = "CustomerPublicId";
        }
        static readonly ActionParamsClass_SCSurveyConfigItemUpsert s_params_SCSurveyConfigItemUpsert = new ActionParamsClass_SCSurveyConfigItemUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SCSurveyConfigItemUpsert SCSurveyConfigItemUpsertParams { get { return s_params_SCSurveyConfigItemUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SCSurveyConfigItemUpsert
        {
            public readonly string CustomerPublicId = "CustomerPublicId";
            public readonly string SurveyConfigId = "SurveyConfigId";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string GICustomerUpsert = "GICustomerUpsert";
                public readonly string Index = "Index";
                public readonly string ROCustomerUserUpsert = "ROCustomerUserUpsert";
                public readonly string SCSurveyConfigItemUpsert = "SCSurveyConfigItemUpsert";
                public readonly string SCSurveyConfigUpsert = "SCSurveyConfigUpsert";
            }
            public readonly string GICustomerUpsert = "~/Views/Customer/GICustomerUpsert.cshtml";
            public readonly string Index = "~/Views/Customer/Index.cshtml";
            public readonly string ROCustomerUserUpsert = "~/Views/Customer/ROCustomerUserUpsert.cshtml";
            public readonly string SCSurveyConfigItemUpsert = "~/Views/Customer/SCSurveyConfigItemUpsert.cshtml";
            public readonly string SCSurveyConfigUpsert = "~/Views/Customer/SCSurveyConfigUpsert.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_CustomerController : BackOffice.Web.Controllers.CustomerController
    {
        public T4MVC_CustomerController() : base(Dummy.Instance) { }

        [NonAction]
        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            IndexOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void GICustomerUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string CustomerPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult GICustomerUpsert(string CustomerPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GICustomerUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "CustomerPublicId", CustomerPublicId);
            GICustomerUpsertOverride(callInfo, CustomerPublicId);
            return callInfo;
        }

        [NonAction]
        partial void ROCustomerUserUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string CustomerPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult ROCustomerUserUpsert(string CustomerPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ROCustomerUserUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "CustomerPublicId", CustomerPublicId);
            ROCustomerUserUpsertOverride(callInfo, CustomerPublicId);
            return callInfo;
        }

        [NonAction]
        partial void SCSurveyConfigUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string CustomerPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult SCSurveyConfigUpsert(string CustomerPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SCSurveyConfigUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "CustomerPublicId", CustomerPublicId);
            SCSurveyConfigUpsertOverride(callInfo, CustomerPublicId);
            return callInfo;
        }

        [NonAction]
        partial void SCSurveyConfigItemUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string CustomerPublicId, string SurveyConfigId);

        [NonAction]
        public override System.Web.Mvc.ActionResult SCSurveyConfigItemUpsert(string CustomerPublicId, string SurveyConfigId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SCSurveyConfigItemUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "CustomerPublicId", CustomerPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "SurveyConfigId", SurveyConfigId);
            SCSurveyConfigItemUpsertOverride(callInfo, CustomerPublicId, SurveyConfigId);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009
