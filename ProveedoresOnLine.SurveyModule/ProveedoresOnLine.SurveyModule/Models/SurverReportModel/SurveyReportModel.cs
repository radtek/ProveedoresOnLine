using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.SurveyModule.Models.SurverReportModel
{
    public class SurveyReportModel
    {
        public int SurveyInfoId { get; set; }
        public int SurveyId { get; set; }
        public int SurveyInfoType { get; set; }
        public int SurveyItemInfoId { get; set; }
        public int ParentSurveyId { get; set; }
        public int SurveyItemInfoType { get; set; }
        public int SurveyConfigItemId { get; set; }
        public int SurveyConfigId { get; set; }
        public int SurveyConfigItemType { get; set; }
        public string SurveyName { get; set; }
        public string CompanyName { get; set; }
        public string IdentificationNumber { get; set; }
        public string Responsable { get; set; }
        public string Status { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Evaluator { get; set; }        
        public string Calification { get; set; }
        public string DescriptiveText { get; set; }
        public string EvaluatorRol { get; set; }
        public string EvaluationArea { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string AreaRol { get; set; }
        public string Comments { get; set; }
        public string ModuleCalification { get; set; }

    }
}
