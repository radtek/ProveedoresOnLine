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

        public TK_QueryIndexModel()
        {
        }

        public TK_QueryIndexModel(ProveedoresOnLine.ThirdKnowledge.Models.TDQueryModel oTDQueryModel)
        {
            this.QueryPublicId = oTDQueryModel.QueryPublicId;
            this.SearchType = oTDQueryModel.SearchType.ItemId.ToString();
            this.CustomerPublicId = oTDQueryModel.CompayPublicId;
            this.QueryStatus = oTDQueryModel.QueryStatus.ItemId.ToString();
            this.FileName = oTDQueryModel.FileName;
            this.IsSuccess = oTDQueryModel.IsSuccess;
            this.CreateDate = oTDQueryModel.CreateDate;
            this.LastModify = oTDQueryModel.LastModify;
            this.Enable = oTDQueryModel.Enable;
            this.User = oTDQueryModel.User;
        }
    }
}
