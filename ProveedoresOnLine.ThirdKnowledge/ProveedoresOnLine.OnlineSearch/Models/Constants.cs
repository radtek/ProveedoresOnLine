using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Models
{
    public class Constants
    {
        public const string C_SettingsModuleName = "ThirdKnowledge";

        public const string C_POL_ThirdKnowledgeConnectionName = "POL_ThirdKnowledgeConnection";

        #region Proc Page Structure

        public const string Proc_EVENTTARGET = "Proc_EVENTTARGET";
        public const string Proc_EVENTARGUMENT = "Proc_EVENTARGUMENT";
        public const string Proc_VIEWSTATE = "Proc_VIEWSTATE";
        public const string Proc_VIEWSTATEGENERATOR = "Proc_VIEWSTATEGENERATOR";
        public const string Proc_EVENTVALIDATION = "Proc_EVENTVALIDATION";
        public const string Proc_TipoID = "Proc_TipoID";
        public const string Proc_NumID = "Proc_NumID";
        public const string Proc_QAnswer = "Proc_QAnswer";
        public const string Proc_btnSearch = "Proc_btnSearch";
        public const string Proc_divSec = "Proc_divSec";
        public const string Proc_Questionlbl = "Proc_Questionlbl";
        public const string Proc_Url = "Proc_Url";
        #endregion
        #region Panama Papers
        public const string PanamaPapers_Url = "PanamaPapers_Url";

        public const string PanamaPapersResult_Url = "PanamaPapersResult_Url";
        #endregion

        #region Judicial Process

        public const string JudicialProcess_Url = "JudicialP_Url";

        public const string JudicialProcessResult_Url = "JudicialPResult_Url";

        public const string JudicialP_Proxy = "JudicialP_Proxy";

        #endregion

        #region Register Page Service

        public const string RegisterServiceURL = "RegisterService_URL";

        public const string RegisterServiceToken = "RegisterService_Token";
        #endregion
    }
}
