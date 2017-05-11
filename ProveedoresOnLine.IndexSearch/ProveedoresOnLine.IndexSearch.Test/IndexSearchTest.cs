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
        public void GetAllCustomerCompanyIndexSearch()
        {
            List<CompanyIndexModel> oReturn =
                Controller.IndexSearch.GetCompanyCustomerIndex();

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
            settings.DefaultIndex("prod_companyindex");
            #region Original Search
            ElasticClient CustomerProviderClient = new ElasticClient(settings);

            Nest.ISearchResponse<CompanyIndexModel> result = CustomerProviderClient.Search<CompanyIndexModel>((s => s
                .From(0)
                .TrackScores(true)
                .Size(20)
                .Aggregations
                    (agg => agg
                        .Nested("status_avg", x => x.
                            Path(p => p.oCustomerProviderIndexModel.Where(xx => xx.CustomerPublicId == "26D388E3").Select(xx => xx).ToArray()).
                                Aggregations(aggs => aggs.Terms("status", term => term.Field(fi => fi.oCustomerProviderIndexModel.First().StatusId)
                                )
                            )
                        )
                    .Nested("myproviders_avg", x => x.
                        Path(p => p.oCustomerProviderIndexModel).
                            Aggregations(aggs => aggs.Terms("myproviders", term => term.Field(fi => fi.oCustomerProviderIndexModel.First().CustomerPublicId)
                            )
                        )
                    )
                    .Terms("ica", aggv => aggv
                        .Field(fi => fi.ICAId))
                    .Terms("city", aggv => aggv
                        .Field(fi => fi.CityId))
                    .Terms("country", c => c
                        .Field(fi => fi.CountryId))
                    .Terms("blacklist", bl => bl
                        .Field(fi => fi.InBlackList)))
                .Query(q => q.
                    Filtered(f => f
                        //.Query(q1 => q1.MatchAll() && q.QueryString(qs => qs.Query("Nilsen")))
                    .Filter(f2 =>
                    {
                        QueryContainer qb = null;

                        #region Basic Providers Filters
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.City).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Term(m => m.CityId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.City).Select(y => y.Item1).FirstOrDefault());
                        //}
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.Country).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Term(m => m.CountryId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.Country).Select(y => y.Item1).FirstOrDefault());
                        //}
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.RestrictiveListProvider).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Term(m => m.InBlackList, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.RestrictiveListProvider).Select(y => y.Item1).FirstOrDefault());
                        //}
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.EconomicActivity).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Term(m => m.ICAId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.EconomicActivity).Select(y => y.Item1).FirstOrDefault());
                        //}
                        #endregion

                        #region My Providers Filter
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.MyProviders).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Nested(n => n
                        //    .Path(p => p.oCustomerProviderIndexModel)
                        //    .Query(fq => fq
                        //        .Match(match => match
                        //        .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                        //        .Query(SessionModel.CurrentCompany.CompanyPublicId)
                        //        )
                        //   ));
                        //}
                        #endregion

                        #region Other Providers Filter
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.OtherProviders).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Nested(n => n
                        //    .Path(p => p.oCustomerProviderIndexModel)
                        //    .Query(fq => fq
                        //        .Match(match => match
                        //        .Field(field => field.oCustomerProviderIndexModel.Where(y => y.CustomerPublicId != SessionModel.CurrentCompany.CompanyPublicId).Select(y => y).First().CustomerPublicId)
                        //        )
                        //   ));
                        //}
                        #endregion

                        #region Provider Status
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.ProviderStatus).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Nested(n => n
                        //     .Path(p => p.oCustomerProviderIndexModel)
                        //    .Query(fq => fq
                        //        .Match(match => match
                        //        .Field(field => field.oCustomerProviderIndexModel.First().StatusId)
                        //        .Query(lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.ProviderStatus).Select(y => y.Item1).FirstOrDefault())
                        //        )
                        //      )
                        //   );
                        //}

                        #endregion

                        #region Can see other Providers?
                        //if (SessionModel.CurrentCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.OtherProviders).Select(x => x.Value).FirstOrDefault() == "1")
                        //{
                        //    qb &= q.Nested(n => n
                        //    .Path(p => p.oCustomerProviderIndexModel)
                        //        .Query(fq => fq
                        //            .Match(match => match
                        //            .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId))
                        //          ));
                        //}
                        //else
                        //{
                        //    qb &= q.Nested(n => n
                        //    .Path(p => p.oCustomerProviderIndexModel)
                        //        .Query(fq => fq
                        //            .Match(match => match
                        //            .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                        //            .Query(SessionModel.CurrentCompany.CompanyPublicId))
                        //        ));
                        //}
                        #endregion

                        qb &= q.Nested(n => n
                           .Path(p => p.oCustomerProviderIndexModel)
                               .Query(fq => fq
                                   .Match(match => match
                                   .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                                   .Query("26D388E3"))
                               ));
                        return qb;
                    })
                    )))
                );
            #endregion
        }

        [TestMethod]
        public void SearchCustomerProvider()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex("prod_customerproviderindex");
            int page = 0;
            #region Original Search
            ElasticClient CustomerProviderClient = new ElasticClient(settings);

            Nest.ISearchResponse<CustomerProviderIndexModel> result = CustomerProviderClient.Search<CustomerProviderIndexModel>((s => s
                .From(0)
                .TrackScores(true)
                .Size(20)
                .Aggregations
                    (agg => agg
                    .Terms("status", aggv => aggv
                        .Field(fi => fi.StatusId)))
                .Query(q => q.
                    Filtered(f => f
                        .Query(q1 => q.Term(m => m.CustomerPublicId, "26d388e3"))
                    .Filter(f2 =>
                    {
                        QueryContainer qb = null;

                        #region Basic Providers Filters
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.City).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Term(m => m.CityId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.City).Select(y => y.Item1).FirstOrDefault());
                        //}
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.Country).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Term(m => m.CountryId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.Country).Select(y => y.Item1).FirstOrDefault());
                        //}
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.RestrictiveListProvider).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Term(m => m.InBlackList, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.RestrictiveListProvider).Select(y => y.Item1).FirstOrDefault());
                        //}
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.EconomicActivity).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Term(m => m.ICAId, lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.EconomicActivity).Select(y => y.Item1).FirstOrDefault());
                        //}
                        #endregion

                        #region My Providers Filter
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.MyProviders).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Nested(n => n
                        //    .Path(p => p.oCustomerProviderIndexModel)
                        //    .Query(fq => fq
                        //        .Match(match => match
                        //        .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                        //        .Query(SessionModel.CurrentCompany.CompanyPublicId)
                        //        )
                        //   ));
                        //}
                        #endregion

                        #region Other Providers Filter
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.OtherProviders).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Nested(n => n
                        //    .Path(p => p.oCustomerProviderIndexModel)
                        //    .Query(fq => fq
                        //        .Match(match => match
                        //        .Field(field => field.oCustomerProviderIndexModel.Where(y => y.CustomerPublicId != SessionModel.CurrentCompany.CompanyPublicId).Select(y => y).First().CustomerPublicId)
                        //        )
                        //   ));
                        //}
                        #endregion

                        #region Provider Status
                        //if (lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.ProviderStatus).Select(y => y).FirstOrDefault() != null)
                        //{
                        //    qb &= q.Nested(n => n
                        //     .Path(p => p.oCustomerProviderIndexModel)
                        //    .Query(fq => fq
                        //        .Match(match => match
                        //        .Field(field => field.oCustomerProviderIndexModel.First().StatusId)
                        //        .Query(lstSearchFilter.Where(y => int.Parse(y.Item3) == (int)enumFilterType.ProviderStatus).Select(y => y.Item1).FirstOrDefault())
                        //        )
                        //      )
                        //   );
                        //}

                        #endregion

                        #region Can see other Providers?
                        //if (SessionModel.CurrentCompany.CompanyInfo.Where(x => x.ItemInfoType.ItemId == (int)enumCompanyInfoType.OtherProviders).Select(x => x.Value).FirstOrDefault() == "1")
                        //{
                        //    qb &= q.Nested(n => n
                        //    .Path(p => p.oCustomerProviderIndexModel)
                        //        .Query(fq => fq
                        //            .Match(match => match
                        //            .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId))
                        //          ));
                        //}
                        //else
                        //{
                        //    qb &= q.Nested(n => n
                        //    .Path(p => p.oCustomerProviderIndexModel)
                        //        .Query(fq => fq
                        //            .Match(match => match
                        //            .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                        //            .Query(SessionModel.CurrentCompany.CompanyPublicId))
                        //        ));
                        //}
                        #endregion

                        //qb &= q.Term(m => m.CustomerPublicId, "26D388E3");

                        return qb;
                    })
                    )))
                );

            if (result != null)
            {
                page = 2;
            }
            #endregion
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
            settings.DefaultIndex("dev_surveyindex");
            ElasticClient CompanySurveyClient = new ElasticClient(settings);

            Nest.ISearchResponse<SurveyIndexSearchModel> result = CompanySurveyClient.Search<SurveyIndexSearchModel>(s => s
            .From(0)
                .Size(20)
                //    .Aggregations
                //     (agg => agg                        
                //        .Terms("status", aggv => aggv
                //            .Field(fi => fi.SurveyStatusId)                       

                //))
                .Query(q => q.QueryString(qr => qr.Fields(fds => fds.Field(f => f.CustomerPublicId)).Query("DA5C572E")))

                );
        }

        [TestMethod]
        public void DeleteCompanySurveyIndex()
        {
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex("dev_surveyindex");
            ElasticClient CompanySurveyClient = new ElasticClient(settings);

            CompanySurveyClient.DeleteIndex("dev_surveyindex");

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

        [TestMethod]
        public void CompanySurveyIndexUpdate()
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

            oModelToIndex.oSurveyIndexModel = new List<SurveyIndexModel>(){
                new SurveyIndexModel(){
                    CompanyPublicId = "18474D1D",
                    CustomerPublicId = "DA5C572E",
                    SurveyPublicId = "DAVIDDDD",
                    SurveyStatus = "Progreso",
                    SurveyStatusId = 1206003,
                    SurveyType = "EVALUACION DE DESEMPEÑO PROVEEDORES ONLINE 1",
                    SurveyTypeId = 44,
                },
            };


        }

        [TestMethod]
        public void CompanySurveyDeleteIndex()
        {
            Uri nodeToIndex = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settingsToIndex = new ConnectionSettings(nodeToIndex);
            settingsToIndex.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value);
            ElasticClient clientToIndex = new ElasticClient(settingsToIndex);

            clientToIndex.DeleteIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value);
        }

        [TestMethod]
        public void SurveyIndexation()
        {
            List<SurveyIndexSearchModel> SurveyndexModelList = ProveedoresOnLine.IndexSearch.Controller.IndexSearch.GetSurveyIndex();

            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DefaultIndex("prod_surveyindex");
            ElasticClient client = new ElasticClient(settings);

            ICreateIndexResponse oElasticResponse = client.
                    CreateIndex("prod_surveyindex", c => c
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
            client.Map<SurveyIndexSearchModel>(m => m.AutoMap());
            var Index = client.IndexMany(SurveyndexModelList, "prod_surveyindex");
        }


        #endregion

        #region Third Knowledge
        [TestMethod]
        public void QueryModelIndeAll()
        {
           Controller.IndexSearch.QueryModelIndeAll();            
        }       

        #endregion
    }
}

