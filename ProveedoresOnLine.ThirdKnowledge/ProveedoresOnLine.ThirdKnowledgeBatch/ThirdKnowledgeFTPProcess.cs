using ProveedoresOnLine.ThirdKnowledgeBatch.Models;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LinqToExcel;
using LinqToExcel.Domain;
using NetOffice.ExcelApi;
using NetOffice.ExcelApi.Enums;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System.Data;
using Nest;
using ProveedoresOnLine.IndexSearch.Models;
using System.Threading;
using Autofac;

namespace ProveedoresOnLine.ThirdKnowledgeBatch
{
    public class ThirdKnowledgeFTPProcess
    {
        public static void StartProcess()
        {
            try
            {
                //Get queries to process
                List<ProveedoresOnLine.ThirdKnowledge.Models.TDQueryModel> oQueryResult = new List<TDQueryModel>();

                oQueryResult = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetQueriesInProgress();

                if (oQueryResult != null)
                {
                    LogFile("Start Process:: Date" + DateTime.Now);
                    //Set access
                    string S3path = ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Setings_File_S3FilePath].Value;
                    string LocalPath = ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_File_TempDirectory].Value;

                    foreach (var oQuery in oQueryResult)
                    {
                        try
                        {
                            LogFile("Start Process:: QueryPublicId::" + oQuery.QueryPublicId);
                            //Download File from S3                            
                            //Local Path
                            string strFolder = ThirdKnowledge.Models.InternalSettings.Instance[ThirdKnowledge.Models.Constants.C_Settings_File_TempDirectory].Value;
                            if (!Directory.Exists(strFolder))
                                System.IO.Directory.CreateDirectory(strFolder);

                            //Download file from s3
                            using (WebClient webClient = new WebClient())
                            {
                                //Get file from S3 using File Name           
                                webClient.DownloadFile(ThirdKnowledge.Models.InternalSettings.Instance[
                                                    ThirdKnowledge.Models.Constants.C_Setings_File_S3FilePath].Value + oQuery.FileName, strFolder + oQuery.FileName);
                            }

                            //Read Excel File
                            System.Data.DataTable DT_Excel = ReadExcelFile(strFolder + oQuery.FileName);

                            oQuery.RelatedQueryInfoModel = new List<TDQueryInfoModel>();
                            List<Tuple<string, string>> oCoincidences = new List<Tuple<string, string>>();
                            Tuple<List<Tuple<string, string>>, TDQueryModel> oResult = new Tuple<List<Tuple<string, string>>, TDQueryModel>(oCoincidences, oQuery);
                            if (DT_Excel != null)
                            {
                                //Call Function to search excel file 
                                var task = SearchInfoFromFile(DT_Excel, oQuery);
                                oResult = task.Result;

                                //Update Status query
                                oQuery.QueryStatus = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledgeBatch.Models.Enumerations.enumThirdKnowledgeQueryStatus.Finalized,
                                };
                                oResult.Item2.QueryPublicId = ThirdKnowledge.Controller.ThirdKnowledgeModule.QueryCreate(oResult.Item2).Result;
                                ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.QueryUpsert(oResult.Item2);
                                CreateQueryInfo(oQuery, oResult.Item1);
                                CreateReadyResultNotification(oQuery);

                                #region Index TDQueryInfo

                                var oModelToIndex = new ProveedoresOnLine.IndexSearch.Models.TK_QueryIndexModel(oQuery);

                                oModelToIndex.Domain = oQuery.User.Split('@')[1];

                                Uri node = new Uri(ThirdKnowledge.Models.InternalSettings.Instance[ThirdKnowledge.Models.Constants.C_Settings_ElasticSearchUrl].Value);
                                var settings = new ConnectionSettings(node);
                                settings.DefaultIndex(ThirdKnowledge.Models.InternalSettings.Instance[ThirdKnowledge.Models.Constants.C_Settings_TD_QueryIndex].Value);
                                ElasticClient client = new ElasticClient(settings);

                                ICreateIndexResponse oElasticResponse = client.
                                        CreateIndex(ThirdKnowledge.Models.InternalSettings.Instance[ThirdKnowledge.Models.Constants.C_Settings_TD_QueryIndex].Value, c => c
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

                                LogFile("Success:: QueryPublicId '" + oQuery.QueryPublicId + "' :: Validation is success");
                            }
                            else
                            {
                                LogFile("Error:: QueryPublicId '" + oQuery.QueryPublicId + "' :: Excel Datatable is Empty");
                            }
                            //Remove all Files
                            //remove temporal file
                            if (File.Exists(strFolder + oQuery.FileName))
                                System.IO.File.Delete(strFolder + oQuery.FileName);
                        }
                        catch (Exception err)
                        {
                            LogFile("Error:: QueryPublicId '" + oQuery.QueryPublicId + "' :: " + err.Message + "Inner Exception::" + err.InnerException);
                        }
                    }

                }
                else
                {
                    //log file
                    LogFile("End Process No Files to Vaildate");
                }
            }
            catch (Exception err)
            {
                //log file for fatal error
                LogFile("Fatal error::" + err.Message + " - " + err.StackTrace + "Inner Exception::" + err.InnerException);
            }
        }

        #region Log File

        private static void LogFile(string LogMessage)
        {
            try
            {
                //get file Log
                string LogFile = AppDomain.CurrentDomain.BaseDirectory.Trim().TrimEnd(new char[] { '\\' }) + "\\" +
                    System.Configuration.ConfigurationManager.AppSettings[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.C_AppSettings_LogFile].Trim().TrimEnd(new char[] { '\\' });

                if (!System.IO.Directory.Exists(LogFile))
                    System.IO.Directory.CreateDirectory(LogFile);

                LogFile += "\\" + "Log_ThirdKnowledgeProcess_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(LogFile))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "::" + LogMessage);
                    sw.Close();
                }
            }
            catch { }
        }

        #endregion

        #region Message

        public static void CreateReadyResultNotification(TDQueryModel oQuery)
        {
            #region Email
            //Create message object

            MessageModule.Client.Models.ClientMessageModel oMessageToSend = new MessageModule.Client.Models.ClientMessageModel()
            {
                Agent = ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_TK_ReadyResultAgent].Value,
                User = "Proveedore OnLine ThirdKnowledge",
                ProgramTime = DateTime.Now,
                MessageQueueInfo = new List<Tuple<string, string>>(),
            };

            oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>("To", oQuery.User));

            oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                ("URLToRedirect", ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_TK_QueryUrl].Value.Replace("{QueryPublicId}", oQuery.QueryPublicId)));


            MessageModule.Client.Controller.ClientController.CreateMessage(oMessageToSend);
            #endregion

            #region Notification

            //TODO: Manage Notification
            //MessageModule.Client.Models.NotificationModel oNotification = new MessageModule.Client.Models.NotificationModel()
            //{
            //    CompanyPublicId = oQuery.CompayPublicId,
            //    NotificationType = (int)ThirdKnowledge.Models.Enumerations.enumNotificationType.ThirdKnowledgeNotification,
            //    Url = ThirdKnowledge.Models.InternalSettings.Instance
            //                    [ThirdKnowledge.Models.Constants.N_UrlThirdKnowledgeQuery].Value.Replace("{QueryPublicId}", oQuery.QueryPublicId),
            //    User = oQuery.User,
            //    Label = ThirdKnowledge.Models.InternalSettings.Instance
            //                    [ThirdKnowledge.Models.Constants.N_ThirdKnowledgeEndMassiveMessage].Value,
            //    Enable = true,
            //};

            //MessageModule.Client.Controller.ClientController.NotificationUpsert(oNotification);

            #endregion
        }
        #endregion

        #region Private Functions

        private static void CreateQueryInfo(TDQueryModel oQuery, List<Tuple<string, string>> oCoincidences)
        {
            try
            {
                //Local Path
                string strFolder = ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_File_TempDirectory].Value;
                if (!System.IO.Directory.Exists(strFolder))
                    System.IO.Directory.CreateDirectory(strFolder);
                oQuery.FileName = oQuery.FileName.Replace("xml", "xlsx");

                using (WebClient webClient = new WebClient())
                {
                    //Get file from S3 using File Name           
                    webClient.DownloadFile(ThirdKnowledge.Models.InternalSettings.Instance[
                                        ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Setings_File_S3FilePath].Value + oQuery.FileName, strFolder + oQuery.FileName);

                    //Set model Params
                    List<ProveedoresOnLine.ThirdKnowledgeBatch.Models.ExcelModel> oExcelToProcessInfo = null;

                    System.Data.DataTable DT_Excel = ReadExcelFile(strFolder + oQuery.FileName);
                    if (DT_Excel != null)
                    {
                        oExcelToProcessInfo = new List<ExcelModel>();
                        foreach (DataRow item in DT_Excel.Rows)
                        {
                            oExcelToProcessInfo.Add(new ExcelModel(item));
                        }
                    }

                    //Exclude Coincidences
                    List<ProveedoresOnLine.ThirdKnowledgeBatch.Models.ExcelModel> oExclude = null;
                    if (oCoincidences != null && oCoincidences.Count > 0)
                    {
                        oExclude = new List<ProveedoresOnLine.ThirdKnowledgeBatch.Models.ExcelModel>();
                        oCoincidences.All(x =>
                        {
                            oExclude.Add(new ProveedoresOnLine.ThirdKnowledgeBatch.Models.ExcelModel()
                            {
                                SEARCHCRITERY = x.Item1,
                                SEARCHPARAM = x.Item2,
                            });
                            return true;
                        });
                    }

                    if (oExclude != null)
                    {
                        oExclude.All(x =>
                        {
                            x.SEARCHCRITERY = string.IsNullOrEmpty(x.SEARCHCRITERY) ? "" : x.SEARCHCRITERY;
                            x.SEARCHPARAM = string.IsNullOrEmpty(x.SEARCHPARAM) ? "" : x.SEARCHPARAM;

                            oExcelToProcessInfo = oExcelToProcessInfo.Where(y => y.SEARCHCRITERY.ToLower() != x.SEARCHCRITERY.ToLower() || y.SEARCHPARAM != x.SEARCHPARAM).Select(y => y).ToList();
                            return true;
                        });
                    }

                    if (oExcelToProcessInfo != null)
                    {
                        List<string> Params = ThirdKnowledge.Models.InternalSettings.Instance[
                                        ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.Param_Critery].Value.Split(';').ToList();
                        oExcelToProcessInfo.All(x =>
                        {
                            if (!string.IsNullOrEmpty(x.SEARCHCRITERY) && !string.IsNullOrEmpty(x.SEARCHPARAM))
                            {
                                //Create QueryInfo
                                oQuery.RelatedQueryInfoModel = new List<TDQueryInfoModel>();

                                TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();
                                oInfoCreate.QueryPublicId = oQuery.QueryPublicId;

                                if (x.SEARCHPARAM != Params[3])
                                {
                                    oInfoCreate.QueryIdentification = !string.IsNullOrEmpty(x.SEARCHPARAM) ? x.SEARCHPARAM : string.Empty;
                                }
                                else if (x.SEARCHPARAM == Params[3])
                                {
                                    oInfoCreate.QueryName = !string.IsNullOrEmpty(x.SEARCHPARAM) ? x.SEARCHPARAM : string.Empty;
                                }

                                oInfoCreate.GroupName = "SIN COINCIDENCIAS";
                                oQuery.RelatedQueryInfoModel.Add(oInfoCreate);
                                Monitor.Enter(oQuery);
                                lock (oQuery)
                                {
                                    ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.QueryUpsert(oQuery);
                                }
                            }

                            return true;
                        });
                    }

                    //Remove all Files
                    //remove temporal file
                    if (System.IO.File.Exists(strFolder + oQuery.FileName))
                        System.IO.File.Delete(strFolder + oQuery.FileName);

                    //remove temporal file
                    if (System.IO.File.Exists(strFolder + oQuery.FileName.Replace("xlsx", "xls")))
                        System.IO.File.Delete(strFolder + oQuery.FileName.Replace("xlsx", "xls"));
                }
            }
            catch (Exception err)
            {
                //log file for fatal error
                LogFile("Function::CreateQueryInfo Fatal error::" + err.Message + " - " + err.StackTrace + "Inner Exception::" + err.InnerException);
            }

        }

        //Implement of EPPLUS package to read an excel file
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
                System.Data.DataTable DT_Excel = new System.Data.DataTable();
                foreach (var FirstRowCell in WS.Cells[1, 1, 1, WS.Dimension.End.Column])
                {
                    DT_Excel.Columns.Add(HasHeader ? FirstRowCell.Text : string.Format("Column {0}", FirstRowCell.Start.Column));
                }
                var StartRow = HasHeader ? 2 : 1;
                for (var rowNum = StartRow; rowNum <= WS.Dimension.End.Row; rowNum++)
                {
                    var WsRow = WS.Cells[rowNum, 1, rowNum, WS.Dimension.End.Column];
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

        /// <summary>
        /// This function send the excel file info to Elasticsearch motor and return the results
        /// </summary>
        private static async Task<Tuple<List<Tuple<string, string>>, TDQueryModel>> SearchInfoFromFile(System.Data.DataTable ExcelDs, TDQueryModel Query)
        {
            Tuple<List<Tuple<string, string>>, TDQueryModel> oReturn;
            TDQueryModel oModelSearch = new TDQueryModel();
            Uri node = new Uri(ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex(ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_ThirdKnowledgeIndex].Value);
            List<Tuple<string, string>> oCoincidences = new List<Tuple<string, string>>();

            ElasticClient ThirdKnowledgeClient = new ElasticClient(settings);
            int page = 0;

            for (int i = 0; i < ExcelDs.Rows.Count; i++)
            {
                string SearchCritery = "";
                string SearchParam = "";

                if (ExcelDs.Rows[i].ItemArray.Count() > 0)
                {
                    SearchCritery = ExcelDs.Rows[i][ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.MP_CP_ColSearchCritery].Value].ToString();
                    SearchParam = ExcelDs.Rows[i][ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.MP_CP_ColSearchParam].Value].ToString();
                }
                //log file
                LogFile("SearchCritery::" + " " + SearchCritery + " - " + "SearchParam::" + " " + SearchParam);
                if (SearchCritery == ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.Param_Critery].Value.Split(';')[0])
                    oModelSearch = await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest(Query.PeriodPublicId, 1, SearchParam, Query);
                else if (SearchCritery == ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.Param_Critery].Value.Split(';')[1])
                    oModelSearch = await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest(Query.PeriodPublicId, 2, SearchParam, Query);
                else if (SearchCritery == ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.Param_Critery].Value.Split(';')[2])
                    oModelSearch = await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest(Query.PeriodPublicId, 3, SearchParam, Query);
                else if (SearchCritery == ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.Param_Critery].Value.Split(';')[3])
                    oModelSearch = await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest(Query.PeriodPublicId, 4, SearchParam, Query);

                if (oModelSearch != null && oModelSearch.QueryPublicId != null)
                {
                    oModelSearch.RelatedQueryInfoModel.All(x =>
                    {
                        oCoincidences.Add(new Tuple<string, string>(x.IdentificationResult, x.NameResult));
                        return true;
                    });
                }
            }
            oReturn = new Tuple<List<Tuple<string, string>>, TDQueryModel>(oCoincidences, Query);
            return oReturn;
        }
        #endregion

        public static List<Tuple<string, List<string>, List<string>>> JudicialProcessSearch(int IdType, string Name, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLineJudicialProcess>().As<OnlineSearch.Interfaces.IOnLineSearch>();
            var container = builder.Build();
            return container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, Name, IndentificationNumber).Result;
        }
        public static List<Tuple<string, List<string>, List<string>>> OnLnieSearch(int IdType, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLineProcImplement>().As<OnlineSearch.Interfaces.IOnLineSearch>();

            var container = builder.Build();
            return container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, "", IndentificationNumber).Result;
        }
    }
}
