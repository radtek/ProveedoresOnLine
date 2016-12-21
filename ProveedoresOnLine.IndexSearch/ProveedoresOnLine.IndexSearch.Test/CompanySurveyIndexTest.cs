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
            Uri node = new Uri(ProveedoresOnLine.IndexSearch.Models.Util.InternalSettings.Instance[ProveedoresOnLine.IndexSearch.Models.Constants.C_Settings_ElasticSearchUrl].Value);
            var settings = new ConnectionSettings(node);
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex("prod_companysurveyindex");
            ElasticClient CustomerSurvey = new ElasticClient(settings);
            int page = 0;
            Nest.ISearchResponse<CompanySurveyIndexModel> result = CustomerSurvey.Search<CompanySurveyIndexModel>(s => s
            .From(0)
                .TrackScores(true)
                .From(page)
                .Size(20)
                .Query(q => q.
                    Filtered(f => f
                    .Query(q1 => q1.MatchAll())
                    .Filter(f2 =>
                    {
                        QueryContainer qb = null;

                        //qb &= q.Term(m => m.CompanyName, "colsein");

                        if (true)
                        {
                            //qb &= q.Term(m => m.CityId, 1512);
                        }
                        if (true)
                        {
                            //qb &= q.Term(m => m.CountryId, 988);
                        }
                        if (true)
                        {
                            // qb &= q.Nested(n => n
                            //  .Path(p => p.oCustomerProviderIndexModel)
                            // .Query(fq => fq
                            //     .Match(match => match
                            //     .Field(field => field.oCustomerProviderIndexModel.First().StatusId)
                            //     .Query("902005")
                            //     )
                            //   )
                            //);
                        }

                        qb &= q.Nested(n => n
                             .Path(p => p.oSurveyIndexModel)
                            .Query(fq => fq
                                .Match(match => match
                                .Field(field => field.oSurveyIndexModel.First().CustomerPublicId)
                                .Query("DA5C572E")
                                )
                              )
                           );
                        return qb;
                    })
                    ))
                    .Aggregations
                    (agg => agg
                        .Nested("surveystatus_svg", x => x.
                            Path(p => p.oSurveyIndexModel).
                            Aggregations(aggs => aggs.Terms("status", term => term.Field(fi => fi.oSurveyIndexModel.First().SurveyStatusId)
                            )
                        )
                    )
                    .Nested("type_avg", x => x.
                            Path(p => p.oSurveyIndexModel).
                            Aggregations(aggs => aggs.Terms("type", term => term.Field(fi => fi.oSurveyIndexModel.First().SurveyTypeId)
                            )
                        ))                   
                ));       
        }
    }
}
