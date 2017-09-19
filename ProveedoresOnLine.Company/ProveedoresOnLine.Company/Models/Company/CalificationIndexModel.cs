using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Company.Models.Company
{
    [ElasticsearchType(Name = "calification_index")]
    public class CalificationIndexModel
    {
        public CalificationIndexModel()
        {

        }
        [Number]
        public int CalificationaProjectId { get; set; }

        [String(Index = FieldIndexOption.Analyzed)]
        public string CalificationProjectName { get; set; }

        [String]
        public string CustomerPublicId { get; set; }

        [String]
        public string ProviderPublicId { get; set; }

        [Number]
        public double TotalScore { get; set; }

        [String(Index = FieldIndexOption.Analyzed)]
        public string TotalResult { get; set; }
        
    }
}
