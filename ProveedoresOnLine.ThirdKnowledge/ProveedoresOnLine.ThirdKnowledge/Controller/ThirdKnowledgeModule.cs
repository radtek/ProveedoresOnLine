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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PeriodPublicId">Period To descount queries</param>
        /// <param name="IdType">1 CC, 2 NIT, 3 CC Extranjera, 4 Denominación o Razon Social</param>
        /// <param name="IdentificationNumber"></param>
        /// <param name="Name"></param>
        /// <param name="oQueryToCreate"></param>
        /// <returns></returns>
        public static async Task<TDQueryModel> SimpleRequest(string PeriodPublicId, int IdType, string SearchParam, TDQueryModel oQueryToCreate)
        {
            try
            {   
                List<Tuple<string, List<string>, List<string>>> procResult = new List<Tuple<string, List<string>, List<string>>>();
                List<Tuple<string, List<string>, List<string>>> ppResult = new List<Tuple<string, List<string>, List<string>>>();
                List<Tuple<string, List<string>, List<string>>> judProcResult = new List<Tuple<string, List<string>, List<string>>>();
                List<Tuple<string, List<string>, List<string>>> RegDianResult = new List<Tuple<string, List<string>, List<string>>>();
                List<Tuple<string, List<string>, List<string>>> RegEntityResult = new List<Tuple<string, List<string>, List<string>>>();
                List<Tuple<string, List<string>, List<string>>> RUESResult = new List<Tuple<string, List<string>, List<string>>>();

                var oSearchResult = ElasticSearch(IdType, SearchParam);

                oQueryToCreate.RelatedQueryInfoModel = new List<TDQueryInfoModel>();

                //Identify personType and Call the respective function
                //Persons
                if (IdType != 4)
                {
                    if (!string.IsNullOrEmpty(SearchParam))
                    {   
                        //Judicial proces Search
                        judProcResult = await JudicialProcessSearch(3, null, SearchParam);

                        //Proc Request                    
                        procResult = await OnLnieSearch(IdType, SearchParam);

                        //Register Search                    
                        RegDianResult = await RegisterSearch(IdType, null, SearchParam);

                        if (IdType == 1)
                        {
                            //Register Search Vote information                    
                            RegEntityResult = await RegisterEntitySearch(IdType, null, SearchParam);
                        }                       

                        if (RegDianResult.Count > 0 && !string.IsNullOrEmpty(RegDianResult.FirstOrDefault().Item1))
                            ppResult = await PPSearch(1, RegDianResult.FirstOrDefault().Item1, SearchParam);

                        //RUES Implement
                        if (IdType == 2)                        
                            RUESResult = await RUESSearch(2,"", SearchParam);
                    }
                }

                #region Procuraduria
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
                        QueryIdentification = SearchParam,
                        IdentificationResult = SearchParam,
                        QueryName = !string.IsNullOrEmpty(RegDianResult.FirstOrDefault().Item1) ? RegDianResult.FirstOrDefault().Item1 : string.Empty,
                        IdList = "Procuraduría General de la Nación",
                        IdentificationNumber = SearchParam,
                        GroupName = "Procuraduría General de la Nación - Criticidad Media",
                        Link = InternalSettings.Instance[Constants.Proc_Url].Value,
                        ListName = "Procuraduría General de la Nación",
                        ChargeOffense = "Presenta antecedentes en la Prcuraduría General de la Nación.",
                        Zone = "Colombia",
                        ElasticId = (int)enumElasticGroupId.ProcElasticId,
                    };

                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                } 
                #endregion

                #region Panama Papers
                if (ppResult != null && ppResult.Count > 0)
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel()
                    {
                        AKA = string.Empty,
                        DocumentType = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : "",
                        Offense = "Presenta Reporte en Panama Papers",
                        NameResult = !string.IsNullOrEmpty(RegDianResult.FirstOrDefault().Item1) ? RegDianResult.FirstOrDefault().Item1 : string.Empty,
                        MoreInfo = "Panama Papers no hace refierencia necesariamente a un delito o una investigación por LA/FT.",
                        Priority = "2",
                        Status = "Vigente",
                        Enable = true,
                        QueryPublicId = oQueryToCreate.QueryPublicId,
                        QueryIdentification = "N/A",
                        QueryName = "",
                        IdList = "Panama Papers",
                        IdentificationNumber = SearchParam,
                        GroupName = "Panama Papers - Criticidad Baja",
                        Link = ppResult.FirstOrDefault().Item1,
                        ListName = "Panama Papers",
                        Zone = "N/A",
                        ChargeOffense = "Presenta antecedentes en la Prcuraduría General de la Nación.",
                        ElasticId = (int)enumElasticGroupId.PanamaPElasticId,
                    };
                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                } 
                #endregion

                #region Registraduría/Dian get Name
                if (RegDianResult.Count > 0 &&!string.IsNullOrEmpty(RegDianResult.FirstOrDefault().Item1))
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel()
                    {
                        NameResult = !string.IsNullOrEmpty(RegDianResult.FirstOrDefault().Item1) ? RegDianResult.FirstOrDefault().Item1 : "",
                        QueryPublicId = oQueryToCreate.QueryPublicId,
                        IdList = "Registraduria/Dian",
                        DocumentType = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : IdType == 4 ? "Denominación o Razon Social" : "",
                        GroupName = "Registraduria/Dian",
                        ListName = "Registraduria/Dian",
                        ElasticId = (int)enumElasticGroupId.RegisterDian,
                    };
                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                } 
                #endregion

                #region Registraduría Puesto de Votación
                if (RegEntityResult.Count > 0 && !string.IsNullOrEmpty(RegEntityResult.FirstOrDefault().Item1))
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel()
                    {
                        QueryPublicId = oQueryToCreate.QueryPublicId,
                        DocumentType = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : IdType == 4 ? "Denominación o Razon Social" : "",
                        IdList = "Registraduria",
                        GroupName = "Registraduria - Puesto de Votación",
                        Link = RegEntityResult.FirstOrDefault().Item1,
                        ListName = "Registraduria - Puesto de Votación",
                        ElasticId = (int)enumElasticGroupId.RegistersList,
                    };
                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                }
                #endregion

                #region RUES

                if (RUESResult.Count > 0 && RUESResult.FirstOrDefault().Item2 != null && RUESResult.FirstOrDefault().Item2.Count > 0)
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel()
                    {
                        QueryPublicId = oQueryToCreate.QueryPublicId,
                        IdList = "RUES",
                        DocumentType = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : IdType == 4 ? "Denominación o Razon Social" : "",
                        GroupName = "RUES",
                        NameResult = !string.IsNullOrEmpty(RUESResult.FirstOrDefault().Item2[1]) ? RUESResult.FirstOrDefault().Item2[1] : "No aparece registro en RUES",
                        IdentificationResult = !string.IsNullOrEmpty(RUESResult.FirstOrDefault().Item2[0]) ? RUESResult.FirstOrDefault().Item2[0] : "No aparece registro en RUES",
                        Status = !string.IsNullOrEmpty(RUESResult.FirstOrDefault().Item2[3]) ? RUESResult.FirstOrDefault().Item2[3] : "No aparece registro en RUES",
                        Link = !string.IsNullOrEmpty(RUESResult.FirstOrDefault().Item2[4]) ? RUESResult.FirstOrDefault().Item2[4] : "No aparece registro en RUES",
                        ListName = "RUES",
                        ElasticId = (int)enumElasticGroupId.RUES,
                    };
                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                } 
                #endregion

                #region Judicial Process
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
                        QueryIdentification = SearchParam,
                        IdentificationResult = SearchParam,
                        FullName = judProcResult.FirstOrDefault().Item2[1],
                        QueryName = "",
                        IdList = "RAMA JUDICIAL DEL PODER PUBLICO",
                        IdentificationNumber = SearchParam,
                        GroupName = "RAMA JUDICIAL DEL PODER PUBLICO - Criticidad Media",
                        Link = judProcResult.FirstOrDefault().Item1,
                        ListName = "RAMA JUDICIAL DEL PODER PUBLICO, CONSEJO SUPERIOR DE LA JUDICATURA y/o JUZGADOS DE EJECUCION DE PENAS Y MEDIDAS DE SEGURIDAD",
                        Zone = "N/A",
                        ChargeOffense = "El tercero " + judProcResult.FirstOrDefault().Item2[1] + "Con Identificación No. " + judProcResult.FirstOrDefault().Item2[0] + "Presenta Antecedentes Judiciales",
                        ElasticId = (int)enumElasticGroupId.JudicialProces,
                    };
                    oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                }
                #endregion
                try
                {
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
                            if (x.ListType == "FIGURAS PUBLICAS" || x.ListType == "PEPS INTERNACIONALES"
                                                                 || x.ListType == "CONSEJO NACIONAL ELECTORAL"
                                                                 || x.ListType == "FUERZAS MILITARES"
                                                                 || x.ListType == "GOBIERNO DEPARTAMENTAL"
                                                                 || x.ListType == "GOBIERNO MUNICIPAL"
                                                                 || x.ListType == "GOBIERNO NACIONAL"
                                                                 || x.ListType == "ESTRUCTURA DE GOBIERNO"
                                                                 || x.ListType == "PARTIDOS Y MOVIMIENTOS POLITICOS")
                                oInfoCreate.Peps = x.ListType;
                            else
                                oInfoCreate.Peps = "N/A";

                            oInfoCreate.Enable = true;
                            oInfoCreate.QueryPublicId = oQueryToCreate.QueryPublicId;
                            oInfoCreate.IdentificationNumber = !string.IsNullOrEmpty(SearchParam) ? SearchParam : string.Empty;
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
                                                             || x.ListType == "SECTORAL SANCTIONS IDENTIFICATIONS_LIST_EEUU"
                                                             || x.ListType == "SPECIALLY DESIGNATED NATIONALS LIST_EEUU"
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
                            oInfoCreate.QueryIdentification = oInfoCreate.QueryName = !string.IsNullOrEmpty(SearchParam) ? SearchParam : string.Empty; ;

                            oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);
                            return true;
                        });
                        oQueryToCreate.IsSuccess = true;
                    }
                    else
                    {
                        TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();
                        oInfoCreate.QueryPublicId = oQueryToCreate.QueryPublicId;
                        if (IdType != 4)
                            oInfoCreate.QueryName = SearchParam;
                        else
                            oInfoCreate.QueryIdentification = !string.IsNullOrEmpty(SearchParam) ? SearchParam : string.Empty;
                        oInfoCreate.GroupName = "SIN COINCIDENCIAS";
                        oQueryToCreate.RelatedQueryInfoModel.Add(oInfoCreate);

                        oQueryToCreate.IsSuccess = false;
                    }
                    oQueryToCreate.QueryPublicId = await QueryCreate(oQueryToCreate);

                    Task.Run(async () => await QueryUpsert(oQueryToCreate));
                }
                catch (Exception)
                {

                    throw;
                }
                
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

        public static List<Models.TDQueryModel> ThirdKnowledgeSearch(string CustomerPublicId, string RelatedUser, string Domain, string StartDate, string EndtDate, int PageNumber, int RowCount, string SearchType, string Status, out int TotalRows)
        {
            return ThirdKnowledgeDataController.Instance.ThirdKnowledgeSearch(CustomerPublicId, RelatedUser, Domain, StartDate, EndtDate, PageNumber, RowCount, SearchType, Status, out TotalRows);
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

        public static void CreateUploadNotification(MessageModule.Client.Models.ClientMessageModel DataMessage)
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
                    MessageQueueInfo = DataMessage.MessageQueueInfo,
                };                

                MessageModule.Client.Controller.ClientController.CreateMessage(oMessageToSend);

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

        public static async Task<List<Tuple<string, List<string>, List<string>>>> RegisterSearch(int IdType, string Name, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLineRegImplement>().As<OnlineSearch.Interfaces.IOnLineSearch>();
            var container = builder.Build();
            return await container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, Name, IndentificationNumber);
        }

        public static async Task<List<Tuple<string, List<string>, List<string>>>> RegisterEntitySearch(int IdType, string Name, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLineRegistreImplement>().As<OnlineSearch.Interfaces.IOnLineSearch>();
            var container = builder.Build();
            return await container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, Name, IndentificationNumber);
        }

        public static async Task<List<Tuple<string, List<string>, List<string>>>> ParadisePapersSearch(int IdType, string Name, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLineParadiseImplement>().As<OnlineSearch.Interfaces.IOnLineSearch>();
            var container = builder.Build();
            return await container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, Name, IndentificationNumber);
        }
        public static async Task<List<Tuple<string, List<string>, List<string>>>> RUESSearch(int IdType, string Name, string IndentificationNumber)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<OnlineSearch.Core.ProveedoresOnLineRUESImplement>().As<OnlineSearch.Interfaces.IOnLineSearch>();
            var container = builder.Build();
            return await container.Resolve<OnlineSearch.Interfaces.IOnLineSearch>().Search(IdType, Name, IndentificationNumber);
        }

        #endregion

        #region Private Function

        public static ISearchResponse<ProveedoresOnLine.IndexSearch.Models.ThirdknowledgeIndexSearchModel> ElasticSearch(int IdType, string SearchaParam)
        {
            Uri node = new Uri(InternalSettings.Instance[Constants.C_Settings_ElasticSearchUrl].Value);

            var settings = new ConnectionSettings(node);
            settings.DefaultIndex(InternalSettings.Instance[Constants.C_Settings_ThirdKnowledgeIndex].Value);
            settings.DisableDirectStreaming(true);
            ElasticClient client = new ElasticClient(settings);

            ISearchResponse<ProveedoresOnLine.IndexSearch.Models.ThirdknowledgeIndexSearchModel> oSearchResult = null;
            if (IdType != 4)
            {
                oSearchResult = client.Search<ProveedoresOnLine.IndexSearch.Models.ThirdknowledgeIndexSearchModel>(s => s
                .TrackScores(true)
                .From(0)
                .Size(5)
                 .Query(q => q.QueryString(qr => qr.Fields(fds => fds.Field(f => f.TypeId)).Query(SearchaParam))
                 )
                );
            }
            else
            {
                oSearchResult = client.Search<ProveedoresOnLine.IndexSearch.Models.ThirdknowledgeIndexSearchModel>(s => s
                .TrackScores(true)
                .From(0)
                .Size(2)
                 .Query(q => q.QueryString(qr => qr.Fields(fds => fds.Field(f => f.CompleteName)).Query(SearchaParam))
                 )
                );
            }

            return oSearchResult;
        }

        #endregion
    }
}