using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using ProveedoresOnLine.SurveyModule.Models.Index;

namespace ProveedoresOnLine.IndexSearch.Test
{
    /// <summary>
    /// Summary description for CompanySurveyIndexTest
    /// </summary>
    [TestClass]
    public class CompanySurveyIndexTest
    {
        [TestMethod]
        public void StartProcess()
        {
            ProveedoresOnLine.SurveyIndexSearch.SurveyIndexSearchProcess.StartProcess();
        }

        [TestMethod]
        public void SurveySearch()
        {
            List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> oSurveyTypeModel = new List<Company.Models.Util.ElasticSearchFilter>();

            List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> oSurveyStatusModel = new List<Company.Models.Util.ElasticSearchFilter>();


            List<Tuple<string, string, string>> lstSearchFilter = new List<Tuple<string, string, string>>();

            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);

            settings.DefaultIndex(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_CompanySurveyIndex].Value);
            settings.DisableDirectStreaming(true);
            ElasticClient client = new ElasticClient(settings);

            Nest.ISearchResponse<SurveyIndexModel> oModel = client.Search<ProveedoresOnLine.SurveyModule.Models.Index.SurveyIndexModel>(s => s
            .From(0)
            .TrackScores(true)
            .Size(20)
            .Aggregations
                (agg => agg
                .Nested("survey_avg", x => x.
                    Path(p => p).
                    Aggregations(aggs => aggs.Terms("survey", term => term.Field(fi => fi.SurveyTypeId)))
                )
                .Nested("surveystatus_avg", x => x.
                    Path(p => p).
                    Aggregations(aggs => aggs.Terms("surveystatus", term => term.Field(fi => fi.SurveyStatusId)))
                )
                //.Terms("city", aggv => aggv
                //    .Field(fi => fi.CityId))
                //.Terms("country", c => c
                //    .Field(fi => fi.CountryId))
            )
                //.Query(q =>
                //    q.Nested(n => n
                //        .Path(p => p.oCustomerProviderIndexModel)
                //            .Query(fq => fq
                //                .Match(match => match
                //                .Field(field => field.oCustomerProviderIndexModel.First().CustomerPublicId)
                //                .Query(SessionModel.CurrentCompany.CompanyPublicId)
                //                )
                //              ).ScoreMode(NestedScoreMode.Max)
                //            )
                //    )
            .Query(q =>
                q.Nested(n => n
                    .Path(p => p)
                        .Query(fq => fq
                            .Match(match => match
                            .Field(field => field.CustomerPublicId)
                            .Query("26D388E3")
                            )
                          ).ScoreMode(NestedScoreMode.Max)
                        )
                )
            .Query(q => q.
                Filtered(f => f
                .Query(q1 => q1.MatchAll() && q.QueryString(qs => qs.Query("")))
                .Filter(f2 =>
                {
                    QueryContainer qb = null;

                    qb &= q.Nested(n => n
                        .Path(p => p)
                       .Query(fq => fq
                           .Match(match => match
                           .Field(field => field.CustomerPublicId)
                           .Query("26D388E3")
                           )
                         )
                      );

                    return qb;
                })
                ))
            );

            if (oModel != null)
            {
                string a = "";
            }

            //#region Survey Status Aggregation

            //oModel.Aggs.Nested("surveystatus_avg").Terms("surveystatus").Buckets.All(x =>
            //{
            //    oSurveyStatusModel.Add(new Company.Models.Util.ElasticSearchFilter()
            //    {
            //        FilterCount = (int)x.DocCount,
            //        FilterType = x.Key.Split('.')[0],
            //        FilterName = x.Key.Split('.')[0],
            //    });

            //    return true;
            //});

            //#endregion

            //#region Survey Type Aggregation

            //oModel.Aggs.Nested("survey_avg").Terms("survey").Buckets.All(x =>
            //{
            //    oSurveyTypeModel.Add(new Company.Models.Util.ElasticSearchFilter()
            //    {
            //        FilterCount = (int)x.DocCount,
            //        FilterType = x.Key.Split('.')[0],
            //        FilterName = x.Key.Split('.')[0],
            //    });

            //    return true;
            //});

            //#endregion
        }
    }
}
