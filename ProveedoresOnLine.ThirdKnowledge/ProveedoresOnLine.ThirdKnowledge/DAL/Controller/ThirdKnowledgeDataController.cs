using ProveedoresOnLine.ThirdKnowledge.Interfaces;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProveedoresOnLine.ThirdKnowledge.DAL.Controller
{
    internal class ThirdKnowledgeDataController : IThirdKnowledgeData
    {
        private static ProveedoresOnLine.ThirdKnowledge.Interfaces.IThirdKnowledgeData oInstance;

        internal static ProveedoresOnLine.ThirdKnowledge.Interfaces.IThirdKnowledgeData Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new ThirdKnowledgeDataController();
                return oInstance;
            }
        }

        private ProveedoresOnLine.ThirdKnowledge.Interfaces.IThirdKnowledgeData DataFactory;

        #region Constructor

        public ThirdKnowledgeDataController()
        {
            ThirdKnowledgeDataFactory factory = new ThirdKnowledgeDataFactory();
            DataFactory = factory.GetThirdKnowledgeInstance();
        }

        #endregion Constructor

        #region Config

        public List<Models.PlanModel> GetAllPlanByCustomer(string CustomerPublicId, bool Enable)
        {
            return DataFactory.GetAllPlanByCustomer(CustomerPublicId, Enable);
        }

        public string PlanUpsert(string PlanPublicId, string CompanyPublicId, int QueriesByPeriod, bool IsLimited, int DaysByPeriod, TDCatalogModel Status, DateTime InitDate, DateTime EndDate, bool Enable)
        {
            return DataFactory.PlanUpsert(PlanPublicId, CompanyPublicId, QueriesByPeriod, IsLimited, DaysByPeriod, Status, InitDate, EndDate, Enable);
        }

        public string PeriodUpsert(string PeriodPublicId, string PlanPublicId, int AssignedQueries, bool IsLimited, int TotalQueries, DateTime InitDate, DateTime EndDate, bool Enable)
        {
            return DataFactory.PeriodUpsert(PeriodPublicId, PlanPublicId, AssignedQueries, IsLimited, TotalQueries, InitDate, EndDate, Enable);
        }

        public List<PeriodModel> GetPeriodByPlanPublicId(string PlanPublicId, bool Enable)
        {
            return DataFactory.GetPeriodByPlanPublicId(PlanPublicId, Enable);
        }

        public List<TDQueryModel> GetQueriesByPeriodPublicId(string PeriodPublicId, bool Enable)
        {
            return DataFactory.GetQueriesByPeriodPublicId(PeriodPublicId, Enable);
        }

        #endregion Config

        #region MarketPlace

        public List<Models.PlanModel> GetCurrenPeriod(string CustomerPublicId, bool Enable)
        {
            return DataFactory.GetCurrenPeriod(CustomerPublicId, Enable);
        }

        public List<Models.TDQueryModel> ThirdKnowledgeSearch(string CustomerPublicId, string RelatedUser, string StartDate, string EndtDate, int PageNumber, int RowCount, string SearchType, string Status, out int TotalRows)
        {
            return DataFactory.ThirdKnowledgeSearch(CustomerPublicId, RelatedUser, StartDate, EndtDate, PageNumber, RowCount, SearchType, Status, out TotalRows);
        }

        public List<Models.TDQueryModel> ThirdKnowledgeSearchByPublicId(string QueryPublicId)
        {
            return DataFactory.ThirdKnowledgeSearchByPublicId(QueryPublicId);
        }

        #endregion MarketPlace

        #region Query

        public Task<string> QueryUpsert(string QueryPublicId, string PeriodPublicId, int SearchType, string User, string FileName, bool isSuccess, int QueryStatusId, bool Enable)
        {
            return DataFactory.QueryUpsert(QueryPublicId, PeriodPublicId, SearchType, User, FileName, isSuccess, QueryStatusId, Enable);
        }

        public Task<string> QueryInfoInsert(string vQueryPublicId, string vNameResult, string vIdentificationResult, string vPriority,
                                    string vPeps, string vStatus, string vOffense, string vDocumentType,
                                     string vIdentificationNumber, string vFullName, string vIdList, string vListName,
                                     string vAKA, string vChargeOffense, string vMessage, string vQueryIdentification,
                                     string vQueryName, int vElasticId, string vGroupName, string vGroupId,
                                    string vLink, string vMoreInfo, string vZone, string vUrlFile,
                                    bool Enable)
        {
            return DataFactory.QueryInfoInsert(vQueryPublicId, vNameResult, vIdentificationResult, vPriority,
                                                vPeps, vStatus, vOffense, vDocumentType,
                                                vIdentificationNumber, vFullName, vIdList, vListName,
                                                vAKA, vChargeOffense, vMessage, vQueryIdentification,
                                                vQueryName, vElasticId, vGroupName, vGroupId,
                                                vLink, vMoreInfo, vZone, vUrlFile, Enable);
        }

        public TDQueryInfoModel GetQueryInfoByInfoPublicId(string QueryInfoPublicId)
        {
            return DataFactory.GetQueryInfoByInfoPublicId(QueryInfoPublicId);
        }

        public TDQueryInfoModel GetQueryInfoByQueryPublicIdAndElasticId(string vQueryPublicId, int vElasticId)
        {
            return DataFactory.GetQueryInfoByQueryPublicIdAndElasticId(vQueryPublicId, vElasticId);
        }

        public List<TDQueryInfoModel> GetQueryInfoByQueryPublicId(string QueryPublicId)
        {
            return DataFactory.GetQueryInfoByQueryPublicId(QueryPublicId);
        }
        #endregion Query

        #region Utils

        public List<TDCatalogModel> CatalogGetThirdKnowledgeOptions()
        {
            return DataFactory.CatalogGetThirdKnowledgeOptions();
        }

        #endregion Utils

        #region BatchProcess

        public List<TDQueryModel> GetQueriesInProgress()
        {
            return DataFactory.GetQueriesInProgress();
        }

        #endregion
    }
}