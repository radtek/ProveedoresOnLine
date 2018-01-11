using Nest;
using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.IndexSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using ProveedoresOnLine.SurveyModule.Models.Index;
using Newtonsoft.Json;
using ProveedoresOnLine.ThirdKnowledge.Models;

namespace ProveedoresOnLine.IndexSearch.Controller
{
    public class IndexSearch
    {
        #region Company Index

        public static List<CompanyIndexModel> GetCompanyIndex()
        {
            return DAL.Controller.IndexSearchDataController.Instance.GetCompanyIndex();
        }

        public static List<CustomerProviderIndexModel> GetCustomerProviderIndex()
        {
            return DAL.Controller.IndexSearchDataController.Instance.GetCustomerProviderIndex();
        }

        public static List<CompanyIndexModel> GetCompanyCustomerIndex()
        {
            return DAL.Controller.IndexSearchDataController.Instance.GetCompanyCustomerIndex();
        }

        public static bool CompanyIndexationFunction()
        {
            List<CompanyIndexModel> oCompanyToIndex = GetCompanyIndex();
            try
            {
                LogFile("Start Process: " + "ProvidersToIndex:::" + oCompanyToIndex.Count());

                Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);
                settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanyIndex].Value);
                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanyIndex].Value, c => c
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
                client.Map<CompanyIndexModel>(m => m.AutoMap());
                var Index = client.IndexMany(oCompanyToIndex, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanyIndex].Value);
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for Company: " + err.Message + "Inner Exception::" + err.InnerException);
            }
            LogFile("Index Process Successfull for: " + oCompanyToIndex.Count());
            return true;
        }

        public static bool CompanyCustomerIndexationFunction()
        {
            List<CompanyIndexModel> oCompanyToIndex = GetCompanyCustomerIndex();
            try
            {
                LogFile("Start Process: " + "ProvidersToIndex:::" + oCompanyToIndex.Count());

                Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);
                settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanyCustomerIndex].Value);
                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex("prod_companycustomerindex", c => c
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
                client.Map<CompanyIndexModel>(m => m.AutoMap());
                var Index = client.IndexMany(oCompanyToIndex, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanyCustomerIndex].Value);
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for Company: " + err.Message + "Inner Exception::" + err.InnerException);
            }
            LogFile("Index Process Successfull for: " + oCompanyToIndex.Count());
            return true;
        }

        public static bool CustomerProviderIdexationFunction()
        {
            int CustomerProviderId = 0;
            var Counter = 0;
            try
            {
                List<CustomerProviderIndexModel> oCustomerProviderToIndex = GetCustomerProviderIndex();
                LogFile("About to index: " + oCustomerProviderToIndex.Count + " CustomerProviders");

                Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);
                settings.DefaultIndex("prod_customerproviderindex");
                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CustomerProviderIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(20)
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
                            ).NumberOfShards(10)
                        )
                    );
                client.Map<CustomerProviderIndexModel>(m => m.AutoMap());
                var Index = client.IndexMany(oCustomerProviderToIndex, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CustomerProviderIndex].Value);
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for CustomerProvider: " + CustomerProviderId + err.Message + "Inner Exception::" + err.InnerException);
            }

            LogFile("Index Process Successfull for: " + Counter + " Customers-Providers");
            return true;
        }

        public static bool CalificationIdexationFunction()
        {
            int CustomerProviderId = 0;
            var Counter = 0;
            try
            {
                List<CalificationIndexModel> oCalificationToIndex = Company.Controller.Company.CalificationGetAll();
                LogFile("About to index: " + oCalificationToIndex.Count + " CalificationIndex");

                Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);
                settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CalificationIndex].Value);
                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CalificationIndex].Value, c => c
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
                client.Map<CalificationIndexModel>(m => m.AutoMap());
                var Index = client.IndexMany(oCalificationToIndex, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CalificationIndex].Value);
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for CustomerProvider: " + CustomerProviderId + err.Message + "Inner Exception::" + err.InnerException);
            }

            LogFile("Index Process Successfull for: " + Counter + " Customers-Providers");
            return true;
        }

        public static bool CustomFiltersIdexationFunction()
        {
            int CustomerProviderId = 0;
            var Counter = 0;
            try
            {
                List<CustomFiltersIndexModel> oCustomFiltersToIndex = Company.Controller.Company.CustomFiltersGetAll();
                LogFile("About to index: " + oCustomFiltersToIndex.Count + " CustomFiltersIndex");

                Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);
                settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CustomFiltesIndex].Value);
                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CustomFiltesIndex].Value, c => c
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
                client.Map<CustomFiltersIndexModel>(m => m.AutoMap());
                var Index = client.IndexMany(oCustomFiltersToIndex, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CustomFiltesIndex].Value);
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for CustomFilters: " + CustomerProviderId + err.Message + "Inner Exception::" + err.InnerException);
            }

            LogFile("Index Process Successfull for: " + Counter + " Custom-Filters");
            return true;

        }


        #endregion

        #region Survey Index

        public static bool SurveyIndexationFunction()
        {
            List<CompanySurveyIndexModel> oCompanySurveyIndexSearch = GetCompanySurveyIndex();

            try
            {
                LogFile("Start Process: " + "ProviderSurveyToIndex:::" + oCompanySurveyIndexSearch.Count());

                Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);

                var settings = new ConnectionSettings(node);

                settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value);

                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value, c => c
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

                client.Map<CompanySurveyIndexModel>(m => m.AutoMap());

                var Index = client.IndexMany(oCompanySurveyIndexSearch, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_SurveyIndex].Value);
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for Company: " + err.Message + "Inner Exception::" + err.InnerException);
            }

            LogFile("Index Process Successfull for: " + oCompanySurveyIndexSearch.Count());

            return true;
        }

        public static List<CompanySurveyIndexModel> GetCompanySurveyIndex()
        {
            return DAL.Controller.IndexSearchDataController.Instance.GetCompanySurveyIndex();
        }

        public static List<SurveyIndexSearchModel> GetSurveyIndex()
        {
            return DAL.Controller.IndexSearchDataController.Instance.GetSurveyIndex();
        }

        #region Survey Info Index

        public static List<SurveyInfoIndexSearchModel> GetSurveyInfoIndex()
        {
            return DAL.Controller.IndexSearchDataController.Instance.GetSurveyInfoIndex();
        }

        #endregion

        #endregion

        #region ThirdKnowledge Index

        public static bool GetThirdknowledgeIndex(int RowFrom, int RowTo)
        {
            try
            {
                LogFile("Start Process: " + "ThirdKnowledgeToIndex:::");

                List<ThirdknowledgeIndexSearchModel> oToIndex = DAL.Controller.IndexSearchDataController.Instance.GetThirdknowledgeIndex(RowFrom, RowTo);

                Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);
                settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ThirdKnowledgeIndex].Value);
                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ThirdKnowledgeIndex].Value, c => c
                        .Settings(s => s.NumberOfReplicas(1).NumberOfShards(1)
                        .TotalShardsPerNode(2)
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

                client.Map<ThirdknowledgeIndexSearchModel>(m => m.AutoMap());
                RowFrom = 0;
                for (int i = -1; i < (oToIndex.FirstOrDefault().TotalRows / 10000); i++)
                {
                    List<ThirdknowledgeIndexSearchModel> oToIterIndex = new List<ThirdknowledgeIndexSearchModel>();
                    LogFile("Index Process Count::: " + RowFrom + "__:::__" + RowTo);

                    oToIterIndex = DAL.Controller.IndexSearchDataController.Instance.GetThirdknowledgeIndex(RowFrom, 10000);
                    RowFrom = RowFrom + 10000;

                    client.Map<ThirdknowledgeIndexSearchModel>(m => m.AutoMap());
                    var IndexIter = client.IndexManyAsync(oToIterIndex, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ThirdKnowledgeIndex].Value);
                    LogFile("Index Process Status::: " + IndexIter.AsyncState + "::" + IndexIter.Result);
                }

                LogFile("Index Process Successfull for: " + oToIndex.Count());
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for Thirdknowledge: " + err.Message + "Inner Exception::" + err.InnerException);
            }
            return true;
        }

        public static void QueryModelIndeAll()
        {
            List<TK_QueryIndexModel> oToIndex = DAL.Controller.IndexSearchDataController.Instance.GetAllQueryModelIndex();
            try
            {
                LogFile("Start Process: " + "QueryIndex:::" + oToIndex.Count());

                Uri node = new Uri(Models.Util.InternalSettings.Instance[Models.Constants.C_Settings_ElasticSearchUrl].Value);
                var settings = new ConnectionSettings(node);
                settings.DefaultIndex(Models.Util.InternalSettings.Instance[Models.Constants.C_Settings_TD_QueryIndex].Value);
                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex(Models.Util.InternalSettings.Instance[Models.Constants.C_Settings_TD_QueryIndex].Value, c => c
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
                var Index = client.IndexMany(oToIndex, Models.Util.InternalSettings.Instance[Models.Constants.C_Settings_TD_QueryIndex].Value);
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for Company: " + err.Message + "Inner Exception::" + err.InnerException);
            }
            LogFile("Index Process Successfull for: " + oToIndex.Count());

        }

        public static void QueryModelIndexByItem(TK_QueryIndexModel oModelToIndex)
        {
            try
            {
                if (oModelToIndex != null)
                {
                    LogFile("Start Process::: FunctionName::: QueryModelIndexByItem::: " + "QueryIndex By Item:::" + oModelToIndex.QueryPublicId);

                    Uri node = new Uri(Models.Util.InternalSettings.Instance[Models.Constants.C_Settings_ElasticSearchUrl].Value);
                    var settings = new ConnectionSettings(node);
                    settings.DefaultIndex(Models.Util.InternalSettings.Instance[Models.Constants.C_Settings_TD_QueryIndex].Value);
                    ElasticClient client = new ElasticClient(settings);

                    ICreateIndexResponse oElasticResponse = client.
                            CreateIndex(Models.Util.InternalSettings.Instance[Models.Constants.C_Settings_TD_QueryIndex].Value, c => c
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
                }
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for Company: " + err.Message + "Inner Exception::" + err.InnerException);
            }
            LogFile("Index Process Successfull for: " + oModelToIndex.QueryPublicId);
        }

        public static bool QueryModelIndexSearch()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);

            settings.DefaultIndex("dev_queryindex");
            settings.DisableDirectStreaming(true);
            ElasticClient client = new ElasticClient(settings);

            var result = client.Search<TK_QueryIndexModel>(s => s
           .From(string.IsNullOrEmpty("0") ? 0 : Convert.ToInt32(0) * 20)
           .TrackScores(true)
           .Size(20)
           .Aggregations
               (agg => agg
               .Terms("status", aggv => aggv
                   .Field(fi => fi.QueryStatus))
               .Terms("date", aggv => aggv
                   .Field(fi => fi.LastModify))
               .Terms("searchtype", c => c
                   .Field(fi => fi.SearchType))
               .Terms("domain", c => c
                   .Field(fi => fi.Domain))
               .Terms("useremail", bl => bl
                   .Field(fi => fi.User)))
               .Query(q => q.Filtered(f => f.
                   Filter(f2 =>
                   {
                       QueryContainer qb = null;
                        //qb &= q.Term(m => m.CustomerPublicId, "26D388E3");
                        //qb &= q.Term(m => m.CustomerPublicId, "DA5C572E");

                        qb &= f2.Terms(tms => tms
                           .Field(fi => fi.CustomerPublicId.ToLower())
                            .Terms<string>("da5c572e")
                           );
                        //if (!string.IsNullOrEmpty("")
                        // || !string.IsNullOrEmpty("")
                        // || !string.IsNullOrEmpty("")
                        // || !string.IsNullOrEmpty("sebastian.martinez@")
                        // || !string.IsNullOrEmpty("")
                        // || !string.IsNullOrEmpty(""))
                        //{
                        //    //qb &= q.Terms(tms => tms
                        //    //    .Field(fi => fi.QueryPublicId.ToLower())
                        //    //     .Terms<string>(oQueryModel.Select(x => x.QueryPublicId.ToLower()).ToList())
                        //    //    );
                        //}
                        //else
                        //{
                        //    qb &= q.Term(t => t.CustomerPublicId, "DA5C572E");
                        //}

                        return qb;
                   }
                   )
               )
               )
            );

            if (result.Documents != null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region LogFile
        private static void LogFile(string LogMessage)
        {
            try
            {
                //get file Log
                string LogFile = AppDomain.CurrentDomain.BaseDirectory.Trim().TrimEnd(new char[] { '\\' }) + "\\" +
                    System.Configuration.ConfigurationManager.AppSettings[ProveedoresOnLine.IndexSearch.Models.Constants.C_AppSettings_LogFile].Trim().TrimEnd(new char[] { '\\' });

                if (!System.IO.Directory.Exists(LogFile))
                    System.IO.Directory.CreateDirectory(LogFile);

                LogFile += "\\" + "Log_ThirdKnowledgeIndexProcess_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(LogFile))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "::" + LogMessage);
                    sw.Close();
                }
            }
            catch { }
        }
        #endregion
    }
}
