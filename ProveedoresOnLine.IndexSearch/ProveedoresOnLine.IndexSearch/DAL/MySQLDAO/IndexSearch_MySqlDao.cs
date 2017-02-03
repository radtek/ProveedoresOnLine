using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.IndexSearch.Interfaces;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.SurveyModule.Models.Index;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.IndexSearch.DAL.MySQLDAO
{
    internal class IndexSearch_MySqlDao : IIndexSearch
    {
        private ADO.Interfaces.IADO DataInstance;
        private ADO.Interfaces.IADO DataInstanceTopbls;

        public IndexSearch_MySqlDao()
        {
            DataInstance = new ADO.MYSQL.MySqlImplement(Models.Constants.C_POL_SearchConnectionName);
            DataInstanceTopbls = new ADO.MYSQL.MySqlImplement(Models.Constants.C_Topbls_SearchConnectionName);
        }

        #region Company Index

        public List<CompanyIndexModel> GetCompanyIndex()
        {
            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataSet,
                CommandText = "C_IndexCompany",
                CommandType = System.Data.CommandType.StoredProcedure,
            });

            List<Company.Models.Company.CompanyIndexModel> oReturn = null;

            if (response.DataSetResult != null &&
                response.DataSetResult.Tables[0] != null &&
                response.DataSetResult.Tables[0].Rows.Count > 0 &&
                response.DataSetResult.Tables[1] != null &&
                response.DataSetResult.Tables[1].Rows.Count > 0)
            {
                oReturn =
                    (from ci in response.DataSetResult.Tables[1].AsEnumerable()
                     where !ci.IsNull("CompanyPublicId")
                     group ci by new
                     {
                         CompanyPublicId = ci.Field<string>("CompanyPublicId"),
                         CompanyName = ci.Field<string>("CompanyName"),
                         CompanyIdentificationTypeId = !ci.IsNull("CompanyIdentificationTypeId") ? ci.Field<int>("CompanyIdentificationTypeId") : 0,
                         CompanyIdentificationType = ci.Field<string>("CompanyIdentificationType"),
                         CompanyIdentificationNumber = ci.Field<string>("CompanyIdentificationNumber"),
                         CompanyEnable = ci.Field<UInt64>("CompanyEnable") == 1 ? true : false,
                         LogoUrl = ci.Field<string>("LogoUrl"),
                         CountryId = !ci.IsNull("CountryId") ? ci.Field<int>("CountryId") : 0,
                         Country = ci.Field<string>("Country"),
                         CityId = !ci.IsNull("CityId") ? ci.Field<int>("CityId") : 0,
                         City = ci.Field<string>("City"),
                         ICAId = !ci.IsNull("ICAId") ? ci.Field<int>("ICAId") : 0,
                         ICA = ci.Field<string>("ICA"),
                         InBlackList = ci.Field<int>("InBlackList") == 1 ? true : false,
                     }
                         into cig
                     select new CompanyIndexModel()
                     {
                         CompanyPublicId = cig.Key.CompanyPublicId,
                         CompanyName = cig.Key.CompanyName,
                         IdentificationTypeId = cig.Key.CompanyIdentificationTypeId,

                         IdentificationType = cig.Key.CompanyIdentificationType,

                         IdentificationNumber = cig.Key.CompanyIdentificationNumber,

                         CompanyEnable = cig.Key.CompanyEnable,
                         LogoUrl = cig.Key.LogoUrl,

                         CountryId = cig.Key.CountryId,
                         Country = cig.Key.Country,
                         CityId = cig.Key.CityId,
                         City = cig.Key.City,

                         ICAId = cig.Key.ICAId,
                         ICA = cig.Key.ICA,
                         InBlackList = cig.Key.InBlackList,

                         oCustomerProviderIndexModel =
                            (from cp in response.DataSetResult.Tables[0].AsEnumerable()
                             where !cp.IsNull("CustomerProviderId") &&
                                   cp.Field<string>("ProviderPublicId") == cig.Key.CompanyPublicId
                             group cp by new
                             {
                                 CustomerProviderId = !cp.IsNull("CustomerProviderId") ? cp.Field<int>("CustomerProviderId") : 0,
                                 CustomerPublicId = cp.Field<string>("CustomerPublicId"),
                                 StatusId = !cp.IsNull("StatusId") ? cp.Field<int>("StatusId") : 0,
                                 Status = cp.Field<string>("Status"),
                                 CustomerProviderEnable = cp.Field<UInt64>("CustomerProviderEnable") == 1 ? true : false,
                             }
                                 into cpg
                             select new CustomerProviderIndexModel()
                             {
                                 CustomerProviderId = cpg.Key.CustomerProviderId,
                                 CustomerPublicId = cpg.Key.CustomerPublicId,
                                 StatusId = cpg.Key.StatusId,
                                 Status = cpg.Key.Status,
                                 CustomerProviderEnable = cpg.Key.CustomerProviderEnable,
                             }).ToList()
                     }).ToList();
            }

            return oReturn;
        }

        public List<CompanyIndexModel> GetCompanyCustomerIndex()
        {
            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "C_IndexCustomerCompany",
                CommandType = System.Data.CommandType.StoredProcedure,
            });

            List<Company.Models.Company.CompanyIndexModel> oReturn = null;

            if (response.DataTableResult != null && response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                    (from ci in response.DataTableResult.AsEnumerable()
                     where !ci.IsNull("CompanyPublicId")
                     group ci by new
                     {
                         CompanyPublicId = ci.Field<string>("CompanyPublicId"),
                         CompanyName = ci.Field<string>("CompanyName"),
                         CompanyIdentificationType = ci.Field<string>("IdentificationType"),
                         CompanyIdentificationNumber = ci.Field<string>("Identification"),
                         CompanyEnable = ci.Field<UInt64>("Enable") == 1 ? true : false,
                         LogoUrl = ci.Field<string>("LogoUrl"),                                                  
                     }
                     into cig
                     select new CompanyIndexModel()
                     {
                         CompanyPublicId = cig.Key.CompanyPublicId,
                         CompanyName = cig.Key.CompanyName,                         
                         IdentificationType = cig.Key.CompanyIdentificationType,
                         IdentificationNumber = cig.Key.CompanyIdentificationNumber,
                         CompanyEnable = cig.Key.CompanyEnable,
                         LogoUrl = cig.Key.LogoUrl,                                                     
                     }).ToList();
            }

            return oReturn;
        }
        public List<CustomerProviderIndexModel> GetCustomerProviderIndex()
        {
            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "C_IndexCustomerProvider",
                CommandType = CommandType.StoredProcedure,
            });

            List<CustomerProviderIndexModel> oRetun = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oRetun =
                    (from cp in response.DataTableResult.AsEnumerable()
                     where !cp.IsNull("CustomerProviderId")
                     group cp by new
                     {
                         CustomerProviderId = cp.Field<int>("CustomerProviderId"),
                         CustomerPublicId = cp.Field<string>("CustomerPublicId"),
                         ProviderPublicId = cp.Field<string>("ProviderPublicId"),
                         StatusId = cp.Field<int>("StatusId"),
                         Status = cp.Field<string>("Status"),
                         CustomerProviderEnable = cp.Field<UInt64>("CustomerProviderEnable") == 1 ? true : false,
                     }
                         into cpg
                     select new CustomerProviderIndexModel()
                     {
                         CustomerProviderId = cpg.Key.CustomerProviderId,
                         CustomerPublicId = cpg.Key.CustomerPublicId,
                         ProviderPublicId = cpg.Key.ProviderPublicId,
                         StatusId = cpg.Key.StatusId,
                         Status = cpg.Key.Status,
                         CustomerProviderEnable = cpg.Key.CustomerProviderEnable,
                     }).ToList();
            }

            return oRetun;
        }

        #endregion

        #region Survey Index

        public List<CompanySurveyIndexModel> GetCompanySurveyIndex()
        {
            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataSet,
                CommandText = "C_IndexCompanySurvey",
                CommandType = CommandType.StoredProcedure,
            });

            List<CompanySurveyIndexModel> oReturn = null;

            if (response.DataSetResult != null &&
                response.DataSetResult.Tables[0] != null &&
                response.DataSetResult.Tables[0].Rows.Count > 0 &&
                response.DataSetResult.Tables[1] != null &&
                response.DataSetResult.Tables[1].Rows.Count > 0 &&
                response.DataSetResult.Tables[2] != null &&
                response.DataSetResult.Tables[2].Rows.Count > 0)
            {
                oReturn =
                    (from ci in response.DataSetResult.Tables[1].AsEnumerable()
                     where !ci.IsNull("CompanyPublicId")
                     group ci by new
                     {
                         CompanyPublicId = ci.Field<string>("CompanyPublicId"),
                         CompanyName = ci.Field<string>("CompanyName"),
                         CompanyIdentificationTypeId = !ci.IsNull("CompanyIdentificationTypeId") ? ci.Field<int>("CompanyIdentificationTypeId") : 0,
                         CompanyIdentificationType = ci.Field<string>("CompanyIdentificationType"),
                         CompanyIdentificationNumber = ci.Field<string>("CompanyIdentificationNumber"),
                         CompanyEnable = ci.Field<UInt64>("CompanyEnable") == 1 ? true : false,
                         LogoUrl = ci.Field<string>("LogoUrl"),
                         CountryId = !ci.IsNull("CountryId") ? ci.Field<int>("CountryId") : 0,
                         Country = ci.Field<string>("Country"),
                         CityId = !ci.IsNull("CityId") ? ci.Field<int>("CityId") : 0,
                         City = ci.Field<string>("City"),
                         ICAId = !ci.IsNull("ICAId") ? ci.Field<int>("ICAId") : 0,
                         ICA = ci.Field<string>("ICA"),
                     }
                         into cig
                     select new CompanySurveyIndexModel()
                     {
                         CompanyPublicId = cig.Key.CompanyPublicId,
                         CompanyName = cig.Key.CompanyName,
                         IdentificationTypeId = cig.Key.CompanyIdentificationTypeId,

                         IdentificationType = cig.Key.CompanyIdentificationType,

                         IdentificationNumber = cig.Key.CompanyIdentificationNumber,

                         CompanyEnable = cig.Key.CompanyEnable,
                         LogoUrl = cig.Key.LogoUrl,

                         CountryId = cig.Key.CountryId,
                         Country = cig.Key.Country,
                         CityId = cig.Key.CityId,
                         City = cig.Key.City,

                         ICAId = cig.Key.ICAId,
                         ICA = cig.Key.ICA,

                         oCustomerProviderIndexModel =
                            (from cp in response.DataSetResult.Tables[0].AsEnumerable()
                             where !cp.IsNull("CustomerProviderId") &&
                                   cp.Field<string>("ProviderPublicId") == cig.Key.CompanyPublicId
                             group cp by new
                             {
                                 CustomerProviderId = !cp.IsNull("CustomerProviderId") ? cp.Field<int>("CustomerProviderId") : 0,
                                 CustomerPublicId = cp.Field<string>("CustomerPublicId"),
                                 StatusId = !cp.IsNull("StatusId") ? cp.Field<int>("StatusId") : 0,
                                 Status = cp.Field<string>("Status"),
                                 CustomerProviderEnable = cp.Field<UInt64>("CustomerProviderEnable") == 1 ? true : false,
                             }
                                 into cpg
                             select new CustomerProviderIndexModel()
                             {
                                 CustomerProviderId = cpg.Key.CustomerProviderId,
                                 CustomerPublicId = cpg.Key.CustomerPublicId,
                                 StatusId = cpg.Key.StatusId,
                                 Status = cpg.Key.Status,
                                 CustomerProviderEnable = cpg.Key.CustomerProviderEnable,
                             }).ToList(),
                         oSurveyIndexModel =
                            (from si in response.DataSetResult.Tables[2].AsEnumerable()
                             where !si.IsNull("SurveyId") &&
                                    si.Field<string>("CompanyPublicId") == cig.Key.CompanyPublicId
                             group si by new
                             {
                                 SurveyPublicId = si.Field<string>("SurveyPublicId"),
                                 SurveyTypeId = si.Field<int>("SurveyTypeId"),
                                 SurveyType = si.Field<string>("SurveyType"),
                                 SurveyStatusId = si.Field<int>("SurveyStatusId"),
                                 SurveyStatus = si.Field<string>("SurveyStatus"),
                                 CompanyPublicId = si.Field<string>("CompanyPublicId"),
                                 CustomerPublicId = si.Field<string>("CustomerPublicId"),
                             }
                             into sig
                             select new SurveyIndexModel()
                             {
                                 CompanyPublicId = sig.Key.CompanyPublicId,
                                 CustomerPublicId = sig.Key.CustomerPublicId,
                                 SurveyPublicId = sig.Key.SurveyPublicId,
                                 SurveyTypeId = sig.Key.SurveyTypeId,
                                 SurveyType = sig.Key.SurveyType,
                                 SurveyStatusId = sig.Key.SurveyStatusId,
                                 SurveyStatus = sig.Key.SurveyStatus
                             }).ToList(),
                     }).ToList();
            }

            return oReturn;
        }


        public List<SurveyIndexSearchModel> GetSurveyIndex()
        {
            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "C_IndexSurvey",
                CommandType = CommandType.StoredProcedure,
            });

            List<SurveyIndexSearchModel> oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                    (from sv in response.DataTableResult.AsEnumerable()
                     where !sv.IsNull("CompanyPublicId")
                     group sv by new
                     {
                         CustomerPublicId = sv.Field<string>("CustomerPublicId"),
                         CompanyPublicId = sv.Field<string>("CompanyPublicId"),
                         SurveyPublicId = sv.Field<string>("SurveyPublicId"),
                         SurveyTypeId = sv.Field<int?>("SurveyTypeId").ToString(),
                         SurveyType = sv.Field<string>("SurveyType"),
                         SurveyStatusId = sv.Field<int?>("SurveyStatusId").ToString(),
                         SurveyStatus = sv.Field<string>("SurveyStatus"),
                         //SurveyUserId = sv.Field<int?>("UserId").ToString(),
                         //SurveyUser = sv.Field<string>("User"),
                     }
                         into svg
                     select new SurveyIndexSearchModel()
                     {
                         CustomerPublicId = svg.Key.CustomerPublicId,
                         CompanyPublicId = svg.Key.CompanyPublicId,
                         SurveyPublicId = svg.Key.SurveyPublicId,
                         SurveyTypeId = svg.Key.SurveyTypeId,
                         SurveyType = svg.Key.SurveyType,
                         SurveyStatusId = svg.Key.SurveyStatusId,
                         SurveyStatus = svg.Key.SurveyStatus,
                         //UserId = svg.Key.SurveyUserId,
                         //User = svg.Key.SurveyUser,
                     }).ToList();
            }

            return oReturn;
        }

        #region Survey Info Index

        public List<SurveyInfoIndexSearchModel> GetSurveyInfoIndex()
        {
            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "C_IndexSurveyInfo",
                CommandType = CommandType.StoredProcedure,
            });

            List<SurveyInfoIndexSearchModel> oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                    (from svinf in response.DataTableResult.AsEnumerable()
                     where !svinf.IsNull("ParentSurveyPublicId")
                     group svinf by new
                     {
                         ParentSurveyPublicId = svinf.Field<string>("ParentSurveyPublicId"),
                         SurveyPublicId = svinf.Field<string>("SurveyPublicId"),
                         UserEmail = svinf.Field<string>("UserEmail"),
                     }
                         into svinfg
                     select new SurveyInfoIndexSearchModel()
                     {
                         ParentSurveyPublicId = svinfg.Key.ParentSurveyPublicId,
                         SurveyPublicId = svinfg.Key.SurveyPublicId,
                         SurveyUserEmail = svinfg.Key.UserEmail,
                     }).ToList();
            }

            return oReturn;
        }

        #endregion

        #endregion

        #region Thirdknowledge Index

        public List<ThirdknowledgeIndexSearchModel> GetThirdknowledgeIndex(int vRowFrom, int vRowTo)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<System.Data.IDbDataParameter>();

            lstParams.Add(DataInstanceTopbls.CreateTypedParameter("vRowFrom", vRowFrom));
            lstParams.Add(DataInstanceTopbls.CreateTypedParameter("vRowTo", vRowTo));

            ADO.Models.ADOModelResponse response = DataInstanceTopbls.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "MP_TK_GetAllTOPBLSDATA",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });
            List<ThirdknowledgeIndexSearchModel> oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                    (from thk in response.DataTableResult.AsEnumerable()
                     where !thk.IsNull("REGISTRO")
                     group thk by new
                     {
                         Registry = Convert.ToInt64(thk.Field<string>("REGISTRO")),
                         TotalRows = thk.Field<int>("TotalRows"),
                         ListOrigin = thk.Field<string>("ORIGEN_LISTA"),
                         ListType = thk.Field<string>("TIPO_LISTA"),
                         Code = thk.Field<string>("CODIGO"),
                         CompleteName = thk.Field<string>("NOMBRECOMPLETO"),
                         FirstName = thk.Field<string>("PRIMER_NOMBRE"),
                         SecondName = thk.Field<string>("SEGUNDO_NOMBRE"),
                         FirstLastName = thk.Field<string>("PRIMER_APELLIDO"),
                         SecondLastName = thk.Field<string>("SEGUNDO_APELLIDO"),
                         PersonType = thk.Field<string>("TIPO_PERSONA"),
                         TypeId = thk.Field<string>("ID"),                         
                         RelatedWiht = thk.Field<string>("RELACIONADO_CON"),
                         ORoldescription1 = thk.Field<string>("ROL_O_DESCRIPCION1"),
                         ORoldescription2 = thk.Field<string>("ROL_O_DESCRIPCION2"),
                         AKA = thk.Field<string>("AKA"),
                         Source = thk.Field<string>("FUENTE"),
                         LastModify = thk.Field<string>("FECHA_UPDATE"),
                         FinalDateRol = thk.Field<string>("FECHA_FINAL_ROL"),
                         NationalitySourceCountry = thk.Field<string>("NACIONALIDAD_PAISDEORIGEN"),
                         Address = thk.Field<string>("DIRECCION"),
                         Status = thk.Field<string>("ESTADO"),
                         ImageKey = thk.Field<string>("LLAVEIMAGEN"),
                     }
                     into thkg 
                     select new ThirdknowledgeIndexSearchModel()
                     {
                         Registry = (UInt64)thkg.Key.Registry,
                         TotalRows = thkg.Key.TotalRows,
                         ListOrigin = thkg.Key.ListOrigin,
                         ListType = thkg.Key.ListType,
                         Code = thkg.Key.Code,
                         CompleteName = thkg.Key.CompleteName,
                         FirstName = thkg.Key.FirstName,
                         SecondName = thkg.Key.SecondName,
                         FirstLastName = thkg.Key.FirstLastName,
                         SecondLastName = thkg.Key.SecondLastName,
                         PersonType = thkg.Key.PersonType,
                         TypeId = thkg.Key.TypeId,                         
                         RelatedWiht = thkg.Key.RelatedWiht,
                         ORoldescription1 = thkg.Key.ORoldescription1,
                         ORoldescription2 = thkg.Key.ORoldescription2,
                         AKA = thkg.Key.AKA,
                         Source = thkg.Key.Source,
                         LastModify = thkg.Key.LastModify,
                         FinalDateRol = thkg.Key.FinalDateRol,
                         NationalitySourceCountry = thkg.Key.NationalitySourceCountry,
                         Address = thkg.Key.Address,
                         Status = thkg.Key.Status,
                         ImageKey = thkg.Key.ImageKey,

                     }).ToList();
            }

            return oReturn;
        }

        #endregion
    }
}