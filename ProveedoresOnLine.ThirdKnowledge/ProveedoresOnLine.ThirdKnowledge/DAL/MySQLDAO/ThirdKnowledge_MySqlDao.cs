using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ProveedoresOnLine.ThirdKnowledge.Interfaces;

namespace ProveedoresOnLine.ThirdKnowledge.DAL.MySQLDAO
{
    internal class ThirdKnowledge_MySqlDao : ProveedoresOnLine.ThirdKnowledge.Interfaces.IThirdKnowledgeData
    {
        private ADO.Interfaces.IADO DataInstance;

        public ThirdKnowledge_MySqlDao()
        {
            DataInstance = new ADO.MYSQL.MySqlImplement(ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_POL_ThirdKnowledgeConnectionName);
        }

        #region Config

        public List<Models.PlanModel> GetAllPlanByCustomer(string CustomerPublicId, bool Enable)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vCompanyPublicId", CustomerPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "TD_GetAllPlanByCustomer",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            List<Models.PlanModel> oReturn = null;

            if (response.DataTableResult != null &&
               response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                    (from cm in response.DataTableResult.AsEnumerable()
                     where !cm.IsNull("PlanId")
                     group cm by new
                     {
                         PlanId = cm.Field<int>("PlanId"),
                         PlanPublicId = cm.Field<string>("PlanPublicId"),
                         CompanyPublicId = cm.Field<string>("CompanyPublicId"),
                         IsLimited = Enable = cm.Field<UInt64>("IsLimited") == 1 ? true : false,
                         QueriesByPeriod = cm.Field<int>("QueriesByPeriod"),
                         InitDate = cm.Field<DateTime>("InitDate"),
                         EndDate = cm.Field<DateTime>("EndDate"),

                         StatusId = cm.Field<int>("StatusId"),
                         StatusName = cm.Field<string>("StatusName"),
                                                  
                         DaysByPeriod = cm.Field<int>("DaysByPeriod"),
                         Enable = cm.Field<UInt64>("Enable") == 1 ? true : false,
                         LastModify = cm.Field<DateTime>("LastModify"),
                         CreateDate = cm.Field<DateTime>("CreateDate"),
                     } into cmg
                     select new PlanModel()
                     {
                         CompanyPublicId = CustomerPublicId,
                         DaysByPeriod = cmg.Key.DaysByPeriod,
                         PlanPublicId = cmg.Key.PlanPublicId,
                         QueriesByPeriod = cmg.Key.QueriesByPeriod,
                         IsLimited = cmg.Key.IsLimited,
                         InitDate = cmg.Key.InitDate,
                         EndDate = cmg.Key.EndDate,
                         Status = new TDCatalogModel()
                         {
                             ItemId = cmg.Key.StatusId,
                             ItemName = cmg.Key.StatusName,
                         },
                         Enable = cmg.Key.Enable,
                         CreateDate = cmg.Key.CreateDate,
                         LastModify = cmg.Key.LastModify,
                         RelatedPeriodModel =
                            (from pinf in response.DataTableResult.AsEnumerable()
                             where !pinf.IsNull("PeriodPublicId") &&
                             pinf.Field<int>("PlanId") == cmg.Key.PlanId
                             group pinf by new
                             {
                                 PeriodPublicId = pinf.Field<string>("PeriodPublicId"),
                                 AssignedQueries = pinf.Field<int>("AssignedQueries"),
                                 PerIsLimited = pinf.Field<UInt64>("PerIsLimited") == 1 ? true : false,
                                 InitDate = pinf.Field<DateTime>("InfoInitDate"),
                                 EndDate = pinf.Field<DateTime>("InfoEndDate"),
                                 TotalQueries = pinf.Field<int>("TotalQueries"),
                                 Enable = pinf.Field<UInt64>("PeriodEnable") == 1 ? true : false,
                                 LastModify = pinf.Field<DateTime>("LastModify"),
                                 CreateDate = pinf.Field<DateTime>("CreateDate"),
                             } into pinfgr
                             select new PeriodModel()
                             {
                                 AssignedQueries = pinfgr.Key.AssignedQueries,
                                 PeriodPublicId = pinfgr.Key.PeriodPublicId,
                                 IsLimited = pinfgr.Key.PerIsLimited,
                                 InitDate = pinfgr.Key.InitDate,
                                 EndDate = pinfgr.Key.EndDate,
                                 TotalQueries = pinfgr.Key.TotalQueries,
                                 Enable = pinfgr.Key.Enable,
                                 LastModify = pinfgr.Key.LastModify,
                                 CreateDate = pinfgr.Key.CreateDate,
                             }).ToList(),
                     }).ToList();
            }
            return oReturn;
        }

        public string PlanUpsert(string PlanPublicId, string CompanyPublicId, int QueriesByPeriod, bool IsLimited, int DaysByPeriod, TDCatalogModel Status, DateTime InitDate, DateTime EndDate, bool Enable)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vPlanPublicId", PlanPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vCompanyPublicId", CompanyPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vQueriesByPeriod", QueriesByPeriod));
            lstParams.Add(DataInstance.CreateTypedParameter("vDaysByPeriod", DaysByPeriod));
            lstParams.Add(DataInstance.CreateTypedParameter("vStatus", Status.ItemId));
            lstParams.Add(DataInstance.CreateTypedParameter("vInitDate", InitDate));
            lstParams.Add(DataInstance.CreateTypedParameter("vEndDate", EndDate));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));
            lstParams.Add(DataInstance.CreateTypedParameter("vIsLimited", IsLimited == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "TD_PlanUpsert",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            if (response.ScalarResult != null)
                return response.ScalarResult.ToString();
            else
                return null;
        }

        public string PeriodUpsert(string PeriodPublicId, string PlanPublicId, int AssignedQueries, bool IsLimited, int TotalQueries, DateTime InitDate, DateTime EndDate, bool Enable)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vPeriodPublicId", PeriodPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vPlanPublicId", PlanPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vAssignedQueries", AssignedQueries));
            lstParams.Add(DataInstance.CreateTypedParameter("vTotalQueries", TotalQueries));
            lstParams.Add(DataInstance.CreateTypedParameter("vInitDate", InitDate));
            lstParams.Add(DataInstance.CreateTypedParameter("vEndDate", EndDate));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));
            lstParams.Add(DataInstance.CreateTypedParameter("vIsLimited", IsLimited == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "TD_PeriodUpsert",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            if (response.ScalarResult != null)
                return response.ScalarResult.ToString();
            else
                return null;
        }

        public List<PeriodModel> GetPeriodByPlanPublicId(string PlanPublicId, bool Enable)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vPlanPublicId", PlanPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "TD_GetPeriodByPlan",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            List<Models.PeriodModel> oReturn = null;

            if (response.DataTableResult != null &&
               response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                     (from cm in response.DataTableResult.AsEnumerable()
                      where !cm.IsNull("PeriodId")
                      group cm by new
                      {
                          PeriodPublicId = cm.Field<string>("PeriodPublicId"),
                          IsLimited = Enable = cm.Field<UInt64>("IsLimited") == 1 ? true : false,
                          AssignedQueries = cm.Field<int>("AssignedQueries"),
                          InitDate = cm.Field<DateTime>("InitDate"),
                          EndDate = cm.Field<DateTime>("EndDate"),
                          TotalQueries = cm.Field<int>("TotalQueries"),
                          Enable = cm.Field<UInt64>("PeriodEnable") == 1 ? true : false,
                          LastModify = cm.Field<DateTime>("LastModify"),
                          CreateDate = cm.Field<DateTime>("CreateDate"),
                      } into cmg
                      select new PeriodModel()
                      {
                          AssignedQueries = cmg.Key.AssignedQueries,
                          IsLimited = cmg.Key.IsLimited,
                          PeriodPublicId = cmg.Key.PeriodPublicId,
                          InitDate = cmg.Key.InitDate,
                          EndDate = cmg.Key.EndDate,
                          TotalQueries = cmg.Key.TotalQueries,
                          Enable = cmg.Key.Enable,
                          LastModify = cmg.Key.LastModify,
                          CreateDate = cmg.Key.CreateDate
                      }).ToList();
            }
            return oReturn;
        }

        public List<TDQueryModel> GetQueriesByPeriodPublicId(string PeriodPublicId, bool Enable)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vPeriodPublicId", PeriodPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "TD_TK_GetQueriesByPeriod",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            List<TDQueryModel> oReturn = null;

            if (response.DataTableResult != null &&
               response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                     (from cm in response.DataTableResult.AsEnumerable()
                      where !cm.IsNull("QueryId")
                      group cm by new
                      {
                          QueryPublicId = cm.Field<string>("QueryPublicId"),
                          PeriodId = cm.Field<Int32>("PeriodId"),
                          SearchType = cm.Field<Int32>("SearchType"),
                          SearhTypeName = cm.Field<string>("SearchTypeName"),
                          User = cm.Field<string>("User"),
                          QueryStatusId = cm.Field<Int32>("QueryStatusId"),
                          QueryStatusName = cm.Field<string>("QueryStatusName"),
                          IsSuccess = cm.Field<UInt64>("IsSuccess") == 1 ? true : false,
                          CreateDate = cm.Field<DateTime>("CreateDate"),
                          Enable = cm.Field<UInt64>("QueryEnable") == 1 ? true : false,
                      } into cmg
                      select new TDQueryModel()
                      {
                          QueryPublicId = cmg.Key.QueryPublicId,
                          PeriodPublicId = cmg.Key.PeriodId.ToString(),
                          SearchType = new TDCatalogModel()
                          {
                              ItemId = cmg.Key.SearchType,
                              ItemName = cmg.Key.SearhTypeName,
                          },
                          User = cmg.Key.User,
                          QueryStatus = new TDCatalogModel()
                          {
                              ItemId = cmg.Key.QueryStatusId,
                              ItemName = cmg.Key.QueryStatusName,
                          },
                          IsSuccess = cmg.Key.IsSuccess,
                          CreateDate = cmg.Key.CreateDate,
                          Enable = cmg.Key.Enable
                      }).ToList();
            }
            return oReturn;
        }

        #endregion Config

        #region MarketPlace

        public List<Models.PlanModel> GetCurrenPeriod(string CustomerPublicId, bool Enable)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vCompanyPublicId", CustomerPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "TD_GetCurrentPeriod",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            List<Models.PlanModel> oReturn = null;

            if (response.DataTableResult != null &&
               response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                    (from cm in response.DataTableResult.AsEnumerable()
                     where !cm.IsNull("PlanId")
                     group cm by new
                     {
                         PlanId = cm.Field<int>("PlanId"),
                         PlanPublicId = cm.Field<string>("PlanPublicId"),
                         CompanyPublicId = cm.Field<string>("CompanyPublicId"),
                         QueriesByPeriod = cm.Field<int>("QueriesByPeriod"),
                         InitDate = cm.Field<DateTime>("InitDate"),
                         EndDate = cm.Field<DateTime>("EndDate"),
                         PlanIsLimited = cm.Field<UInt64>("IsLimited") == 1 ? true : false,

                         StatusId = cm.Field<int>("StatusId"),
                         StatusName = cm.Field<string>("StatusName"),

                         DaysByPeriod = cm.Field<int>("DaysByPeriod"),
                         Enable = cm.Field<UInt64>("Enable") == 1 ? true : false,
                         LastModify = cm.Field<DateTime>("LastModify"),
                         CreateDate = cm.Field<DateTime>("CreateDate"),
                     } into cmg
                     select new PlanModel()
                     {
                         CompanyPublicId = CustomerPublicId,
                         DaysByPeriod = cmg.Key.DaysByPeriod,
                         IsLimited = cmg.Key.PlanIsLimited,
                         PlanPublicId = cmg.Key.PlanPublicId,
                         QueriesByPeriod = cmg.Key.QueriesByPeriod,
                         InitDate = cmg.Key.InitDate,
                         EndDate = cmg.Key.EndDate,
                         Status = new TDCatalogModel()
                         {
                             ItemId = cmg.Key.StatusId,
                             ItemName = cmg.Key.StatusName,
                         },
                         Enable = cmg.Key.Enable,
                         CreateDate = cmg.Key.CreateDate,
                         LastModify = cmg.Key.LastModify,
                         RelatedPeriodModel =
                            (from pinf in response.DataTableResult.AsEnumerable()
                             where !pinf.IsNull("PeriodPublicId") &&
                             pinf.Field<int>("PlanId") == cmg.Key.PlanId
                             group pinf by new
                             {
                                 PeriodPublicId = pinf.Field<string>("PeriodPublicId"),
                                 AssignedQueries = pinf.Field<int>("AssignedQueries"),
                                 PerIsLimited = pinf.Field<UInt64>("PerIsLimited") == 1 ? true : false,
                                 InitDate = pinf.Field<DateTime>("PerInitDate"),
                                 EndDate = pinf.Field<DateTime>("PerEndDate"),
                                 TotalQueries = pinf.Field<int>("TotalQueries"),
                                 Enable = pinf.Field<UInt64>("PeriodEnable") == 1 ? true : false,
                                 LastModify = pinf.Field<DateTime>("LastModify"),
                                 CreateDate = pinf.Field<DateTime>("CreateDate"),
                             } into pinfgr
                             select new PeriodModel()
                             {
                                 AssignedQueries = pinfgr.Key.AssignedQueries,
                                 PlanPublicId = cmg.Key.PlanPublicId,
                                 IsLimited = pinfgr.Key.PerIsLimited,
                                 PeriodPublicId = pinfgr.Key.PeriodPublicId,
                                 InitDate = pinfgr.Key.InitDate,
                                 EndDate = pinfgr.Key.EndDate,
                                 TotalQueries = pinfgr.Key.TotalQueries,
                                 Enable = pinfgr.Key.Enable,
                                 LastModify = pinfgr.Key.LastModify,
                                 CreateDate = pinfgr.Key.CreateDate,
                             }).ToList(),
                     }).ToList();
            }
            return oReturn;
        }

        public List<Models.TDQueryModel> ThirdKnowledgeSearch(string CustomerPublicId, string RelatedUser, string StartDate, string EndtDate, int PageNumber, int RowCount, string SearchType, string Status, out int TotalRows)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vCustomerPublicId", CustomerPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vStartDate", StartDate));
            lstParams.Add(DataInstance.CreateTypedParameter("vDateEnd", EndtDate));
            lstParams.Add(DataInstance.CreateTypedParameter("vPageNumber", PageNumber));
            lstParams.Add(DataInstance.CreateTypedParameter("vRowCount", RowCount));
            lstParams.Add(DataInstance.CreateTypedParameter("vSearchType", SearchType));
            lstParams.Add(DataInstance.CreateTypedParameter("vStatus", Status));
            lstParams.Add(DataInstance.CreateTypedParameter("vRelatedUser", RelatedUser));

            TotalRows = 0;

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "MP_TK_Search_Pager",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            List<Models.TDQueryModel> oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                TotalRows = response.DataTableResult.Rows[0].Field<int>("TotalRows");

                oReturn =
                    (from q in response.DataTableResult.AsEnumerable()
                     where !q.IsNull("QueryPublicId")
                     group q by new
                     {
                         SearchTypeId = q.Field<int>("SearchTypeId"),
                         SearchTypeName = q.Field<string>("SearchTypeName"),
                         User = q.Field<string>("User"),
                         QueryStatusId = q.Field<int>("QueryStatusId"),
                         QueryStatusName = q.Field<string>("QueryStatusName"),
                         IsSuccess = q.Field<UInt64>("IsSuccess"),
                         QueryEnable = q.Field<UInt64>("QueryEnable"),
                         PeriodPublicId = q.Field<string>("PeriodPublicId"),
                         QueryPublicId = q.Field<string>("QueryPublicId"),
                         FileName = q.Field<string>("FileName"),
                         QueryCreateDate = q.Field<DateTime>("QueryCreateDate"),
                     }
                         into qg
                     select new Models.TDQueryModel()
                     {
                         QueryPublicId = qg.Key.QueryPublicId,
                         IsSuccess = qg.Key.IsSuccess == 1 ? true : false,
                         FileName = qg.Key.FileName,
                         SearchType = new TDCatalogModel()
                         {
                             ItemId = qg.Key.SearchTypeId,
                             ItemName = qg.Key.SearchTypeName,
                         },
                         User = qg.Key.User,
                         QueryStatus = new TDCatalogModel()
                         {
                             ItemId = qg.Key.QueryStatusId,
                             ItemName = qg.Key.QueryStatusName,
                         },
                         Enable = qg.Key.QueryEnable == 1 ? true : false,
                         PeriodPublicId = qg.Key.PeriodPublicId,
                         CreateDate = qg.Key.QueryCreateDate,
                         
                     }).ToList();
            }
            return oReturn;
        }

        public List<Models.TDQueryModel> ThirdKnowledgeSearchByPublicId(string CustomerPublicId, string QueryPublicId, bool Enable, int PageNumber, int RowCount, out int TotalRows)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vCustomerPublicId", CustomerPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vQueryPublicId", QueryPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable));
            lstParams.Add(DataInstance.CreateTypedParameter("vPageNumber", PageNumber));
            lstParams.Add(DataInstance.CreateTypedParameter("vRowCount", RowCount));


            TotalRows = 0;

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "MP_TK_GetQueryByPublicId",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            List<Models.TDQueryModel> oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                TotalRows = response.DataTableResult.Rows[0].Field<int>("TotalRows");

                oReturn =
                    (from q in response.DataTableResult.AsEnumerable()
                     where !q.IsNull("QueryPublicId")
                     group q by new
                     {
                         SearchTypeId = q.Field<int>("SearchTypeId"),
                         SearchTypeName = q.Field<string>("SearchTypeName"),
                         User = q.Field<string>("User"),
                         QueryStatusId = q.Field<int>("QueryStatusId"),
                         QueryStatusName = q.Field<string>("QueryStatusName"),
                         IsSuccess = q.Field<UInt64>("IsSuccess"),
                         QueryEnable = q.Field<UInt64>("QueryEnable"),
                         PeriodPublicId = q.Field<string>("PeriodPublicId"),
                         QueryPublicId = q.Field<string>("QueryPublicId"),
                         QueryCreateDate = q.Field<DateTime>("QueryCreateDate"),
                     }
                         into qg
                     select new Models.TDQueryModel()
                     {
                         QueryPublicId = qg.Key.QueryPublicId,
                         IsSuccess = qg.Key.IsSuccess == 1 ? true : false,
                         SearchType = new TDCatalogModel()
                         {
                             ItemId = qg.Key.SearchTypeId,
                             ItemName = qg.Key.SearchTypeName,
                         },
                         User = qg.Key.User,
                         QueryStatus = new TDCatalogModel()
                         {
                             ItemId = qg.Key.QueryStatusId,
                             ItemName = qg.Key.QueryStatusName,
                         },
                         Enable = qg.Key.QueryEnable == 1 ? true : false,
                         PeriodPublicId = qg.Key.PeriodPublicId,
                         CreateDate = qg.Key.QueryCreateDate,
                         RelatedQueryInfoModel =
                            (from qinf in response.DataTableResult.AsEnumerable()
                             where !qinf.IsNull("QueryBasicInfoId") &&
                                    qinf.Field<string>("QueryPublicId") == qg.Key.QueryPublicId
                             group qinf by new
                             {
                                 QueryBasicInfoId = qinf.Field<int>("QueryBasicInfoId"),
                                 QueryBasicPublicId = qinf.Field<string>("QueryBasicPublicId"),
                                 QueryPublicId = qinf.Field<string>("QueryPublicId"),
                                 NameResult = qinf.Field<string>("NameResult"),
                                 IdentificationResult = qinf.Field<string>("IdentificationResult"),
                                 Priority = qinf.Field<string>("Priority"),
                                 Peps = qinf.Field<string>("Peps"),
                                 Status = qinf.Field<string>("Status"),
                                 Alias = qinf.Field<string>("Alias"),
                                 Offense = qinf.Field<string>("Offense"),
                                 InfoLastModify = qinf.Field<DateTime>("InfoLastModify"),
                                 InfoCreateDate = qinf.Field<DateTime>("InfoCreateDate"),
                                 QueryInfoEnable = qinf.Field<UInt64>("QueryInfoEnable") == 1 ? true : false,
                             }
                                 into qinfg
                             select new Models.TDQueryInfoModel()
                             {
                                 //QueryBasicInfoId = qinfg.Key.QueryBasicInfoId,
                                 //QueryBasicPublicId = qinfg.Key.QueryBasicPublicId,
                                 //QueryPublicId = qinfg.Key.QueryPublicId,
                                 //NameResult = qinfg.Key.NameResult,
                                 //IdentificationResult = qinfg.Key.IdentificationResult,
                                 //Priority = qinfg.Key.Priority,
                                 //Peps = qinfg.Key.Peps,
                                 //Status = qinfg.Key.Status,
                                 //Alias = qinfg.Key.Alias,
                                 //Offense = qinfg.Key.Offense,
                                 //LastModify = qinfg.Key.InfoLastModify,
                                 //CreateDate = qinfg.Key.InfoCreateDate,
                                 //DetailInfo =
                                 //    (from qdinf in response.DataTableResult.AsEnumerable()
                                 //     where !qdinf.IsNull("QueryDetailInfoId") &&
                                 //     qdinf.Field<int>("QueryBasicInfoId") == qinfg.Key.QueryBasicInfoId
                                 //     group qdinf by new
                                 //     {
                                 //         QueryDetailInfoId = qdinf.Field<int>("QueryDetailInfoId"),
                                 //         QueryDetailInfoTypeId = qdinf.Field<int>("QueryDetailInfoTypeId"),
                                 //         QueryDetailInfoTypeName = qdinf.Field<string>("QueryDetailInfoTypeName"),
                                 //         QueryDetailInfoValue = qdinf.Field<string>("QueryDetailInfoValue"),
                                 //         QueryDetailInfoLargeValue = qdinf.Field<string>("QueryDetailInfoLargeValue"),
                                 //         QueryDetailInfoCreateDate = qdinf.Field<DateTime>("QueryDetailInfoCreateDate"),
                                 //         QueryDetailInfoLastModify = qdinf.Field<DateTime>("QueryDetailInfoLastModify"),
                                 //         QueryDetailInfoEnable = qdinf.Field<UInt64>("QueryDetailInfoEnable"),
                                 //     }
                                 //     into qdinfg
                                 //     select new TDQueryDetailInfoModel()
                                 //     {
                                 //         QueryDetailInfoId = qdinfg.Key.QueryDetailInfoId,
                                 //         QueryBasicPublicId = qinfg.Key.QueryBasicPublicId,
                                 //         ItemInfoType = new TDCatalogModel()
                                 //         {
                                 //             ItemId = qdinfg.Key.QueryDetailInfoTypeId,
                                 //             ItemName = qdinfg.Key.QueryDetailInfoTypeName,
                                 //         },
                                 //         Value = qdinfg.Key.QueryDetailInfoValue,
                                 //         LargeValue = qdinfg.Key.QueryDetailInfoLargeValue,
                                 //         LastModify = qdinfg.Key.QueryDetailInfoLastModify,
                                 //         CreateDate = qdinfg.Key.QueryDetailInfoCreateDate,
                                 //         Enable = qdinfg.Key.QueryDetailInfoEnable == 1 ? true : false,
                                 //     }).ToList(),
                             }).ToList()
                     }).ToList();
            }

            return oReturn;
        }

        #region Queries

        public async Task<string> QueryUpsert(string QueryPublicId, string PeriodPublicId, int SearchType, string User, string FileName, bool isSuccess, int QueryStatusId, bool Enable)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vPeriodPublicId", PeriodPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vQueryPublicId", QueryPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vSearchType", SearchType));
            lstParams.Add(DataInstance.CreateTypedParameter("vFileName", FileName));
            lstParams.Add(DataInstance.CreateTypedParameter("vQueryStatusId", QueryStatusId));
            lstParams.Add(DataInstance.CreateTypedParameter("vUser", User));
            lstParams.Add(DataInstance.CreateTypedParameter("vIsSuccess", isSuccess == true ? 1 : 0));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "MP_TK_QueryUpsert",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            if (response.ScalarResult != null)
                return response.ScalarResult.ToString();
            else
                return null;
        }

        public async Task<string> QueryInfoInsert(string vQueryPublicId, string vNameResult, string vIdentificationResult, string vPriority, 
                                    string vPeps, string vStatus, string vOffense, string vDocumentType,
                                     string vIdentificationNumber, string vFullName, string vIdList, string vListName,
                                     string vAKA, string vChargeOffense, string vMessage, string vQueryIdentification,
                                     string vQueryName, int vElasticId, string vGroupName, string vGroupId,
                                    string vLink, string vMoreInfo, string vZone, string vUrlFile,
                                    bool Enable)
        {
            List<IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vQueryPublicId", vQueryPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vNameResult", vNameResult));
            lstParams.Add(DataInstance.CreateTypedParameter("vIdentificationResult", vIdentificationResult));
            lstParams.Add(DataInstance.CreateTypedParameter("vPriority", vPriority));
            lstParams.Add(DataInstance.CreateTypedParameter("vPeps", vPeps));
            lstParams.Add(DataInstance.CreateTypedParameter("vStatus", vStatus));
            lstParams.Add(DataInstance.CreateTypedParameter("vOffense", vOffense));
            lstParams.Add(DataInstance.CreateTypedParameter("vDocumentType", vDocumentType));
            lstParams.Add(DataInstance.CreateTypedParameter("vIdentificationNumber", vIdentificationNumber));
            lstParams.Add(DataInstance.CreateTypedParameter("vFullName", vFullName));
            lstParams.Add(DataInstance.CreateTypedParameter("vIdList", vIdList));
            lstParams.Add(DataInstance.CreateTypedParameter("vListName", vListName));
            lstParams.Add(DataInstance.CreateTypedParameter("vAKA", vAKA));
            lstParams.Add(DataInstance.CreateTypedParameter("vChargeOffense", vChargeOffense));
            lstParams.Add(DataInstance.CreateTypedParameter("vMessage", vMessage));
            lstParams.Add(DataInstance.CreateTypedParameter("vQueryIdentification", vQueryIdentification));
            lstParams.Add(DataInstance.CreateTypedParameter("vQueryName", vQueryName));
            lstParams.Add(DataInstance.CreateTypedParameter("vElasticId", vElasticId));
            lstParams.Add(DataInstance.CreateTypedParameter("vGroupName", vGroupName));
            lstParams.Add(DataInstance.CreateTypedParameter("vGroupId", vGroupId));
            lstParams.Add(DataInstance.CreateTypedParameter("vLink", vLink));
            lstParams.Add(DataInstance.CreateTypedParameter("vMoreInfo", vMoreInfo));
            lstParams.Add(DataInstance.CreateTypedParameter("vZone", vZone));
            lstParams.Add(DataInstance.CreateTypedParameter("vUrlFile", vUrlFile));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "MP_TK_QueryInfoInsert",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            if (response.ScalarResult != null)
                return  response.ScalarResult.ToString();
            else
                return null;
        }
        public TDQueryInfoModel GetQueryInfoByInfoPublicId(string QueryInfoPublicId)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vQueryInfoPublicId", QueryInfoPublicId));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "TD_QueryInfo_GetByInfoPublicId",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            TDQueryInfoModel oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oReturn = new TDQueryInfoModel()
                {
                    QueryInfoId = response.DataTableResult.Rows[0].Field<int>("QueryInfoId"),
                    QueryInfoPublicId = response.DataTableResult.Rows[0].Field<string>("QueryInfoPublicId"),
                    QueryPublicId = response.DataTableResult.Rows[0].Field<string>("QueryPublicId"),
                    NameResult = response.DataTableResult.Rows[0].Field<string>("NameResult"),
                    IdentificationResult = response.DataTableResult.Rows[0].Field<string>("IdentificationResult"),
                    Priority = response.DataTableResult.Rows[0].Field<string>("Priority"),
                    Peps = response.DataTableResult.Rows[0].Field<string>("Peps"),
                    Status = response.DataTableResult.Rows[0].Field<string>("Status"),
                    Offense = response.DataTableResult.Rows[0].Field<string>("Offense"),
                    DocumentType = response.DataTableResult.Rows[0].Field<string>("DocumentType"),
                    IdentificationNumber = response.DataTableResult.Rows[0].Field<string>("IdentificationNumber"),
                    FullName = response.DataTableResult.Rows[0].Field<string>("FullName"),
                    IdList = response.DataTableResult.Rows[0].Field<string>("IdList"),
                    ListName = response.DataTableResult.Rows[0].Field<string>("ListName"),
                    AKA = response.DataTableResult.Rows[0].Field<string>("AKA"),
                    ChargeOffense = response.DataTableResult.Rows[0].Field<string>("ChargeOffense"),
                    Message = response.DataTableResult.Rows[0].Field<string>("Message"),
                    QueryIdentification = response.DataTableResult.Rows[0].Field<string>("QueryIdentification"),
                    QueryName = response.DataTableResult.Rows[0].Field<string>("QueryName"),
                    ElasticId = Convert.ToInt32(response.DataTableResult.Rows[0].Field<string>("ElasticId")),
                    GroupName = response.DataTableResult.Rows[0].Field<string>("GroupName"),
                    Link = response.DataTableResult.Rows[0].Field<string>("Link"),
                    MoreInfo = response.DataTableResult.Rows[0].Field<string>("MoreInfo"),
                    Zone = response.DataTableResult.Rows[0].Field<string>("Zone"),
                    UrlFile = response.DataTableResult.Rows[0].Field<string>("UrlFile"),
                    GroupId = response.DataTableResult.Rows[0].Field<string>("GroupId"),               
                    LastModify = response.DataTableResult.Rows[0].Field<DateTime>("LastModify"),
                    CreateDate = response.DataTableResult.Rows[0].Field<DateTime>("CreateDate"),
                    Enable = response.DataTableResult.Rows[0].Field<UInt64>("Enable") == 1 ? true : false,                   
                };
            }
            return oReturn;
        }

        public TDQueryInfoModel GetQueryInfoByQueryPublicIdAndElasticId(string vQueryPublicId, int vElasticId)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vQueryPublicId", vQueryPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vElasticId", vElasticId));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "MP_TK_GetQueryByQueryAndElasticId",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            TDQueryInfoModel oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oReturn = new TDQueryInfoModel()
                {
                    QueryInfoId = response.DataTableResult.Rows[0].Field<int>("QueryInfoId"),
                    QueryInfoPublicId = response.DataTableResult.Rows[0].Field<string>("QueryInfoPublicId"),
                    QueryPublicId = response.DataTableResult.Rows[0].Field<string>("QueryPublicId"),
                    NameResult = response.DataTableResult.Rows[0].Field<string>("NameResult"),
                    IdentificationResult = response.DataTableResult.Rows[0].Field<string>("IdentificationResult"),
                    Priority = response.DataTableResult.Rows[0].Field<string>("Priority"),
                    Peps = response.DataTableResult.Rows[0].Field<string>("Peps"),
                    Status = response.DataTableResult.Rows[0].Field<string>("Status"),
                    Offense = response.DataTableResult.Rows[0].Field<string>("Offense"),
                    DocumentType = response.DataTableResult.Rows[0].Field<string>("DocumentType"),
                    IdentificationNumber = response.DataTableResult.Rows[0].Field<string>("IdentificationNumber"),
                    FullName = response.DataTableResult.Rows[0].Field<string>("FullName"),
                    IdList = response.DataTableResult.Rows[0].Field<string>("IdList"),
                    ListName = response.DataTableResult.Rows[0].Field<string>("ListName"),
                    AKA = response.DataTableResult.Rows[0].Field<string>("AKA"),
                    ChargeOffense = response.DataTableResult.Rows[0].Field<string>("ChargeOffense"),
                    Message = response.DataTableResult.Rows[0].Field<string>("Message"),
                    QueryIdentification = response.DataTableResult.Rows[0].Field<string>("QueryIdentification"),
                    QueryName = response.DataTableResult.Rows[0].Field<string>("QueryName"),
                    ElasticId = Convert.ToInt32(response.DataTableResult.Rows[0].Field<string>("ElasticId")),
                    GroupName = response.DataTableResult.Rows[0].Field<string>("GroupName"),
                    Link = response.DataTableResult.Rows[0].Field<string>("Link"),
                    MoreInfo = response.DataTableResult.Rows[0].Field<string>("MoreInfo"),
                    Zone = response.DataTableResult.Rows[0].Field<string>("Zone"),
                    UrlFile = response.DataTableResult.Rows[0].Field<string>("UrlFile"),
                    GroupId = response.DataTableResult.Rows[0].Field<string>("GroupId"),
                    LastModify = response.DataTableResult.Rows[0].Field<DateTime>("LastModify"),
                    CreateDate = response.DataTableResult.Rows[0].Field<DateTime>("CreateDate"),
                    Enable = response.DataTableResult.Rows[0].Field<UInt64>("Enable") == 1 ? true : false,
                };
            }
            return oReturn;
        }

        #endregion Queries

        #endregion MarketPlace

        #region Util

        public List<TDCatalogModel> CatalogGetThirdKnowledgeOptions()
        {
            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "U_Catalog_GetThirdKnowledgeOptions",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = null
            });

            List<TDCatalogModel> oReturn = new List<TDCatalogModel>();

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                    (from c in response.DataTableResult.AsEnumerable()
                     where !c.IsNull("ItemId")
                     select new TDCatalogModel()
                     {
                         CatalogId = c.Field<int>("CatalogId"),
                         CatalogName = c.Field<string>("CatalogName"),
                         ItemId = c.Field<int>("ItemId"),
                         ItemName = c.Field<string>("ItemName"),
                     }).ToList();
            }
            return oReturn;
        }

        #endregion Util

        #region BatchProcess

        public List<TDQueryModel> GetQueriesInProgress()
        {
            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "BP_TK_GetQueryToProcess",
                CommandType = System.Data.CommandType.StoredProcedure,
            });

            List<Models.TDQueryModel> oReturn = null;

            if (response.DataTableResult != null &&
               response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                     (from cm in response.DataTableResult.AsEnumerable()
                      where !cm.IsNull("QueryId")
                      group cm by new
                      {
                          QueryId = cm.Field<int>("QueryId"),
                          CompanyPublicId = cm.Field<string>("CompanyPublicId"),
                          QueryPublicId = cm.Field<string>("QueryPublicId"),
                          PeriodPublicId = cm.Field<string>("PeriodPublicId"),
                          SearchTypeId = cm.Field<int>("SearchTypeId"),
                          SearchTypeName = cm.Field<string>("SearchTypeName"),
                          User = cm.Field<string>("User"),
                          FileName = cm.Field<string>("FileName"),
                          QueryStatusId = cm.Field<int>("QueryStatusId"),
                          QueryStatusName = cm.Field<string>("QueryStatusName"),
                          CreateDate = cm.Field<DateTime>("QueryCreateDate"),
                          LastModify = cm.Field<DateTime>("QueryLastModify"),
                          Enable = cm.Field<UInt64>("QueryEnable") == 1 ? true : false,
                          IsSuccess = cm.Field<UInt64>("IsSuccess") == 1 ? true : false,
                      } into cmg
                      select new TDQueryModel()
                      {
                          PeriodPublicId = cmg.Key.PeriodPublicId,
                          CompayPublicId = cmg.Key.CompanyPublicId,
                          QueryPublicId = cmg.Key.QueryPublicId,
                          SearchType = new TDCatalogModel()
                          {
                              ItemId = cmg.Key.SearchTypeId,
                              ItemName = cmg.Key.SearchTypeName,
                          },
                          CreateDate = cmg.Key.CreateDate,
                          FileName = cmg.Key.FileName,
                          User = cmg.Key.User,
                          LastModify = cmg.Key.LastModify,
                          IsSuccess = cmg.Key.IsSuccess,
                          QueryStatus = new TDCatalogModel()
                          {
                              ItemId = cmg.Key.QueryStatusId,
                              ItemName = cmg.Key.QueryStatusName,
                          },
                      }).ToList();
            }
            return oReturn;
        }       

        #endregion BatchProcess

        public TreeModel GetAnswerByTreeidAndQuestion(int TreeType, string Question)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vTreeTypeId", TreeType));
            lstParams.Add(DataInstance.CreateTypedParameter("vQuestion", Question));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "U_GetAnswerByTreeIdAndQuestion",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            TreeModel oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oReturn = new TreeModel()
                {
                    TreeId = response.DataTableResult.Rows[0].Field<int>("TreeId"),
                    TreeName = response.DataTableResult.Rows[0].Field<string>("TreeName"),
                    TreeType = new TDCatalogModel()
                    {
                        ItemId = response.DataTableResult.Rows[0].Field<int>("TreeTypeId"),
                        ItemName = response.DataTableResult.Rows[0].Field<string>("TreeTypeName"),
                    },
                    LastModify = response.DataTableResult.Rows[0].Field<DateTime>("LastModify"),
                    CreateDate = response.DataTableResult.Rows[0].Field<DateTime>("CreateDate"),
                    Enable = response.DataTableResult.Rows[0].Field<UInt64>("Enable") == 1 ? true : false,

                    TreeItem = (from tinf in response.DataTableResult.AsEnumerable()
                                group tinf by new
                                {
                                    ParentItemId = tinf.Field<int>(""),
                                    ParentItemName = tinf.Field<string>(""),
                                    ChildItemId = tinf.Field<int>(""),
                                    ChildItemName = tinf.Field<string>(""),
                                }
                                    into gtinf
                                    select new TreeItemModel()
                                    {
                                        ParentItem = new TDCatalogModel()
                                        {
                                            ItemId = gtinf.Key.ParentItemId,
                                            ItemName = gtinf.Key.ParentItemName,
                                        },
                                        ChildItem = new TDCatalogModel() 
                                        {
                                            ItemId = gtinf.Key.ChildItemId,
                                            ItemName = gtinf.Key.ChildItemName,
                                        }
                                    }
                                ).ToList()
                };
            }
            return oReturn;
        }

    }
}