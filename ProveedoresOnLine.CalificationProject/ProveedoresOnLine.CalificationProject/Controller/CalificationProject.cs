using ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch;
using ProveedoresOnLine.CalificationProject.Models.CalificationProject;
using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.CompanyProvider.Models.Provider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProveedoresOnLine.CalificationProject.Controller
{
    public class CalificationProject
    {
        #region CalificationProjectConfigModule

        #region ProjectConfig

        public static CalificationProjectConfigModel CalificationProjectConfigUpsert(CalificationProjectConfigModel oConfigProject)
        {
            LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();

            try
            {
                if (oConfigProject != null)
                {
                    oConfigProject.CalificationProjectConfigId = DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigUpsert
                        (
                            oConfigProject.CalificationProjectConfigId,
                            oConfigProject.Company.CompanyPublicId,
                            oConfigProject.CalificationProjectConfigName,
                            oConfigProject.Enable
                        );
                    oLog.IsSuccess = true;
                }
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + " - " + err.StackTrace;
                throw err;
            }
            finally
            {
                oLog.User = "BatchProcess";
                oLog.LogObject = oConfigProject;
                oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                {
                    LogInfoType = "CalificationProjectConfigId",
                    Value = oConfigProject.CalificationProjectConfigId.ToString()
                });
                LogManager.ClientLog.AddLog(oLog);
            }
            return oConfigProject;
        }

        public static List<CalificationProjectConfigModel> CalificationProjectConfigGetByCompanyId(string CompanyPublicId, bool Enable)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfig_GetByCompanyId(CompanyPublicId, Enable);
        }

        public static List<Models.CalificationProject.CalificationProjectConfigModel> CalificationProjectConfig_GetAll()
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfig_GetAll();
        }

        public static Models.CalificationProject.CalificationProjectConfigModel CalificationProjectConfig_GetByCalificationProjectConfigId(int CalificationProjectConfigId)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfig_GetByCalificationProjectConfigId(CalificationProjectConfigId);
        }

        public static List<CalificationProjectConfigModel> CalificationProjectConfigGetByProvider(string ProviderPublicId)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigGetByProvider(ProviderPublicId);
        }
        #endregion

        #region ProjectConfigInfo

        public static Models.CalificationProject.ConfigInfoModel CalificationProjectConfigInfoUpsert(ConfigInfoModel oConfigInfo)
        {
            LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();
            try
            {
                if (oConfigInfo != null)
                {
                    oConfigInfo.CalificationProjectConfigInfoId = DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigInfoUpsert
                        (oConfigInfo.CalificationProjectConfigInfoId,
                        oConfigInfo.RelatedCalificationProjectConfig.Company.CompanyPublicId,
                        oConfigInfo.RelatedCalificationProjectConfig.CalificationProjectConfigId,
                        oConfigInfo.Status,
                        oConfigInfo.Enable
                        );
                    oLog.IsSuccess = true;
                }
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + "-" + err.StackTrace;
                throw err;
            }
            finally
            {
                oLog.User = "BatchProcess";
                oLog.LogObject = oConfigInfo;
                oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                {
                    LogInfoType = "CalificationProjectCongfigInfo",
                    Value = oConfigInfo.CalificationProjectConfigInfoId.ToString()
                });
                LogManager.ClientLog.AddLog(oLog);
            }
            return oConfigInfo;
        }

        public static List<ConfigInfoModel> CalificationProjectConfigInfoGetAll()
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigInfoGetAll();
        }

        public static List<ConfigInfoModel> CalificationProjectConfigInfoGetByProvider(string ProviderPublicId, bool Enable)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigInfoGetByProvider(ProviderPublicId, Enable);
        }
        public static ConfigInfoModel CalificationProjectConfigInfoGetByProviderAndCustomer(string CustomerPublicId, string ProviderPublicId, bool Enable)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigInfoGetByProviderAndCustomer(CustomerPublicId, ProviderPublicId, Enable);
        }

        #endregion

        #region ConfigItem

        public static CalificationProjectConfigModel CalificationProjectConfigItemUpsert(CalificationProjectConfigModel oCalificationProjectConfigModel)
        {
            if (oCalificationProjectConfigModel != null &&
                oCalificationProjectConfigModel.ConfigItemModel != null &&
                oCalificationProjectConfigModel.ConfigItemModel.Count > 0)
            {
                oCalificationProjectConfigModel.ConfigItemModel.All(cit =>
                {
                    LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();

                    try
                    {
                        cit.CalificationProjectConfigItemId = DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigItemUpsert
                            (oCalificationProjectConfigModel.CalificationProjectConfigId,
                            cit.CalificationProjectConfigItemId,
                            cit.CalificationProjectConfigItemName,
                            cit.CalificationProjectConfigItemType.ItemId,
                            cit.Enable);

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
                        oLog.User = "BatchProcess";
                        oLog.LogObject = cit;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "CalificationProjectConfigItemId",
                            Value = cit.CalificationProjectConfigItemId.ToString(),
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }

            return oCalificationProjectConfigModel;
        }

        public static List<ConfigItemModel> CalificationProjectConfigItem_GetByCalificationProjectConfigId(int CalificationProjectConfigId, bool Enable)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigItem_GetByCalificationProjectConfigId(CalificationProjectConfigId, Enable);
        }

        #endregion

        #region ConfigItemInfo

        public static ConfigItemModel CalificationProjectConfigItemInfoUpsert(ConfigItemModel oConfigItemModel)
        {
            if (oConfigItemModel != null &&
                oConfigItemModel.CalificationProjectConfigItemInfoModel != null &&
                oConfigItemModel.CalificationProjectConfigItemInfoModel.Count > 0)
            {
                oConfigItemModel.CalificationProjectConfigItemInfoModel.All(cinf =>
                {
                    LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();
                    try
                    {
                        cinf.CalificationProjectConfigItemInfoId = DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigItemInfoUpsert
                        (oConfigItemModel.CalificationProjectConfigItemId,
                        cinf.CalificationProjectConfigItemInfoId,
                        cinf.Question.ItemId,
                        cinf.Rule.ItemId,
                        cinf.ValueType.ItemId,
                        cinf.Value,
                        cinf.Score,
                        cinf.Enable);

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
                        oLog.User = "BatchProcess";
                        oLog.LogObject = cinf;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "SurveyConfigId",
                            Value = cinf.CalificationProjectConfigItemInfoId.ToString(),
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }

            return oConfigItemModel;
        }

        public static List<ConfigItemInfoModel> CalificationProjectConfigItemInfo_GetByCalificationProjectConfigItemId(int CalificationProjectConfigItemId, bool Enable)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigItemInfo_GetByCalificationProjectConfigItemId(CalificationProjectConfigItemId, Enable);
        }

        #endregion

        #region ConfigValidate

        public static ConfigValidateModel CalificationProjectConfigValidate_Upsert(ConfigValidateModel oConfigValidateModel)
        {
            LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();
            try
            {
                if (oConfigValidateModel != null)
                {
                    oConfigValidateModel.CalificationProjectConfigValidateId = DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigValidateUpsert
                        (
                        oConfigValidateModel.CalificationProjectConfigValidateId,
                            oConfigValidateModel.CalificationProjectConfigId,
                            oConfigValidateModel.Operator.ItemId,
                            oConfigValidateModel.Value,
                            oConfigValidateModel.Result,
                            oConfigValidateModel.Enable
                        );
                    oLog.IsSuccess = true;
                }
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + " - " + err.StackTrace;
                throw err;
            }
            finally
            {
                oLog.User = "BatchProcess";
                oLog.LogObject = oConfigValidateModel;

                oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                {
                    LogInfoType = "CalificationProjectValidateId",
                    Value = oConfigValidateModel.CalificationProjectConfigValidateId.ToString(),
                });

                LogManager.ClientLog.AddLog(oLog);
            }
            return oConfigValidateModel;
        }

        public static List<ConfigValidateModel> CalificationProjectValidate_GetByProjectConfigId(int ConfigProjectId, bool Enable)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigValidate_GetByProjectConfigId(ConfigProjectId, Enable);
        }

        #endregion

        #endregion

        #region CalificationProjectConfigOptions

        public static List<ProveedoresOnLine.Company.Models.Util.CatalogModel> CalificationProjectConfigOptions()
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigOptions();
        }

        public static List<Models.CalificationProject.CalificationProjectCategoryModel> CalificationProjectConfigCategoryOptions()
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigCategoryOptions();
        }

        public static List<CatalogModel> CalificationProjectConfigAditionalDocumentsOptions(string CustomerPublicId)
        {
            return DAL.Controller.CalificationProjectDataController.Instance.CalificationProjectConfigAditionalDocumentsOptions(CustomerPublicId);
        }

        #endregion

        #region Backoffice

        public static void StartProcessByProviderAndCustomer(string ProviderPublicId, int CalificationId)
        {
            try
            {
                //Get all calification project configInfo
                var oCalificationProjectConfigInfoModel = ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigInfoGetAll();
                //oCalificationProjectConfigInfoModel = oCalificationProjectConfigInfoModel.Where(x => x.RelatedCalificationProjectConfig.CalificationProjectConfigId == 13).Select(x => x).ToList();
                //Select All a la nueva tabla ObjNuevo
                //cruzar ObjNuevo vs oCalificationProjectConfigModel oCalificationProjectConfigModel  == oCalificationProjectConfigModel  Cruzado
                List<CalificationProjectConfigModel> oCalificationProjectConfigModel = CalificationProjectConfig_GetAll();

                //oCalificationProjectConfigModel = oCalificationProjectConfigModel.Where(x => x.Company.CompanyPublicId == CustomerPublicId).ToList();

                oCalificationProjectConfigModel = oCalificationProjectConfigModel.Where(x => x.CalificationProjectConfigId == CalificationId).Select(x => x).ToList();
                var oRelatedProvider = new List<CompanyModel>();

                oCalificationProjectConfigModel = oCalificationProjectConfigModel.Where(x => oCalificationProjectConfigInfoModel.Any(y => x.CalificationProjectConfigId == y.RelatedCalificationProjectConfig.CalificationProjectConfigId)).Select(x => x).ToList();

                //validate calification project config list
                if (oCalificationProjectConfigModel != null &&
                    oCalificationProjectConfigModel.Count > 0)
                {
                    oCalificationProjectConfigModel.All(cnf =>
                    {
                        //Get all related provider by customer
                        oRelatedProvider = new List<CompanyModel>();
                        oRelatedProvider.AddRange(oCalificationProjectConfigInfoModel.Where(x => x.RelatedCalificationProjectConfig.CalificationProjectConfigId == cnf.CalificationProjectConfigId).Select(x => x.RelatedProvider));
                        oRelatedProvider = oRelatedProvider.Where(x => x.CompanyPublicId == ProviderPublicId).Select(x => x).ToList();
                        var oModelToUpsert = new ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectBatchModel();

                        //validate provider list
                        if (oRelatedProvider != null &&
                            oRelatedProvider.Count > 0 &&
                            oRelatedProvider[0] != null)
                        {

                            oRelatedProvider.All(prv =>
                            {
                                try
                                {
                                    //Get calification process by provider
                                    var oRelatedCalificationProject = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProject_GetProviderByCustomer(cnf.Company.CompanyPublicId, prv.CompanyPublicId, cnf.CalificationProjectConfigId);

                                    //validate calification project list
                                    if (oRelatedCalificationProject != null &&
                                        oRelatedCalificationProject.Count > 0)
                                    {
                                        //update calification project                               
                                        #region Validate calification project with config

                                        //validate all calification project config (Calification project - calification project item)
                                        oRelatedCalificationProject.All(cp =>
                                        {
                                            //get related calification project config
                                            cp.ProjectConfigModel = oCalificationProjectConfigModel.Where(config => config.CalificationProjectConfigId == cp.ProjectConfigModel.CalificationProjectConfigId).Select(config => config).FirstOrDefault();

                                            //validate calification project config is enable
                                            if (cp.ProjectConfigModel != null && cp.ProjectConfigModel.Enable)
                                            {
                                                cp.CalificationProjectItemBatchModel.All(cpi =>
                                                {
                                                    //get related calification project item config
                                                    cpi.CalificationProjectConfigItem = cp.ProjectConfigModel.ConfigItemModel.Where(configit => configit.CalificationProjectConfigItemId == cpi.CalificationProjectConfigItem.CalificationProjectConfigItemId).Select(configit => configit).FirstOrDefault();

                                                    //validate calification project config item is enable
                                                    if (!cpi.CalificationProjectConfigItem.Enable)
                                                    {
                                                        //disable calification project config item
                                                        cpi.Enable = false;

                                                        cpi.CalificatioProjectItemInfoModel.All(cpinf =>
                                                        {
                                                            cpinf.Enable = false;

                                                            return true;
                                                        });
                                                    }

                                                    return true;
                                                });

                                                //upsert
                                                cp = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProjectUpsert(cp);
                                            }
                                            else
                                            {
                                                //disable calification project
                                                cp.Enable = false;

                                                cp.CalificationProjectItemBatchModel.All(cpi =>
                                                {
                                                    cpi.Enable = false;

                                                    cpi.CalificatioProjectItemInfoModel.All(cpiinf =>
                                                    {
                                                        cpiinf.Enable = false;

                                                        return true;
                                                    });

                                                    return true;
                                                });

                                                //upsert
                                                cp = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProjectUpsert(cp);
                                            }
                                            return true;
                                        });

                                        #endregion

                                        #region run calification project

                                        oRelatedCalificationProject.Where(cp => cp.Enable == true).All(cp =>
                                        {
                                            //get related calification project config
                                            cp.ProjectConfigModel = oCalificationProjectConfigModel.Where(config => config.CalificationProjectConfigId == cp.ProjectConfigModel.CalificationProjectConfigId).Select(config => config).FirstOrDefault();

                                            //set data to model to upsert
                                            oModelToUpsert = new ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectBatchModel()
                                            {
                                                CalificationProjectId = cp.CalificationProjectId,
                                                CalificationProjectPublicId = cp.CalificationProjectPublicId,
                                                Enable = cp.Enable,
                                                ProjectConfigModel = cp.ProjectConfigModel,
                                                RelatedProvider = cp.RelatedProvider,
                                                TotalScore = cp.TotalScore,
                                                CalificationProjectItemBatchModel = new List<ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel>(),
                                            };

                                            cp.ProjectConfigModel.ConfigItemModel.Where(ci => ci.Enable == true).All(ci =>
                                            {
                                                //validate related config with calification project
                                                if (cp.CalificationProjectItemBatchModel.Any(cpib => cpib.CalificationProjectConfigItem.CalificationProjectConfigItemId == ci.CalificationProjectConfigItemId))
                                                {
                                                    cp.CalificationProjectItemBatchModel.Where(cpib => cpib.CalificationProjectConfigItem.CalificationProjectConfigItemId == ci.CalificationProjectConfigItemId).All(cpib =>
                                                    {
                                                        //get related calification project item config
                                                        cpib.CalificationProjectConfigItem = ci;

                                                        //update validation module
                                                        switch (ci.CalificationProjectConfigItemType.ItemId)
                                                        {
                                                            #region LegalModule

                                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_LegalModule:

                                                                cpib = ProveedoresOnLine.CalificationBatch.CalificationProjectModule.LegalModule.LegalRule(prv.CompanyPublicId, cpib.CalificationProjectConfigItem, cpib);

                                                                oModelToUpsert.CalificationProjectItemBatchModel.Add(cpib);

                                                                break;

                                                            #endregion

                                                            #region FinancialModule

                                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_FinancialModule:

                                                                cpib = ProveedoresOnLine.CalificationBatch.CalificationProjectModule.FinancialModule.FinancialRule(prv.CompanyPublicId, cpib.CalificationProjectConfigItem, cpib);

                                                                oModelToUpsert.CalificationProjectItemBatchModel.Add(cpib);

                                                                break;

                                                            #endregion

                                                            #region CommercialModule

                                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_CommercialModule:

                                                                cpib = ProveedoresOnLine.CalificationBatch.CalificationProjectModule.CommercialModule.CommercialRule(prv.CompanyPublicId, cpib.CalificationProjectConfigItem, cpib);

                                                                oModelToUpsert.CalificationProjectItemBatchModel.Add(cpib);

                                                                break;

                                                            #endregion

                                                            #region HSEQModule

                                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_HSEQModule:

                                                                cpib = ProveedoresOnLine.CalificationBatch.CalificationProjectModule.HSEQModule.HSEQRule(prv.CompanyPublicId, cpib.CalificationProjectConfigItem, cpib);

                                                                oModelToUpsert.CalificationProjectItemBatchModel.Add(cpib);

                                                                break;

                                                            #endregion

                                                            #region BalanceModule

                                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_BalanceModule:

                                                                cpib = ProveedoresOnLine.CalificationBatch.CalificationProjectModule.BalanceModule.BalanceRule(prv.CompanyPublicId, cpib.CalificationProjectConfigItem, cpib);

                                                                oModelToUpsert.CalificationProjectItemBatchModel.Add(cpib);

                                                                break;

                                                            #endregion

                                                            #region AditonalDocumentModule

                                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_AditionalDocumentModule:

                                                                cpib = ProveedoresOnLine.CalificationBatch.CalificationProjectModule.AditionalDocumentModule.AditionalDocumentationRule(prv.CompanyPublicId, cpib.CalificationProjectConfigItem, cpib);

                                                                oModelToUpsert.CalificationProjectItemBatchModel.Add(cpib);

                                                                break;

                                                                #endregion
                                                        }

                                                        return true;
                                                    });

                                                    oModelToUpsert.TotalScore = 0;

                                                    oModelToUpsert.CalificationProjectItemBatchModel.All(cpi =>
                                                    {
                                                        oModelToUpsert.TotalScore += cpi.ItemScore;

                                                        return true;
                                                    });

                                                    //Upsert
                                                    oModelToUpsert = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProjectUpsert(oModelToUpsert);

                                                    if (oCalificationProjectConfigInfoModel.Where(x => x.RelatedCalificationProjectConfig.CalificationProjectConfigId == cp.ProjectConfigModel.CalificationProjectConfigId && x.RelatedProvider.CompanyPublicId == prv.CompanyPublicId).Select(x => x.CalificationProjectConfigInfoId).FirstOrDefault() > 0)
                                                    {
                                                        ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigInfoUpsert(new Models.CalificationProject.ConfigInfoModel()
                                                        {
                                                            CalificationProjectConfigInfoId = oCalificationProjectConfigInfoModel.Where(x => x.RelatedCalificationProjectConfig.CalificationProjectConfigId == cp.ProjectConfigModel.CalificationProjectConfigId && x.RelatedProvider.CompanyPublicId == prv.CompanyPublicId).Select(x => x.CalificationProjectConfigInfoId).FirstOrDefault(),
                                                            RelatedCalificationProjectConfig = new Models.CalificationProject.CalificationProjectConfigModel()
                                                            {
                                                                Company = new CompanyModel()
                                                                {
                                                                    CompanyPublicId = prv.CompanyPublicId
                                                                },
                                                                CalificationProjectConfigId = cp.ProjectConfigModel.CalificationProjectConfigId
                                                            },
                                                            Enable = true,
                                                            Status = true
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    //add item
                                                    switch (ci.CalificationProjectConfigItemType.ItemId)
                                                    {
                                                        #region LegalModule

                                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_LegalModule:

                                                            ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oLegalModule =
                                                                ProveedoresOnLine.CalificationBatch.CalificationProjectModule.LegalModule.LegalRule(prv.CompanyPublicId, ci, null);

                                                            oModelToUpsert.CalificationProjectItemBatchModel.Add(oLegalModule);

                                                            break;

                                                        #endregion

                                                        #region FinancialModule

                                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_FinancialModule:

                                                            ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oFinancialModule =
                                                                ProveedoresOnLine.CalificationBatch.CalificationProjectModule.FinancialModule.FinancialRule(prv.CompanyPublicId, ci, null);

                                                            oModelToUpsert.CalificationProjectItemBatchModel.Add(oFinancialModule);

                                                            break;

                                                        #endregion

                                                        #region CommercialModule

                                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_CommercialModule:

                                                            ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oCommercialModule =
                                                                ProveedoresOnLine.CalificationBatch.CalificationProjectModule.CommercialModule.CommercialRule(prv.CompanyPublicId, ci, null);

                                                            oModelToUpsert.CalificationProjectItemBatchModel.Add(oCommercialModule);

                                                            break;

                                                        #endregion

                                                        #region HSEQModule

                                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_HSEQModule:

                                                            ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oCertificationModule =
                                                                ProveedoresOnLine.CalificationBatch.CalificationProjectModule.HSEQModule.HSEQRule(prv.CompanyPublicId, ci, null);

                                                            oModelToUpsert.CalificationProjectItemBatchModel.Add(oCertificationModule);

                                                            break;

                                                        #endregion

                                                        #region BalanceModule

                                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_BalanceModule:

                                                            ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oBalanceModule =
                                                                ProveedoresOnLine.CalificationBatch.CalificationProjectModule.BalanceModule.BalanceRule(prv.CompanyPublicId, ci, null);

                                                            oModelToUpsert.CalificationProjectItemBatchModel.Add(oBalanceModule);

                                                            break;

                                                        #endregion

                                                        #region AditonalDocumentModule

                                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_AditionalDocumentModule:

                                                            ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oAditionalDocumentModule =
                                                                ProveedoresOnLine.CalificationBatch.CalificationProjectModule.AditionalDocumentModule.AditionalDocumentationRule(prv.CompanyPublicId, ci, null);

                                                            oModelToUpsert.CalificationProjectItemBatchModel.Add(oAditionalDocumentModule);

                                                            break;

                                                            #endregion
                                                    }

                                                    oModelToUpsert.TotalScore = 0;

                                                    oModelToUpsert.CalificationProjectItemBatchModel.All(cpi =>
                                                    {
                                                        oModelToUpsert.TotalScore += cpi.ItemScore;

                                                        return true;
                                                    });

                                                    //Upsert
                                                    oModelToUpsert = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProjectUpsert(oModelToUpsert);

                                                    if (oCalificationProjectConfigInfoModel.Where(x => x.RelatedCalificationProjectConfig.CalificationProjectConfigId == cp.ProjectConfigModel.CalificationProjectConfigId && x.RelatedProvider.CompanyPublicId == prv.CompanyPublicId).Select(x => x.CalificationProjectConfigInfoId).FirstOrDefault() > 0)
                                                    {
                                                        ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigInfoUpsert(new Models.CalificationProject.ConfigInfoModel()
                                                        {
                                                            CalificationProjectConfigInfoId = oCalificationProjectConfigInfoModel.Where(x => x.RelatedCalificationProjectConfig.CalificationProjectConfigId == cp.ProjectConfigModel.CalificationProjectConfigId && x.RelatedProvider.CompanyPublicId == prv.CompanyPublicId).Select(x => x.CalificationProjectConfigInfoId).FirstOrDefault(),
                                                            RelatedCalificationProjectConfig = new Models.CalificationProject.CalificationProjectConfigModel()
                                                            {
                                                                Company = new CompanyModel()
                                                                {
                                                                    CompanyPublicId = prv.CompanyPublicId
                                                                },
                                                                CalificationProjectConfigId = cp.ProjectConfigModel.CalificationProjectConfigId
                                                            },
                                                            Enable = true,
                                                            Status = true
                                                        });
                                                    }
                                                }
                                                return true;
                                            });
                                            return true;
                                        });

                                        #endregion
                                    }
                                    else
                                    {
                                        #region New Calification project

                                        //new calification project
                                        ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectBatchModel oCalificationProjectUpsert = new ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectBatchModel()
                                        {
                                            CalificationProjectId = 0,
                                            CalificationProjectPublicId = "",
                                            ProjectConfigModel = new Models.CalificationProject.CalificationProjectConfigModel()
                                            {
                                                CalificationProjectConfigId = cnf.CalificationProjectConfigId,
                                            },
                                            RelatedProvider = new Company.Models.Company.CompanyModel()
                                            {
                                                CompanyPublicId = prv.CompanyPublicId,
                                            },
                                            CalificationProjectItemBatchModel = new List<ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel>(),
                                            Enable = true,
                                        };

                                        //execute all calification process
                                        cnf.ConfigItemModel.Where(md => md.Enable == true).All(md =>
                                        {
                                            switch (md.CalificationProjectConfigItemType.ItemId)
                                            {
                                                #region LegalModule

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_LegalModule:

                                                    ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oLegalModule =
                                                        ProveedoresOnLine.CalificationBatch.CalificationProjectModule.LegalModule.LegalRule(prv.CompanyPublicId, md, null);

                                                    oCalificationProjectUpsert.CalificationProjectItemBatchModel.Add(oLegalModule);

                                                    break;

                                                #endregion

                                                #region FinancialModule

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_FinancialModule:

                                                    ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oFinancialModule =
                                                        ProveedoresOnLine.CalificationBatch.CalificationProjectModule.FinancialModule.FinancialRule(prv.CompanyPublicId, md, null);

                                                    oCalificationProjectUpsert.CalificationProjectItemBatchModel.Add(oFinancialModule);

                                                    break;

                                                #endregion

                                                #region CommercialModule

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_CommercialModule:

                                                    ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oCommercialModule =
                                                        ProveedoresOnLine.CalificationBatch.CalificationProjectModule.CommercialModule.CommercialRule(prv.CompanyPublicId, md, null);

                                                    oCalificationProjectUpsert.CalificationProjectItemBatchModel.Add(oCommercialModule);

                                                    break;

                                                #endregion

                                                #region HSEQModule

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_HSEQModule:

                                                    ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oCertificationModule =
                                                        ProveedoresOnLine.CalificationBatch.CalificationProjectModule.HSEQModule.HSEQRule(prv.CompanyPublicId, md, null);

                                                    oCalificationProjectUpsert.CalificationProjectItemBatchModel.Add(oCertificationModule);

                                                    break;

                                                #endregion

                                                #region BalanceModule

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_BalanceModule:

                                                    ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oBalanceModule =
                                                        ProveedoresOnLine.CalificationBatch.CalificationProjectModule.BalanceModule.BalanceRule(prv.CompanyPublicId, md, null);

                                                    oCalificationProjectUpsert.CalificationProjectItemBatchModel.Add(oBalanceModule);

                                                    break;

                                                #endregion

                                                #region AditonalDocumentModule

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumModuleType.CP_AditionalDocumentModule:

                                                    ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel oAditionalDocumentModule =
                                                        ProveedoresOnLine.CalificationBatch.CalificationProjectModule.AditionalDocumentModule.AditionalDocumentationRule(prv.CompanyPublicId, md, null);

                                                    oCalificationProjectUpsert.CalificationProjectItemBatchModel.Add(oAditionalDocumentModule);

                                                    break;

                                                    #endregion
                                            }

                                            return true;
                                        });

                                        //get total score
                                        oCalificationProjectUpsert.CalificationProjectItemBatchModel.Where(sit => sit.Enable == true).All(sit =>
                                        {
                                            oCalificationProjectUpsert.TotalScore += sit.ItemScore;

                                            return true;
                                        });

                                        //Upsert
                                        oCalificationProjectUpsert = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProjectUpsert(oCalificationProjectUpsert);

                                        ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigInfoUpsert(new Models.CalificationProject.ConfigInfoModel()
                                        {
                                            CalificationProjectConfigInfoId = oCalificationProjectConfigInfoModel.Where(x => x.RelatedCalificationProjectConfig.CalificationProjectConfigId == oCalificationProjectUpsert.ProjectConfigModel.CalificationProjectConfigId && x.RelatedProvider.CompanyPublicId == prv.CompanyPublicId).Select(x => x.CalificationProjectConfigInfoId).FirstOrDefault(),
                                            RelatedCalificationProjectConfig = new Models.CalificationProject.CalificationProjectConfigModel()
                                            {
                                                Company = new CompanyModel()
                                                {
                                                    CompanyPublicId = prv.CompanyPublicId
                                                },
                                                CalificationProjectConfigId = oCalificationProjectUpsert.ProjectConfigModel.CalificationProjectConfigId
                                            },
                                            Enable = true,
                                            Status = true
                                        });
                                        #endregion
                                    }

                                    //Get related config by customer public id
                                    var oRelatedCalificationProjectConfig =
                                        ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProjectConfig_GetByCustomerPublicId(cnf.Company.CompanyPublicId, true);

                                    var oCalProject = new List<ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectBatchModel>();

                                    //Get relation between calification project and provider
                                    var oCalProjectConfigInfo =
                                        ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigInfoGetByProviderAndCustomer(cnf.Company.CompanyPublicId, ProviderPublicId, true);

                                    if (oCalProjectConfigInfo != null && oCalProjectConfigInfo.CalificationProjectConfigInfoId > 0)
                                    {
                                        oRelatedCalificationProjectConfig = oRelatedCalificationProjectConfig.Where(x => x.CalificationProjectConfigId == oCalProjectConfigInfo.RelatedCalificationProjectConfig.CalificationProjectConfigId).Select(x => x).ToList();
                                        oCalProject = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.
                                                                            CalificationProject_GetByCustomer(cnf.Company.CompanyPublicId, ProviderPublicId, true);
                                    }
                                    else
                                    {
                                        oRelatedCalificationProjectConfig =
                                        ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProjectConfig_GetByCustomerPublicId("", true);

                                        oCalProjectConfigInfo =
                                         ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigInfoGetByProviderAndCustomer("", ProviderPublicId, true);

                                        if (oRelatedCalificationProjectConfig != null && oRelatedCalificationProjectConfig.Count > 0 && oCalProjectConfigInfo
                                                != null && oCalProjectConfigInfo.CalificationProjectConfigInfoId > 0)
                                        {
                                            oRelatedCalificationProjectConfig =
                                          oRelatedCalificationProjectConfig.Where(x => x.CalificationProjectConfigId == oCalProjectConfigInfo.RelatedCalificationProjectConfig.CalificationProjectConfigId).Select(x => x).ToList();

                                            oCalProject =
                                            ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.CalificationProject_GetByCustomer("", ProviderPublicId, true);
                                        }
                                    }

                                    List<ProveedoresOnLine.CalificationProject.Models.CalificationProject.ConfigValidateModel> oValidateModel = new List<ProveedoresOnLine.CalificationProject.Models.CalificationProject.ConfigValidateModel>();

                                    if (oCalProject != null &&
                                        oCalProject.Count > 0)
                                    {
                                        oValidateModel = CalificationProjectValidate_GetByProjectConfigId(oCalProject.FirstOrDefault().ProjectConfigModel.CalificationProjectConfigId, true);
                                        string Calification = GetCalificationScore(oCalProject, oValidateModel);
                                        int oTotalScore = oCalProject.FirstOrDefault().TotalScore;

                                        ProviderModel oProvider = new ProviderModel();
                                        oProvider.RelatedCompany = Company.Controller.Company.CompanyGetBasicInfo(ProviderPublicId);
                                        int InfoId = 0;
                                        if (oProvider.RelatedCompany.CompanyInfo.Any(x => x.ItemInfoType.ItemId == 203021))                                        
                                            InfoId = oProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == 203021).Select(x => x).FirstOrDefault().ItemInfoId;

                                        GenericItemInfoModel oInfo = new GenericItemInfoModel()
                                        {
                                            ItemInfoId = InfoId,
                                            ItemInfoType = new CatalogModel()
                                            {
                                                ItemId = 203021,
                                                ItemEnable = true,
                                            },
                                            Value = cnf.Company.CompanyPublicId + '_' + oTotalScore.ToString() + '_' + Calification,
                                            Enable = true,
                                        };
                                        oProvider.RelatedCompany.CompanyInfo.Add(oInfo);

                                        ProveedoresOnLine.Company.Controller.Company.CompanyInfoUpsert(oProvider.RelatedCompany);
                                    }
                                    else
                                    {

                                    }

                                }
                                catch (Exception ex)
                                {

                                }


                                return true;
                            });

                        }
                        else
                        {
                            //Provider list is empty
                            ProveedoresOnLine.CalificationBatch.CalificationProcess.LogFile("Error:: customer public id: " + cnf.Company.CompanyPublicId + " :: related provider list is empty.");
                        }
                        return true;
                    });

                }
                else
                {
                }
            }
            catch (Exception err)
            {

            }
        }


        private static string GetCalificationScore(List<CalificationProjectBatchModel> oProviderCalModel, List<ProveedoresOnLine.CalificationProject.Models.CalificationProject.ConfigValidateModel> oValidate)
        {
            string oTotalScore = "";
            if (oProviderCalModel != null && oProviderCalModel.Count > 0 && oValidate != null && oValidate.Count > 0)
            {
                oProviderCalModel.FirstOrDefault().ProjectConfigModel.ConfigValidateModel = oValidate;
                oProviderCalModel.FirstOrDefault().ProjectConfigModel.ConfigValidateModel.All(x =>
                {
                    switch (x.Operator.ItemId)
                    {
                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorQue:
                            if (oProviderCalModel.FirstOrDefault().TotalScore > int.Parse(x.Value))
                            {
                                oTotalScore = x.Result;
                            }
                            break;

                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorQue:
                            if (oProviderCalModel.FirstOrDefault().TotalScore < int.Parse(x.Value))
                            {
                                oTotalScore = x.Result;
                            }
                            break;

                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorOIgual:
                            if (oProviderCalModel.FirstOrDefault().TotalScore <= int.Parse(x.Value))
                            {
                                oTotalScore = x.Result;
                            }
                            break;

                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorOIgual:
                            if (oProviderCalModel.FirstOrDefault().TotalScore >= int.Parse(x.Value))
                            {
                                oTotalScore = x.Result;
                            }
                            break;

                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Entre:

                            int minValue = 0;
                            int maxValue = 0;

                            string[] oValue = x.Value.Split(',');
                            minValue = int.Parse(oValue[0]);
                            maxValue = int.Parse(oValue[1]);

                            if (oProviderCalModel.FirstOrDefault().TotalScore <= maxValue && oProviderCalModel.FirstOrDefault().TotalScore >= minValue)
                            {
                                oTotalScore = x.Result;
                            }

                            break;
                    }
                    return true;
                });

            }
            return oTotalScore;
        }

        #endregion
    }
}
