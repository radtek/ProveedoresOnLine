using Autofac;
using Nest;
using ProveedoresOnLine.ThirdKnowledge.DAL.Controller;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProveedoresOnLine.ThirdKnowledge.Controller
{
    public class ThirdKnowledgeModule
    {
        public static async Task<TDQueryModel> SimpleRequest(string PeriodPublicId, int IdType, string IdentificationNumber, string Name, TDQueryModel oQueryToCreate)
        {
            try
            {
                Task<List<Tuple<string, List<string>, List<string>>>> taskProc = Task.Run(() => OnLnieSearch(IdType, IdentificationNumber));
                List<Tuple<string, List<string>, List<string>>> procResult = new List<Tuple<string, List<string>, List<string>>>();

                Task<List<Tuple<string, List<string>, List<string>>>> taskPP = Task.Run(() => PPSearch(IdType == 2 ? 0 : 1, Name, IdentificationNumber));
                List<Tuple<string, List<string>, List<string>>> ppResult = new List<Tuple<string, List<string>, List<string>>>();
                //Proc Request
                if (!string.IsNullOrEmpty(IdentificationNumber) && IdType != 0)
                    procResult = await taskProc;

                //PanamaPapers Search
                if (!string.IsNullOrEmpty(Name))
                    ppResult = await taskPP;

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

                oQueryToCreate.RelatedQueryBasicInfoModel = new List<TDQueryInfoModel>();
                if (procResult != null && procResult.Count > 0)
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();
                    oInfoCreate.Alias = string.Empty;
                    oInfoCreate.IdentificationResult = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : "";
                    oInfoCreate.Offense = "Presenta Antecedentes Procuraduría Nacional";
                    oInfoCreate.NameResult = procResult.FirstOrDefault().Item1;

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
                    #region Group by Priority
                    oInfoCreate.Priority = "1";
                    #endregion

                    oInfoCreate.Status = "Vigente";
                    oInfoCreate.Enable = true;
                    oInfoCreate.QueryPublicId = oQueryToCreate.QueryPublicId;
                    oInfoCreate.DetailInfo = new List<TDQueryDetailInfoModel>();

                    #region Create Detail
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdNumberRequest,
                        },
                        Value = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.RequestName,
                        },
                        Value = !string.IsNullOrEmpty(Name) ? Name : string.Empty,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdList,
                        },
                        Value = "Procuraduría General de la Nación",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Priotity,
                        },
                        Value = !string.IsNullOrEmpty(oInfoCreate.Priority) ? oInfoCreate.Priority : string.Empty,
                        Enable = true,
                    });

                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Offense,
                        },
                        Value = "Antecedentes reportados en la Procuraduría General de la Nación",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdentificationNumberResult,
                        },
                        Value = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Status,
                        },
                        Value = "Vigente",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.GroupName,
                        },
                        Value = "Procuraduría General de la Nación - Criticidad Media",
                        Enable = true,
                    });

                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Link,
                        },
                        Value = InternalSettings.Instance[Constants.Proc_Url].Value,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.NameResult,
                        },
                        Value = !string.IsNullOrEmpty(procResult.FirstOrDefault().Item1) ? procResult.FirstOrDefault().Item1 : string.Empty,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.ListName,
                        },
                        Value = "Procuraduría General de la Nación",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.MoreInfo,
                        },
                        LargeValue = detailMoreInfo,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Zone,
                        },
                        Value = "Colombia",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.TypeDocument,
                        },
                        Value = IdType.ToString(),
                        Enable = true,
                    });
                    #endregion
                    oQueryToCreate.RelatedQueryBasicInfoModel.Add(oInfoCreate);
                }

                if (ppResult != null && ppResult.Count > 0)
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();
                    oInfoCreate.Alias = string.Empty;
                    oInfoCreate.IdentificationResult = IdType == 1 ? "CC" : IdType == 2 ? "Pasaporte" : IdType == 3 ? "C. Extranjería" : "";
                    oInfoCreate.Offense = "Presenta Reporte en Panama Papers";
                    oInfoCreate.NameResult = ppResult.FirstOrDefault().Item1;

                    string detailMoreInfo = "Panama Papers no hace refierencia necesariamente un delito o una investigación.";

                    #region Group by Priority
                    oInfoCreate.Priority = "2";
                    #endregion

                    oInfoCreate.Status = "Vigente";
                    oInfoCreate.Enable = true;
                    oInfoCreate.QueryPublicId = oQueryToCreate.QueryPublicId;
                    oInfoCreate.DetailInfo = new List<TDQueryDetailInfoModel>();

                    #region Create Detail
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdNumberRequest,
                        },
                        Value = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.RequestName,
                        },
                        Value = !string.IsNullOrEmpty(Name) ? Name : string.Empty,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdList,
                        },
                        Value = "Panama Papers",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Priotity,
                        },
                        Value = !string.IsNullOrEmpty(oInfoCreate.Priority) ? oInfoCreate.Priority : string.Empty,
                        Enable = true,
                    });

                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Offense,
                        },
                        Value = "Reporte en Panama Papers",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdentificationNumberResult,
                        },
                        Value = "N/A",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Status,
                        },
                        Value = "Vigente",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.GroupName,
                        },
                        Value = "Panama Papers - Criticidad Baja",
                        Enable = true,
                    });

                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Link,
                        },
                        Value = ppResult.FirstOrDefault().Item1,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.NameResult,
                        },
                        Value = Name,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.ListName,
                        },
                        Value = "Panama Papers",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.MoreInfo,
                        },
                        LargeValue = detailMoreInfo,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Zone,
                        },
                        Value = "N/A",
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.TypeDocument,
                        },
                        Value = IdType.ToString(),
                        Enable = true,
                    });
                    #endregion
                    oQueryToCreate.RelatedQueryBasicInfoModel.Add(oInfoCreate);
                }

                if (oSearchResult.Documents.Count() > 0 || procResult.Count > 0 || ppResult.Count > 0)
                {
                    oSearchResult.Documents.All(x =>
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
                            oInfoCreate.QueryPublicId = oQueryToCreate.QueryPublicId;
                            oInfoCreate.DetailInfo = new List<TDQueryDetailInfoModel>();

                            #region Create Detail
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdNumberRequest,
                                },
                                Value = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.RequestName,
                                },
                                Value = !string.IsNullOrEmpty(Name) ? Name : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Alias,
                                },
                                Value = !string.IsNullOrEmpty(x.AKA) ? x.AKA : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdList,
                                },
                                Value = !string.IsNullOrEmpty(x.ListType) ? x.ListType : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Priotity,
                                },
                                Value = !string.IsNullOrEmpty(oInfoCreate.Priority) ? oInfoCreate.Priority : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.RegisterDate,
                                },
                                Value = !string.IsNullOrEmpty(x.LastModify) ? x.LastModify : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.LastModifyDate,
                                },
                                Value = !string.IsNullOrEmpty(x.LastModify) ? x.LastModify : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.Offense,
                                },
                                Value = !string.IsNullOrEmpty(x.RelatedWiht) ? x.RelatedWiht : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
                                ItemInfoType = new TDCatalogModel()
                                {
                                    ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdentificationNumberResult,
                                },
                                Value = !string.IsNullOrEmpty(x.TypeId) ? x.TypeId : string.Empty,
                                Enable = true,
                            });
                            oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                            {
                                QueryBasicPublicId = oInfoCreate.QueryPublicId,
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
                                         || x.ListType == "SUPER SOCIEDADES"
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

                            oQueryToCreate.RelatedQueryBasicInfoModel.Add(oInfoCreate);
                            return true;
                        });
                    oQueryToCreate.IsSuccess = true;
                  await  QueryUpsert(oQueryToCreate);
                }
                else
                {
                    TDQueryInfoModel oInfoCreate = new TDQueryInfoModel();
                    oInfoCreate.QueryPublicId = oQueryToCreate.QueryPublicId;
                    oInfoCreate.DetailInfo = new List<TDQueryDetailInfoModel>();

                    #region Create Detail
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.RequestName,
                        },
                        Value = !string.IsNullOrEmpty(Name) ? Name : string.Empty,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.IdNumberRequest,
                        },
                        Value = !string.IsNullOrEmpty(IdentificationNumber) ? IdentificationNumber : string.Empty,
                        Enable = true,
                    });
                    oInfoCreate.DetailInfo.Add(new TDQueryDetailInfoModel()
                    {
                        QueryBasicPublicId = oInfoCreate.QueryPublicId,
                        ItemInfoType = new TDCatalogModel()
                        {
                            ItemId = (int)ProveedoresOnLine.ThirdKnowledge.Models.Enumerations.enumThirdKnowledgeColls.GroupName,
                        },
                        Value = "SIN COINCIDENCIAS",
                        Enable = true,
                    });
                    #endregion

                    oQueryToCreate.RelatedQueryBasicInfoModel.Add(oInfoCreate);

                    oQueryToCreate.IsSuccess = false;                    
                }

                return oQueryToCreate;
                await QueryUpsert(oQueryToCreate);
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
            try
            {
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
            catch (Exception)
            {
                throw;
            }
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

        public static List<Models.TDQueryModel> ThirdKnowledgeSearchByPublicId(string CustomerPublicId, string QueryPublic, bool Enable, int PageNumber, int RowCount, out int TotalRows)
        {
            return ThirdKnowledgeDataController.Instance.ThirdKnowledgeSearchByPublicId(CustomerPublicId, QueryPublic, Enable, PageNumber, RowCount, out TotalRows);
        }

        #endregion MarketPlace

        #region Queries

        public static async Task<TDQueryModel> QueryUpsert(TDQueryModel QueryModelToUpsert)
        {
            if (QueryModelToUpsert != null &&
                !string.IsNullOrEmpty(QueryModelToUpsert.PeriodPublicId))
            {
                QueryModelToUpsert.QueryPublicId = await ThirdKnowledgeDataController.Instance.QueryUpsert(QueryModelToUpsert.QueryPublicId, QueryModelToUpsert.PeriodPublicId,
                    QueryModelToUpsert.SearchType.ItemId, QueryModelToUpsert.User, QueryModelToUpsert.FileName, QueryModelToUpsert.IsSuccess, QueryModelToUpsert.QueryStatus.ItemId, true);

                if (QueryModelToUpsert.RelatedQueryBasicInfoModel != null)
                {
                    QueryModelToUpsert.RelatedQueryBasicInfoModel.All(qInf =>
                    {
                        LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();
                        try
                        {
                            qInf.QueryBasicPublicId =
                               ThirdKnowledgeDataController.Instance.QueryBasicInfoInsert
                                (QueryModelToUpsert.QueryPublicId,
                                qInf.NameResult, qInf.IdentificationResult, qInf.Priority, qInf.Peps, qInf.Status, qInf.Alias
                                , qInf.Offense, true);

                            //if (qInf.DetailInfo != null)
                            //{
                            //    qInf.DetailInfo.All(det =>
                            //    {
                            //        Task.Run(() => ThirdKnowledgeDataController.Instance.QueryDetailInfoInsert(qInf.QueryBasicPublicId, det.ItemInfoType.ItemId, det.Value, det.LargeValue, det.Enable));                                                                        
                            //        return true;
                            //    });
                            //}
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

        public static async void QueryDetailUpsert(TDQueryModel QueryModelToUpsert)
        {
            if (QueryModelToUpsert.RelatedQueryBasicInfoModel != null)
            {
                QueryModelToUpsert.RelatedQueryBasicInfoModel.All(qInf =>
                {
                    LogManager.Models.LogModel oLog = Company.Controller.Company.GetGenericLogModel();
                    try
                    {
                        if (qInf.DetailInfo != null)
                        {
                            qInf.DetailInfo.All(det =>
                            {
                                Task.Run(() => ThirdKnowledgeDataController.Instance.QueryDetailInfoInsert(qInf.QueryBasicPublicId, det.ItemInfoType.ItemId, det.Value, det.LargeValue, det.Enable));
                                return true;
                            });
                        }
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

        public static TDQueryInfoModel QueryDetailGetByBasicPublicID(string QueryBasicInfoPublicId)
        {
            return ThirdKnowledgeDataController.Instance.QueryDetailGetByBasicPublicID(QueryBasicInfoPublicId);
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
    }
}