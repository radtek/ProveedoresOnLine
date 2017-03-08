﻿using Nest;
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
                settings.DefaultIndex("prod_companyindex");
                ElasticClient client = new ElasticClient(settings);

                ICreateIndexResponse oElasticResponse = client.
                        CreateIndex("prod_companyindex", c => c
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
                var Index = client.IndexMany(oCompanyToIndex, "prod_companyindex");
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
                settings.DefaultIndex("prod_companycustomerindex");
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
                var Index = client.IndexMany(oCompanyToIndex, "prod_companycustomerindex");
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
                        CreateIndex("prod_customerproviderindex", c => c
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
                client.Map<CustomerProviderIndexModel>(m => m.AutoMap());
                var Index = client.IndexMany(oCustomerProviderToIndex, "prod_customerproviderindex");
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for CustomerProvider: " + CustomerProviderId + err.Message + "Inner Exception::" + err.InnerException);
            }

            LogFile("Index Process Successfull for: " + Counter + " Customers-Providers");
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

                var Index = client.IndexMany(oCompanySurveyIndexSearch, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value);
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
                    var IndexIter = client.IndexMany(oToIterIndex, ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ThirdKnowledgeIndex].Value);
                    LogFile("Index Process Status::: " + IndexIter.IsValid + "::" + IndexIter.DebugInformation);
			    }             
                
                LogFile("Index Process Successfull for: " + oToIndex.Count());
            }
            catch (Exception err)
            {
                LogFile("Index Process Failed for Thirdknowledge: " + err.Message + "Inner Exception::" + err.InnerException);
            }            
            return true;
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
