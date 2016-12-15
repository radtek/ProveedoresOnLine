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

        #region InternalSettings

        public const string C_SettingsModuleName = "IndexSearch";

        public const string C_Settings_ElasticSearchUrl = "ElasticSearch_Url";

        public const string C_Settings_CompanyIndex = "CompanyIndex";

        public const string C_Settings_CompanySurveyIndex = "CompanySurveyIndex";

        public const string C_Settings_CustomerProviderIndex = "CustomerProviderIndex";

        public const string C_Settings_SurveyIndex = "SurveyIndex";

        public const string C_Settings_ThirdKnowledgeIndex = "ThirdknowledgeIndex";

        #endregion

        #region AppSettings
        public const string C_AppSettings_LogFile = "LogFile";
        #endregion
    }
}
