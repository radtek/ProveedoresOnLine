﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.Controller
{
    public class IntegrationPlatform
    {
        #region Integration

        public static List<Models.Integration.CustomDataModel> CustomerProvider_GetField(List<ProveedoresOnLine.Company.Models.Company.CompanyModel> RelatedCustomer, string ProviderPublicId)
        {
            List<Models.Integration.CustomDataModel> oReturn = new List<Models.Integration.CustomDataModel>();

            if (RelatedCustomer != null &&
                RelatedCustomer.Count > 0)
            {
                RelatedCustomer.All(Customer =>
                {
                    oReturn.Add(DAL.Controller.IntegrationPlatformDataController.Instance.CustomerProvider_GetCustomData(Customer, ProviderPublicId));

                    return true;
                });
            }

            return oReturn;
        }

        #region Integration Sanofi

        public static Models.Integration.CustomDataModel CustomerProvider_CustomData_Upsert(Models.Integration.CustomDataModel CustomData, string ProviderPublicId)
        {
            string RelatedCustomer = CustomData.RelatedCompany.CompanyPublicId;

            switch (RelatedCustomer)
            {
                case Models.Constants.C_POL_CustomerPublicId_Sanofi: //SANOFI

                    #region Custom Data

                    if (CustomData.CustomData != null &&
                        CustomData.CustomData.Count > 0)
                    {
                        CustomData.CustomData.All(dt =>
                        {
                            LogManager.Models.LogModel oLog = ProveedoresOnLine.Company.Controller.Company.GetGenericLogModel();
                            try
                            {
                                dt.ItemId = DAL.Controller.IntegrationPlatformDataController.Instance.Sanofi_AditionalData_Upsert(
                                    dt.ItemId,
                                    ProviderPublicId,
                                    dt.ItemType != null ? dt.ItemType.ItemId : 0,
                                    dt.ItemName,
                                    dt.Enable);

                                dt = CustomerProvider_CustomDataInfo_Upsert(dt);

                                oLog.IsSuccess = true;
                            }
                            catch (Exception err)
                            {
                                oLog.IsSuccess = false;
                                oLog.Message = err.Message + " - " + err.StackTrace;

                                throw err;
                            }
                            finally
                            {
                                oLog.LogObject = dt;

                                oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                                {
                                    LogInfoType = "CustomData",
                                    Value = dt.ItemId.ToString(),
                                });

                                LogManager.ClientLog.AddLog(oLog);
                            }

                            return true;
                        });
                    }
                    #endregion

                    break;
            }

            return CustomData;
        }

        public static ProveedoresOnLine.Company.Models.Util.GenericItemModel CustomerProvider_CustomDataInfo_Upsert(ProveedoresOnLine.Company.Models.Util.GenericItemModel oCustomDataToUpsert)
        {
            if (oCustomDataToUpsert != null &&
                oCustomDataToUpsert.ItemInfo != null &&
                oCustomDataToUpsert.ItemInfo.Count > 0)
            {
                oCustomDataToUpsert.ItemInfo.All(dti =>
                {
                    LogManager.Models.LogModel oLog = ProveedoresOnLine.Company.Controller.Company.GetGenericLogModel();

                    try
                    {
                        dti.ItemInfoId = DAL.Controller.IntegrationPlatformDataController.Instance.Sanofi_AditionalDataInfo_Upsert(
                            dti.ItemInfoId,
                            oCustomDataToUpsert.ItemId,
                            dti.ItemInfoType != null ? dti.ItemInfoType.ItemId : 0,
                            dti.Value,
                            dti.LargeValue,
                            dti.Enable);

                        oLog.IsSuccess = true;
                    }
                    catch (Exception err)
                    {
                        oLog.IsSuccess = false;
                        oLog.Message = err.Message + " - " + err.StackTrace;

                        throw err;
                    }
                    finally
                    {
                        oLog.LogObject = dti;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "CustomDataInfo",
                            Value = dti.ItemInfoId.ToString(),
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }

            return oCustomDataToUpsert;
        }

        #endregion

        #endregion
    }
}
