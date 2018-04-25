namespace ProveedoresOnLine.Reports.Models
{
    public class Enumerations
    {
        #region Util
        public enum enumCategoryInfoType
        {
            //Format Report Type
            Excel = 120001,
            PDF = 120002,
            TXT = 120003,
            CSV = 120004,
        }
        #endregion

        #region Report

        public enum enumReportType
        {
            RP_SurveyReport = 1501001,
            RP_GerencialReport = 1501002,
            RP_SelectionProcess = 1501003,
            RP_SurveyEvaluatorDetailReport = 1501004,
            RP_SurveyGeneralReport = 1501005,
            RP_ThirdKnowledgeQueryReport = 1501006,
            RP_ThirdKnowledgeQueryDetailReport = 1501007,
            RP_GIBlackListQueryReport = 1501008,
            RP_FinancialReport = 1501009,
            RP_GIBlackListDetailQueryReport = 1501010,
            RP_CalificationReport = 1501011,
            RP_ThirdKnowledgeMyQueriesReport = 1501012
        }

        #endregion

        #region Dynamic Report

        public enum enumDynamicReportType
        {
            RP_InfoPOLReport = 2014001,
            RP_InfoSurveyReport = 2014002,
            RP_InfoThirdknowLedgeReport = 2014003,
        }

        public enum enumDynamicReportInfoPOL
        {
            RP_InfoPOLReportGeneralInformation = 2016001,
            RP_InfoPOLReportLegalInformation = 2016002,
            RP_InfoPOLReportFinancialInformation = 2016003,
            RP_InfoPOLReportCommercialInformation = 2016004,
            RP_InfoPOLReportHseq = 2016005,
            RP_InfoPOLReportAditionalInformation = 2016006,
            RP_InfoPOLReportTracing = 2016007,
        }

        public enum enumDynamicReportInfoSurvey
        {
            RP_InfoPOLReport = 2014001,   //Modify

        }

        public enum enumDynamicReportInfoTK
        {
            RP_InfoPOLReport = 2014001,//Modify
            RP_InfoSurveyReport = 2014002,//Modify
            RP_InfoThirdknowLedgeReport = 2014003,//Modify
        }

        public enum enumDynamicReporInfotType
        {
            RP_InfoType = 2015001,
            RP_InfoField = 2015002,
            RP_InfoEnable = 2015003,
        }

        public enum enumDynamicReportFilters
        {
            RP_CustomerFilter = 2015001,
            RP_ProviderFilter = 2015001,

        }


        #endregion
    }
}
