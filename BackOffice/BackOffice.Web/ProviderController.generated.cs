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
    public partial class ProviderController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ProviderController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ProviderController(Dummy d) { }

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
        public virtual System.Web.Mvc.ActionResult ProviderUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProviderUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult CompanyContactUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CompanyContactUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult PersonContactUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PersonContactUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult BranchUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.BranchUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ExperiencesUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ExperiencesUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult EconomicActivityUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.EconomicActivityUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult CertificationsUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CertificationsUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult HealtyPoliticUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.HealtyPoliticUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult RiskPoliciesUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.RiskPoliciesUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ChaimberOfCommerceUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ChaimberOfCommerceUpsert);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ProviderController Actions { get { return MVC.Provider; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Provider";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Provider";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string ProviderUpsert = "ProviderUpsert";
            public readonly string CompanyContactUpsert = "CompanyContactUpsert";
            public readonly string PersonContactUpsert = "PersonContactUpsert";
            public readonly string BranchUpsert = "BranchUpsert";
            public readonly string ExperiencesUpsert = "ExperiencesUpsert";
            public readonly string EconomicActivityUpsert = "EconomicActivityUpsert";
            public readonly string CertificationsUpsert = "CertificationsUpsert";
            public readonly string HealtyPoliticUpsert = "HealtyPoliticUpsert";
            public readonly string RiskPoliciesUpsert = "RiskPoliciesUpsert";
            public readonly string ChaimberOfCommerceUpsert = "ChaimberOfCommerceUpsert";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string ProviderUpsert = "ProviderUpsert";
            public const string CompanyContactUpsert = "CompanyContactUpsert";
            public const string PersonContactUpsert = "PersonContactUpsert";
            public const string BranchUpsert = "BranchUpsert";
            public const string ExperiencesUpsert = "ExperiencesUpsert";
            public const string EconomicActivityUpsert = "EconomicActivityUpsert";
            public const string CertificationsUpsert = "CertificationsUpsert";
            public const string HealtyPoliticUpsert = "HealtyPoliticUpsert";
            public const string RiskPoliciesUpsert = "RiskPoliciesUpsert";
            public const string ChaimberOfCommerceUpsert = "ChaimberOfCommerceUpsert";
        }


        static readonly ActionParamsClass_ProviderUpsert s_params_ProviderUpsert = new ActionParamsClass_ProviderUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ProviderUpsert ProviderUpsertParams { get { return s_params_ProviderUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ProviderUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_CompanyContactUpsert s_params_CompanyContactUpsert = new ActionParamsClass_CompanyContactUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_CompanyContactUpsert CompanyContactUpsertParams { get { return s_params_CompanyContactUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_CompanyContactUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_PersonContactUpsert s_params_PersonContactUpsert = new ActionParamsClass_PersonContactUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_PersonContactUpsert PersonContactUpsertParams { get { return s_params_PersonContactUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_PersonContactUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_BranchUpsert s_params_BranchUpsert = new ActionParamsClass_BranchUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_BranchUpsert BranchUpsertParams { get { return s_params_BranchUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_BranchUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_ExperiencesUpsert s_params_ExperiencesUpsert = new ActionParamsClass_ExperiencesUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ExperiencesUpsert ExperiencesUpsertParams { get { return s_params_ExperiencesUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ExperiencesUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_EconomicActivityUpsert s_params_EconomicActivityUpsert = new ActionParamsClass_EconomicActivityUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_EconomicActivityUpsert EconomicActivityUpsertParams { get { return s_params_EconomicActivityUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_EconomicActivityUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_CertificationsUpsert s_params_CertificationsUpsert = new ActionParamsClass_CertificationsUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_CertificationsUpsert CertificationsUpsertParams { get { return s_params_CertificationsUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_CertificationsUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_HealtyPoliticUpsert s_params_HealtyPoliticUpsert = new ActionParamsClass_HealtyPoliticUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_HealtyPoliticUpsert HealtyPoliticUpsertParams { get { return s_params_HealtyPoliticUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_HealtyPoliticUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_RiskPoliciesUpsert s_params_RiskPoliciesUpsert = new ActionParamsClass_RiskPoliciesUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_RiskPoliciesUpsert RiskPoliciesUpsertParams { get { return s_params_RiskPoliciesUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_RiskPoliciesUpsert
        {
            public readonly string ProviderPublicId = "ProviderPublicId";
        }
        static readonly ActionParamsClass_ChaimberOfCommerceUpsert s_params_ChaimberOfCommerceUpsert = new ActionParamsClass_ChaimberOfCommerceUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ChaimberOfCommerceUpsert ChaimberOfCommerceUpsertParams { get { return s_params_ChaimberOfCommerceUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ChaimberOfCommerceUpsert
        {
            public readonly string CompanyPublicId = "CompanyPublicId";
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
                public readonly string BranchUpsert = "BranchUpsert";
                public readonly string CertificationsUpsert = "CertificationsUpsert";
                public readonly string ChaimberOfCommerceUpsert = "ChaimberOfCommerceUpsert";
                public readonly string CompanyContactUpsert = "CompanyContactUpsert";
                public readonly string EconomicActivityUpsert = "EconomicActivityUpsert";
                public readonly string ExperiencesUpsert = "ExperiencesUpsert";
                public readonly string HealtyPoliticUpsert = "HealtyPoliticUpsert";
                public readonly string Index = "Index";
                public readonly string PersonContactUpsert = "PersonContactUpsert";
                public readonly string ProviderUpsert = "ProviderUpsert";
                public readonly string RiskPoliciesUpsert = "RiskPoliciesUpsert";
            }
            public readonly string BranchUpsert = "~/Views/Provider/BranchUpsert.cshtml";
            public readonly string CertificationsUpsert = "~/Views/Provider/CertificationsUpsert.cshtml";
            public readonly string ChaimberOfCommerceUpsert = "~/Views/Provider/ChaimberOfCommerceUpsert.cshtml";
            public readonly string CompanyContactUpsert = "~/Views/Provider/CompanyContactUpsert.cshtml";
            public readonly string EconomicActivityUpsert = "~/Views/Provider/EconomicActivityUpsert.cshtml";
            public readonly string ExperiencesUpsert = "~/Views/Provider/ExperiencesUpsert.cshtml";
            public readonly string HealtyPoliticUpsert = "~/Views/Provider/HealtyPoliticUpsert.cshtml";
            public readonly string Index = "~/Views/Provider/Index.cshtml";
            public readonly string PersonContactUpsert = "~/Views/Provider/PersonContactUpsert.cshtml";
            public readonly string ProviderUpsert = "~/Views/Provider/ProviderUpsert.cshtml";
            public readonly string RiskPoliciesUpsert = "~/Views/Provider/RiskPoliciesUpsert.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_ProviderController : BackOffice.Web.Controllers.ProviderController
    {
        public T4MVC_ProviderController() : base(Dummy.Instance) { }

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
        partial void ProviderUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult ProviderUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProviderUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            ProviderUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void CompanyContactUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult CompanyContactUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CompanyContactUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            CompanyContactUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void PersonContactUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult PersonContactUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PersonContactUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            PersonContactUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void BranchUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult BranchUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.BranchUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            BranchUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void ExperiencesUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult ExperiencesUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ExperiencesUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            ExperiencesUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void EconomicActivityUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult EconomicActivityUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.EconomicActivityUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            EconomicActivityUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void CertificationsUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult CertificationsUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CertificationsUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            CertificationsUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void HealtyPoliticUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult HealtyPoliticUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.HealtyPoliticUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            HealtyPoliticUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void RiskPoliciesUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string ProviderPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult RiskPoliciesUpsert(string ProviderPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.RiskPoliciesUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ProviderPublicId", ProviderPublicId);
            RiskPoliciesUpsertOverride(callInfo, ProviderPublicId);
            return callInfo;
        }

        [NonAction]
        partial void ChaimberOfCommerceUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string CompanyPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult ChaimberOfCommerceUpsert(string CompanyPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ChaimberOfCommerceUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "CompanyPublicId", CompanyPublicId);
            ChaimberOfCommerceUpsertOverride(callInfo, CompanyPublicId);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009
