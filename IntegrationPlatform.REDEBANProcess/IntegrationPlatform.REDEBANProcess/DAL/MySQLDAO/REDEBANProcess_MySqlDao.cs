using IntegrationPlatform.REDEBANProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADO.Interfaces;
using IntegrationPlatform.REDEBANProcess.Interfaces;
using IntegrationPlatform.REDEBANProcess.Models;
using System.Data;
using IntegrationPlatform.REDEBANProcess.Models.Utils;

namespace IntegrationPlatform.REDEBANProcess.DAL.MySQLDAO
{
    internal class REDEBANProcess_MySqlDao : IIntegrationPlatformREDEBANProcessData
    {
        private ADO.Interfaces.IADO DataInstance;

        public REDEBANProcess_MySqlDao()
        {
            DataInstance = new ADO.MYSQL.MySqlImplement(Models.Constants.C_SettingsModuleName);
        }

        public List<CompanyModel> GetAllProviders()
        {

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "Redeban_GetAllProviders",
                CommandType = CommandType.StoredProcedure,
            });

            List<CompanyModel> oReturn = new List<CompanyModel>();

            if (response.DataTableResult != null && response.DataTableResult.Rows.Count > 0)
            {
                oReturn =
                    (
                        from rp in response.DataTableResult.AsEnumerable()
                        where !rp.IsNull("CompanyPublicId")
                        group rp by new
                        {
                            CompanyPublicId = rp.Field<string>("CompanyPublicId"),
                            CompanyName = rp.Field<string>("CompanyName"),
                            IdentificationNumber = rp.Field<string>("IdentificationNumber"),
                            Enable = rp.Field<UInt64>("Enable"),
                            LastModify = rp.Field<DateTime>("LastModify"),
                            CreateDate = rp.Field<DateTime>("CreateDate")
                        }
                         into rpg
                        select new CompanyModel()
                        {
                            CompanyPublicId = rpg.Key.CompanyPublicId,
                            CompanyName = rpg.Key.CompanyName,
                            IdentificationNumber = rpg.Key.IdentificationNumber,
                            Enable = rpg.Key.Enable == 1 ? true : false
                        }).ToList();
            }
            return oReturn;
        }

        public REDEBANInfoModel GetProviderInfo(string CustomerPublicId, string ProviderPublicId)
        {
            List<IDbDataParameter> lstparams = new List<IDbDataParameter>();

            lstparams.Add(DataInstance.CreateTypedParameter("vCustomerPublicId", CustomerPublicId));
            lstparams.Add(DataInstance.CreateTypedParameter("vProviderPublicId", ProviderPublicId));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataSet,
                CommandText = "Redeban_GetProviderInfo",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstparams
            });
            var oReturn = new REDEBANInfoModel();
            if (response.DataSetResult != null && response.DataSetResult.Tables.Count > 0)
            {
                oReturn =
                (
                    from pi in response.DataSetResult.Tables[0].AsEnumerable()
                    where !pi.IsNull("CompanyId")
                    group pi by new
                    {
                        CompanyId = pi.Field<int>("CompanyId"),
                    }
                    into pig
                    select new REDEBANInfoModel()
                    {
                        ProviderBasicInfo = new CompanyModel()
                        {
                            CompanyName = response.DataSetResult.Tables[0].Rows[0].Field<string>("CompanyName"),
                            CompanyPublicId = response.DataSetResult.Tables[0].Rows[0].Field<string>("CompanyPublicId"),
                            IdentificationNumber = response.DataSetResult.Tables[1].Rows[0].Field<string>("IdentificationNumber"),
                            Status = response.DataSetResult.Tables[0].Rows[0].Field<string>("Status"),
                            Representant = response.DataSetResult.Tables[1].Rows[0].Field<string>("Representant"),
                            Telephone = response.DataSetResult.Tables[1].Rows[0].Field<string>("Tel"),
                            City = response.DataSetResult.Tables[1].Rows[0].Field<string>("City"),
                            Enable = response.DataSetResult.Tables[0].Rows[0].Field<UInt64>("Enable") == 1 ? true : false
                        },

                        ProviderFullInfo = new CompanyModel()
                        {
                            
                        },

                        #region Legal Information-Chaimber Of Commerce

                        LegalInfo_ChaimberOfCommerce =
                        (
                            from coc in response.DataSetResult.Tables[2].AsEnumerable()
                            where !coc.IsNull("LegalId")
                            group coc by new
                            {
                                LegalId = coc.Field<int>("LegalId"),
                                LegalTypeId = coc.Field<int>("LegalTypeId"),
                                LegalTypeName = coc.Field<string>("LegalTypeName"),
                            }
                            into cocg
                            select new GenericItemModel()
                            {
                                ItemId = cocg.Key.LegalId,
                                ItemName = cocg.Key.LegalTypeName,

                                ItemInfo =
                                (
                                  from coci in response.DataSetResult.Tables[2].AsEnumerable()
                                  where !coci.IsNull("LegalId")
                                  group coci by new
                                  {
                                      LegalInfoId = coci.Field<int>("LegalInfoId"),
                                      LegalInfoTypeId = coci.Field<int>("LegalInfoTypeId"),
                                      LegalTypeInfoName = coci.Field<string>("LegalInfoTypeName"),
                                      ItemId = coci.Field<int>("LegalInfoTypeId"),
                                      Value = coci.Field<string>("Value"),
                                      Enable = coci.Field<UInt64>("LegalInfoEnable") == 1 ? true : false,
                                      LastModify = coci.Field<DateTime>("LegalInfoLastModify"),
                                      CreateDate = coci.Field<DateTime>("LegalInfoCreateDate")
                                  }
                                  into cocig
                                  select new GenericItemInfoModel()
                                  {
                                      ItemInfoId = cocig.Key.LegalInfoId,
                                      ItemInfoType = new CatalogModel()
                                      {
                                          ItemId = cocig.Key.ItemId,
                                          ItemName = cocig.Key.LegalTypeInfoName,
                                      },
                                      Value = cocig.Key.Value,
                                      CreateDate = cocig.Key.CreateDate,
                                      LastModify = cocig.Key.LastModify,
                                      Enable = cocig.Key.Enable,
                                  }).ToList(),
                            }).ToList(),
                        #endregion
                        #region Legal Information - RUT

                        LegalInfo_RUT =
                        (
                            from rut in response.DataSetResult.Tables[3].AsEnumerable()
                            where !rut.IsNull("LegalId")
                            group rut by new
                            {
                                LegalId = rut.Field<int>("LegalId"),
                                LegalTypeId = rut.Field<int>("LegalTypeId"),
                                LegalTypeName = rut.Field<string>("LegalTypeName"),
                            }
                            into rutg
                            select new GenericItemModel()
                            {
                                ItemId = rutg.Key.LegalId,
                                ItemName = rutg.Key.LegalTypeName,

                                ItemInfo =
                                (
                                  from ruti in response.DataSetResult.Tables[3].AsEnumerable()
                                  where !ruti.IsNull("LegalInfoId") && ruti.Field<int>("LegalId") == rutg.Key.LegalId
                                  group ruti by new
                                  {
                                      LegalInfoId = ruti.Field<int>("LegalInfoId"),
                                      LegalInfoTypeId = ruti.Field<int>("LegalInfoTypeId"),
                                      LegalTypeInfoName = ruti.Field<string>("LegalInfoTypeName"),
                                      ItemId = ruti.Field<int>("LegalInfoTypeId"),
                                      Value = ruti.Field<string>("Value"),
                                      Enable = ruti.Field<UInt64>("LegalInfoEnable") == 1 ? true : false,
                                      LastModify = ruti.Field<DateTime>("LegalInfoLastModify"),
                                      CreateDate = ruti.Field<DateTime>("LegalInfoCreateDate")
                                  }
                                  into rutig
                                  select new GenericItemInfoModel()
                                  {
                                      ItemInfoId = rutig.Key.LegalInfoId,
                                      ItemInfoType = new CatalogModel()
                                      {
                                          ItemId = rutig.Key.ItemId,
                                          ItemName = rutig.Key.LegalTypeInfoName,
                                      },
                                      Value = rutig.Key.Value,
                                      CreateDate = rutig.Key.CreateDate,
                                      LastModify = rutig.Key.LastModify,
                                      Enable = rutig.Key.Enable,
                                  }).ToList(),
                            }).ToList(),
                        #endregion
                        #region Financial Information - Financial Stats

                        FinancialInfo_FinStats =
                        (
                            from fn in response.DataSetResult.Tables[4].AsEnumerable()
                            where !fn.IsNull("FinancialId")
                            group fn by new
                            {
                                FinancialId = fn.Field<int>("FinancialId"),
                                FinancialTypeId = fn.Field<int>("FinancialTypeId"),
                                FinancialTypeName = fn.Field<string>("FinancialTypeName"),
                            }
                            into fng
                            select new GenericItemModel()
                            {
                                ItemId = fng.Key.FinancialId,
                                ItemName = fng.Key.FinancialTypeName,

                                ItemInfo =
                                (
                                  from fni in response.DataSetResult.Tables[4].AsEnumerable()
                                  where !fni.IsNull("FinancialId") && fni.Field<int>("FinancialId") == fng.Key.FinancialId
                                  group fni by new
                                  {
                                      FinancialId = fni.Field<int>("FinancialId"),
                                      FinancialInfoId = fni.Field<int>("FinancialInfoId"),
                                      FinancialInfoTypeId = fni.Field<int>("FinancialInfoTypeId"),
                                      FinancialTypeInfoName = fni.Field<string>("FinancialInfoTypeName"),
                                      ItemId = fni.Field<int>("FinancialInfoTypeId"),
                                      Value = fni.Field<string>("Value"),
                                      Enable = fni.Field<UInt64>("FinancialInfoEnable") == 1 ? true : false,
                                      LastModify = fni.Field<DateTime>("FinancialInfoLastModify"),
                                      CreateDate = fni.Field<DateTime>("FinancialInfoCreateDate")
                                  }
                                  into fnig
                                  select new GenericItemInfoModel()
                                  {
                                      ItemInfoId = fnig.Key.FinancialInfoId,
                                      ItemInfoType = new CatalogModel()
                                      {
                                          ItemId = fnig.Key.ItemId,
                                          ItemName = fnig.Key.FinancialTypeInfoName,
                                      },
                                      Value = fnig.Key.Value,
                                      CreateDate = fnig.Key.CreateDate,
                                      LastModify = fnig.Key.LastModify,
                                      Enable = fnig.Key.Enable,
                                  }).ToList(),
                            }).ToList(),
                        #endregion
                        #region Financial Information - Bank Certification

                        FinancialInfo_BankCert =
                        (
                            from fn in response.DataSetResult.Tables[5].AsEnumerable()
                            where !fn.IsNull("FinancialId")
                            group fn by new
                            {
                                FinancialId = fn.Field<int>("FinancialId"),
                                FinancialTypeId = fn.Field<int>("FinancialTypeId"),
                                FinancialTypeName = fn.Field<string>("FinancialTypeName"),
                            }
                            into fng
                            select new GenericItemModel()
                            {
                                ItemId = fng.Key.FinancialId,
                                ItemName = fng.Key.FinancialTypeName,

                                ItemInfo =
                                (
                                  from fni in response.DataSetResult.Tables[5].AsEnumerable()
                                  where !fni.IsNull("FinancialId") && fni.Field<int>("FinancialId") == fng.Key.FinancialId
                                  group fni by new
                                  {
                                      FinancialId = fni.Field<int>("FinancialId"),
                                      FinancialInfoId = fni.Field<int>("FinancialInfoId"),
                                      FinancialInfoTypeId = fni.Field<int>("FinancialInfoTypeId"),
                                      FinancialTypeInfoName = fni.Field<string>("FinancialInfoTypeName"),
                                      ItemId = fni.Field<int>("FinancialInfoTypeId"),
                                      Value = fni.Field<string>("Value"),
                                      Enable = fni.Field<UInt64>("FinancialInfoEnable") == 1 ? true : false,
                                      LastModify = fni.Field<DateTime>("FinancialInfoLastModify"),
                                      CreateDate = fni.Field<DateTime>("FinancialInfoCreateDate")
                                  }
                                  into fnig
                                  select new GenericItemInfoModel()
                                  {
                                      ItemInfoId = fnig.Key.FinancialInfoId,
                                      ItemInfoType = new CatalogModel()
                                      {
                                          ItemId = fnig.Key.ItemId,
                                          ItemName = fnig.Key.FinancialTypeInfoName,
                                      },
                                      Value = fnig.Key.Value,
                                      CreateDate = fnig.Key.CreateDate,
                                      LastModify = fnig.Key.LastModify,
                                      Enable = fnig.Key.Enable,
                                  }).ToList(),
                            }).ToList(),
                        #endregion
                        #region Commercial Info - Experience Certification

                        Commercial_CertExp =
                        (
                            from fn in response.DataSetResult.Tables[6].AsEnumerable()
                            where !fn.IsNull("CommercialId")
                            group fn by new
                            {
                                CommercialId = fn.Field<int>("CommercialId"),
                                CommercialTypeId = fn.Field<int>("CommercialTypeId"),
                                CommercialTypeName = fn.Field<string>("CommercialTypeName"),
                            }
                            into fng
                            select new GenericItemModel()
                            {
                                ItemId = fng.Key.CommercialId,
                                ItemName = fng.Key.CommercialTypeName,

                                ItemInfo =
                                (
                                  from fni in response.DataSetResult.Tables[6].AsEnumerable()
                                  where !fni.IsNull("CommercialId") && fni.Field<int>("CommercialId") == fng.Key.CommercialId
                                  group fni by new
                                  {
                                      CommercialId = fni.Field<int>("CommercialId"),
                                      CommercialInfoId = fni.Field<int>("CommercialInfoId"),
                                      CommercialInfoTypeId = fni.Field<int>("CommercialInfoTypeId"),
                                      CommercialTypeInfoName = fni.Field<string>("CommercialInfoTypeName"),
                                      ItemId = fni.Field<int>("CommercialInfoTypeId"),
                                      Value = fni.Field<string>("Value"),
                                      Enable = fni.Field<UInt64>("CommercialInfoEnable") == 1 ? true : false,
                                      LastModify = fni.Field<DateTime>("CommercialInfoLastModify"),
                                      CreateDate = fni.Field<DateTime>("CommercialInfoCreateDate")
                                  }
                                  into fnig
                                  select new GenericItemInfoModel()
                                  {
                                      ItemInfoId = fnig.Key.CommercialInfoId,
                                      ItemInfoType = new CatalogModel()
                                      {
                                          ItemId = fnig.Key.ItemId,
                                          ItemName = fnig.Key.CommercialTypeInfoName,
                                      },
                                      Value = fnig.Key.Value,
                                      CreateDate = fnig.Key.CreateDate,
                                      LastModify = fnig.Key.LastModify,
                                      Enable = fnig.Key.Enable,
                                  }).ToList(),
                            }).ToList(),
                        #endregion
                        #region Certification HSEQ

                        HSEQ_Cert =
                        (
                            from fn in response.DataSetResult.Tables[7].AsEnumerable()
                            where !fn.IsNull("CertificationId")
                            group fn by new
                            {
                                CertificationId = fn.Field<int>("CertificationId"),
                                CertificationTypeId = fn.Field<int>("CertificationTypeId"),
                                CertificationTypeName = fn.Field<string>("CertificationTypeName"),
                            }
                            into fng
                            select new GenericItemModel()
                            {
                                ItemId = fng.Key.CertificationId,
                                ItemName = fng.Key.CertificationTypeName,
                                ItemInfo =
                                (
                                  from fni in response.DataSetResult.Tables[7].AsEnumerable()
                                  where !fni.IsNull("CertificationId") && fni.Field<int>("CertificationId") == fng.Key.CertificationId
                                  group fni by new
                                  {
                                      CertificationId = fni.Field<int>("CertificationId"),
                                      CertificationInfoId = fni.Field<int>("CertificationInfoId"),
                                      CertificationInfoTypeId = fni.Field<int>("CertificationInfoTypeId"),
                                      CertificationTypeInfoName = fni.Field<string>("CertificationInfoTypeName"),
                                      ItemId = fni.Field<int>("CertificationInfoTypeId"),
                                      Value = fni.Field<string>("Value"),
                                      Enable = fni.Field<UInt64>("CertificationInfoEnable") == 1 ? true : false,
                                      LastModify = fni.Field<DateTime>("CertificationInfoLastModify"),
                                      CreateDate = fni.Field<DateTime>("CertificationInfoCreateDate")
                                  }
                                  into fnig
                                  select new GenericItemInfoModel()
                                  {
                                      ItemInfoId = fnig.Key.CertificationInfoId,
                                      ItemInfoType = new CatalogModel()
                                      {
                                          ItemId = fnig.Key.ItemId,
                                          ItemName = fnig.Key.CertificationTypeInfoName,
                                      },
                                      Value = fnig.Key.Value,
                                      CreateDate = fnig.Key.CreateDate,
                                      LastModify = fnig.Key.LastModify,
                                      Enable = fnig.Key.Enable,
                                  }).ToList(),
                            }).ToList(),
                        #endregion
                        #region HSEQ Riskes
                        HSEQ_Riskes = (
                                      from fn in response.DataSetResult.Tables[8].AsEnumerable()
                                      where !fn.IsNull("CertificationId")
                                      group fn by new
                                      {
                                          CertificationId = fn.Field<int>("CertificationId"),
                                          CertificationTypeId = fn.Field<int>("CertificationTypeId"),
                                          CertificationTypeName = fn.Field<string>("CertificationTypeName"),
                                      }
                            into fng
                                      select new GenericItemModel()
                                      {
                                          ItemId = fng.Key.CertificationId,
                                          ItemName = fng.Key.CertificationTypeName,
                                          ItemInfo =
                                          (
                                            from fni in response.DataSetResult.Tables[8].AsEnumerable()
                                            where !fni.IsNull("CertificationId") && fni.Field<int>("CertificationId") == fng.Key.CertificationId
                                            group fni by new
                                            {
                                                CertificationId = fni.Field<int>("CertificationId"),
                                                CertificationInfoId = fni.Field<int>("CertificationInfoId"),
                                                CertificationInfoTypeId = fni.Field<int>("CertificationInfoTypeId"),
                                                CertificationTypeInfoName = fni.Field<string>("CertificationInfoTypeName"),
                                                ItemId = fni.Field<int>("CertificationInfoTypeId"),
                                                Value = fni.Field<string>("Value"),
                                                Enable = fni.Field<UInt64>("CertificationInfoEnable") == 1 ? true : false,
                                                LastModify = fni.Field<DateTime>("CertificationInfoLastModify"),
                                                CreateDate = fni.Field<DateTime>("CertificationInfoCreateDate")
                                            }
                                            into fnig
                                            select new GenericItemInfoModel()
                                            {
                                                ItemInfoId = fnig.Key.CertificationInfoId,
                                                ItemInfoType = new CatalogModel()
                                                {
                                                    ItemId = fnig.Key.ItemId,
                                                    ItemName = fnig.Key.CertificationTypeInfoName,
                                                },
                                                Value = fnig.Key.Value,
                                                CreateDate = fnig.Key.CreateDate,
                                                LastModify = fnig.Key.LastModify,
                                                Enable = fnig.Key.Enable,
                                            }).ToList(),
                                      }).ToList(),

                        #endregion
                        #region HSEQ - Health

                        HSEQ_Health =
                        (
                            from fn in response.DataSetResult.Tables[9].AsEnumerable()
                            where !fn.IsNull("CertificationId")
                            group fn by new
                            {
                                CertificationId = fn.Field<int>("CertificationId"),
                                CertificationTypeId = fn.Field<int>("CertificationTypeId"),
                                CertificationTypeName = fn.Field<string>("CertificationTypeName"),
                            }
                            into fng
                            select new GenericItemModel()
                            {
                                ItemId = fng.Key.CertificationId,
                                ItemName = fng.Key.CertificationTypeName,
                                ItemInfo =
                                (
                                  from fni in response.DataSetResult.Tables[9].AsEnumerable()
                                  where !fni.IsNull("CertificationId") && fni.Field<int>("CertificationId") == fng.Key.CertificationId
                                  group fni by new
                                  {
                                      CertificationId = fni.Field<int>("CertificationId"),
                                      CertificationInfoId = fni.Field<int>("CertificationInfoId"),
                                      CertificationInfoTypeId = fni.Field<int>("CertificationInfoTypeId"),
                                      CertificationTypeInfoName = fni.Field<string>("CertificationInfoTypeName"),
                                      ItemId = fni.Field<int>("CertificationInfoTypeId"),
                                      Value = fni.Field<string>("Value"),
                                      Enable = fni.Field<UInt64>("CertificationInfoEnable") == 1 ? true : false,
                                      LastModify = fni.Field<DateTime>("CertificationInfoLastModify"),
                                      CreateDate = fni.Field<DateTime>("CertificationInfoCreateDate")
                                  }
                                  into fnig
                                  select new GenericItemInfoModel()
                                  {
                                      ItemInfoId = fnig.Key.CertificationInfoId,
                                      ItemInfoType = new CatalogModel()
                                      {
                                          ItemId = fnig.Key.ItemId,
                                          ItemName = fnig.Key.CertificationTypeInfoName,
                                      },
                                      Value = fnig.Key.Value,
                                      CreateDate = fnig.Key.CreateDate,
                                      LastModify = fnig.Key.LastModify,
                                      Enable = fnig.Key.Enable,
                                  }).ToList(),
                            }).ToList(),
                        #endregion                        
                        #region HSEQ Accidents

                        HSEQ_Acc =
                        (
                                      from fn in response.DataSetResult.Tables[10].AsEnumerable()
                                      where !fn.IsNull("CertificationId")
                                      group fn by new
                                      {
                                          CertificationId = fn.Field<int>("CertificationId"),
                                          CertificationTypeId = fn.Field<int>("CertificationTypeId"),
                                          CertificationTypeName = fn.Field<string>("CertificationTypeName"),
                                      }
                            into fng
                                      select new GenericItemModel()
                                      {
                                          ItemId = fng.Key.CertificationId,
                                          ItemName = fng.Key.CertificationTypeName,
                                          ItemInfo =
                                          (
                                            from fni in response.DataSetResult.Tables[10].AsEnumerable()
                                            where !fni.IsNull("CertificationId") && fni.Field<int>("CertificationId") == fng.Key.CertificationId
                                            group fni by new
                                            {
                                                CertificationId = fni.Field<int>("CertificationId"),
                                                CertificationInfoId = fni.Field<int>("CertificationInfoId"),
                                                CertificationInfoTypeId = fni.Field<int>("CertificationInfoTypeId"),
                                                CertificationTypeInfoName = fni.Field<string>("CertificationInfoTypeName"),
                                                ItemId = fni.Field<int>("CertificationInfoTypeId"),
                                                Value = fni.Field<string>("Value"),
                                                Enable = fni.Field<UInt64>("CertificationInfoEnable") == 1 ? true : false,
                                                LastModify = fni.Field<DateTime>("CertificationInfoLastModify"),
                                                CreateDate = fni.Field<DateTime>("CertificationInfoCreateDate")
                                            }
                                            into fnig
                                            select new GenericItemInfoModel()
                                            {
                                                ItemInfoId = fnig.Key.CertificationInfoId,
                                                ItemInfoType = new CatalogModel()
                                                {
                                                    ItemId = fnig.Key.ItemId,
                                                    ItemName = fnig.Key.CertificationTypeInfoName,
                                                },
                                                Value = fnig.Key.Value,
                                                CreateDate = fnig.Key.CreateDate,
                                                LastModify = fnig.Key.LastModify,
                                                Enable = fnig.Key.Enable,
                                            }).ToList(),
                                      }).ToList(),
                        #endregion
                        #region Aditional Documents
                        AditionalDocuments = new List<GenericItemModel>()
                        #endregion

                    }).First();
            }
            return oReturn;
        }

        public int RedebanProcessLogUpsert(int RedebanProcessLogId, string ProceesName, string FileName, bool SendStatus, bool IsSucces, bool Enable)
        {
            List<IDbDataParameter> lstparams = new List<IDbDataParameter>();

            lstparams.Add(DataInstance.CreateTypedParameter("vRedebanProcessLogId", RedebanProcessLogId));
            lstparams.Add(DataInstance.CreateTypedParameter("vProcessName", ProceesName));
            lstparams.Add(DataInstance.CreateTypedParameter("vFileName", FileName));
            lstparams.Add(DataInstance.CreateTypedParameter("vSendStatus", SendStatus == true ? 1 : 0));
            lstparams.Add(DataInstance.CreateTypedParameter("vIsSucces", IsSucces == true ? 1 : 0));
            lstparams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));


            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "Redeban_ProcessLog_Upsert",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstparams
            });

            return Convert.ToInt32(response.ScalarResult);
        }
    }
}
