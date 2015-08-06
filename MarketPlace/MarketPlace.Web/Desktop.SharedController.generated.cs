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
namespace T4MVC.Desktop
{
    public class SharedController
    {

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
                public readonly string _CH_ProjectByMonth = "_CH_ProjectByMonth";
                public readonly string _CH_ProjectByState = "_CH_ProjectByState";
                public readonly string _CH_StateProviders = "_CH_StateProviders";
                public readonly string _CH_Survey = "_CH_Survey";
                public readonly string _CH_SurveyByEvaluators = "_CH_SurveyByEvaluators";
                public readonly string _CH_SurveyByMonth = "_CH_SurveyByMonth";
                public readonly string _CH_SurveyByName = "_CH_SurveyByName";
                public readonly string _CM_CompareMenu = "_CM_CompareMenu";
                public readonly string _L_Footer = "_L_Footer";
                public readonly string _L_Header = "_L_Header";
                public readonly string _Layout = "_Layout";
                public readonly string _P_CI_ExperiencesInfo = "_P_CI_ExperiencesInfo";
                public readonly string _P_FI_Balance = "_P_FI_Balance";
                public readonly string _P_FI_BalanceItem = "_P_FI_BalanceItem";
                public readonly string _P_FI_Indicators = "_P_FI_Indicators";
                public readonly string _P_FI_IndicatorsItem = "_P_FI_IndicatorsItem";
                public readonly string _P_HI_CertificationsInfo = "_P_HI_CertificationsInfo";
                public readonly string _P_HI_HealtyPoliticInfo = "_P_HI_HealtyPoliticInfo";
                public readonly string _P_HI_RiskPoliciesInfo = "_P_HI_RiskPoliciesInfo";
                public readonly string _P_LI_ChaimberOfCommerceInfo = "_P_LI_ChaimberOfCommerceInfo";
                public readonly string _P_LI_RutInfo = "_P_LI_RutInfo";
                public readonly string _P_LI_SARLAFTInfo = "_P_LI_SARLAFTInfo";
                public readonly string _P_ProviderLite = "_P_ProviderLite";
                public readonly string _P_ProviderMenu = "_P_ProviderMenu";
                public readonly string _P_Search_Compare_Result = "_P_Search_Compare_Result";
                public readonly string _P_Search_Comparison = "_P_Search_Comparison";
                public readonly string _P_Search_Filter = "_P_Search_Filter";
                public readonly string _P_Search_Order = "_P_Search_Order";
                public readonly string _P_Search_Project = "_P_Search_Project";
                public readonly string _P_Search_Result = "_P_Search_Result";
                public readonly string _P_Search_Result_Item = "_P_Search_Result_Item";
                public readonly string _P_Search_Result_Pager = "_P_Search_Result_Pager";
                public readonly string _P_SV_ReportByDate = "_P_SV_ReportByDate";
                public readonly string _PJ_ProjectDetail_Header = "_PJ_ProjectDetail_Header";
                public readonly string _PJ_ProjectDetail_Provider_Result = "_PJ_ProjectDetail_Provider_Result";
                public readonly string _PJ_ProjectProviderDetail_Actions = "_PJ_ProjectProviderDetail_Actions";
                public readonly string _PJ_ProjectProviderDetail_Menu = "_PJ_ProjectProviderDetail_Menu";
                public readonly string _PJ_ProjectProviderDetail_Results = "_PJ_ProjectProviderDetail_Results";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail = "_PJ_ProjectProviderDetail_ResultsDetail";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404001 = "_PJ_ProjectProviderDetail_ResultsDetail_1404001";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404002 = "_PJ_ProjectProviderDetail_ResultsDetail_1404002";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404003 = "_PJ_ProjectProviderDetail_ResultsDetail_1404003";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404004 = "_PJ_ProjectProviderDetail_ResultsDetail_1404004";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404005 = "_PJ_ProjectProviderDetail_ResultsDetail_1404005";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404006 = "_PJ_ProjectProviderDetail_ResultsDetail_1404006";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404007 = "_PJ_ProjectProviderDetail_ResultsDetail_1404007";
                public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404008 = "_PJ_ProjectProviderDetail_ResultsDetail_1404008";
                public readonly string _PJ_Search_Filter = "_PJ_Search_Filter";
                public readonly string _PJ_SearchProject_Result = "_PJ_SearchProject_Result";
                public readonly string _PJ_SearchProject_Result_Item = "_PJ_SearchProject_Result_Item";
                public readonly string _PR_General = "_PR_General";
                public readonly string _RP_SV_SurveyGeneralInfoReport = "_RP_SV_SurveyGeneralInfoReport";
                public readonly string _ST_StatsMenu = "_ST_StatsMenu";
                public readonly string _SV_ProgramSurvey = "_SV_ProgramSurvey";
                public readonly string _SV_SurveyReport = "_SV_SurveyReport";
                public readonly string _SV_SurveySearch_Result_Item = "_SV_SurveySearch_Result_Item";
                public readonly string _SV_SurveySearch_Result_Pager = "_SV_SurveySearch_Result_Pager";
                public readonly string _TK_SingleSearch = "_TK_SingleSearch";
            }
            public readonly string _CH_ProjectByMonth = "~/Areas/Desktop/Views/Shared/_CH_ProjectByMonth.cshtml";
            public readonly string _CH_ProjectByState = "~/Areas/Desktop/Views/Shared/_CH_ProjectByState.cshtml";
            public readonly string _CH_StateProviders = "~/Areas/Desktop/Views/Shared/_CH_StateProviders.cshtml";
            public readonly string _CH_Survey = "~/Areas/Desktop/Views/Shared/_CH_Survey.cshtml";
            public readonly string _CH_SurveyByEvaluators = "~/Areas/Desktop/Views/Shared/_CH_SurveyByEvaluators.cshtml";
            public readonly string _CH_SurveyByMonth = "~/Areas/Desktop/Views/Shared/_CH_SurveyByMonth.cshtml";
            public readonly string _CH_SurveyByName = "~/Areas/Desktop/Views/Shared/_CH_SurveyByName.cshtml";
            public readonly string _CM_CompareMenu = "~/Areas/Desktop/Views/Shared/_CM_CompareMenu.cshtml";
            public readonly string _L_Footer = "~/Areas/Desktop/Views/Shared/_L_Footer.cshtml";
            public readonly string _L_Header = "~/Areas/Desktop/Views/Shared/_L_Header.cshtml";
            public readonly string _Layout = "~/Areas/Desktop/Views/Shared/_Layout.cshtml";
            public readonly string _P_CI_ExperiencesInfo = "~/Areas/Desktop/Views/Shared/_P_CI_ExperiencesInfo.cshtml";
            public readonly string _P_FI_Balance = "~/Areas/Desktop/Views/Shared/_P_FI_Balance.cshtml";
            public readonly string _P_FI_BalanceItem = "~/Areas/Desktop/Views/Shared/_P_FI_BalanceItem.cshtml";
            public readonly string _P_FI_Indicators = "~/Areas/Desktop/Views/Shared/_P_FI_Indicators.cshtml";
            public readonly string _P_FI_IndicatorsItem = "~/Areas/Desktop/Views/Shared/_P_FI_IndicatorsItem.cshtml";
            public readonly string _P_HI_CertificationsInfo = "~/Areas/Desktop/Views/Shared/_P_HI_CertificationsInfo.cshtml";
            public readonly string _P_HI_HealtyPoliticInfo = "~/Areas/Desktop/Views/Shared/_P_HI_HealtyPoliticInfo.cshtml";
            public readonly string _P_HI_RiskPoliciesInfo = "~/Areas/Desktop/Views/Shared/_P_HI_RiskPoliciesInfo.cshtml";
            public readonly string _P_LI_ChaimberOfCommerceInfo = "~/Areas/Desktop/Views/Shared/_P_LI_ChaimberOfCommerceInfo.cshtml";
            public readonly string _P_LI_RutInfo = "~/Areas/Desktop/Views/Shared/_P_LI_RutInfo.cshtml";
            public readonly string _P_LI_SARLAFTInfo = "~/Areas/Desktop/Views/Shared/_P_LI_SARLAFTInfo.cshtml";
            public readonly string _P_ProviderLite = "~/Areas/Desktop/Views/Shared/_P_ProviderLite.cshtml";
            public readonly string _P_ProviderMenu = "~/Areas/Desktop/Views/Shared/_P_ProviderMenu.cshtml";
            public readonly string _P_Search_Compare_Result = "~/Areas/Desktop/Views/Shared/_P_Search_Compare_Result.cshtml";
            public readonly string _P_Search_Comparison = "~/Areas/Desktop/Views/Shared/_P_Search_Comparison.cshtml";
            public readonly string _P_Search_Filter = "~/Areas/Desktop/Views/Shared/_P_Search_Filter.cshtml";
            public readonly string _P_Search_Order = "~/Areas/Desktop/Views/Shared/_P_Search_Order.cshtml";
            public readonly string _P_Search_Project = "~/Areas/Desktop/Views/Shared/_P_Search_Project.cshtml";
            public readonly string _P_Search_Result = "~/Areas/Desktop/Views/Shared/_P_Search_Result.cshtml";
            public readonly string _P_Search_Result_Item = "~/Areas/Desktop/Views/Shared/_P_Search_Result_Item.cshtml";
            public readonly string _P_Search_Result_Pager = "~/Areas/Desktop/Views/Shared/_P_Search_Result_Pager.cshtml";
            public readonly string _P_SV_ReportByDate = "~/Areas/Desktop/Views/Shared/_P_SV_ReportByDate.cshtml";
            public readonly string _PJ_ProjectDetail_Header = "~/Areas/Desktop/Views/Shared/_PJ_ProjectDetail_Header.cshtml";
            public readonly string _PJ_ProjectDetail_Provider_Result = "~/Areas/Desktop/Views/Shared/_PJ_ProjectDetail_Provider_Result.cshtml";
            public readonly string _PJ_ProjectProviderDetail_Actions = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_Actions.cshtml";
            public readonly string _PJ_ProjectProviderDetail_Menu = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_Menu.cshtml";
            public readonly string _PJ_ProjectProviderDetail_Results = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_Results.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404001 = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail_1404001.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404002 = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail_1404002.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404003 = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail_1404003.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404004 = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail_1404004.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404005 = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail_1404005.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404006 = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail_1404006.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404007 = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail_1404007.cshtml";
            public readonly string _PJ_ProjectProviderDetail_ResultsDetail_1404008 = "~/Areas/Desktop/Views/Shared/_PJ_ProjectProviderDetail_ResultsDetail_1404008.cshtml";
            public readonly string _PJ_Search_Filter = "~/Areas/Desktop/Views/Shared/_PJ_Search_Filter.cshtml";
            public readonly string _PJ_SearchProject_Result = "~/Areas/Desktop/Views/Shared/_PJ_SearchProject_Result.cshtml";
            public readonly string _PJ_SearchProject_Result_Item = "~/Areas/Desktop/Views/Shared/_PJ_SearchProject_Result_Item.cshtml";
            public readonly string _PR_General = "~/Areas/Desktop/Views/Shared/_PR_General.cshtml";
            public readonly string _RP_SV_SurveyGeneralInfoReport = "~/Areas/Desktop/Views/Shared/_RP_SV_SurveyGeneralInfoReport.cshtml";
            public readonly string _ST_StatsMenu = "~/Areas/Desktop/Views/Shared/_ST_StatsMenu.cshtml";
            public readonly string _SV_ProgramSurvey = "~/Areas/Desktop/Views/Shared/_SV_ProgramSurvey.cshtml";
            public readonly string _SV_SurveyReport = "~/Areas/Desktop/Views/Shared/_SV_SurveyReport.cshtml";
            public readonly string _SV_SurveySearch_Result_Item = "~/Areas/Desktop/Views/Shared/_SV_SurveySearch_Result_Item.cshtml";
            public readonly string _SV_SurveySearch_Result_Pager = "~/Areas/Desktop/Views/Shared/_SV_SurveySearch_Result_Pager.cshtml";
            public readonly string _TK_SingleSearch = "~/Areas/Desktop/Views/Shared/_TK_SingleSearch.cshtml";
        }
    }

}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
