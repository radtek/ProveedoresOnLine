using ProveedoresOnLine.Company.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.CalificationProject.Models.CalificationProject
{
    public class ConfigInfoModel
    {
        public int CalificationProjectConfigInfoId { get; set; }
        public CompanyModel RelatedCustomer { get; set; }
        public CompanyModel RelatedProvider { get; set; }   
        public int CompanyId { get; set; }  
        public CalificationProjectConfigModel RelatedCalificationProjectConfig { get; set; }
        public bool Status { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
