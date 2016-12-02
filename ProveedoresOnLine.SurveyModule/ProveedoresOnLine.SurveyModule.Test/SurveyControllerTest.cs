using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ProveedoresOnLine.Company.Models.Util;
using System.Linq;

namespace ProveedoresOnLine.SurveyModule.Test
{
    [TestClass]
    public class SurveyControllerTest
    {
        [TestMethod]
        public void SurveyRecalculate()
        {
            ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyRecalculate("1C4FB681", 31, "johann.martinez@publicar.com");

            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void GetSurveyByResponsable()
        {
            List<ProveedoresOnLine.Company.Models.Util.GenericChartsModelInfo> oreturn =
            ProveedoresOnLine.SurveyModule.Controller.SurveyModule.GetSurveyByResponsable("1EA5A78A", "johann.martinez@publicar.com", DateTime.Now);

            Assert.IsTrue(oreturn.Count > 0);
        }

        [TestMethod]
        public void GetSurveyByEvaluator()
        {
            List<ProveedoresOnLine.Company.Models.Util.GenericChartsModelInfo> oreturn =
            ProveedoresOnLine.SurveyModule.Controller.SurveyModule.GetSurveyByEvaluator("DA5C572E", "");

            Assert.IsTrue(oreturn.Count > 0);
        }


        [TestMethod]
        public void GetSurveyByUser()
        {
            Models.SurveyModel oReturn = Controller.SurveyModule.SurveyGetByUser("13B48042", "spenuela@alpina.com.co");
            List<GenericItemModel> Areas = new List<GenericItemModel>();
            List<GenericItemModel> Questions = new List<GenericItemModel>();
            List<GenericItemModel> Answers = new List<GenericItemModel>();

            Areas = oReturn.RelatedSurveyConfig.RelatedSurveyConfigItem.Where(x => x.ItemType.ItemId == 1202001 && x.ParentItem == null).Select(x => x).ToList();
            Areas.All(ar =>
            {
                Questions.AddRange(oReturn.RelatedSurveyConfig.RelatedSurveyConfigItem.Where(x => x.ItemType.ItemId == 1202002 && x.ParentItem != null && x.ParentItem.ItemId == ar.ItemId).Select(x => x).ToList());
                return true;
            });
            Questions.All(q =>
            {
                Answers.Add(oReturn.RelatedSurveyItem.Where(x => x != null && x.RelatedSurveyConfigItem != null && x.RelatedSurveyConfigItem.ItemId == q.ItemId).Select(x => new GenericItemModel()
                                                        {
                                                            ItemId = x.ItemId,
                                                            ItemInfo = x.ItemInfo,
                                                            CreateDate = x.CreateDate,
                                                            ItemName = oReturn.RelatedSurveyConfig.RelatedSurveyConfigItem.
                                                                        Where(inf => inf.ItemId == int.Parse(x.ItemInfo.Where(subinf => subinf.ItemInfoType.ItemId == 1205003).
                                                                                Select(subinf => subinf.Value).FirstOrDefault())).Select(inf => inf.ItemName).FirstOrDefault(),
                                                            ParentItem = new GenericItemModel()
                                                                {
                                                                    ItemId = x.RelatedSurveyConfigItem.ItemId
                                                                }
                                                        }).FirstOrDefault());
                return true;
            });

            List<Tuple<GenericItemModel, string, GenericItemModel, GenericItemModel>> objtToBuildReport = new List<Tuple<GenericItemModel, string, GenericItemModel, GenericItemModel>>();

            if (Answers != null)
            {
                Answers.All(asw =>
                {
                    if (asw != null)
                    {
                        GenericItemModel oQuestionsToAdd = Questions.Where(qs => qs.ItemId == asw.ParentItem.ItemId).Select(qs => qs).FirstOrDefault();
                        GenericItemModel oAreasToAdd = Areas.Where(a => a.ItemId == oQuestionsToAdd.ParentItem.ItemId).Select(a => a).FirstOrDefault();

                        objtToBuildReport.Add(new Tuple<GenericItemModel, string, GenericItemModel, GenericItemModel>
                            (oAreasToAdd, "spenuela@alpina.com.co", oQuestionsToAdd, asw));
                    }
                    
                    return true;
                });               
            }
            
            Assert.IsTrue(oReturn != null);
        }

        [TestMethod]
        public void SurveyConfigGetById()
        {
            ProveedoresOnLine.SurveyModule.Models.SurveyConfigModel oReturn = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyConfigGetById(44);

            Assert.AreEqual(true, oReturn != null && oReturn.ItemId > 0);
        }

        [TestMethod]
        public void  SurveyGeneralReport()
        {
            List<SurveyModule.Models.SurveyModel> oReturn = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.ReportAllSurvey("DD8AC84A", "1EA5A78A");
            oReturn.GroupBy(p => p.SurveyId);
            Assert.AreEqual(true, oReturn != null);
        }
        [TestMethod]
        public void SurveyGetAllByCustomer()
        {
            List<SurveyModule.Models.SurveyModel> oReturn = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetAllByCustomer("1EA5A78A");

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }
        #region Util

        [TestMethod]
        public void GetSurveyName()
        {
            List<CatalogModel> oReturn = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.CatalogGetSurveyName();

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }

        [TestMethod]
        public void GetSurveyById()
        {
            List<ProveedoresOnLine.SurveyModule.Models.SurveyConfigModel> oReturn = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.MP_SurveyConfigSearch("DA5C572E", "44", 1, 15000);

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }
        #endregion
    }
}
