using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.SurveyModule.Models.SurverReportModel
{
    public class SurveyReportModel
    {
        public Int64 SurveyInfoId { get; set; }
        public Int64 SurveyIdTable1 { get; set; }
        public Int64 SurveyIdTable2 { get; set; }
        public Int64 SurveyInfoType { get; set; }
        public Int64 SurveyItemInfoId { get; set; }
        public Int64 ParentSurveyIdTable1 { get; set; }
        public Int64 ParentSurveyIdTable2 { get; set; }
        public Int64 SurveyItemInfoType { get; set; }
        public Int64 SurveyConfigItemId { get; set; }
        public Int64 SurveyConfigIdTable1 { get; set; }
        public Int64 SurveyConfigIdTable3 { get; set; }
        public Int64 SurveyConfigItemType { get; set; }
        public string SurveyName { get; set; }
        public string CompanyName { get; set; }
        public string IdentificationNumber { get; set; }
        public string Responsable { get; set; }
        public string Status { get; set; }
        public string SendDate { get; set; }
        public string ExpirationDate { get; set; }
        public DateTime LastModify { get; set; }
        public string EvaluatorTable1 { get; set; }
        public string EvaluatorTable2 { get; set; }
        public string Calification { get; set; }
        public string DescriptiveText { get; set; }
        public string EvaluatorRol { get; set; }
        public string EvaluationArea { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string AreaRol { get; set; }
        public string Comments { get; set; }
        public string ModuleCalification { get; set; }
        public string Project { get; set; }
        public string AreaComment { get; set; }

    }
}
