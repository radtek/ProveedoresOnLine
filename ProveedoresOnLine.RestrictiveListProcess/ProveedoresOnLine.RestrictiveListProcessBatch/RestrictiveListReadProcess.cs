using Nest;
using NetOffice.ExcelApi;
using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.CompanyProvider.Models.Provider;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.RestrictiveListProcess.Models.RestrictiveListProcess;
using ProveedoresOnLine.RestrictiveListProcessBatch.Models;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;


namespace ProveedoresOnLine.RestrictiveListProcessBatch
{
    public class RestrictiveListReadProcess
    {
        public static void StartProcess()
        {
            try
            {
                //Start Process
                //Get all BlackListProcess
                List<RestrictiveListProcessModel> oProcessToValidate = ProveedoresOnLine.RestrictiveListProcess.Controller.RestrictiveListProcessModule.GetAllProvidersInProcess();
                if (oProcessToValidate != null)
                {
                    string strFolder = ProveedoresOnLine.RestrictiveListProcessBatch.Models.General.InternalSettings.Instance[ProveedoresOnLine.RestrictiveListProcessBatch.Models.Constants.C_Settings_File_TempDirectory].Value;
                    LogFile("Blacklist Read Process Start for: " + oProcessToValidate.Count + " Provider Status");
                    oProcessToValidate.All(Process =>
                    {
                        //Instance App to read excel
                        Application app = new Application();

                        string FileName = Process.FilePath.Split('/').LastOrDefault();

                        //Call Function to get Coincidences
                        List<ExcelModel> oExcelFileProcess = null;

                        if (FileName != null)
                        {
                            //Download Current File
                            System.Data.DataTable DT_Excel;
                            using (WebClient webClient = new WebClient())
                            {
                                //Get file from S3 using File Name           
                                webClient.DownloadFile(Process.FilePath, strFolder + FileName);
                                //Call function to get Excel info into Datatable
                                DT_Excel = ReadExcelFile(strFolder + FileName);

                                if (DT_Excel != null)
                                {
                                    oExcelFileProcess = new List<ExcelModel>();
                                    foreach (DataRow item in DT_Excel.Rows)
                                    {
                                        oExcelFileProcess.Add(new ExcelModel(item));
                                    }

                                }

                            }
                            var objCoincidences = SearchInfoFromFile(DT_Excel, new TDQueryModel());
                            List<TDQueryInfoModel> oCoincidences = new List<TDQueryInfoModel>(objCoincidences.Item2.RelatedQueryInfoModel);
                            //Get Provider by Status                        
                            Process.RelatedProvider = new List<ProviderModel>();

                            //Compare Company
                            List<ProviderModel> oProvidersToCompare = new List<ProviderModel>();

                            //Compare Persons                        
                            List<ProviderModel> oCompanyPersonToCompare = new List<ProviderModel>();
                            List<GenericItemModel> oRelatedLegaToComapare = new List<GenericItemModel>();

                            //Get Providers By Status
                            Process.RelatedProvider = ProveedoresOnLine.RestrictiveListProcess.Controller.RestrictiveListProcessModule.GetProviderByStatus(Convert.ToInt32(Process.ProviderStatus), ProveedoresOnLine.RestrictiveListProcessBatch.Models.General.InternalSettings.Instance[ProveedoresOnLine.RestrictiveListProcessBatch.Models.Constants.C_Settings_PublicarPublicId].Value);

                            Process.RelatedProvider.All(prv =>
                                {
                                    ProviderModel oProvider = new ProviderModel();
                                    oProvider.RelatedCompany = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(prv.RelatedCompany.CompanyPublicId);

                                    oProvider.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                                    {
                                        ItemInfoId = oProvider.RelatedCompany.CompanyInfo != null ? oProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.UpdateAlert).
                                                Select(x => x.ItemInfoId).DefaultIfEmpty(0).FirstOrDefault() : 0,
                                        ItemInfoType = new CatalogModel()
                                        {
                                            ItemId = (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.UpdateAlert,
                                        },
                                        Enable = true,
                                        Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                    });

                                    oProvider.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                                    {
                                        ItemInfoId = oProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.Alert).
                                                    Select(x => x.ItemInfoId).DefaultIfEmpty(0).FirstOrDefault(),
                                        ItemInfoType = new CatalogModel()
                                        {
                                            ItemId = (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.Alert,
                                        },
                                        Enable = true,
                                        Value = ((int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumBlackList.BL_DontShowAlert).ToString(),
                                    });

                                    //Save DateTime of last Update Data
                                    ProveedoresOnLine.Company.Controller.Company.CompanyInfoUpsert(oProvider.RelatedCompany);
                                    return true;
                                });

                            oProvidersToCompare = Process.RelatedProvider.Where(y => oCoincidences.Any(c => c.IdentificationResult == y.RelatedCompany.IdentificationNumber && y.RelatedCompany.CompanyName == c.NameResult)).ToList();

                            //Get persons coincidences                           
                            Process.RelatedProvider.All(prv =>
                            {
                                if (prv.RelatedLegal != null && prv.RelatedLegal.Count > 0)
                                {
                                    oRelatedLegaToComapare.AddRange(prv.RelatedLegal.Where(y => oCoincidences.Any(c => c.NameResult == y.ItemInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumLegalDesignationsInfoType.CD_PartnerName).Select(inf => inf.Value).FirstOrDefault() ||
                                                                                                                   c.IdentificationResult == y.ItemInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumLegalDesignationsInfoType.CD_PartnerIdentificationNumber).Select(inf => inf.Value).FirstOrDefault())).ToList());
                                }

                                return true;
                            });

                            TDQueryModel oQuery = new TDQueryModel()
                            {
                                CompayPublicId = Process.RelatedProvider.FirstOrDefault().RelatedCompany.CompanyPublicId,
                                FileName = Process.FilePath,

                            };

                            //Create Private Function to update blackList                            
                            if (CreateBlackListProcess(oProvidersToCompare, oRelatedLegaToComapare, oCoincidences))
                            {
                                Process.ProcessStatus = true;
                                ProveedoresOnLine.RestrictiveListProcess.Controller.RestrictiveListProcessModule.BlackListProcessUpsert(Process);
                            }

                            //Remove all Files
                            //remove temporal file
                            if (System.IO.File.Exists(strFolder + FileName))
                                System.IO.File.Delete(strFolder + FileName);

                            //remove temporal file
                            if (System.IO.File.Exists(strFolder + FileName.Replace("xlsx", "xls")))
                                System.IO.File.Delete(strFolder + FileName.Replace("xlsx", "xls"));

                            LogFile("Success::" + "End Process::" + "::" + Process.ProviderStatus);

                        }
                        else
                        {
                            LogFile("Success::" + "Not Found File in FTP");
                        }
                        return true;
                    });
                }
                else
                {
                    LogFile("Success::" + "No Files To Validate");
                }
            }
            catch (Exception err)
            {
                LogFile("Fatal error::" + err.Message + " - " + err.StackTrace);
            }
        }

        private static bool CreateBlackListProcess(List<ProviderModel> oProvidersToUpdate, List<GenericItemModel> oRelatedPersons, List<TDQueryInfoModel> oCoincidences)
        {
            try
            {
                #region Update blackList For Providers
                if (oProvidersToUpdate != null && oProvidersToUpdate.Count > 0)
                {
                    List<TDQueryInfoModel> oCurrentCoincidences = new List<TDQueryInfoModel>();
                    oProvidersToUpdate = oProvidersToUpdate.GroupBy(x => x.RelatedCompany.IdentificationNumber).Select(x => x.First()).ToList();
                    //For each  provider create de black List                  
                    oProvidersToUpdate.All(prv =>
                    {
                        if (ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.BlackListClearProvider(prv.RelatedCompany.CompanyPublicId) && prv.RelatedCompany != null)
                        {
                            ProviderModel oProvider = new ProviderModel();
                            oProvider.RelatedCompany = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(prv.RelatedCompany.CompanyPublicId);

                            oProvider.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                            {
                                ItemInfoId = oProvider.RelatedCompany.CompanyInfo != null ? oProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.UpdateAlert).
                                        Select(x => x.ItemInfoId).DefaultIfEmpty(0).FirstOrDefault() : 0,
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.UpdateAlert,
                                },
                                Enable = true,
                                Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            });

                            oProvider.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                            {
                                ItemInfoId = oProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.Alert).
                                            Select(x => x.ItemInfoId).DefaultIfEmpty(0).FirstOrDefault(),
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.Alert,
                                },
                                Enable = true,
                                Value = ((int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumBlackList.BL_DontShowAlert).ToString(),
                            });

                            //Save DateTime of last Update Data
                            ProveedoresOnLine.Company.Controller.Company.CompanyInfoUpsert(oProvider.RelatedCompany);

                            //Get coincidences for current provider
                            oCurrentCoincidences = oCoincidences.Where(x => x.NameResult == prv.RelatedCompany.CompanyName && x.IdentificationResult == prv.RelatedCompany.IdentificationNumber).Select(x => x).ToList();
                            oCurrentCoincidences = oCurrentCoincidences.Distinct().ToList();
                            oCurrentCoincidences.All(c =>
                            {
                                #region Operation

                                ProviderModel oProviderToInsert = new ProviderModel();
                                oProviderToInsert.RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel();
                                oProviderToInsert.RelatedCompany.CompanyInfo = new List<GenericItemInfoModel>();
                                oProviderToInsert.RelatedCompany.CompanyPublicId = prv.RelatedCompany.CompanyPublicId;
                                oProviderToInsert.RelatedBlackList = new List<BlackListModel>();

                                CompanyModel BasicInfo = new CompanyModel();
                                BasicInfo = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(prv.RelatedCompany.CompanyPublicId);
                                oProviderToInsert.RelatedBlackList.Add(new BlackListModel
                                {
                                    BlackListStatus = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumBlackList.BL_ShowAlert,
                                    },
                                    User = "Proveedores OnLine Process",
                                    FileUrl = "",
                                    BlackListInfo = new List<GenericItemInfoModel>()
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Razón Social",
                                    },
                                    Value = c.NameResult,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Identificación Consultada",
                                    },
                                    Value = c.IdentificationResult,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Alias",
                                    },
                                    Value = c.AKA,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Peps",
                                    },
                                    Value = c.Peps,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Prioridad",
                                    },
                                    Value = c.Priority,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Estado",
                                    },
                                    Value = c.Status,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Fecha Registro",
                                    },
                                    Value = c.CreateDate.ToString(),
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Nombre Completo",
                                    },
                                    Value = c.FullName,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Documento de Identidad",
                                    },
                                    Value = c.IdentificationNumber,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Fecha de Actualizacion",
                                    },
                                    Value = c.UpdateDate,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Nombre del Grupo",
                                    },
                                    Value = c.GroupName,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Nombre de la Lista",
                                    },
                                    Value = c.ListName,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Cargo o Delito",
                                    },
                                    Value = c.Offense,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Zona",
                                    },
                                    Value = c.Zone,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Link",
                                    },
                                    Value = c.Link,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Otra Información",
                                    },
                                    Value = c.MoreInfo,
                                    Enable = true,
                                });
                                List<ProviderModel> oProviderResultList = new List<ProviderModel>();
                                oProviderResultList.Add(ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.BlackListInsert(oProviderToInsert));

                                #region Set Provider Info

                                oProviderToInsert.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = BasicInfo.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.Alert)
                                                .Select(x => x.ItemInfoId).FirstOrDefault() != 0 ? BasicInfo.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.Alert)
                                                .Select(x => x.ItemInfoId).FirstOrDefault() : 0,
                                    ItemInfoType = new CatalogModel()
                                    {
                                        ItemId = (int)enumCompanyInfoType.Alert,
                                    },
                                    Value = ((int)enumBlackList.BL_ShowAlert).ToString(),
                                    Enable = true,
                                });

                                #endregion Set Provider Info
                                ProveedoresOnLine.Company.Controller.Company.CompanyInfoUpsert(oProviderToInsert.RelatedCompany);
                                #endregion Operation
                                return true;
                            });
                        }
                        return true;
                    });
                }

                #endregion

                #region Update blackList For Persons
                if (oRelatedPersons != null && oRelatedPersons.Count > 0)
                {
                    List<TDQueryInfoModel> oCurrentCoincidences = new List<TDQueryInfoModel>();
                    //List<ProviderModel> oPersonProvidersToUpdate = new List<ProviderModel>();
                    List<Tuple<ProviderModel, string>> oPersonsTuple = new List<Tuple<ProviderModel, string>>();
                    oRelatedPersons.All(prs =>
                        {
                            oPersonsTuple.Add(new Tuple<ProviderModel, string>(new ProviderModel()
                            {
                                RelatedCompany = new CompanyModel()
                                {
                                    CompanyPublicId = ProveedoresOnLine.RestrictiveListProcess.Controller.RestrictiveListProcessModule.GetCompanyPublicIdByLegalId(prs.ItemId),
                                    CompanyName = prs.ItemInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumLegalDesignationsInfoType.CD_PartnerName).Select(inf => inf.Value).FirstOrDefault(),
                                    IdentificationNumber = prs.ItemInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumLegalDesignationsInfoType.CD_PartnerIdentificationNumber).Select(inf => inf.Value).FirstOrDefault(),
                                },
                            },
                                prs.ItemInfo.Where(inf => inf.ItemInfoType.ItemId == (int)enumLegalDesignationsInfoType.CD_PartnerRank).
                                Select(inf => inf.Value).FirstOrDefault()));
                            return true;
                        });

                    //For each  provider create the black List  
                    oPersonsTuple = oPersonsTuple.GroupBy(x => x.Item1.RelatedCompany.IdentificationNumber).Select(x => x.FirstOrDefault()).ToList();
                    oPersonsTuple.All(prv =>
                    {
                        //Valid differents providers
                        //if (oProvidersToUpdate != null && oProvidersToUpdate.Count > 0 && !oProvidersToUpdate.Any(x => x.RelatedCompany.IdentificationNumber != oPersonsTuple.Select(y => y.Item1.RelatedCompany.IdentificationNumber).FirstOrDefault()))
                        ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.BlackListClearProvider(prv.Item1.RelatedCompany.CompanyPublicId);

                        if (prv.Item1.RelatedCompany != null)
                        {
                            ProviderModel oProvider = new ProviderModel();
                            oProvider.RelatedCompany = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(prv.Item1.RelatedCompany.CompanyPublicId);

                            oProvider.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                            {
                                ItemInfoId = oProvider.RelatedCompany.CompanyInfo != null ? oProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.UpdateAlert).
                                        Select(x => x.ItemInfoId).DefaultIfEmpty(0).FirstOrDefault() : 0,
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.UpdateAlert,
                                },
                                Enable = true,
                                Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            });

                            oProvider.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                            {
                                ItemInfoId = oProvider.RelatedCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.Alert).
                                            Select(x => x.ItemInfoId).DefaultIfEmpty(0).FirstOrDefault(),
                                ItemInfoType = new CatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumCompanyInfoType.Alert,
                                },
                                Enable = true,
                                Value = ((int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumBlackList.BL_DontShowAlert).ToString(),
                            });

                            //Save DateTime of last Update Data
                            ProveedoresOnLine.Company.Controller.Company.CompanyInfoUpsert(oProvider.RelatedCompany);

                            //Get coincidences for current provider
                            oCurrentCoincidences = oCoincidences.Where(x => x.NameResult == prv.Item1.RelatedCompany.CompanyName || x.IdentificationResult == prv.Item1.RelatedCompany.IdentificationNumber).Select(x => x).ToList();

                            oCurrentCoincidences.All(c =>
                            {
                                #region Operation

                                ProviderModel oProviderToInsert = new ProviderModel();
                                oProviderToInsert.RelatedCompany = new ProveedoresOnLine.Company.Models.Company.CompanyModel();
                                oProviderToInsert.RelatedCompany.CompanyInfo = new List<GenericItemInfoModel>();
                                oProviderToInsert.RelatedCompany.CompanyPublicId = prv.Item1.RelatedCompany.CompanyPublicId;
                                oProviderToInsert.RelatedBlackList = new List<BlackListModel>();

                                CompanyModel BasicInfo = new CompanyModel();
                                BasicInfo = ProveedoresOnLine.Company.Controller.Company.CompanyGetBasicInfo(prv.Item1.RelatedCompany.CompanyPublicId);
                                oProviderToInsert.RelatedBlackList.Add(new BlackListModel
                                {
                                    BlackListStatus = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemId = (int)ProveedoresOnLine.RestrictiveListProcessBatch.Models.enumBlackList.BL_ShowAlert,
                                    },
                                    User = "Proveedores OnLine Process",
                                    FileUrl = "",
                                    BlackListInfo = new List<GenericItemInfoModel>()
                                });

                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Alias",
                                    },
                                    Value = c.AKA,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Nombre Consultado",
                                    },
                                    Value = c.NameResult,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Identificación Consultada",
                                    },
                                    Value = c.IdentificationResult,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Cargo o Delito",
                                    },
                                    Value = c.Offense,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Peps",
                                    },
                                    Value = c.Peps,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Prioridad",
                                    },
                                    Value = c.Priority,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Estado",
                                    },
                                    Value = c.Status,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Fecha Registro",
                                    },
                                    Value = c.CreateDate.ToString(),
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Documento de Identidad",
                                    },
                                    Value = c.IdentificationNumber,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Fecha de Actualizacion",
                                    },
                                    Value = c.UpdateDate,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Nombre del Grupo",
                                    },
                                    Value = c.GroupName,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Nombre Completo",
                                    },
                                    Value = c.FullName,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Nombre de la Lista",
                                    },
                                    Value = c.ListName,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Otra Información",
                                    },
                                    Value = c.MoreInfo,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Zona",
                                    },
                                    Value = c.Zone,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Link",
                                    },
                                    Value = c.Link,
                                    Enable = true,
                                });
                                oProviderToInsert.RelatedBlackList.FirstOrDefault().BlackListInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = 0,
                                    ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                    {
                                        ItemName = "Cargo",
                                    },
                                    Value = prv.Item2,
                                    Enable = true,
                                });

                                List<ProviderModel> oProviderResultList = new List<ProviderModel>();
                                oProviderResultList.Add(ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.BlackListInsert(oProviderToInsert));

                                var idResult = oProviderResultList.FirstOrDefault().RelatedBlackList.Where(x => x.BlackListInfo != null).Select(x => x.BlackListInfo.Select(y => y.ItemInfoId)).FirstOrDefault();

                                #region Set Provider Info

                                oProviderToInsert.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                                {
                                    ItemInfoId = BasicInfo.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.Alert)
                                                .Select(x => x.ItemInfoId).FirstOrDefault() != 0 ? BasicInfo.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.Alert)
                                                .Select(x => x.ItemInfoId).FirstOrDefault() : 0,
                                    ItemInfoType = new CatalogModel()
                                    {
                                        ItemId = (int)enumCompanyInfoType.Alert,
                                    },
                                    Value = ((int)enumBlackList.BL_ShowAlert).ToString(),
                                    Enable = true,
                                });

                                //Set large value With the items found
                                //oProviderToInsert.RelatedCompany.CompanyInfo.Add(new GenericItemInfoModel()
                                //{
                                //    ItemInfoId = BasicInfo.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.ListId)
                                //                .Select(x => x.ItemInfoId).FirstOrDefault() != 0 ? BasicInfo.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.ListId)
                                //                .Select(x => x.ItemInfoId).FirstOrDefault() : 0,
                                //    ItemInfoType = new CatalogModel()
                                //    {
                                //        ItemId = (int)enumCompanyInfoType.ListId,
                                //    },
                                //    LargeValue = string.Join(",", idResult),
                                //    Enable = true,
                                //});

                                #endregion Set Provider Info
                                ProveedoresOnLine.Company.Controller.Company.CompanyInfoUpsert(oProviderToInsert.RelatedCompany);
                                #endregion Operation

                                return true;
                            });
                        }
                        return true;
                    });
                }
                #endregion

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private static System.Data.DataTable ReadExcelFile(string path)
        {
            bool HasHeader = true;
            using (var ExcelPackage = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    ExcelPackage.Load(stream);
                }

                var WS = ExcelPackage.Workbook.Worksheets.First();

                var FirstRow = 0;

                var DimEndRow = WS.Dimension.End.Row;

                var DimEndColumn = WS.Dimension.End.Column;

                System.Data.DataTable DT_Excel = new System.Data.DataTable();

                //set all the cells from the Excel File
                foreach (var FirstRowCell in WS.Cells[1, 1, 1, DimEndColumn])
                {
                    FirstRow = FirstRowCell.Start.Column;
                    DT_Excel.Columns.Add(HasHeader ? FirstRowCell.Text : string.Format("Column {0}", FirstRow));
                }

                var StartRow = HasHeader ? 2 : 1;
                for (var rowNum = StartRow; rowNum <= DimEndRow; rowNum++)
                {
                    var WsRow = WS.Cells[rowNum, 1, rowNum, DimEndColumn];
                    DataRow row = DT_Excel.Rows.Add();

                    foreach (var cell in WsRow)
                    {
                        if (cell.Text != null && cell.Text != " " && cell.Text != "")
                        {

                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                }
                return DT_Excel;
            }
        }
        
        #region Log File

        private static void LogFile(string LogMessage)
        {
            try
            {
                //get file Log
                string LogFile = AppDomain.CurrentDomain.BaseDirectory.Trim().TrimEnd(new char[] { '\\' }) + "\\" +
                    System.Configuration.ConfigurationManager.AppSettings[ProveedoresOnLine.RestrictiveListProcessBatch.Models.Constants.C_AppSettings_LogFile].Trim().TrimEnd(new char[] { '\\' });

                if (!System.IO.Directory.Exists(LogFile))
                    System.IO.Directory.CreateDirectory(LogFile);

                LogFile += "\\" + "Log_BlacListReadProcess_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(LogFile))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "::" + LogMessage);
                    sw.Close();
                }
            }
            catch { }
        }

        #endregion

        #region Search for Coincidences
        /// <summary>
        /// This function send the excel file info to Elasticsearch motor and return the results
        /// </summary>
        private static Tuple<List<Tuple<string, string>>, TDQueryModel> SearchInfoFromFile(System.Data.DataTable ExcelDs, TDQueryModel Query)
        {
            Tuple<List<Tuple<string, string>>, TDQueryModel> oReturn;
            Query.RelatedQueryInfoModel = new List<TDQueryInfoModel>();

            Uri node = new Uri(ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex(ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_ThirdKnowledgeIndex].Value);
            List<Tuple<string, string>> oCoincidences = new List<Tuple<string, string>>();

            ElasticClient ThirdKnowledgeClient = new ElasticClient(settings);
            int page = 0;

            for (int i = 0; i < ExcelDs.Rows.Count; i++)
            {
                string Name = ExcelDs.Rows[i][ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.C_Settings_ThirdKnowledgeNameCollumn].Value].ToString();
                string IdentificationNumber = ExcelDs.Rows[i][ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.C_Settings_ThirdKnowledgeIdNumberCollumn].Value].ToString();

                Nest.ISearchResponse<ThirdknowledgeIndexSearchModel> result = ThirdKnowledgeClient.Search<ThirdknowledgeIndexSearchModel>(s => s
               .From(0)
                   .TrackScores(true)
                   .From(page)
                   .Size(10)
                    .Query(q => q.QueryString(qr => qr.Fields(fds => fds.Field(f => f.CompleteName)).Query(Name)) ||
                            q.QueryString(qr => qr.Fields(fds => fds.Field(f => f.TypeId)).Query(IdentificationNumber))
                 ).MinScore(2));

                if (result.Documents.Count() > 0)
                {
                    result.Documents.All(x =>
                    {
                        TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();
                        oInfoCreate.AKA = x.AKA;
                        oInfoCreate.IdentificationResult = x.TypeId;
                        oInfoCreate.Offense = x.RelatedWiht;
                        oInfoCreate.NameResult = x.CompleteName;

                        if (x.ListType == "FIGURAS PUBLICAS" || x.ListType == "PEPS INTERNACIONALES")
                            oInfoCreate.Peps = x.ListType;
                        else
                            oInfoCreate.Peps = "N/A";

                        #region Group by Priority
                        if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(IdentificationNumber) && x.TypeId == IdentificationNumber.Trim() && x.CompleteName == Name.Trim())
                            oInfoCreate.Priority = "1";
                        else if (!string.IsNullOrEmpty(IdentificationNumber) && x.TypeId == IdentificationNumber.Trim() && x.CompleteName != Name.Trim())
                            oInfoCreate.Priority = "2";
                        else if (!string.IsNullOrEmpty(Name) && x.TypeId != IdentificationNumber.Trim() && x.CompleteName == Name.Trim())
                            oInfoCreate.Priority = "3";
                        else
                            oInfoCreate.Priority = "3";
                        #endregion

                        oInfoCreate.Status = x.Status;
                        oInfoCreate.Enable = true;
                        oInfoCreate.QueryPublicId = Query.QueryPublicId;


                        #region Create Detail
                        oInfoCreate.IdentificationNumber = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty;                        
                        oInfoCreate.QueryName = !string.IsNullOrEmpty(Name) ? Name : string.Empty;                        
                        oInfoCreate.AKA = !string.IsNullOrEmpty(x.AKA) ? x.AKA : string.Empty;                        
                        oInfoCreate.IdList = !string.IsNullOrEmpty(x.ListType) ? x.ListType : string.Empty;                        
                        oInfoCreate.Priority = !string.IsNullOrEmpty(oInfoCreate.Priority) ? oInfoCreate.Priority : string.Empty;

                        
                        oInfoCreate.CreateDate = DateTime.Now;
                        oInfoCreate.LastModify = DateTime.Now;

                        oInfoCreate.Offense = !string.IsNullOrEmpty(x.RelatedWiht) ? x.RelatedWiht : string.Empty;                        
                        oInfoCreate.IdentificationResult = !string.IsNullOrEmpty(x.TypeId) ? x.TypeId : string.Empty;                        
                        oInfoCreate.Status = !string.IsNullOrEmpty(x.Status) ? x.Status : string.Empty;                                                
                        oInfoCreate.GroupName = !string.IsNullOrEmpty(x.ListType) &&
                                     x.ListType.Contains("BOLETIN")
                                     || x.ListType == "FOREIGN CORRUPT PRACTICES ACT EEUU"
                                     || x.ListType == "FOREIGN FINANCIAL INSTITUTIONS PART 561_EEUU"
                                     || x.ListType == "FOREIGN SANCTIONS EVADERS LIST_EEUU"
                                     || x.ListType == "FOREIGN_TERRORIST_ORGANIZATIONS_EEUU_FTO"
                                     || x.ListType == "INTERPOL"
                                     || x.ListType == "MOST WANTED FBI"
                                     || x.ListType == "NACIONES UNIDAS"
                                     || x.ListType == "NON-SDN IRANIAN SANCTIONS ACT LIST (NS-ISA)_EEUU"
                                     || x.ListType == "OFAC"
                                     || x.ListType == "PALESTINIAN LEGISLATIVE COUNCIL LIST_EEUU"
                                     || x.ListType == "VINCULADOS" ?
                                     "LISTAS RESTRICTIVAS" + " - Criticidad Alta" :
                                     x.ListType == "CONSEJO NACIONAL ELECTORAL"
                                     || x.ListType == "CONSEJO SUPERIOR DE LA JUDICATURA"
                                     || x.ListType == "CORTE CONSTITUCIONAL"
                                     || x.ListType == "CORTE SUPREMA DE JUSTICIA"
                                     || x.ListType == "DENIED PERSONS LIST_EEUU"
                                     || x.ListType == "DESMOVILIZADOS"
                                     || x.ListType == "EMBAJADAS EN COLOMBIA"
                                     || x.ListType == "EMBAJADAS EN EL EXTERIOR"
                                     || x.ListType == "ENTITY_LIST_EEUU"
                                     || x.ListType == "FUERZAS MILITARES"
                                     || x.ListType == "GOBIERNO DEPARTAMENTAL"
                                     || x.ListType == "GOBIERNO MUNICIPAL"
                                     || x.ListType == "GOBIERNO NACIONAL"
                                     || x.ListType == "HM_TREASURY (BOE)"
                                     || x.ListType == "ONU_RESOLUCION_1929"
                                     || x.ListType == "ONU_RESOLUCION_1970"
                                     || x.ListType == "ONU_RESOLUCION_1973"
                                     || x.ListType == "ONU_RESOLUCION_1975"
                                     || x.ListType == "ONU_RESOLUCION_1988"
                                     || x.ListType == "ONU_RESOLUCION_1988"
                                     || x.ListType == "ONU_RESOLUCION_1988"
                                     || x.ListType == "ONU_RESOLUCION_2023"
                                     || x.ListType == "SECTORAL SANCTIONS IDENTIFICATIONS_LIST_EEUU"
                                     || x.ListType == "SPECIALLY DESIGNATED NATIONALS LIST_EEUU"
                                     || x.ListType == "SUPER SOCIEDADES"
                                     || x.ListType == "UNVERIFIED_LIST_EEUU" ?
                                     x.ListType + " - Criticidad Media" :
                                     x.ListType == "ESTRUCTURA DE GOBIERNO"
                                     || x.ListType == "FIGURAS PUBLICAS"
                                     || x.ListType == "PANAMA PAPERS"
                                     || x.ListType == "PARTIDOS Y MOVIMIENTOS POLITICOS"
                                     || x.ListType == "PEPS INTERNACIONALES" ?
                                     x.ListType + " - Criticidad Baja" : "NA";
                        
                        oInfoCreate.GroupId = !string.IsNullOrEmpty(x.Code) ? x.Code : string.Empty;                        
                        oInfoCreate.IdList = !string.IsNullOrEmpty(x.TableCodeID) ? x.TableCodeID : string.Empty;                        
                        oInfoCreate.Link = !string.IsNullOrEmpty(x.Source) ? x.Source : string.Empty;                        
                        oInfoCreate.FullName = !string.IsNullOrEmpty(x.CompleteName) ? x.CompleteName : string.Empty;                        
                        oInfoCreate.ListName = !string.IsNullOrEmpty(x.ListType) ? x.ListType : string.Empty;                        
                        oInfoCreate.MoreInfo = !string.IsNullOrEmpty(x.ORoldescription1) || !string.IsNullOrEmpty(x.ORoldescription2) ? x.ORoldescription1 : string.Empty;                        
                        oInfoCreate.Zone = !string.IsNullOrEmpty(x.NationalitySourceCountry) ? x.NationalitySourceCountry : string.Empty;
                        
                        #endregion

                        //Create Info Conincidences                                        
                        oCoincidences.Add(new Tuple<string, string>(IdentificationNumber, Name));
                        Query.RelatedQueryInfoModel.Add(oInfoCreate);
                        return true;
                    });
                }
            }
            oReturn = new Tuple<List<Tuple<string, string>>, TDQueryModel>(oCoincidences, Query);
            return oReturn;
        }

        #endregion
    }
}
