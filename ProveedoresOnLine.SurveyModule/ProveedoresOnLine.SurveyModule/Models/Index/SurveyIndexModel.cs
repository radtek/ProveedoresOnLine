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

        public string Id { get { return SurveyPublicId; } }

        [String]
        public string SurveyPublicId { get; set; }

        [String]
        public string CompanyPublicId { get; set; }

        [String]
        public string CustomerPublicId { get; set; }

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
