using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Company.Models.Company
{
    [ElasticsearchType(Name = "CustomerProvider_Info")]
    public class CustomerProviderIndexModel
    {
        public CustomerProviderIndexModel()
        {}

        [Number]
        public int Id { get { return CustomerProviderId; } }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public int CustomerProviderId { get; set; }

        [String(Analyzer = "customWhiteSpace", SearchAnalyzer = "customWhiteSpace")]
        public string CustomerPublicId { get; set; }

        [String]
        public string ProviderPublicId { get; set; }

        [Number]
        public int StatusId { get; set; }

        [String]
        public string Status { get; set; }

        [Boolean]
        public bool CustomerProviderEnable { get; set; }
    }
}
