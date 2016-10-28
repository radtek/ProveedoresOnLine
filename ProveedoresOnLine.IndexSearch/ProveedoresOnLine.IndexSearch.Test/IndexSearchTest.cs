using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.Company.Models.Company;
using System.Linq;
using Nest;
using System.Collections;
using ProveedoresOnLine.SurveyModule.Models.Index;

namespace ProveedoresOnLine.IndexSearch.Test
{
    [TestClass]
    public class IndexSearchTest
    {
        [TestMethod]
        public void GetAllCompanyIndexSearch()
        {
            List<CompanyIndexModel> oReturn =
                Controller.IndexSearch.GetCompanyIndex();

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }

        [TestMethod]
        public void GetAllCustomerProviderIndexSearch()
        {
            List<CustomerProviderIndexModel> oReturn = ProveedoresOnLine.IndexSearch.Controller.IndexSearch.GetCustomerProviderIndex();

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }

        [TestMethod]
        public void CompanyIndexationFunction()
        {
            bool oReturn = Controller.IndexSearch.CompanyIndexationFunction();

            Assert.AreEqual(true, oReturn != null && oReturn == true);
        }

        [TestMethod]
        public void CustomerProviderIndexationFunction()
        {
            bool oReturn = Controller.IndexSearch.CustomerProviderIdexationFunction();

            Assert.AreEqual(true, oReturn != null && oReturn == true);
        }

        [TestMethod]
        public void GetCustomerProviderByCustomer()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);

            settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CustomerProviderIndex].Value);
            ElasticClient CustomerProviderClient = new ElasticClient(settings);
            var result = CustomerProviderClient.Search<CustomerProviderIndexModel>(s => s
            .From(0)
            .Size(20)
             .Query(q => q
                            .Match(m => m.Field("customerPublicId").Query("DA5C572E"))
            ));
        }

        [TestMethod]
        public void SearchCompany()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex("dev_companyindex");
            ElasticClient CustomerProviderClient = new ElasticClient(settings);
            int page = 0;
            Nest.ISearchResponse<CompanyIndexModel> result = CustomerProviderClient.Search<CompanyIndexModel>(s => s
            .From(0)
                .TrackScores(true)
                .From(page)
                .Size(20)
                .Query(q =>
                    q.Nested(n => n
                    .Path(p => p.oCustomerProviderIndexModel)
                        .Query(fq => fq
                            .Match(match => match
                            .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                            .Query("DA5C572E")
                            )
                            ).ScoreMode(NestedScoreMode.Max)
                        )
                    )
                    .Aggregations(agg => agg                        
                        .Nested("myproviders_avg", x => x.
                        Path(p => p.oCustomerProviderIndexModel).
                        Aggregations(aggs => aggs.Terms("myproviders", term => term.Field(fi => fi.oCustomerProviderIndexModel.First().CustomerPublicId)
                            )
                        )
                    )
                    .Terms("city", aggv => aggv
                        .Field(fi => fi.CityId))
                    .Terms("country", c => c
                        .Field(fi => fi.CountryId))
                    .Terms("blacklist", bl => bl
                        .Field(fi => fi.InBlackList)))
                .Query(q => q.
                    Filtered(f => f
                    .Query(q1 => q1.MatchAll())
                    .Filter(f2 =>
                    {
                        QueryContainer qb = null;

                        //qb &= q.Term(m => m.CompanyName, "colsein");

                        if (true)
                        {
                            qb &= q.Term(m => m.CityId, 1512);
                        }
                        if (true)
                        {
                            qb &= q.Term(m => m.CountryId, 988);
                        }
                        if (true)
                        {
                            qb &= q.Nested(n => n
                             .Path(p => p.oCustomerProviderIndexModel)
                            .Query(fq => fq
                                .Match(match => match
                                .Field(field => field.oCustomerProviderIndexModel.First().StatusId)
                                .Query("902005")
                                )
                              )
                           );
                        }

                        qb &= q.Nested(n => n
                             .Path(p => p.oCustomerProviderIndexModel)
                            .Query(fq => fq
                                .Match(match => match
                                .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                                .Query("DA5C572E")
                                )
                              )
                           );

                        return qb;
                    })
                    ))
                );
        }

        [TestMethod]
        public void SearchCompanyByPublicId()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex("dev_companyindex");
            ElasticClient CustomerProviderClient = new ElasticClient(settings);

            Nest.ISearchResponse<CompanyIndexModel> oResult = CustomerProviderClient.Search<CompanyIndexModel>(s => s
                .From(0)
                .Size(1)
                .Query(q => q.QueryString(qs => qs.Query("1B686E34"))));

            //Nest.ISearchResponse<CompanyIndexModel> oResult = CustomerProviderClient.Search<CompanyIndexModel>(s => s
            //    .From(0)
            //    .TrackScores(true)
            //    .Size(1)
            //    .Query(query => query.Filtered(filtered => filtered
            //        .Query(q => q.MatchAll())
            //        .Filter(f => f.Nested(nf => nf
            //            .InnerHits()
            //            .Path(p => p.CompanyPublicId == "1CA3A147"))))));
        }

        #region Survey

        [TestMethod]
        public void TestGetCompanySurveyIndex()
        {
            List<CompanySurveyIndexModel> oReturn =
                ProveedoresOnLine.IndexSearch.Controller.IndexSearch.GetCompanySurveyIndex();

            CompanySurveyIndexModel oModel = oReturn.Where(x => x.oSurveyIndexModel != null && x.oSurveyIndexModel.Count > 0).FirstOrDefault();

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }

        [TestMethod]
        public void GetAllSurveyIndexSearch()
        {
            List<SurveyIndexSearchModel> oReturn =
                Controller.IndexSearch.GetSurveyIndex();

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }

        [TestMethod]
        public void GetAllSurveyInfoIndexSearch()
        {
            List<SurveyInfoIndexSearchModel> oReturn =
                Controller.IndexSearch.GetSurveyInfoIndex();

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }

        [TestMethod]
        public void SearchCompanySurvey()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex("dev_companysurveyindex");
            ElasticClient CompanySurveyClient = new ElasticClient(settings);

            Nest.ISearchResponse<CompanySurveyIndexModel> result = CompanySurveyClient.Search<CompanySurveyIndexModel>(s => s
            .From(0)
                .Size(20)
                    .Aggregations
                     (agg => agg
                        .Nested("status_avg", x => x.
                            Path(p => p.oCustomerProviderIndexModel).
                            Aggregations(aggs => aggs.
                                Terms("status", term => term.
                                    Field(fi => fi.oCustomerProviderIndexModel.First().StatusId)
                                )
                            )
                        )
                        .Terms("city", aggv => aggv
                            .Field(fi => fi.CityId))
                        .Terms("country", c => c
                            .Field(fi => fi.CountryId))
                        .Terms("blacklist", bl => bl
                            .Field(fi => fi.InBlackList)))
                .Query(q =>
                    q.Nested(n => n
                        .Path(p => p.oCustomerProviderIndexModel)
                            .Query(fq => fq
                                .Match(match => match
                                                    .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                                                    .Query("")
                                )
                              ).ScoreMode(NestedScoreMode.Max)
                           )
                    )
                .Query(q =>
                     q.Term(p => p.CompanyName, ""))
                );
        }

        [TestMethod]
        public void DeleteCompanySurveyIndex()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex("dev_companysurveyindex");
            ElasticClient CompanySurveyClient = new ElasticClient(settings);

            //CompanySurveyClient.Delete("dev_companysurveyindex");
        }

        [TestMethod]
        public void CompanySurveyIndexationFunction()
        {
            bool oReturn = ProveedoresOnLine.IndexSearch.Controller.IndexSearch.SurveyIndexationFunction();

            Assert.AreEqual(true, oReturn);
        }

        [TestMethod]
        public void CompanySurveyIndexPartialFunction()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value);
            settings.DisableDirectStreaming(true);
            ElasticClient client = new ElasticClient(settings);

            Nest.ISearchResponse<CompanySurveyIndexModel> oResult = client.Search<CompanySurveyIndexModel>(s => s
                .From(0)
                .Size(1)
                .Query(q => q.QueryString(qs => qs.Query("11CC0EBC"))));

            CompanySurveyIndexModel oModelToIndex = new CompanySurveyIndexModel(oResult.Documents.FirstOrDefault());

            oModelToIndex.oSurveyIndexModel.Add(new SurveyIndexModel()
            {
                CompanyPublicId = "18474D1D",
                CustomerPublicId = "DA5C572E",
                SurveyPublicId = "BB7DAVID",
                SurveyStatus = "Programada",
                SurveyStatusId = 1206001,
                SurveyType = "EVALUACION DE DESEMPEÑO PROVEEDORES ONLINE 1",
                SurveyTypeId = 44,
            });

            Uri nodeToIndex = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settingsToIndex = new ConnectionSettings(nodeToIndex);
            settingsToIndex.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value);
            ElasticClient clientToIndex = new ElasticClient(settingsToIndex);

            ICreateIndexResponse oElasticResponse = clientToIndex.CreateIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value, c => c
                            .Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)
                            .Analysis(a => a.Analyzers(an => an.Custom("customWhiteSpace", anc => anc.Filters("asciifolding", "lowercase")
                                .Tokenizer("whitespace")
                                )).TokenFilters(tf => tf
                                        .EdgeNGram("customEdgeNGram", engrf => engrf
                                        .MinGram(1)
                                        .MaxGram(10)))).NumberOfShards(1)
                            ));

            var Index = clientToIndex.Index(oModelToIndex);
        }
        #endregion
    }
}
