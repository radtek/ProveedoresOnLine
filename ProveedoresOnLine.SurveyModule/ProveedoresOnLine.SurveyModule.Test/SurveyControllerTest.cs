using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ProveedoresOnLine.Company.Models.Util;

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
            ProveedoresOnLine.SurveyModule.Models.SurveyModel oReturn =
                ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGetByUser("9D766F92", "david.moncayo@publicar.com");

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
            List<ProveedoresOnLine.SurveyModule.Models.SurverReportModel.SurveyReportModelTable1> oReturn = ProveedoresOnLine.SurveyModule.Controller.SurveyModule.SurveyGeneralReport("1EA5A78A");

            Assert.AreEqual(true, oReturn != null && oReturn.Count >0);
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
