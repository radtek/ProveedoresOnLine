using Autofac;
using Nest;
using ProveedoresOnLine.ThirdKnowledge.DAL.Controller;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using static ProveedoresOnLine.ThirdKnowledge.Models.Enumerations;

namespace ProveedoresOnLine.ThirdKnowledge.Controller
{
    public class ThirdKnowledgeModule
    {
        public static async Task<TDQueryModel> SimpleRequest(string PeriodPublicId, int IdType, string IdentificationNumber, string Name, TDQueryModel oQueryToCreate)
        {
            try
            {
                List<Tuple<string, List<string>, List<string>>> procResult = new List<Tuple<string, List<string>, List<string>>>();
                List<Tuple<string, List<string>, List<string>>> ppResult = new List<Tuple<string, List<string>, List<string>>>();
                List<Tuple<string, List<string>, List<string>>> judProcResult = new List<Tuple<string, List<string>, List<string>>>();

                if (!string.IsNullOrEmpty(IdentificationNumber))
                    judProcResult = await JudicialProcessSearch(3, Name, IdentificationNumber);

                //Proc Request
                if (!string.IsNullOrEmpty(IdentificationNumber) && IdType != 0)
                    procResult = await OnLnieSearch(IdType, IdentificationNumber);

                //PanamaPapers Search
                if (!string.IsNullOrEmpty(Name))
                    ppResult = await PPSearch(IdType == 2 ? 0 : 1, Name, IdentificationNumber);

                if (!string.IsNullOrEmpty(Name))
                {
                    if (Name.ToLower().Contains("sas"))
                        Name = Name.ToLower().Replace("sas", "");
                    else if (Name.ToLower().Contains("s.a.s"))
                        Name = Name.ToLower().Replace("s.a.s", "");
                    else if (Name.ToLower().Contains("s.a"))
                        Name = Name.ToLower().Replace("s.a", "");
                    else if (Name.ToLower().Contains("ltda"))
                        Name = Name.ToLower().Replace("ltda", "");
                    else if (Name.ToLower().Contains("l.t.d.a"))
                        Name = Name.ToLower().Replace("l.t.d.a", "");
                }
                List<PlanModel> oPlanModel = new List<PlanModel>();
                PeriodModel oCurrentPeriod = new PeriodModel();

                //Search Elastic
                Uri node = new Uri(InternalSettings.Instance[Constants.C_Settings_ElasticSearchUrl].Value);

                var settings = new ConnectionSettings(node);
                settings.DefaultIndex(InternalSettings.Instance[Constants.C_Settings_ThirdKnowledgeIndex].Value);
                settings.DisableDirectStreaming(true);
                ElasticClient client = new ElasticClient(settings);

                var oSearchResult = client.Search<ProveedoresOnLine.IndexSearch.Models.ThirdknowledgeIndexSearchModel>(s => s
                .TrackScores(true)
                .From(0)
                .Size(10)
                 .Query(q => q.QueryString(qr => qr.Fields(fds => fds.Field(f => f.CompleteName)).Query(Name)) ||
                            q.QueryString(qr => qr.Fields(fds => fds.Field(f => f.TypeId)).Query(IdentificationNumber))
                 ).MinScore(2));

                oQueryToCreate.RelatedQueryInfoModel = new List<TDQueryInfoModel>();
                if (procResult != null && procResult.Count > 0)
                {
                    string detailMoreInfo = "";
                    procResult.All(x =>
                    {
                        x.Item3.All(p =>
                        {
                            detailMoreInfo += p + ", ";
                            return true;
                        });
                        detailMoreInfo += " - ";

                        return true;
                    });

                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel()
                    {
                        AKA = string.Empty,
                        DocumentType = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : "",
                        Offense = "Presenta Antecedentes Procuraduría Nacional",
                        NameResult = procResult.FirstOrDefault().Item1,
                        MoreInfo = detailMoreInfo,
                        Priority = "1",
                        Status = "Vigente",
                        Enable = true,
                        QueryPublicId = oQueryToCreate.QueryPublicId,
                        QueryIdentification = IdentificationNumber,
                        IdentificationResult = IdentificationNumber,
                        QueryName = Name,
                        IdList = "Procuraduría General de la Nación",
                        IdentificationNumber = IdentificationNumber,
                        GroupName = "Procuraduría General de la Nación - Criticidad Media",
                        Link = InternalSettings.Instance[Constants.Proc_Url].Value,
                        ListName = "Procuraduría General de la Nación",
                        ChargeOffense = "Presenta antecedentes en la Prcuraduría General de la Nación.",
                        Zone = "Colombia",
                        ElasticId = (int)enumElasticGroupId.ProcElasticId,
                    };

                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                }

                if (ppResult != null && ppResult.Count > 0)
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel()
                    {
                        AKA = string.Empty,
                        DocumentType = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : "",
                        Offense = "Presenta Reporte en Panama Papers",
                        NameResult = Name,
                        MoreInfo = "Panama Papers no hace refierencia necesariamente a un delito o una investigación por LA/FT.",
                        Priority = "2",
                        Status = "Vigente",
                        Enable = true,
                        QueryPublicId = oQueryToCreate.QueryPublicId,
                        QueryIdentification = "N/A",
                        QueryName = Name,
                        IdList = "Panama Papers",
                        IdentificationNumber = IdentificationNumber,
                        GroupName = "Panama Papers - Criticidad Baja",
                        Link = ppResult.FirstOrDefault().Item1,
                        ListName = "Panama Papers",
                        Zone = "N/A",
                        ChargeOffense = "Presenta antecedentes en la Prcuraduría General de la Nación.",
                        ElasticId = (int)enumElasticGroupId.PanamaPElasticId,
                    };
                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                }

                if (judProcResult != null && judProcResult.Count > 0)
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel()
                    {
                        AKA = string.Empty,
                        DocumentType = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : "",
                        Offense = "El tercero " + judProcResult.FirstOrDefault().Item2[1] + "Con Identificación No. " + judProcResult.FirstOrDefault().Item2[0] + "Presenta Antecedentes Judiciales",
                        NameResult = judProcResult.FirstOrDefault().Item2[1],
                        MoreInfo = "El tercero " + judProcResult.FirstOrDefault().Item2[1] + "Con Identificación No. " + judProcResult.FirstOrDefault().Item2[0] + "Presenta Antecedentes Judiciales vigentes de acuerdo a la Fuente oficial de la RAMA JUDICIAL DEL PODER PUBLICO, CONSEJO SUPERIOR DE LA JUDICATURA y/o JUZGADOS DE EJECUCION DE PENAS Y MEDIDAS DE SEGURIDAD",
                        Priority = "2",
                        Status = "Vigente",
                        Enable = true,
                        QueryPublicId = oQueryToCreate.QueryPublicId,
                        QueryIdentification = IdentificationNumber,
                        IdentificationResult = IdentificationNumber,
                        FullName = judProcResult.FirstOrDefault().Item2[1],
                        QueryName = Name,
                        IdList = "RAMA JUDICIAL DEL PODER PUBLICO",
                        IdentificationNumber = IdentificationNumber,
                        GroupName = "RAMA JUDICIAL DEL PODER PUBLICO - Criticidad Media",
                        Link = judProcResult.FirstOrDefault().Item1,
                        ListName = "RAMA JUDICIAL DEL PODER PUBLICO, CONSEJO SUPERIOR DE LA JUDICATURA y/o JUZGADOS DE EJECUCION DE PENAS Y MEDIDAS DE SEGURIDAD",
                        Zone = "N/A",
                        ChargeOffense = "El tercero " + judProcResult.FirstOrDefault().Item2[1] + "Con Identificación No. " + judProcResult.FirstOrDefault().Item2[0] + "Presenta Antecedentes Judiciales",
                        ElasticId = (int)enumElasticGroupId.JudicialProces,
                    };
                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                }

                if (oSearchResult.Documents.Count() > 0 || procResult.Count > 0 || ppResult != null && ppResult.Count > 0
                    || judProcResult != null && judProcResult.Count > 0)
                {
                    oSearchResult.Documents.All(x =>
                        {
                            TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();

                            oInfoCreate.AKA = x.AKA;
                            oInfoCreate.DocumentType = x.TypeId;
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

                            oInfoCreate.Enable = true;
                            oInfoCreate.QueryPublicId = oQueryToCreate.QueryPublicId;
                            oInfoCreate.IdentificationNumber = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty;
                            oInfoCreate.QueryName = !string.IsNullOrEmpty(Name) ? Name : string.Empty;
                            oInfoCreate.UpdateDate = !string.IsNullOrEmpty(x.LastModify) ? x.LastModify : string.Empty;
                            oInfoCreate.IdentificationResult = !string.IsNullOrEmpty(x.TypeId) ? x.TypeId : string.Empty;
                            oInfoCreate.Status = !string.IsNullOrEmpty(x.Status) ? x.Status : string.Empty;
                            #region GroupName
                            oInfoCreate.GroupName =
                                                 !string.IsNullOrEmpty(x.ListType) &&
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
                            #endregion
                            oInfoCreate.ElasticId = Convert.ToInt32(x.Id);
                            oInfoCreate.GroupId = x.Code;
                            oInfoCreate.IdList = x.TableCodeID;
                            oInfoCreate.Link = !string.IsNullOrEmpty(x.Source) ? x.Source : string.Empty;
                            oInfoCreate.NameResult = x.CompleteName;
                            oInfoCreate.ListName = !string.IsNullOrEmpty(x.ListType) ? x.ListType : string.Empty;
                            oInfoCreate.MoreInfo = x.RelatedWiht + " " + x.ORoldescription1 + " " + x.ORoldescription2;
                            oInfoCreate.Zone = x.NationalitySourceCountry;
                            oInfoCreate.QueryIdentification = IdentificationNumber;

                            oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                            return true;
                        });
                    oQueryToCreate.IsSuccess = true;
                }
                else
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();
                    oInfoCreate.QueryPublicId = oQueryToCreate.QueryPublicId;
                    oInfoCreate.QueryName = Name;
                    oInfoCreate.QueryIdentification = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty;
                    oInfoCreate.GroupName = "SIN COINCIDENCIAS";

                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);

                    oQueryToCreate.IsSuccess = false;
                }
                oQueryToCreate.QueryPublicId = await QueryCreate(oQueryToCreate);

                Task.Run(async () => await QueryUpsert(oQueryToCreate));
                return oQueryToCreate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Config

        public static PlanModel PlanUpsert(PlanModel oPlanModelToUpsert)
        {
            //LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();
            if (oPlanModelToUpsert != null)
            {
                oPlanModelToUpsert.PlanPublicId = ThirdKnowledgeDataController.Instance.PlanUpsert
                                                  (oPlanModelToUpsert.PlanPublicId,
                                                  oPlanModelToUpsert.CompanyPublicId,
                                                  oPlanModelToUpsert.QueriesByPeriod,
                                                  oPlanModelToUpsert.IsLimited,
                                                  oPlanModelToUpsert.DaysByPeriod,
                                                  oPlanModelToUpsert.Status,
                                                  oPlanModelToUpsert.InitDate,
                                                  oPlanModelToUpsert.EndDate,
                                                  oPlanModelToUpsert.Enable);
                oPlanModelToUpsert.RelatedPeriodModel = CalculatePeriods(oPlanModelToUpsert);
            }
            return oPlanModelToUpsert;
        }

        public static List<PlanModel> GetAllPlanByCustomer(string CustomerPublicId, bool Enable)
        {
            return ThirdKnowledgeDataController.Instance.GetAllPlanByCustomer(CustomerPublicId, Enable);
        }

        public static List<PeriodModel> GetPeriodByPlanPublicId(string PeriodPublicId, bool Enable)
        {
            return ThirdKnowledgeDataController.Instance.GetPeriodByPlanPublicId(PeriodPublicId, Enable);
        }

        public static List<TDQueryModel> GetQueriesByPeriodPublicId(string PeriodPublicId, bool Enable)
        {
            return ThirdKnowledgeDataController.Instance.GetQueriesByPeriodPublicId(PeriodPublicId, Enable);
        }

        public async static Task<string> QueryCreate(TDQueryModel oQueryToCreate)
        {
            return await ThirdKnowledgeDataController.Instance.QueryUpsert(oQueryToCreate.QueryPublicId, oQueryToCreate.PeriodPublicId,
                    oQueryToCreate.SearchType.ItemId, oQueryToCreate.User, oQueryToCreate.FileName, oQueryToCreate.IsSuccess, oQueryToCreate.QueryStatus.ItemId, true);
        }

        #endregion Config

        #region MarketPlace

        public static List<PeriodModel> CalculatePeriods(PlanModel oPlanToReCalculate)
        {
            int DiferenceInDays;
            int TotalPeriods = 0;

            oPlanToReCalculate.RelatedPeriodModel = ThirdKnowledgeDataController.Instance.GetPeriodByPlanPublicId(oPlanToReCalculate.PlanPublicId, true);

            if (oPlanToReCalculate.PlanPublicId != null &&
                oPlanToReCalculate.RelatedPeriodModel != null &&
                oPlanToReCalculate.RelatedPeriodModel.Count > 0)
            {
                oPlanToReCalculate.RelatedPeriodModel.All(x =>
                    {
                        ProveedoresOnLine.ThirdKnowledge.DAL.Controller.ThirdKnowledgeDataController.Instance.PeriodUpsert(
                                                            x.PeriodPublicId,
                                                            oPlanToReCalculate.PlanPublicId,
                                                            oPlanToReCalculate.QueriesByPeriod,
                                                            oPlanToReCalculate.IsLimited,
                                                            x.TotalQueries,
                                                            x.InitDate,
                                                            x.EndDate,
                                                            oPlanToReCalculate.Enable);
                        return true;
                    });
            }
            else
            {
                if (oPlanToReCalculate != null)
                {
                    //Get Days from dates interval
                    DiferenceInDays = (oPlanToReCalculate.EndDate - oPlanToReCalculate.InitDate).Days;

                    TotalPeriods = DiferenceInDays / oPlanToReCalculate.DaysByPeriod;
                    oPlanToReCalculate.RelatedPeriodModel = new List<PeriodModel>();
                }

                DateTime EndPastPeriod = new DateTime();
                for (int i = 0; i < TotalPeriods; i++)
                {
                    if (i == 0)
                    {
                        oPlanToReCalculate.RelatedPeriodModel.Add(new PeriodModel()
                        {
                            AssignedQueries = oPlanToReCalculate.QueriesByPeriod,
                            InitDate = oPlanToReCalculate.InitDate,
                            EndDate = oPlanToReCalculate.InitDate.AddDays(oPlanToReCalculate.DaysByPeriod),
                            CreateDate = DateTime.Now,
                            LastModify = DateTime.Now,
                            PlanPublicId = oPlanToReCalculate.PlanPublicId,
                            TotalQueries = 0,
                        });
                        EndPastPeriod = oPlanToReCalculate.InitDate.AddDays(oPlanToReCalculate.DaysByPeriod);
                    }
                    else
                    {
                        oPlanToReCalculate.RelatedPeriodModel.Add(new PeriodModel()
                        {
                            AssignedQueries = oPlanToReCalculate.QueriesByPeriod,
                            InitDate = EndPastPeriod,
                            EndDate = i == TotalPeriods ? oPlanToReCalculate.EndDate : EndPastPeriod.AddDays(oPlanToReCalculate.DaysByPeriod),
                            CreateDate = DateTime.Now,
                            LastModify = DateTime.Now,
                            PlanPublicId = oPlanToReCalculate.PlanPublicId,
                            TotalQueries = 0,
                        });
                        EndPastPeriod = EndPastPeriod.AddDays(oPlanToReCalculate.DaysByPeriod);
                    }
                }
                oPlanToReCalculate.RelatedPeriodModel.All(x =>
                {
                    ProveedoresOnLine.ThirdKnowledge.DAL.Controller.ThirdKnowledgeDataController.Instance.PeriodUpsert(
                                                            x.PeriodPublicId,
                                                            x.PlanPublicId,
                                                            x.AssignedQueries,
                                                            oPlanToReCalculate.IsLimited,
                                                            x.TotalQueries,
                                                            x.InitDate,
                                                            x.EndDate,
                                                            oPlanToReCalculate.Enable);
                    return true;
                });
            }
            return oPlanToReCalculate.RelatedPeriodModel;
        }

        public static List<TDCatalogModel> CatalogGetThirdKnowledgeOptions()
        {
            return ProveedoresOnLine.ThirdKnowledge.DAL.Controller.ThirdKnowledgeDataController.Instance.CatalogGetThirdKnowledgeOptions();
        }

        public static List<Models.PlanModel> GetCurrenPeriod(string CustomerPublicId, bool Enable)
        {
            return ProveedoresOnLine.ThirdKnowledge.DAL.Controller.ThirdKnowledgeDataController.Instance.GetCurrenPeriod(CustomerPublicId, Enable);
        }

        public static string PeriodoUpsert(PeriodModel oPeriodModel)
        {
            return ProveedoresOnLine.ThirdKnowledge.DAL.Controller.ThirdKnowledgeDataController.Instance.PeriodUpsert(oPeriodModel.PeriodPublicId,
                       oPeriodModel.PlanPublicId, oPeriodModel.AssignedQueries, oPeriodModel.IsLimited, oPeriodModel.TotalQueries, oPeriodModel.InitDate, oPeriodModel.EndDate, oPeriodModel.Enable);
        }

        public static List<Models.TDQueryModel> ThirdKnowledgeSearch(string CustomerPublicId, string RelatedUser, string StartDate, string EndtDate, int PageNumber, int RowCount, string SearchType, string Status, out int TotalRows)
        {
            return ThirdKnowledgeDataController.Instance.ThirdKnowledgeSearch(CustomerPublicId, RelatedUser, StartDate, EndtDate, PageNumber, RowCount, SearchType, Status, out TotalRows);
        }

        public static List<Models.TDQueryModel> ThirdKnowledgeSearchByPublicId(string QueryPublicId, int PageNumber, int RowCount, out int TotalRows)
        {
            return ThirdKnowledgeDataController.Instance.ThirdKnowledgeSearchByPublicId(QueryPublicId, PageNumber, RowCount, out TotalRows);
        }

        public static List<string> GetUsersBycompanyPublicId(string CompanyPublicId)
        {
            return ThirdKnowledgeDataController.Instance.GetUsersBycompanyPublicId(CompanyPublicId);
        }

        #endregion MarketPlace

        #region Queries

        public static async Task<TDQueryModel> QueryUpsert(TDQueryModel QueryModelToUpsert)
        {
            if (QueryModelToUpsert != null &&
                !string.IsNullOrEmpty(QueryModelToUpsert.PeriodPublicId))
            {
                if (QueryModelToUpsert.RelatedQueryInfoModel != null)
                {
                    QueryModelToUpsert.RelatedQueryInfoModel.All(qInf =>
                   {
                       LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();

                       try
                       {
                           var response = Task.Run(() => ThirdKnowledgeDataController.Instance.QueryInfoInsert
                               (QueryModelToUpsert.QueryPublicId, qInf.NameResult, qInf.IdentificationResult, qInf.Priority,
                               qInf.Peps, qInf.Status, qInf.Offense, qInf.DocumentType, qInf.IdentificationNumber,
                               qInf.FullName, qInf.IdList, qInf.ListName, qInf.AKA, qInf.ChargeOffense, qInf.Message,
                               qInf.QueryIdentification, qInf.QueryName, qInf.ElasticId, qInf.GroupName, qInf.GroupId,
                               qInf.Link, qInf.MoreInfo, qInf.Zone, qInf.UrlFile, true));
                           qInf.QueryInfoPublicId = response.Result;

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
                           oLog.LogObject = qInf;

                           oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                           {
                               LogInfoType = "PeriodPublicId",
                               Value = QueryModelToUpsert.PeriodPublicId,
                           });

                           LogManager.ClientLog.AddLog(oLog);
                       }
                       return true;
                   });
                }
            }

            return QueryModelToUpsert;
        }

        public static TDQueryInfoModel GetQueryInfoByInfoPublicId(string QueryInfoPublicId)
        {
            return ThirdKnowledgeDataController.Instance.GetQueryInfoByInfoPublicId(QueryInfoPublicId);
        }

        public static TDQueryInfoModel GetQueryInfoByQueryPublicIdAndElasticId(string QueryPublicId, int ElasticId)
        {
            return ThirdKnowledgeDataController.Instance.GetQueryInfoByQueryPublicIdAndElasticId(QueryPublicId, ElasticId);
        }

        public static List<TDQueryInfoModel> GetQueryInfoByQueryPublicId(string QueryPublicId)
        {
            return ThirdKnowledgeDataController.Instance.GetQueryInfoByQueryPublicId(QueryPublicId);
        }

        #endregion Queries

        #region BatchProcess

        public static List<TDQueryModel> GetQueriesInProgress()
        {
            return ThirdKnowledgeDataController.Instance.GetQueriesInProgress();
        }

        #endregion

        #region Messenger

        public static void CreateUploadNotification(MessageModule.Client.Models.NotificationModel DataMessage)
        {
            try
            {
                #region Email

                //Create message object
                MessageModule.Client.Models.ClientMessageModel oMessageToSend = new MessageModule.Client.Models.ClientMessageModel()
                {
                    Agent = ThirdKnowledge.Models.InternalSettings.Instance[Constants.C_Settings_TK_UploadSuccessFileAgent].Value,
                    User = DataMessage.User,
                    ProgramTime = DateTime.Now,
                    MessageQueueInfo = new List<Tuple<string, string>>(),
                };

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>("To", DataMessage.User));

                //get customer info
                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                    ("CustomerLogo", DataMessage.CompanyLogo));

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                    ("CustomerName", DataMessage.CompanyName));

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                    ("CustomerIdentificationTypeName", DataMessage.IdentificationType));

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                    ("CustomerIdentificationNumber", DataMessage.IdentificationNumber));

                MessageModule.Client.Controller.ClientController.CreateMessage(oMessageToSend);

                #endregion

                #region Notification

                DataMessage.NotificationId = MessageModule.Client.Controller.ClientController.NotificationUpsert(DataMessage);

                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Async Methods

        public static async Task<List<Tuple<string, List<string>, List<string>>>> OnLnieSearch(int IdType, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLineProcImplement>().As<OnlineSearch.Interfaces.IOnLineSearch>();

            var container = builder.Build();
            return await container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, "", IndentificationNumber);
        }

        public static async Task<List<Tuple<string, List<string>, List<string>>>> PPSearch(int IdType, string Name, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLinePPImplement>().As<OnlineSearch.Interfaces.IOnLineSearch>();
            var container = builder.Build();
            return await container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, Name, IndentificationNumber);
        }

        public static async Task<List<Tuple<string, List<string>, List<string>>>> JudicialProcessSearch(int IdType, string Name, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLineJudicialProcess>().As<OnlineSearch.Interfaces.IOnLineSearch>();
            var container = builder.Build();
            return await container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, Name, IndentificationNumber);
        }
        #endregion
    }
}