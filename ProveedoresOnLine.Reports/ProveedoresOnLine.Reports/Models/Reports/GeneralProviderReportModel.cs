using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Reports.Models.Reports
{
    public class GeneralProviderReportModel
    {
        public string IdentificationType { get; set; }
        public string IdentificationNumber { get; set; }
        public string CompanyName { get; set; }
        public string ProviderStatus { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string LegalRepresentative { get; set; }
        public string EmailContact { get; set; }

        public List<Tuple<string, string>> CustomField { get; set; }
    }
}
