﻿using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProveedoresOnLine.ThirdKnowledge.Interfaces
{
    internal interface IThirdKnowledgeData
    {
        #region Config

        List<PlanModel> GetAllPlanByCustomer(string CustomerPublicId, bool Enable);

        string PlanUpsert(string PlanPublicId, string CompanyPublicId, int QueriesByPeriod, bool IsLimited, int DaysByPeriod, TDCatalogModel Status, DateTime InitDate, DateTime EndDate, bool Enable);

        string PeriodUpsert(string PeriodPublicId, string PlanPublicId, int AssignedQueries, bool IsLimited, int TotalQueries, DateTime InitDate, DateTime EndDate, bool Enable);

        #endregion Config

        #region MarketPlace

        List<Models.PlanModel> GetCurrenPeriod(string CustomerPublicId, bool Enable);

        List<Models.TDQueryModel> ThirdKnowledgeSearch(string CustomerPublicId, string RelatedUser, string StartDate, string EndtDate, int PageNumber, int RowCount, string SearchType, string Status, out int TotalRows);

        List<Models.TDQueryModel> ThirdKnowledgeSearchByPublicId(string CustomerPublicId, string QueryPublic, bool Enable, int PageNumber, int RowCount, out int TotalRows);

        #endregion MarketPlace

        #region Queries

        Task<string> QueryUpsert(string QueryPublicId, string PeriodPublicId, int SearchType, string User, string FileName, bool isSuccess, int QueryStatusId, bool Enable);

        Task<string> QueryInfoInsert(string vQueryPublicId, string vNameResult, string vIdentificationResult, string vPriority,
                                   string vPeps, string vStatus, string vOffense, string vDocumentType,
                                    string vIdentificationNumber, string vFullName, string vIdList, string vListName,
                                    string vAKA, string vChargeOffense, string vMessage, string vQueryIdentification,
                                    string vQueryName, int vElasticId, string vGroupName, string vGroupId,
                                   string vLink, string vMoreInfo, string vZone, string vUrlFile,
                                   bool Enable);

        TDQueryInfoModel GetQueryInfoByInfoPublicId(string QueryInfoPublicId);

        #endregion Queries

        #region Utils

        List<TDCatalogModel> CatalogGetThirdKnowledgeOptions();

        List<PeriodModel> GetPeriodByPlanPublicId(string PlanPublicId, bool Enable);

        List<TDQueryModel> GetQueriesByPeriodPublicId(string PeriodPublicId, bool Enable);

        #endregion Utils

        #region BatchProcess

        List<TDQueryModel> GetQueriesInProgress();
        #endregion       
    }
}