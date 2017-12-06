using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.IndexSearch.Models
{
    [ElasticsearchType(Name = "survey_model")]
    public class SurveyIndexSearchModel
    {
        public string Id { get { return SurveyPublicId; } }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string CompanyPublicId { get; set; }
        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string CustomerPublicId { get; set; }
        [String]
        public string SurveyPublicId { get; set; }
        [String]
        public string SurveyTypeId { get; set; }
        [String]
        public string SurveyType { get; set; }
        [String]
        public string SurveyStatusId { get; set; }
        [String]
        public string SurveyStatus { get; set; }
        [String]
        public string UserId { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string User { get; set; }
    }
}
