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


namespace ProveedoresOnLine.ThirdKnowledgeBatch
{
    public class ThirdKnowledgeFTPProcess
    {
        public static void StartProcess()
        {
            try
            {
                //Get queries to process
                List<ProveedoresOnLine.ThirdKnowledge.Models.TDQueryModel> oQueryResult = new List<ProveedoresOnLine.ThirdKnowledge.Models.TDQueryModel>();

                oQueryResult = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetQueriesInProgress();

                if (oQueryResult != null)
                {
                    LogFile("Start Process:: Date" + DateTime.Now ); 
                    //Set access
                    string S3path = ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Setings_File_S3FilePath].Value;
                    string LocalPath = ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_File_TempDirectory].Value;

                    oQueryResult.All(oQuery =>
                    {
                        try
                        {
                            LogFile("Start Process:: QueryPublicId::" + oQuery.QueryPublicId);
                            //Download File from S3                            
                            //Local Path
                            string strFolder = ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_File_TempDirectory].Value;
                            if (!System.IO.Directory.Exists(strFolder))
                                System.IO.Directory.CreateDirectory(strFolder);

                            //Download file from s3
                            using (WebClient webClient = new WebClient())
                            {
                                //Get file from S3 using File Name           
                                webClient.DownloadFile(ThirdKnowledge.Models.InternalSettings.Instance[
                                                    ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Setings_File_S3FilePath].Value + oQuery.FileName, strFolder + oQuery.FileName);
                            }

                            //Read Excel File
                            System.Data.DataTable DT_Excel = ReadExcelFile(strFolder + oQuery.FileName);

                            oQuery.RelatedQueryBasicInfoModel = new List<TDQueryInfoModel>();
                            List<Tuple<string, string>> oCoincidences = new List<Tuple<string, string>>();
                            Tuple<List<Tuple<string, string>>, TDQueryModel> oResult = new Tuple<List<Tuple<string, string>>, TDQueryModel>(oCoincidences, oQuery);
                            if (DT_Excel != null)
                            {
                                //Call Function to search excel file 
                                oResult = SearchInfoFromFile(DT_Excel, oQuery);

                                //Update Status query
                                oQuery.QueryStatus = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledgeBatch.Models.Enumerations.enumThirdKnowledgeQueryStatus.Finalized,
                                };
                                ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.QueryUpsert(oResult.Item2);
                                CreateQueryInfo(oQuery, oResult.Item1);
                                CreateReadyResultNotification(oQuery);
                                LogFile("Success:: QueryPublicId '" + oQuery.QueryPublicId + "' :: Validation is success");
                            }
                            else
                            {
                                LogFile("Error:: QueryPublicId '" + oQuery.QueryPublicId + "' :: Excel Datatable is Empty");
                            }
                            //Remove all Files
                            //remove temporal file
                            if (System.IO.File.Exists(strFolder + oQuery.FileName))
                                System.IO.File.Delete(strFolder + oQuery.FileName);
                        }
                        catch (Exception err)
                        {
                            LogFile("Error:: QueryPublicId '" + oQuery.QueryPublicId + "' :: " + err.Message + "Inner Exception::" + err.InnerException);
                        }
                        return true;
                    });
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
                User = oQuery.User,
                ProgramTime = DateTime.Now,
                MessageQueueInfo = new List<Tuple<string, string>>(),
            };

            oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>("To", oQuery.User));

            oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                ("URLToRedirect", ThirdKnowledge.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledge.Models.Constants.C_Settings_TK_QueryUrl].Value.Replace("{QueryPublicId}", oQuery.QueryPublicId)));


            MessageModule.Client.Controller.ClientController.CreateMessage(oMessageToSend);
            #endregion

            #region Notification

            MessageModule.Client.Models.NotificationModel oNotification = new MessageModule.Client.Models.NotificationModel()
            {
                CompanyPublicId = oQuery.CompayPublicId,
                NotificationType = (int)ThirdKnowledge.Models.Enumerations.enumNotificationType.ThirdKnowledgeNotification,
                Url = ThirdKnowledge.Models.InternalSettings.Instance
                                [ThirdKnowledge.Models.Constants.N_UrlThirdKnowledgeQuery].Value.Replace("{QueryPublicId}", oQuery.QueryPublicId),
                User = oQuery.User,
                Label = ThirdKnowledge.Models.InternalSettings.Instance
                                [ThirdKnowledge.Models.Constants.N_ThirdKnowledgeEndMassiveMessage].Value,
                Enable = true,
            };

            MessageModule.Client.Controller.ClientController.NotificationUpsert(oNotification);

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
                                NUMEIDEN = x.Item1,
                                NOMBRES = x.Item2,
                            });
                            return true;
                        });
                    }

                    if (oExclude != null)
                    {
                        oExclude.All(x =>
                            {
                                oExcelToProcessInfo = oExcelToProcessInfo.Where(y => y.NOMBRES.ToLower() != x.NOMBRES.ToLower() || y.NUMEIDEN != x.NUMEIDEN).Select(y => y).ToList();
                                return true;
                            });
                    }

                    if (oExcelToProcessInfo != null)
                    {
                        oExcelToProcessInfo.All(x =>
                        {
                             //Create QueryInfo
                                oQuery.RelatedQueryBasicInfoModel = new List<TDQueryInfoModel>();

                                TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();
                                oInfoCreate.QueryPublicId = oQuery.QueryPublicId;
                                oInfoCreate.DetailInfo = new List<TDQueryDetailInfoModel>();

                                #region Create Detail

                                oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                                {
                                    ItemInfoType = new TDCatalogModel()
                                    {
                                        ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.RequestName,
                                    },
                                    Value = !string.IsNullOrEmpty(x.NOMBRES) ? x.NOMBRES : string.Empty,
                                    Enable = true,
                                });
                                oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                                {
                                    ItemInfoType = new TDCatalogModel()
                                    {
                                        ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdNumberRequest,
                                    },
                                    Value = !string.IsNullOrEmpty(x.NUMEIDEN) ? x.NUMEIDEN : string.Empty,
                                    Enable = true,
                                });
                                oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                                {
                                    ItemInfoType = new TDCatalogModel()
                                    {
                                        ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.GroupName,
                                    },
                                    Value = "SIN COINCIDENCIAS",
                                    Enable = true,
                                });
                                #endregion

                                oQuery.RelatedQueryBasicInfoModel.Add(oInfoCreate);
                                Monitor.Enter(oQuery);
                                lock (oQuery)
                                {
                                    ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.QueryUpsert(oQuery);
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
        private static Tuple<List<Tuple<string, string>>, TDQueryModel> SearchInfoFromFile(System.Data.DataTable ExcelDs, TDQueryModel Query)
        {
            Tuple<List<Tuple<string, string>>, TDQueryModel> oReturn;

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
                            oInfoCreate.Alias = x.AKA;
                            oInfoCreate.IdentificationResult = x.TypeId;
                            oInfoCreate.Offense = x.RelatedWiht;
                            oInfoCreate.NameResult = x.CompleteName;

                            if (x.ListType == "FIGURAS PUBLICAS" || x.ListType == "PEPS INTERNACIONALES")
                                oInfoCreate.Peps = x.ListType;
                            else
                                oInfoCreate.Peps = "N/A";

                            #region Group by Priority
                            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(IdentificationNumber) && x.TypeId == IdentificationNumber && x.CompleteName == Name)
                                oInfoCreate.Priority = "1";
                            else if (!string.IsNullOrEmpty(IdentificationNumber) && x.TypeId == IdentificationNumber && x.CompleteName != Name)
                                oInfoCreate.Priority = "2";
                            else if (!string.IsNullOrEmpty(Name) && x.TypeId != IdentificationNumber && x.CompleteName == Name)
                                oInfoCreate.Priority = "3";
                            else
                                oInfoCreate.Priority = "3";
                            #endregion

                            oInfoCreate.Status = x.Status;
                            oInfoCreate.Enable = true;
                            oInfoCreate.QueryPublicId = Query.QueryPublicId;
                            oInfoCreate.DetailInfo = new List<TDQueryDetailInfoModel>();

                            #region Create Detail
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdNumberRequest,
                                },
                                Value = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.RequestName,
                                },
                                Value = !string.IsNullOrEmpty(Name) ? Name : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Alias,
                                },
                                Value = !string.IsNullOrEmpty(x.AKA) ? x.AKA : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdList,
                                },
                                Value = !string.IsNullOrEmpty(x.ListType) ? x.ListType : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Priotity,
                                },
                                Value = !string.IsNullOrEmpty(oInfoCreate.Priority) ? oInfoCreate.Priority : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.RegisterDate,
                                },
                                Value = !string.IsNullOrEmpty(x.LastModify) ? x.LastModify : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.LastModifyDate,
                                },
                                Value = !string.IsNullOrEmpty(x.LastModify) ? x.LastModify : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Offense,
                                },
                                Value = !string.IsNullOrEmpty(x.RelatedWiht) ? x.RelatedWiht : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdentificationNumberResult,
                                },
                                Value = !string.IsNullOrEmpty(x.TypeId) ? x.TypeId : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = Query.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Status,
                                },
                                Value = !string.IsNullOrEmpty(x.Status) ? x.Status : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.QueryId,
                                },
                                Value = !string.IsNullOrEmpty(x.Registry.ToString()) ? x.Registry.ToString() : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.GroupName,
                                },
                                Value = !string.IsNullOrEmpty(x.ListType) &&
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
                                         || x.ListType == "UNVERIFIED_LIST_EEUU" ?
                                         x.ListType + " - Criticidad Media" :
                                         x.ListType == "ESTRUCTURA DE GOBIERNO"
                                         || x.ListType == "FIGURAS PUBLICAS"
                                         || x.ListType == "PANAMA PAPERS"
                                         || x.ListType == "PARTIDOS Y MOVIMIENTOS POLITICOS"
                                         || x.ListType == "PEPS INTERNACIONALES" ?
                                         x.ListType + " - Criticidad Baja" : "NA",
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.GroupId,
                                },
                                Value = !string.IsNullOrEmpty(x.Code) ? x.Code : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdList,
                                },
                                Value = !string.IsNullOrEmpty(x.TableCodeID) ? x.TableCodeID : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Link,
                                },
                                Value = !string.IsNullOrEmpty(x.Source) ? x.Source : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.NameResult,
                                },
                                Value = !string.IsNullOrEmpty(x.CompleteName) ? x.CompleteName : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.ListName,
                                },
                                Value = !string.IsNullOrEmpty(x.ListType) ? x.ListType : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.MoreInfo,
                                },
                                Value = !string.IsNullOrEmpty(x.ORoldescription1) || !string.IsNullOrEmpty(x.ORoldescription2) ? x.ORoldescription1 : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Zone,
                                },
                                Value = !string.IsNullOrEmpty(x.NationalitySourceCountry) ? x.NationalitySourceCountry : string.Empty,
                                Enable = true,
                            });
                            #endregion

                            //Create Info Conincidences                                        
                            oCoincidences.Add(new Tuple<string, string>(IdentificationNumber, Name));

                            Query.RelatedQueryBasicInfoModel.Add(oInfoCreate);
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
