using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.SurveyModule.Models.SurverReportModel
{
    public class SurveyReportModelTable2
    {
        #region Table 2
        public Int64 SurveyItemInfoId { get; set; }
        public Int64 SurveyIdTable2 { get; set; }
        public Int64 ParentSurveyIdTable2 { get; set; }
        public Int64 SurveyItemId { get; set; }
        public Int64 SurveyItemInfoType { get; set; }
        public string ModuleCalification { get; set; }
        public string DescriptiveText { get; set; }
        public string EvaluatorRol { get; set; }
        public string EvaluatorTable2 { get; set; }
        public string AreaComment { get; set; }
        #endregion
    }
}
