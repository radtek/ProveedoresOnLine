using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.SurveyModule.Models.Index
{
    [ElasticsearchType(Name = "Survey_Info")]
    public class SurveyIndexModel
    {
        public SurveyIndexModel() { }

        [Number]
        public int Id { get { return SurveyId; } }

        [Number]
        public int SurveyId { get; set; }

        [String]
        public string SurveyPublicId { get; set; }

        [String]
        public string CompanyPublicID { get; set; }

        [Number]
        public int SurveyTypeId { get; set; }

        [String]
        public string SurveyType { get; set; }

        [Number]
        public int SurveyStatusId { get; set; }

        [String]
        public string SurveyStatus { get; set; }
    }
}
