﻿using MarketPlace.Models.General;
using MarketPlace.Models.Provider;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.SurveyModule.Models;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using NetOffice.ExcelApi;
//using LinqToExcel;

namespace MarketPlace.Web.ControllersApi
{
    public class ThirdKnowledgeApiController : BaseApiController
    {
        [HttpPost]
        [HttpGet]
        public ProviderViewModel TKSingleSearch(string TKSingleSearch)
        {
            ProviderViewModel oModel = new ProviderViewModel();
            oModel.RelatedThirdKnowledge = new MarketPlace.Models.Company.ThirdKnowledgeViewModel();
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
                                RelatedQueryInfoModel = new List<TDQueryInfoModel>(),
                            };

                            oModel.RelatedThirdKnowledge.CollumnsResult = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest(oCurrentPeriodList.FirstOrDefault().
                                            RelatedPeriodModel.FirstOrDefault().PeriodPublicId,
                                           System.Web.HttpContext.Current.Request["IdentificationNumber"], System.Web.HttpContext.Current.Request["Name"], oQueryToCreate);

                            //Set New Score
                            oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().TotalQueries = (oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault().TotalQueries + 1);

                            //Period Upsert
                            oModel.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel.FirstOrDefault().PeriodPublicId = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.PeriodoUpsert(
                                oCurrentPeriodList.FirstOrDefault().RelatedPeriodModel.FirstOrDefault());
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
                    #endregion
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
        public FileModel TKLoadFile(string TKLoadFile, string CompanyPublicId)
        {
            FileModel oReturn = new FileModel();

            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Length > 0)
            {
                //get folder
                string strFolder = System.Web.HttpContext.Current.Server.MapPath
                    (MarketPlace.Models.General.InternalSettings.Instance
                    [MarketPlace.Models.General.Constants.C_Settings_File_TempDirectory].Value);

                if (!System.IO.Directory.Exists(strFolder))
                    System.IO.Directory.CreateDirectory(strFolder);

                //get File
                var UploadFile = System.Web.HttpContext.Current.Request.Files["ThirdKnowledge_FileUpload"];

                if (UploadFile != null && !string.IsNullOrEmpty(UploadFile.FileName))
                {
                    string strFile = strFolder.TrimEnd('\\') +
                    "\\ThirdKnowledgeFile_" +
                        CompanyPublicId + "_" +
                        DateTime.Now.ToString("yyyyMMddHHmmss") + "." +
                        UploadFile.FileName.Split('.').DefaultIfEmpty("xls").LastOrDefault();

                    UploadFile.SaveAs(strFile);

                    bool isValidFile = this.FileVerify(strFile, "ThirdKnowledgeFile_" + 
                            CompanyPublicId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");

                    string strRemoteFile = string.Empty;
                    if (isValidFile)
                    {
                        //load file to s3
                        strRemoteFile = ProveedoresOnLine.FileManager.FileController.LoadFile
                            (strFile,
                            MarketPlace.Models.General.InternalSettings.Instance
                                [MarketPlace.Models.General.Constants.C_Settings_File_RemoteDirectory].Value.TrimEnd('\\') +
                            "\\ThirdKnowledge\\" + CompanyPublicId + "_" + DateTime.Now + "\\");                    
                    }                    

                    //remove temporal file
                    if (System.IO.File.Exists(strFile))
                        System.IO.File.Delete(strFile);
                    
                    oReturn = new MarketPlace.Models.General.FileModel()
                    {
                        FileName = UploadFile.FileName,
                        ServerUrl = strRemoteFile,
                        LoadMessage = isValidFile ? "El Archivo " + UploadFile.FileName + " es correto, en unos momentos recibirá un correo con el respectivo resultado de la validación." :
                                                    "El Archivo " + UploadFile.FileName + " no es correto, por favor verifique el nombre de las columnas y el formato." 
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

            List<ProveedoresOnLine.ThirdKnowledge.Models.PlanModel> oPlanModel = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetAllPlanByCustomer(MarketPlace.Models.General.SessionModel.CurrentCompany.CompanyPublicId, true);

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

        #endregion

        #region Private Functions

        [HttpPost]
        [HttpGet]
        public bool FileVerify(string FilePath, string FileName)
        {
            //var excel = new ExcelQueryFactory(FilePath);

            ////get excel rows
            //LinqToExcel.ExcelQueryFactory XlsInfo = new LinqToExcel.ExcelQueryFactory(FilePath);

            //List<ProveedoresOnLine.ThirdKnowledge.Models.> oPrvToProcess =
             //(from x in XlsInfo.Worksheet<ProviderExcelModel>(0)
             // select x).ToList();

            bool isLoaded = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.AccessFTPClient(FileName, FilePath);
            return true;
            //Excel.Application Aplication = new Excel.Application();

            //Excel.Workbook CurrentBook;
            //CurrentBook = Aplication.Workbooks.Open(FilePath);
            //Excel.Worksheet workSheet = (Excel.Worksheet)CurrentBook.Worksheets[1];

            //object[,] values = (object[,])workSheet.Range("A1:C1").Value;

            //string UncodifiedObj = new JavaScriptSerializer().Serialize(values);
            //if (UncodifiedObj.Contains(MarketPlace.Models.General.InternalSettings.Instance
            //                [MarketPlace.Models.General.Constants.MP_CP_ColPersonType].Value)
            //    && UncodifiedObj.Contains(MarketPlace.Models.General.InternalSettings.Instance
            //                [MarketPlace.Models.General.Constants.MP_CP_ColIdNumber].Value)
            //    && UncodifiedObj.Contains(MarketPlace.Models.General.InternalSettings.Instance
            //                [MarketPlace.Models.General.Constants.MP_CP_ColIdName].Value))
            //{
            //    CurrentBook.Close();
            //    bool isLoaded = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.AccessFTPClient(FileName, FilePath);
            //    return true;
            //}
            //else
            //{
            //    CurrentBook.Close();
            //    return false;
            //}            
        }        

        #endregion
    }
}
