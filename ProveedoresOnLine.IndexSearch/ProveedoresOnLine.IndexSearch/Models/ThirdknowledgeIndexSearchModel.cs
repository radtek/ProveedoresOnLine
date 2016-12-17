using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.IndexSearch.Models
{
    [ElasticsearchType(Name = "thirdknowledge_model")]
    public class ThirdknowledgeIndexSearchModel
    {
        public UInt64 Id { get { return Registry; } }
        [Number]
        public UInt64 Registry { get; set; }
        [String]
        public string ListOrigin { get; set; }
        [String]
        public string ListType { get; set; }
        [String]
        public string Code { get; set; }
        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string CompleteName { get; set; }
        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string FirstName { get; set; }
        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string SecondName { get; set; }
        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string FirstLastName { get; set; }
        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string SecondLastName { get; set; }
        [String]
        public string PersonType { get; set; }
        [String]
        public string TableCodeID { get; set; }
        [String]
        public string TypeId { get; set; }                
        [String]
        public string RelatedWiht { get; set; }
        [String]
        public string ORoldescription1 { get; set; }
        [String]
        public string ORoldescription2 { get; set; }
        [String]
        public string AKA { get; set; }
        [String]
        public string Source { get; set; }

        [Number]
        public int TotalRows { get; set; }
        public string LastModify { get; set; }
        public string FinalDateRol { get; set; }
        public string NationalitySourceCountry { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string ImageKey { get; set; }
    }
}
