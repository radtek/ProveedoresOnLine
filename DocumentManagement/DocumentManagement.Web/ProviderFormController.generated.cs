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
namespace DocumentManagement.Web.Controllers
{
    public partial class ProviderFormController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ProviderFormController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ProviderFormController(Dummy d) { }

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
        public virtual System.Web.Mvc.ActionResult Index()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult LoginProvider()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.LoginProvider);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult UpsertGenericStep()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.UpsertGenericStep);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult AdminProvider()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AdminProvider);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ProviderFormController Actions { get { return MVC.ProviderForm; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "ProviderForm";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "ProviderForm";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string LoginProvider = "LoginProvider";
            public readonly string UpsertGenericStep = "UpsertGenericStep";
            public readonly string AdminProvider = "AdminProvider";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string LoginProvider = "LoginProvider";
            public const string UpsertGenericStep = "UpsertGenericStep";
            public const string AdminProvider = "AdminProvider";
        }


        static readonly ActionParamsClass_Index s_params_Index = new ActionParamsClass_Index();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Index IndexParams { get { return s_params_Index; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Index
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
            public readonly string FormPublicId = "FormPublicId";
            public readonly string StepId = "StepId";
        }
        static readonly ActionParamsClass_LoginProvider s_params_LoginProvider = new ActionParamsClass_LoginProvider();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_LoginProvider LoginProviderParams { get { return s_params_LoginProvider; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_LoginProvider
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
            public readonly string FormPublicId = "FormPublicId";
            public readonly string CustomerPublicId = "CustomerPublicId";
        }
        static readonly ActionParamsClass_UpsertGenericStep s_params_UpsertGenericStep = new ActionParamsClass_UpsertGenericStep();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_UpsertGenericStep UpsertGenericStepParams { get { return s_params_UpsertGenericStep; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_UpsertGenericStep
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
            public readonly string FormPublicId = "FormPublicId";
            public readonly string StepId = "StepId";
            public readonly string NewStepId = "NewStepId";
        }
        static readonly ActionParamsClass_AdminProvider s_params_AdminProvider = new ActionParamsClass_AdminProvider();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_AdminProvider AdminProviderParams { get { return s_params_AdminProvider; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_AdminProvider
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
            public readonly string FormPublicId = "FormPublicId";
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
                public readonly string AdminProvider = "AdminProvider";
                public readonly string Index = "Index";
            }
            public readonly string AdminProvider = "~/Views/ProviderForm/AdminProvider.cshtml";
            public readonly string Index = "~/Views/ProviderForm/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_ProviderFormController : DocumentManagement.Web.Controllers.ProviderFormController
    {
        public T4MVC_ProviderFormController() : base(Dummy.Instance) { }

        [NonAction]
        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId, string FormPublicId, string StepId);

        [NonAction]
        public override System.Web.Mvc.ActionResult Index(string ProviderPublicId, string FormPublicId, string StepId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "FormPublicId", FormPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "StepId", StepId);
            IndexOverride(callInfo, ProviderPublicId, FormPublicId, StepId);
            return callInfo;
        }

        [NonAction]
        partial void LoginProviderOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId, string FormPublicId, string CustomerPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult LoginProvider(string ProviderPublicId, string FormPublicId, string CustomerPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.LoginProvider);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "FormPublicId", FormPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "CustomerPublicId", CustomerPublicId);
            LoginProviderOverride(callInfo, ProviderPublicId, FormPublicId, CustomerPublicId);
            return callInfo;
        }

        [NonAction]
        partial void UpsertGenericStepOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId, string FormPublicId, string StepId, string NewStepId);

        [NonAction]
        public override System.Web.Mvc.ActionResult UpsertGenericStep(string ProviderPublicId, string FormPublicId, string StepId, string NewStepId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.UpsertGenericStep);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "FormPublicId", FormPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "StepId", StepId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "NewStepId", NewStepId);
            UpsertGenericStepOverride(callInfo, ProviderPublicId, FormPublicId, StepId, NewStepId);
            return callInfo;
        }

        [NonAction]
        partial void AdminProviderOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId, string FormPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult AdminProvider(string ProviderPublicId, string FormPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AdminProvider);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "FormPublicId", FormPublicId);
            AdminProviderOverride(callInfo, ProviderPublicId, FormPublicId);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009
