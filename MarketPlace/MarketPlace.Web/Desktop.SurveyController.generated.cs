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
namespace MarketPlace.Web.Areas.Desktop.Controllers
{
    public partial class SurveyController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public SurveyController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected SurveyController(Dummy d) { }

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
        public virtual System.Web.Mvc.ActionResult SurveyUpsert()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SurveyUpsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult SurveyFinalize()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SurveyFinalize);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public SurveyController Actions { get { return MVC.Desktop.Survey; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Desktop";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Survey";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Survey";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string SurveyUpsert = "SurveyUpsert";
            public readonly string SurveyFinalize = "SurveyFinalize";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string SurveyUpsert = "SurveyUpsert";
            public const string SurveyFinalize = "SurveyFinalize";
        }


        static readonly ActionParamsClass_Index s_params_Index = new ActionParamsClass_Index();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Index IndexParams { get { return s_params_Index; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Index
        {
            public readonly string SurveyPublicId = "SurveyPublicId";
            public readonly string StepId = "StepId";
        }
        static readonly ActionParamsClass_SurveyUpsert s_params_SurveyUpsert = new ActionParamsClass_SurveyUpsert();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SurveyUpsert SurveyUpsertParams { get { return s_params_SurveyUpsert; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SurveyUpsert
        {
            public readonly string SurveyPublicId = "SurveyPublicId";
            public readonly string StepId = "StepId";
        }
        static readonly ActionParamsClass_SurveyFinalize s_params_SurveyFinalize = new ActionParamsClass_SurveyFinalize();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SurveyFinalize SurveyFinalizeParams { get { return s_params_SurveyFinalize; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SurveyFinalize
        {
            public readonly string SurveyPublicId = "SurveyPublicId";
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
                public readonly string Index = "Index";
                public readonly string SurveyFinalize = "SurveyFinalize";
            }
            public readonly string Index = "~/Areas/Desktop/Views/Survey/Index.cshtml";
            public readonly string SurveyFinalize = "~/Areas/Desktop/Views/Survey/SurveyFinalize.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_SurveyController : MarketPlace.Web.Areas.Desktop.Controllers.SurveyController
    {
        public T4MVC_SurveyController() : base(Dummy.Instance) { }

        [NonAction]
        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string SurveyPublicId, string StepId);

        [NonAction]
        public override System.Web.Mvc.ActionResult Index(string SurveyPublicId, string StepId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "SurveyPublicId", SurveyPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "StepId", StepId);
            IndexOverride(callInfo, SurveyPublicId, StepId);
            return callInfo;
        }

        [NonAction]
        partial void SurveyUpsertOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string SurveyPublicId, string StepId);

        [NonAction]
        public override System.Web.Mvc.ActionResult SurveyUpsert(string SurveyPublicId, string StepId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SurveyUpsert);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "SurveyPublicId", SurveyPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "StepId", StepId);
            SurveyUpsertOverride(callInfo, SurveyPublicId, StepId);
            return callInfo;
        }

        [NonAction]
        partial void SurveyFinalizeOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string SurveyPublicId);

        [NonAction]
        public override System.Web.Mvc.ActionResult SurveyFinalize(string SurveyPublicId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SurveyFinalize);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "SurveyPublicId", SurveyPublicId);
            SurveyFinalizeOverride(callInfo, SurveyPublicId);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009
