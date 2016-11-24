using BackOffice.Models.General;
using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.SurveyModule.Models.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Provider
{
    public class ProviderSearchViewModel
    {
        
        private int oTotalRows;       

        public CompanyModel RelatedCompany { get; private set; }

        public int TotalRows { get; set; }

        public string ImageUrl { get; set; }

        public string ProviderPublicId { get; set; }

        public string ProviderName { get; set; }

        public string ProviderType { get; set; }

        public string IdentificationType { get; set; }

        public string IdentificationNumber { get; set; }

        public bool IsOnRestrictiveList { get; set; }

        public bool Enable { get; set; }

        public ProviderSearchViewModel(CompanyModel oRelatedCompany, int oTotalRows)
        {
            RelatedCompany = oRelatedCompany;
            TotalRows = oTotalRows;
        }

        public ProviderSearchViewModel(CompanyIndexModel oDocumentCompany)
        {
            this.ImageUrl = oDocumentCompany.LogoUrl;
            this.IdentificationNumber = oDocumentCompany.IdentificationNumber;
            this.IdentificationType = oDocumentCompany.IdentificationType;
            this.IsOnRestrictiveList = oDocumentCompany.InBlackList;
            this.ProviderName = oDocumentCompany.CompanyName;
            this.ProviderPublicId = oDocumentCompany.CompanyPublicId;
            this.Enable = oDocumentCompany.CompanyEnable;
        }

        public bool RenderScripts { get; set; }

        public List<ProveedoresOnLine.Company.Models.Util.CatalogModel> ProviderOptions { get; set; }

        public List<ProviderLiteViewModel> ProviderSearchResult { get; set; }

        public List<ProveedoresOnLine.Company.Models.Util.GenericFilterModel> ProviderFilterResult { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> CityFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> StatusFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> BlackListFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> CountryFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> IcaFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> MyProvidersFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> OtherProvidersFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> SurveyType { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> SurveyStatus { get; set; }

        public string SearchParam { get; set; }

        public string SearchFilter { get; set; }

        public BackOffice.Models.General.enumSearchOrderType SearchOrderType { get; set; }

        public bool OrderOrientation { get; set; }

        public int PageNumber { get; set; }

        public int RowCount { get { return Convert.ToInt32(BackOffice.Models.General.InternalSettings.Instance[BackOffice.Models.General.Constants.C_Settings_Grid_RowCountDefault].Value.Trim()); } }
       

        public int TotalPages { get { return (int)Math.Ceiling((decimal)((decimal)TotalRows / (decimal)RowCount)); } }

        public ProveedoresOnLine.Company.Models.Company.CompanyIndexModel CompanyIndexModel { get; set; }

        public Nest.ISearchResponse<CompanyIndexModel> ElasticCompanyModel { get; set; }

        public Nest.ISearchResponse<SurveyIndexSearchModel> ElasticSurveyModel { get; set; }

        public Nest.ISearchResponse<CustomerProviderIndexModel> ElasticCustomerProviderModel { get; set; }

        public Nest.ISearchResponse<CompanySurveyIndexModel> ElasticCompanySurveyModel { get; set; }

        public string RelatedSurveyProviders { get; set; }

        #region Methods

        public List<Tuple<string, string, string>> GetlstSearchFilter()
        {
            List<Tuple<string, string, string>> oReturn = new List<Tuple<string, string, string>>();

            if (!string.IsNullOrEmpty(SearchFilter))
            {
                oReturn = SearchFilter.Replace(" ", "").
                    Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).
                    Where(x => x.IndexOf(';') >= 0).
                    Select(x => new Tuple<string, string, string>(x.Split(';')[0], x.Split(';')[1], x.Split(';')[2])).
                    Where(x => !string.IsNullOrEmpty(x.Item1) && !string.IsNullOrEmpty(x.Item2) && !string.IsNullOrEmpty(x.Item3)).
                    ToList();
            }

            return oReturn;
        }

        public Tuple<int, int> GetStartEndPage()
        {
            int ItemsxPage = 10;

            int oStart = (int)((PageNumber - (ItemsxPage / 2)) > 0 ? (PageNumber - (ItemsxPage / 2)) : 1);
            int oEnd = (int)(TotalPages >= oStart + (ItemsxPage / 2) ? oStart + (ItemsxPage / 2) : TotalPages);

            return new Tuple<int, int>(oStart, oEnd);
        }

        #endregion
    }
}
