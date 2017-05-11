using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.IndexSearch.Models
{
    public class Constants
    {
        public const string C_POL_SearchConnectionName = "POL_SearchConnection";
        public const string C_Topbls_SearchConnectionName = "Topbls_SearchConnection";
        public const string C_ThirdKnowledge_ConnectionName = "TK_ThirdKnowledgeConnection";

        #region InternalSettings

        public const string C_SettingsModuleName = "IndexSearch";

        public const string C_Settings_ElasticSearchUrl = "ElasticSearch_Url";

        public const string C_Settings_CompanyIndex = "CompanyIndex";

        public const string C_Settings_CompanyCustomerIndex = "CompanyCustomerIndex";

        public const string C_Settings_CompanySurveyIndex = "CompanySurveyIndex";

        public const string C_Settings_CustomerProviderIndex = "CustomerProviderIndex";

        public const string C_Settings_SurveyIndex = "SurveyIndex";

        public const string C_Settings_ThirdKnowledgeIndex = "ThirdknowledgeIndex";

        public const string C_Settings_TD_QueryIndex = "QueryIndex";

        #endregion

        #region AppSettings
        public const string C_AppSettings_LogFile = "LogFile";
        #endregion
    }
}
