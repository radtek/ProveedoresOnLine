using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.SurveyModule.Models.SurverReportModel
{
    public class SurveyReportModelTable1
    {
        #region Table 1
        public Int64 SurveyIdTable1 { get; set; }
        public Int64 SurveyConfigIdTable1 { get; set; }
        public Int64 ParentSurveyIdTable1 { get; set; }
        public string CompanyName { get; set; }
        public string IdentificationNumber { get; set; }
        public string Responsable { get; set; }
        public string Status { get; set; }
        public string Calification { get; set; }
        public string SendDate { get; set; }
        public string ExpirationDate { get; set; }
        public DateTime LastModify { get; set; }
        public string EvaluatorTable1 { get; set; }
        public string Project { get; set; }
        public string Comments { get; set; }
        public List<SurveyReportModelTable2> Table2 { get; set; }
        public List<SurveyReportModelTable3> Table3 { get; set;}
        #endregion

        

        












    }
}
