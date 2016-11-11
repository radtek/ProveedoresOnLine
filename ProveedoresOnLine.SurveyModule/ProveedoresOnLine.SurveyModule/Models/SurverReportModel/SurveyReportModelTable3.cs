using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.SurveyModule.Models.SurverReportModel
{
    public class SurveyReportModelTable3
    {
        #region Table 3
        public Int64 SurveyConfigItemId { get; set; }
        public Int64 SurveyConfigIdTable3 { get; set; }
        public Int64 SurveyConfigItemType { get; set; }
        public string SurveyName { get; set; }
        public string EvaluationArea { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string AreaRol { get; set; }

        #endregion
    }
}
