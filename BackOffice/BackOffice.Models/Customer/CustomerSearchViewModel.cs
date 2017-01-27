using BackOffice.Models.General;
using ProveedoresOnLine.Company.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Customer
{
    public class CustomerSearchViewModel
    {
        public ProveedoresOnLine.Company.Models.Company.CompanyModel RelatedCompany { get; private set; }

        public int TotalRows { get; set; }

        public string ImageUrl{  get;  set;}

        public string CustomerPublicId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerType { get; set; }

        public string IdentificationType { get; set; }

        public string IdentificationNumber { get; set; }

        public bool Enable { get; set; }

        public CustomerSearchViewModel(ProveedoresOnLine.Company.Models.Company.CompanyModel oRelatedCompany, int oTotalRows)
        {
            RelatedCompany = oRelatedCompany;
            TotalRows = oTotalRows;
        }
        public CustomerSearchViewModel(CompanyIndexModel oDocumentCompany, int TotalRowsParam)
        {
            this.ImageUrl = oDocumentCompany.LogoUrl;
            this.IdentificationNumber = oDocumentCompany.IdentificationNumber;
            this.IdentificationType = oDocumentCompany.IdentificationType;            
            this.CustomerName = oDocumentCompany.CompanyName;
            this.CustomerPublicId = oDocumentCompany.CompanyPublicId;
            this.Enable = oDocumentCompany.CompanyEnable;
            this.TotalRows = TotalRowsParam;
        }
    }
}
