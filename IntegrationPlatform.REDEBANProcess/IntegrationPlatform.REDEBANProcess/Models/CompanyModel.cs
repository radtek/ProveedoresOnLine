using IntegrationPlatform.REDEBANProcess.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess.Models
{
    public class CompanyModel
    {
        public string CompanyPublicId { get; set; }

        public string CompanyName { get; set; }

        public string IdentificationNumber { get; set; }

        public string Representant { get; set; }

        public string Status { get; set; }

        public string Telephone { get; set; }

        public string City { get; set; }
        
        public bool Enable { get; set; }        
    }
}
