﻿using MarketPlace.Models.Company;
using MarketPlace.Models.General;
using MarketPlace.Models.Provider;
using MarketPlace.Models.ThirdKnowledge;
using Nest;
using NetOffice.ExcelApi;
using NetOffice.ExcelApi.Enums;
using OfficeOpenXml;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MarketPlace.Web.ControllersApi
{
    public class ThirdKnowledgeApiController : BaseApiController
    {
        [HttpPost]
        [HttpGet]
        public async Task<ProviderViewModel> TKSingleSearch(string TKSingleSearch)
        {
            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThirdKnowledge = new ThirdKnowledgeViewModel();
            List<PlanModel> oCurrentPeriodList = new List<PlanModel>();
            
            try
            {
                //Get The Active Plan By Customer
                oCurrentPeriodList = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetCurrenPeriod(SessionModel.CurrentCompany.CompanyPublicId, true);

                if (oCurrentPeriodList != null && oCurrentPeriodList.Count > 0)
                {
                    oModel.RelatedThirdKnowledge.HasPlan = true;

                    //Get The Most Recently Period When Plan is More Than One
                    oModel.RelatedThirdKnowledge.CurrentPlanModel = oCurrentPeriodList.OrderByDescending(x => x.CreateDate).First();

                    #region Upsert Process

                    if (System.Web.HttpContext.Current.Request["UpsertRequest"] == "true")
                    {
                        //Set Current Sale
                        if (oModel.RelatedThirdKnowledge != null)
                        {
                            //Save Query
                            TDQueryModel oQueryToCreate = new TDQueryModel()
                            {
                                CompayPublicId = SessionModel.CurrentCompany.CompanyPublicId,
                                IsSuccess = true,
                                PeriodPublicId = oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId,
                                SearchType = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryType.Simple,
                                },
                                User = SessionModel.CurrentLoginUser.Email,
                                QueryStatus = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryStatus.Finalized
                                },
                            };
                            oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();
                            oModel.RelatedThidKnowledgeSearch.CollumnsResult = new TDQueryModel();

                            //Get Result

                            //Identification Type
                            int IdType = System.Web.HttpContext.Current.Request["ThirdKnowledgeIdType"] == "213002" ? 1 : System.Web.HttpContext.Current.Request["ThirdKnowledgeIdType"] == "213001" ? 2 : 0;
                            string rNamer = System.Web.HttpContext.Current.Request["Name"];
                            string rIdNumber = System.Web.HttpContext.Current.Request["IdentificationNumber"];

                            var qTask = await Task.WhenAll(ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest(oCurrentPeriodList.FirstOrDefault().
                                            RelatedPeriodModel.FirstOrDefault().PeriodPublicId, IdType,
                                           rIdNumber, rNamer, oQueryToCreate)).ConfigureAwait(false);
                            #region Index TDQueryInfo

                            var oModelToIndex = new ProveedoresOnLine.IndexSearch.Models.TK_QueryIndexModel(oQueryToCreate);

                            oModelToIndex.Domain = oQueryToCreate.User.Split('@')[1];

                            Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                            var settings = new ConnectionSettings(node);
                            settings.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_QueryModelIndex].Value);
                            ElasticClient client = new ElasticClient(settings);

                            ICreateIndexResponse oElasticResponse = client.
                                    CreateIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_QueryModelIndex].Value, c => c
                                    .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                                    .Analysis(a => a.
                                        Analyzers(an => an.
                                            Custom("customWhiteSpace", anc => anc.
                                                Filters("asciifolding", "lowercase").
                                                Tokenizer("whitespace")
                                                    )
                                                ).TokenFilters(tf => tf
                                                .EdgeNGram("customEdgeNGram", engrf => engrf
                                                .MinGram(1)
                                                .MaxGram(10))
                                            )
                                        ).NumberOfShards(1)
                                    )
                                );
                            client.Map<TK_QueryIndexModel>(m => m.AutoMap());
                            var Index = client.Index(oModelToIndex);

                            #endregion


                            oModel.RelatedThidKnowledgeSearch.CollumnsResult = qTask.FirstOrDefault();
                            
                            List<Tuple<string, List<ThirdKnowledgeViewModel>>> Group = new List<Tuple<string, List<ThirdKnowledgeViewModel>>>();
                            List<string> Item1 = new List<string>();
                            List<string> ItemGroup = new List<string>();
                            
                            if (oModel.RelatedThidKnowledgeSearch.CollumnsResult != null)
                            {
                                oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.All(x =>
                                {
                                    Item1.Add(x.GroupName);
                                    return true;
                                });                             
                                ItemGroup.Add(Item1.Where(x => x.Contains("Criticidad Alta")).Select(x => x).DefaultIfEmpty("").FirstOrDefault());
                                ItemGroup.AddRange(Item1.Where(x => x.Contains("Criticidad Media")).Select(x => x).DefaultIfEmpty("").Distinct().ToList());
                                ItemGroup.Add(Item1.Where(x => x.Contains("Criticidad Baja")).Select(x => x).DefaultIfEmpty("").FirstOrDefault());
                                ItemGroup.Add(Item1.Where(x => x.Contains("SIN COINCIDENCIAS")).Select(x => x).DefaultIfEmpty("").FirstOrDefault());                                

                                Item1 = ItemGroup.Where(x => !string.IsNullOrEmpty(x)).Select(x => x).ToList();

                                List<ThirdKnowledgeViewModel> oItem2 = new List<ThirdKnowledgeViewModel>();
                                Tuple<string, List<ThirdKnowledgeViewModel>> oTupleItem = new Tuple<string, List<ThirdKnowledgeViewModel>>("", new List<ThirdKnowledgeViewModel>());

                                Item1.All(x =>
                                {
                                    oItem2 = new List<ThirdKnowledgeViewModel>();
                                    if (oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.Where(td => td.GroupName.Contains(x)) != null)
                                    {
                                        if (x.Contains("Criticidad Alta"))
                                        {
                                            oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.Where(td => td.GroupName.Contains("Criticidad Alta")).All(d =>
                                            {
                                                d.QueryPublicId = oModel.RelatedThidKnowledgeSearch.CollumnsResult.QueryPublicId;
                                                oItem2.Add(new ThirdKnowledgeViewModel(d));
                                                return true;
                                            });
                                        }
                                        else if (x.Contains("Criticidad Media"))
                                        {
                                            oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.Where(td => td.GroupName == x).All(d =>
                                            {
                                                d.QueryPublicId = oModel.RelatedThidKnowledgeSearch.CollumnsResult.QueryPublicId;
                                                oItem2.Add(new ThirdKnowledgeViewModel(d));
                                                return true;
                                            });
                                        }
                                        else if (x.Contains("Criticidad Baja"))
                                        {
                                            oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.Where(td => td.GroupName.Contains("Criticidad Baja")).All(d =>
                                            {
                                                d.QueryPublicId = oModel.RelatedThidKnowledgeSearch.CollumnsResult.QueryPublicId;
                                                oItem2.Add(new ThirdKnowledgeViewModel(d));
                                                return true;
                                            });
                                        }
                                        else if (x.Contains("SIN COINCIDENCIAS"))
                                        {
                                            oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.Where(td => td.GroupName.Contains("SIN COINCIDENCIAS")).All(d =>
                                            {
                                                d.QueryPublicId = oModel.RelatedThidKnowledgeSearch.CollumnsResult.QueryPublicId;
                                                oItem2.Add(new ThirdKnowledgeViewModel(d));
                                                return true;
                                            });
                                        }

                                        oTupleItem = new Tuple<string, List<ThirdKnowledgeViewModel>>(x, oItem2);
                                        Group.Add(oTupleItem);
                                    }
                                    return true;
                                });
                                if (Group != null)
                                {
                                    oModel.RelatedSingleSearch = Group;
                                }

                                if (oModel.RelatedThidKnowledgeSearch.CollumnsResult.QueryPublicId != null)
                                {
                                    //Set New Score
                                    oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().TotalQueries = (oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault().TotalQueries + 1);

                                    //Period Upsert
                                    oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.PeriodoUpsert(
                                        oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault());
                                }                                                                                                 
                            }                            
                        }
                        else
                        {
                            TDQueryModel oQueryToCreate = new TDQueryModel()
                            {
                                IsSuccess = false,
                                PeriodPublicId = oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId,
                                SearchType = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryType.Simple,
                                },
                                User = SessionModel.CurrentLoginUser.Email,
                            };
                        }
                    }

                    #endregion Upsert Process
                }
                else
                {
                    oModel.RelatedThirdKnowledge.HasPlan = false;
                }
                return oModel;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpPost]
        [HttpGet]
        public async Task<ProviderViewModel> TKSingleDetail(string QueryPublicId)
        {
            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThirdKnowledge = new ThirdKnowledgeViewModel();
            List<PlanModel> oCurrentPeriodList = new List<PlanModel>();

            try
            {
                //Get The Active Plan By Customer
                oCurrentPeriodList = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetCurrenPeriod(SessionModel.CurrentCompany.CompanyPublicId, true);

                if (oCurrentPeriodList != null && oCurrentPeriodList.Count > 0)
                {
                    oModel.RelatedThirdKnowledge.HasPlan = true;

                    //Get The Most Recently Period When Plan is More Than One
                    oModel.RelatedThirdKnowledge.CurrentPlanModel = oCurrentPeriodList.OrderByDescending(x => x.CreateDate).First();

                    #region Upsert Process

                    if (System.Web.HttpContext.Current.Request["UpsertRequest"] == "true")
                    {
                        //Set Current Sale
                        if (oModel.RelatedThirdKnowledge != null)
                        {
                            //Save Query
                            TDQueryModel oQueryToCreate = new TDQueryModel()
                            {
                                IsSuccess = true,
                                PeriodPublicId = oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId,
                                SearchType = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryType.Simple,
                                },
                                User = SessionModel.CurrentLoginUser.Email,
                                QueryStatus = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryStatus.Finalized
                                },
                            };

                            Task<TDQueryModel> qTask = Task.Run(() =>  ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest(oCurrentPeriodList.FirstOrDefault().
                                            RelatedPeriodModel.FirstOrDefault().PeriodPublicId,1,
                                           System.Web.HttpContext.Current.Request["IdentificationNumber"], System.Web.HttpContext.Current.Request["Name"], oQueryToCreate));

                            oModel.RelatedThidKnowledgeSearch.CollumnsResult = await qTask;

                            oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();
                            oModel.RelatedThidKnowledgeSearch.CollumnsResult = new TDQueryModel();
                            
                            if (oModel.RelatedThidKnowledgeSearch.CollumnsResult.IsSuccess == true)
                            {
                                //Set New Score
                                oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().TotalQueries = (oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault().TotalQueries + 1);

                                //Period Upsert
                                oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.PeriodoUpsert(
                                    oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault());
                            }
                        }
                        else
                        {
                            TDQueryModel oQueryToCreate = new TDQueryModel()
                            {
                                IsSuccess = false,
                                PeriodPublicId = oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId,
                                SearchType = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryType.Simple,
                                },
                                User = SessionModel.CurrentLoginUser.Email,
                            };
                        }
                    }

                    #endregion Upsert Process
                }
                else
                {
                    oModel.RelatedThirdKnowledge.HasPlan = false;
                }
                return oModel;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpPost]
        [HttpGet]
        public async Task<FileModel> TKLoadFile(string TKLoadFile, string CompanyPublicId, string PeriodPublicId)
        {
            FileModel oReturn = new FileModel();

            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Length > 0)
            {
                //get folder
                string strFolder = System.Web.HttpContext.Current.Server.MapPath
                    (Models.General.InternalSettings.Instance
                    [Models.General.Constants.C_Settings_File_TempDirectory].Value);

                if (!System.IO.Directory.Exists(strFolder))
                    System.IO.Directory.CreateDirectory(strFolder);

                //get File
                var UploadFile = System.Web.HttpContext.Current.Request.Files["ThirdKnowledge_FileUpload"];

                if (UploadFile != null && !string.IsNullOrEmpty(UploadFile.FileName))
                {
                    string oFileName = SessionModel.CurrentCompany.CompanyPublicId + "_" + "ThirdKnowledgeFile_" +
                            CompanyPublicId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." +
                        UploadFile.FileName.Split('.').DefaultIfEmpty("xlsx").LastOrDefault();
                    oFileName = oFileName.Split('.').LastOrDefault() == "xls" ? oFileName.Replace("xls", "xlsx") : oFileName;
                    string strFilePath = strFolder.TrimEnd('\\') + "\\" + oFileName;

                    UploadFile.SaveAs(strFilePath);

                    Tuple<bool, string> oVerifyResult = this.FileVerify(strFilePath, oFileName, PeriodPublicId);
                    bool isValidFile = oVerifyResult.Item1;

                    string strRemoteFile = string.Empty;
                    if (isValidFile)
                    {
                        //load file to s3
                        strRemoteFile = ProveedoresOnLine.FileManager.FileController.LoadFile
                            (strFilePath,
                            Models.General.InternalSettings.Instance[Models.General.Constants.C_Settings_File_ThirdKnowledgeRemoteDirectory].Value);

                        TDQueryModel oQueryToCreate = new TDQueryModel()
                        {
                            IsSuccess = isValidFile,
                            PeriodPublicId = PeriodPublicId,
                            QueryStatus = new TDCatalogModel()
                            {
                                ItemId = (int)enumThirdKnowledgeQueryStatus.InProcess
                            },
                            SearchType = new TDCatalogModel()
                            {
                                ItemId = (int)enumThirdKnowledgeQueryType.Masive,
                            },
                            User = SessionModel.CurrentLoginUser.Email,
                            FileName = oFileName,
                        };

                        oQueryToCreate.QueryPublicId = await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.QueryCreate(oQueryToCreate);

                        oQueryToCreate = await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.QueryUpsert(oQueryToCreate);

                        #region Index TDQueryInfo

                        var oModelToIndex = new ProveedoresOnLine.IndexSearch.Models.TK_QueryIndexModel(oQueryToCreate);

                        oModelToIndex.Domain = oQueryToCreate.User.Split('@')[1];

                        Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                        var settings = new ConnectionSettings(node);
                        settings.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_QueryModelIndex].Value);
                        ElasticClient client = new ElasticClient(settings);

                        ICreateIndexResponse oElasticResponse = client.
                                CreateIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_QueryModelIndex].Value, c => c
                                .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                                .Analysis(a => a.
                                    Analyzers(an => an.
                                        Custom("customWhiteSpace", anc => anc.
                                            Filters("asciifolding", "lowercase").
                                            Tokenizer("whitespace")
                                                )
                                            ).TokenFilters(tf => tf
                                            .EdgeNGram("customEdgeNGram", engrf => engrf
                                            .MinGram(1)
                                            .MaxGram(10))
                                        )
                                    ).NumberOfShards(1)
                                )
                            );
                        client.Map<TK_QueryIndexModel>(m => m.AutoMap());
                        var Index = client.Index(oModelToIndex);

                        #endregion

                        //Send Message
                        MessageModule.Client.Models.NotificationModel oDataMessage = new MessageModule.Client.Models.NotificationModel();
                        oDataMessage.CompanyPublicId = CompanyPublicId;
                        oDataMessage.User = SessionModel.CurrentLoginUser.Email;
                        oDataMessage.CompanyLogo = SessionModel.CurrentCompany_CompanyLogo;
                        oDataMessage.CompanyName = SessionModel.CurrentCompany.CompanyName;
                        oDataMessage.IdentificationType = SessionModel.CurrentCompany.IdentificationType.ItemName;
                        oDataMessage.IdentificationNumber = SessionModel.CurrentCompany.IdentificationNumber;

                        #region Notification

                        oDataMessage.Label = Models.General.InternalSettings.Instance
                                [Models.General.Constants.N_ThirdKnowledgeUploadMassiveMessage].Value;
                        oDataMessage.Url = Models.General.InternalSettings.Instance
                                [Models.General.Constants.N_UrlThirdKnowledgeQuery].Value.Replace("{{QueryPublicId}}", oQueryToCreate.QueryPublicId);
                        oDataMessage.NotificationType = (int)MarketPlace.Models.General.enumNotificationType.ThirdKnowledgeNotification;
                        oDataMessage.Enable = true;

                        #endregion

                        ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.CreateUploadNotification(oDataMessage);
                    }

                    //remove temporal file
                    if (System.IO.File.Exists(strFilePath))
                        System.IO.File.Delete(strFilePath);

                    oReturn = new FileModel()
                    {
                        FileName = UploadFile.FileName,
                        ServerUrl = strRemoteFile,
                        LoadMessage = isValidFile ? "El Archivo " + UploadFile.FileName + " es correcto, en unos momentos recibirá un correo con el respectivo resultado de la validación." :
                                                    "El Archivo " + UploadFile.FileName + " no es correcto, por favor verifique el nombre de las columnas y el formato.",
                        AdditionalInfo = oVerifyResult.Item2,
                    };
                }
            }
            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public async Task<FileModel> TKReSearchMasive(string TKReSearchMasive, string CompanyPublicId, string PeriodPublicId, string FileName)
        {
            FileModel oReturn = new FileModel();

            if (!string.IsNullOrEmpty(FileName))
            {
                //get folder
                string strFolder = System.Web.HttpContext.Current.Server.MapPath
                    (Models.General.InternalSettings.Instance
                    [Models.General.Constants.C_Settings_File_TempDirectory].Value);

                if (!System.IO.Directory.Exists(strFolder))
                    System.IO.Directory.CreateDirectory(strFolder);

                //Download Excel to process
                using (WebClient webClient = new WebClient())
                {
                    //Get file from S3 using File Name           
                    webClient.DownloadFile(Models.General.InternalSettings.Instance[
                    Models.General.Constants.TK_File_S3FilePath].Value + FileName, strFolder + "ReSearch_" + FileName);                                     
                }

                string oFileName = "ReSearch_" + FileName;

                string strFilePath = strFolder.TrimEnd('\\') + "\\" + oFileName;

                Tuple<bool, string> oVerifyResult = this.FileVerify(strFilePath, oFileName, PeriodPublicId);
                bool isValidFile = oVerifyResult.Item1;

                string strRemoteFile = string.Empty;
                if (isValidFile)
                {
                    //load file to s3
                    strRemoteFile = ProveedoresOnLine.FileManager.FileController.LoadFile
                        (strFilePath,
                        Models.General.InternalSettings.Instance[Models.General.Constants.C_Settings_File_ThirdKnowledgeRemoteDirectory].Value);

                    TDQueryModel oQueryToCreate = new TDQueryModel()
                    {
                        IsSuccess = isValidFile,
                        PeriodPublicId = PeriodPublicId,
                        QueryStatus = new TDCatalogModel()
                        {
                            ItemId = (int)enumThirdKnowledgeQueryStatus.InProcess
                        },
                        SearchType = new TDCatalogModel()
                        {
                            ItemId = (int)enumThirdKnowledgeQueryType.Masive,
                        },
                        User = SessionModel.CurrentLoginUser.Email,
                        FileName = oFileName,
                    };

                    oQueryToCreate =  await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.QueryUpsert(oQueryToCreate);

                    #region Index TDQueryInfo

                    var oModelToIndex = new ProveedoresOnLine.IndexSearch.Models.TK_QueryIndexModel(oQueryToCreate);

                    oModelToIndex.Domain = oQueryToCreate.User.Split('@')[1];

                    Uri node = new Uri(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_ElasticSearchUrl].Value);
                    var settings = new ConnectionSettings(node);
                    settings.DefaultIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_QueryModelIndex].Value);
                    ElasticClient client = new ElasticClient(settings);

                    ICreateIndexResponse oElasticResponse = client.
                            CreateIndex(MarketPlace.Models.General.InternalSettings.Instance[MarketPlace.Models.General.Constants.C_Settings_QueryModelIndex].Value, c => c
                            .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                            .Analysis(a => a.
                                Analyzers(an => an.
                                    Custom("customWhiteSpace", anc => anc.
                                        Filters("asciifolding", "lowercase").
                                        Tokenizer("whitespace")
                                            )
                                        ).TokenFilters(tf => tf
                                        .EdgeNGram("customEdgeNGram", engrf => engrf
                                        .MinGram(1)
                                        .MaxGram(10))
                                    )
                                ).NumberOfShards(1)
                            )
                        );
                    client.Map<TK_QueryIndexModel>(m => m.AutoMap());
                    var Index = client.Index(oModelToIndex);

                    #endregion

                    //Send Message
                    MessageModule.Client.Models.NotificationModel oDataMessage = new MessageModule.Client.Models.NotificationModel();
                    oDataMessage.CompanyPublicId = CompanyPublicId;
                    oDataMessage.User = SessionModel.CurrentLoginUser.Email;
                    oDataMessage.CompanyLogo = SessionModel.CurrentCompany_CompanyLogo;
                    oDataMessage.CompanyName = SessionModel.CurrentCompany.CompanyName;
                    oDataMessage.IdentificationType = SessionModel.CurrentCompany.IdentificationType.ItemName;
                    oDataMessage.IdentificationNumber = SessionModel.CurrentCompany.IdentificationNumber;

                    #region Notification

                    oDataMessage.Label = Models.General.InternalSettings.Instance
                            [Models.General.Constants.N_ThirdKnowledgeUploadMassiveMessage].Value;
                    oDataMessage.Url = Models.General.InternalSettings.Instance
                            [Models.General.Constants.N_UrlThirdKnowledgeQuery].Value.Replace("{{QueryPublicId}}", oQueryToCreate.QueryPublicId);
                    oDataMessage.NotificationType = (int)MarketPlace.Models.General.enumNotificationType.ThirdKnowledgeNotification;
                    oDataMessage.Enable = true;

                    #endregion

                    ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.CreateUploadNotification(oDataMessage);

                    //remove temporal file
                    if (System.IO.File.Exists(strFilePath))
                        System.IO.File.Delete(strFilePath);

                    oReturn = new FileModel()
                    {
                        FileName = "ReSearch" + FileName,
                        ServerUrl = strRemoteFile,
                        LoadMessage = isValidFile ? "El Archivo " + "ReSearch" + FileName + " es correcto, en unos momentos recibirá un correo con el respectivo resultado de la validación." :
                                                    "El Archivo " + "ReSearch" + FileName + " no es correcto, por favor verifique el nombre de las columnas y el formato.",
                        AdditionalInfo = oVerifyResult.Item2,
                    };
                }
            }
            return oReturn;
        }

        #region ThirdKnowledge Charts

        [HttpPost]
        [HttpGet]
        public List<Tuple<string, int, int>> GetPeriodsByPlan(string GetPeriodsByPlan)
        {
            //Get Charts By Module
            List<GenericChartsModel> oResult = new List<GenericChartsModel>();
            GenericChartsModel oRelatedChart = null;

            oRelatedChart = new GenericChartsModel()
            {
                ChartModuleType = ((int)enumCategoryInfoType.CH_ThirdKnowledgeModule).ToString(),
                GenericChartsInfoModel = new List<GenericChartsModelInfo>(),
            };

            List<ProveedoresOnLine.ThirdKnowledge.Models.PlanModel> oPlanModel = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetAllPlanByCustomer(SessionModel.CurrentCompany.CompanyPublicId, true);

            List<Tuple<string, int, int>> oReturn = new List<Tuple<string, int, int>>();
            if (oPlanModel != null)
            {
                oPlanModel.All(x =>
                {
                    x.RelatedPeriodModel.All(y =>
                    {
                        oReturn.Add(Tuple.Create(y.InitDate.ToString("dd/MM/yy") + " - " + y.EndDate.ToString("dd/MM/yy")
                            , y.TotalQueries, y.AssignedQueries));
                        return true;
                    });
                    return true;
                });
            }

            return oReturn;
        }

        #endregion ThirdKnowledge Charts

        #region Private Functions

        [HttpPost]
        [HttpGet]
        public Tuple<bool, string> FileVerify(string FilePath, string FileName, string PeriodPublicId)
        {
            var Excel = new FileInfo(FilePath);

            Tuple<bool, string> oReturn = new Tuple<bool, string>(false, "");
            List<PlanModel> oCurrentPeriodList = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetCurrenPeriod(SessionModel.CurrentCompany.CompanyPublicId, true);
            using (var package = new ExcelPackage(Excel))
            {
                // Get the work book in the file
                ExcelWorkbook workBook = package.Workbook;

                if (workBook != null)
                {
                    object[,] values = (object[,])workBook.Worksheets.First().Cells["A1:C1"].Value;

                    string UncodifiedObj = new JavaScriptSerializer().Serialize(values);
                    if (UncodifiedObj.Contains(Models.General.InternalSettings.Instance
                                    [Models.General.Constants.MP_CP_ColIdNumber].Value)
                        && UncodifiedObj.Contains(Models.General.InternalSettings.Instance
                                    [Models.General.Constants.MP_CP_ColIdName].Value))
                    {
                        //Get The Active Plan By Customer                            
                        oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault().TotalQueries += (workBook.Worksheets[1].Dimension.End.Row - 1);
                        ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.PeriodoUpsert(oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault());

                        oReturn = new Tuple<bool, string>(true, oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault().TotalQueries.ToString());                        
                    }
                    else
                    {
                        oReturn = new Tuple<bool, string>(false, oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault().TotalQueries.ToString());
                    }
                }
            }
            return oReturn;            
        }

        #endregion Private Functions

        [HttpPost]
        [HttpGet]
        public async Task<ProviderViewModel> TKSingleReSearch(string TKSingleReSearch, string IdentificationNumber, string Name, string IdType)
        {
            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThirdKnowledge = new ThirdKnowledgeViewModel();
            List<PlanModel> oCurrentPeriodList = new List<PlanModel>();

            try
            {
                //Get The Active Plan By Customer
                oCurrentPeriodList = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetCurrenPeriod(SessionModel.CurrentCompany.CompanyPublicId, true);

                if (oCurrentPeriodList != null && oCurrentPeriodList.Count > 0)
                {
                    oModel.RelatedThirdKnowledge.HasPlan = true;

                    //Get The Most Recently Period When Plan is More Than One
                    oModel.RelatedThirdKnowledge.CurrentPlanModel = oCurrentPeriodList.OrderByDescending(x => x.CreateDate).First();

                    #region Upsert Process

                    if (System.Web.HttpContext.Current.Request["UpsertRequest"] == "true")
                    {
                        //Set Current Sale
                        if (oModel.RelatedThirdKnowledge != null)
                        {
                            //Save Query
                            TDQueryModel oQueryToCreate = new TDQueryModel()
                            {
                                IsSuccess = true,
                                PeriodPublicId = oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId,
                                SearchType = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryType.Simple,
                                },
                                User = SessionModel.CurrentLoginUser.Email,
                                QueryStatus = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryStatus.Finalized
                                },
                            };
                            oModel.RelatedThidKnowledgeSearch = new ThirdKnowledgeViewModel();
                            oModel.RelatedThidKnowledgeSearch.CollumnsResult = new TDQueryModel();

                            //Get Reqsult
                            int IdNumberType = IdType == "213002" ? 1 : IdType == "213001" ? 2 : 0;

                            Task<TDQueryModel> qTask = Task.Run(() => ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest(oCurrentPeriodList.FirstOrDefault().
                                            RelatedPeriodModel.FirstOrDefault().PeriodPublicId, IdNumberType, IdentificationNumber, Name, oQueryToCreate));

                            oModel.RelatedThidKnowledgeSearch.CollumnsResult = await qTask;
                            
                            //Init Finally Tuple, Group by ItemGroup Name
                            List<Tuple<string, List<ThirdKnowledgeViewModel>>> Group = new List<Tuple<string, List<ThirdKnowledgeViewModel>>>();
                            List<string> Item1 = new List<string>();
                            if (oModel.RelatedThidKnowledgeSearch.CollumnsResult != null && oModel.RelatedThidKnowledgeSearch.CollumnsResult.IsSuccess)
                            {
                                oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.All(x =>
                                {
                                    Item1.Add(x.GroupName);
                                    return true;
                                });
                                Item1 = Item1.GroupBy(x => x).Select(grp => grp.Last()).ToList();

                                List<ThirdKnowledgeViewModel> oItem2 = new List<ThirdKnowledgeViewModel>();
                                Tuple<string, List<ThirdKnowledgeViewModel>> oTupleItem = new Tuple<string, List<ThirdKnowledgeViewModel>>("", new List<ThirdKnowledgeViewModel>());

                                Item1.All(x =>
                                {
                                    if (oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.Where(td => td.GroupName == x) != null)
                                    {
                                        oItem2 = new List<ThirdKnowledgeViewModel>();
                                        oItem2.Add(new ThirdKnowledgeViewModel(oModel.RelatedThidKnowledgeSearch.CollumnsResult.RelatedQueryInfoModel.Where(td => td.GroupName == x).Select(td => td).FirstOrDefault()));
                                        oTupleItem = new Tuple<string, List<ThirdKnowledgeViewModel>>(x, oItem2);
                                        Group.Add(oTupleItem);
                                    }
                                    return true;
                                });
                                if (Group != null)
                                    oModel.RelatedSingleSearch = Group;


                                if (oModel.RelatedThidKnowledgeSearch.CollumnsResult.QueryPublicId != null)
                                {
                                    //Set New Score
                                    oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().TotalQueries = (oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault().TotalQueries + 1);

                                    //Period Upsert
                                    oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.PeriodoUpsert(
                                        oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault());
                                }
                            }
                        }
                        else
                        {
                            TDQueryModel oQueryToCreate = new TDQueryModel()
                            {
                                IsSuccess = false,
                                PeriodPublicId = oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId,
                                SearchType = new TDCatalogModel()
                                {
                                    ItemId = (int)enumThirdKnowledgeQueryType.Simple,
                                },
                                User = SessionModel.CurrentLoginUser.Email,
                            };
                        }
                    }

                    #endregion Upsert Process
                }
                else
                {
                    oModel.RelatedThirdKnowledge.HasPlan = false;
                }
                return oModel;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
