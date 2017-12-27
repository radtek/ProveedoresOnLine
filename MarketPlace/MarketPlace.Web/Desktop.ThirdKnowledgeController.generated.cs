// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
// 0114: suppress "Foo.BarController.Baz()' hides inherited member 'Qux.BarController.Baz()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword." when an action (with an argument) overrides an action in a parent controller
#pragma warning disable 1591, 3008, 3009, 0108, 0114
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
    public partial class ThirdKnowledgeController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ThirdKnowledgeController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ThirdKnowledgeController(Dummy d) { }

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
        public virtual System.Web.Mvc.ActionResult TKSingleSearch()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKSingleSearch);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult TKDetailSingleSearch()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKDetailSingleSearch);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult TKThirdKnowledgeSearch()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKThirdKnowledgeSearch);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult TKThirdKnowledgeDetail()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKThirdKnowledgeDetail);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult TKThirdKnowledgeDetailNew()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKThirdKnowledgeDetailNew);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.FileResult GetPdfFileBytes()
        {
            return new T4MVC_System_Web_Mvc_FileResult(Area, Name, ActionNames.GetPdfFileBytes);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ThirdKnowledgeController Actions { get { return MVC.Desktop.ThirdKnowledge; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Desktop";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "ThirdKnowledge";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "ThirdKnowledge";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string TKSingleSearch = "TKSingleSearch";
            public readonly string TKMasiveSearch = "TKMasiveSearch";
            public readonly string TKDetailSingleSearch = "TKDetailSingleSearch";
            public readonly string TKThirdKnowledgeSearch = "TKThirdKnowledgeSearch";
            public readonly string TKThirdKnowledgeDetail = "TKThirdKnowledgeDetail";
            public readonly string TKThirdKnowledgeDetailNew = "TKThirdKnowledgeDetailNew";
            public readonly string GetPdfFileBytes = "GetPdfFileBytes";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string TKSingleSearch = "TKSingleSearch";
            public const string TKMasiveSearch = "TKMasiveSearch";
            public const string TKDetailSingleSearch = "TKDetailSingleSearch";
            public const string TKThirdKnowledgeSearch = "TKThirdKnowledgeSearch";
            public const string TKThirdKnowledgeDetail = "TKThirdKnowledgeDetail";
            public const string TKThirdKnowledgeDetailNew = "TKThirdKnowledgeDetailNew";
            public const string GetPdfFileBytes = "GetPdfFileBytes";
        }


        static readonly ActionParamsClass_TKSingleSearch s_params_TKSingleSearch = new ActionParamsClass_TKSingleSearch();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_TKSingleSearch TKSingleSearchParams { get { return s_params_TKSingleSearch; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_TKSingleSearch
        {
            public readonly string Name = "Name";
            public readonly string IdentificationNumber = "IdentificationNumber";
            public readonly string ThirdKnowledgeIdType = "ThirdKnowledgeIdType";
        }
        static readonly ActionParamsClass_TKDetailSingleSearch s_params_TKDetailSingleSearch = new ActionParamsClass_TKDetailSingleSearch();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_TKDetailSingleSearch TKDetailSingleSearchParams { get { return s_params_TKDetailSingleSearch; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_TKDetailSingleSearch
        {
            public readonly string QueryPublicId = "QueryPublicId";
            public readonly string QueryInfoPublicId = "QueryInfoPublicId";
            public readonly string ElasticId = "ElasticId";
            public readonly string ReturnUrl = "ReturnUrl";
        }
        static readonly ActionParamsClass_TKThirdKnowledgeSearch s_params_TKThirdKnowledgeSearch = new ActionParamsClass_TKThirdKnowledgeSearch();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_TKThirdKnowledgeSearch TKThirdKnowledgeSearchParams { get { return s_params_TKThirdKnowledgeSearch; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_TKThirdKnowledgeSearch
        {
            public readonly string PageNumber = "PageNumber";
            public readonly string InitDate = "InitDate";
            public readonly string EndDate = "EndDate";
            public readonly string SearchType = "SearchType";
            public readonly string Status = "Status";
            public readonly string User = "User";
            public readonly string Domain = "Domain";
        }
        static readonly ActionParamsClass_TKThirdKnowledgeDetail s_params_TKThirdKnowledgeDetail = new ActionParamsClass_TKThirdKnowledgeDetail();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_TKThirdKnowledgeDetail TKThirdKnowledgeDetailParams { get { return s_params_TKThirdKnowledgeDetail; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_TKThirdKnowledgeDetail
        {
            public readonly string QueryPublicId = "QueryPublicId";
            public readonly string PageNumber = "PageNumber";
            public readonly string InitDate = "InitDate";
            public readonly string EndDate = "EndDate";
            public readonly string Enable = "Enable";
            public readonly string IsSuccess = "IsSuccess";
        }
        static readonly ActionParamsClass_TKThirdKnowledgeDetailNew s_params_TKThirdKnowledgeDetailNew = new ActionParamsClass_TKThirdKnowledgeDetailNew();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_TKThirdKnowledgeDetailNew TKThirdKnowledgeDetailNewParams { get { return s_params_TKThirdKnowledgeDetailNew; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_TKThirdKnowledgeDetailNew
        {
            public readonly string QueryPublicId = "QueryPublicId";
            public readonly string PageNumber = "PageNumber";
            public readonly string InitDate = "InitDate";
            public readonly string EndDate = "EndDate";
            public readonly string Enable = "Enable";
            public readonly string IsSuccess = "IsSuccess";
        }
        static readonly ActionParamsClass_GetPdfFileBytes s_params_GetPdfFileBytes = new ActionParamsClass_GetPdfFileBytes();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetPdfFileBytes GetPdfFileBytesParams { get { return s_params_GetPdfFileBytes; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetPdfFileBytes
        {
            public readonly string FilePath = "FilePath";
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
                public readonly string TKDetailSingleSearch = "TKDetailSingleSearch";
                public readonly string TKMasiveSearch = "TKMasiveSearch";
                public readonly string TKSingleSearch = "TKSingleSearch";
                public readonly string TKThirdKnowledgeDetail = "TKThirdKnowledgeDetail";
                public readonly string TKThirdKnowledgeSearch = "TKThirdKnowledgeSearch";
            }
            public readonly string Index = "~/Areas/Desktop/Views/ThirdKnowledge/Index.cshtml";
            public readonly string TKDetailSingleSearch = "~/Areas/Desktop/Views/ThirdKnowledge/TKDetailSingleSearch.cshtml";
            public readonly string TKMasiveSearch = "~/Areas/Desktop/Views/ThirdKnowledge/TKMasiveSearch.cshtml";
            public readonly string TKSingleSearch = "~/Areas/Desktop/Views/ThirdKnowledge/TKSingleSearch.cshtml";
            public readonly string TKThirdKnowledgeDetail = "~/Areas/Desktop/Views/ThirdKnowledge/TKThirdKnowledgeDetail.cshtml";
            public readonly string TKThirdKnowledgeSearch = "~/Areas/Desktop/Views/ThirdKnowledge/TKThirdKnowledgeSearch.cshtml";
            static readonly _TK_AppClass s_TK_App = new _TK_AppClass();
            public _TK_AppClass TK_App { get { return s_TK_App; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _TK_AppClass
            {
                static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
                public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
                public class _ViewNamesClass
                {
                }
            }
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_ThirdKnowledgeController : MarketPlace.Web.Areas.Desktop.Controllers.ThirdKnowledgeController
    {
        public T4MVC_ThirdKnowledgeController() : base(Dummy.Instance) { }

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
        partial void TKSingleSearchOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string Name, string IdentificationNumber, string ThirdKnowledgeIdType);

        [NonAction]
        public override System.Web.Mvc.ActionResult TKSingleSearch(string Name, string IdentificationNumber, string ThirdKnowledgeIdType)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKSingleSearch);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Name", Name);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "IdentificationNumber", IdentificationNumber);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ThirdKnowledgeIdType", ThirdKnowledgeIdType);
            TKSingleSearchOverride(callInfo, Name, IdentificationNumber, ThirdKnowledgeIdType);
            return callInfo;
        }

        [NonAction]
        partial void TKMasiveSearchOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult TKMasiveSearch()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKMasiveSearch);
            TKMasiveSearchOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void TKDetailSingleSearchOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string QueryPublicId, string QueryInfoPublicId, string ElasticId, string ReturnUrl);

        [NonAction]
        public override System.Web.Mvc.ActionResult TKDetailSingleSearch(string QueryPublicId, string QueryInfoPublicId, string ElasticId, string ReturnUrl)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKDetailSingleSearch);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "QueryPublicId", QueryPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "QueryInfoPublicId", QueryInfoPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ElasticId", ElasticId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ReturnUrl", ReturnUrl);
            TKDetailSingleSearchOverride(callInfo, QueryPublicId, QueryInfoPublicId, ElasticId, ReturnUrl);
            return callInfo;
        }

        [NonAction]
        partial void TKThirdKnowledgeSearchOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string PageNumber, string InitDate, string EndDate, string SearchType, string Status, string User, string Domain);

        [NonAction]
        public override System.Web.Mvc.ActionResult TKThirdKnowledgeSearch(string PageNumber, string InitDate, string EndDate, string SearchType, string Status, string User, string Domain)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKThirdKnowledgeSearch);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "PageNumber", PageNumber);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "InitDate", InitDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EndDate", EndDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "SearchType", SearchType);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Status", Status);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "User", User);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Domain", Domain);
            TKThirdKnowledgeSearchOverride(callInfo, PageNumber, InitDate, EndDate, SearchType, Status, User, Domain);
            return callInfo;
        }

        [NonAction]
        partial void TKThirdKnowledgeDetailOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string QueryPublicId, string PageNumber, string InitDate, string EndDate, string Enable, string IsSuccess);

        [NonAction]
        public override System.Web.Mvc.ActionResult TKThirdKnowledgeDetail(string QueryPublicId, string PageNumber, string InitDate, string EndDate, string Enable, string IsSuccess)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKThirdKnowledgeDetail);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "QueryPublicId", QueryPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "PageNumber", PageNumber);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "InitDate", InitDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EndDate", EndDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Enable", Enable);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "IsSuccess", IsSuccess);
            TKThirdKnowledgeDetailOverride(callInfo, QueryPublicId, PageNumber, InitDate, EndDate, Enable, IsSuccess);
            return callInfo;
        }

        [NonAction]
        partial void TKThirdKnowledgeDetailNewOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string QueryPublicId, string PageNumber, string InitDate, string EndDate, string Enable, string IsSuccess);

        [NonAction]
        public override System.Web.Mvc.ActionResult TKThirdKnowledgeDetailNew(string QueryPublicId, string PageNumber, string InitDate, string EndDate, string Enable, string IsSuccess)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TKThirdKnowledgeDetailNew);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "QueryPublicId", QueryPublicId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "PageNumber", PageNumber);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "InitDate", InitDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EndDate", EndDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Enable", Enable);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "IsSuccess", IsSuccess);
            TKThirdKnowledgeDetailNewOverride(callInfo, QueryPublicId, PageNumber, InitDate, EndDate, Enable, IsSuccess);
            return callInfo;
        }

        [NonAction]
        partial void GetPdfFileBytesOverride(T4MVC_System_Web_Mvc_FileResult callInfo, string FilePath);

        [NonAction]
        public override System.Web.Mvc.FileResult GetPdfFileBytes(string FilePath)
        {
            var callInfo = new T4MVC_System_Web_Mvc_FileResult(Area, Name, ActionNames.GetPdfFileBytes);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "FilePath", FilePath);
            GetPdfFileBytesOverride(callInfo, FilePath);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
