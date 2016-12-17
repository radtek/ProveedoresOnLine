using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.IndexSearch.Interfaces;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.SurveyModule.Models.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.IndexSearch.DAL.Controller
{
    internal class IndexSearchDataController : IIndexSearch
    {
        #region singleton instance

        private static IIndexSearch oInstance;

        internal static IIndexSearch Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new IndexSearchDataController();
                return oInstance;
            }
        }

        private IIndexSearch DataFactory;

        #endregion

        #region Constructor

        public IndexSearchDataController()
        {
            IndexSearchDataFactory factory = new IndexSearchDataFactory();
            DataFactory = factory.GetIndexSearchInstance();
        }

        #endregion

        #region Company Index

        public List<CompanyIndexModel> GetCompanyIndex()
        {
            return DataFactory.GetCompanyIndex();
        }

        public List<CustomerProviderIndexModel> GetCustomerProviderIndex()
        {
            return DataFactory.GetCustomerProviderIndex();
        }

        #endregion

        #region Survey Index

        public List<CompanySurveyIndexModel> GetCompanySurveyIndex()
        {
            return DataFactory.GetCompanySurveyIndex();    
        }

        public List<SurveyIndexSearchModel> GetSurveyIndex()
        {
            return DataFactory.GetSurveyIndex();
        }

        #region Survey Info Index
        
        public List<SurveyInfoIndexSearchModel> GetSurveyInfoIndex()
        {
            return DataFactory.GetSurveyInfoIndex();
        }



        #endregion

        #endregion

        #region Thirdknowledge Index

        public List<ThirdknowledgeIndexSearchModel> GetThirdknowledgeIndex(int vRowFrom, int vRowTo)
        {
            return DataFactory.GetThirdknowledgeIndex(vRowFrom,vRowTo);
        }
        #endregion
    }
}
