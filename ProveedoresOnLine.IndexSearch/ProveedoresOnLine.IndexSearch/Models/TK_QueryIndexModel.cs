using Nest;
using System;

namespace ProveedoresOnLine.IndexSearch.Models
{
    [ElasticsearchType(Name = "queryindexmodel_model")]
    public class TK_QueryIndexModel
    {
        [String]
        public string QueryPublicId { get; set; }

        [String]
        public string SearchType { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string User { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string Domain { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string CustomerPublicId { get; set; }

        [String]
        public string QueryStatus { get; set; }

        [String]
        public string FileName { get; set; }

        public bool IsSuccess { get; set; }

        [Date]
        public DateTime CreateDate { get; set; }

        [Date]
        public DateTime LastModify { get; set; }

        public bool Enable { get; set; }
    }
}
