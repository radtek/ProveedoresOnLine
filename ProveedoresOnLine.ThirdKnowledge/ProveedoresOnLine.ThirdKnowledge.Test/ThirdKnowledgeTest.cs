﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ProveedoresOnLine.ThirdKnowledge.Test
{
    [TestClass]
    public class ThirdKnowledgeTest
    {
        [TestMethod]
        public async Task SimpleRequest()
        {
            TDQueryModel oQuery = new TDQueryModel();
            await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.SimpleRequest("",2, "800250119", oQuery);
        }

        [TestMethod]
        public void PlanUpsert()
        {
            PlanModel oToUpsert = new PlanModel();
            oToUpsert.InitDate = Convert.ToDateTime("2014/01/01");
            oToUpsert.EndDate = Convert.ToDateTime("2015/01/01");
            oToUpsert.CompanyPublicId = "AAAAA";
            oToUpsert.CreateDate = DateTime.Now;
            oToUpsert.DaysByPeriod = 30;
            oToUpsert.Enable = true;
            oToUpsert.LastModify = DateTime.Now;
            oToUpsert.QueriesByPeriod = 100;
            oToUpsert.Status = new TDCatalogModel()
                {
                    ItemId = 101001
                };
            PlanModel oReturn = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.PlanUpsert(oToUpsert);

            Assert.IsNotNull(oReturn);
        }

        [TestMethod]
        public void GetAllPlanByCustomer()
        {
            List<PlanModel> oReturn = new List<PlanModel>();
            oReturn = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetAllPlanByCustomer("DA5C572E", true);
            Assert.IsNull(oReturn);
        }

        [TestMethod]
        public void GetPeriodByPlanPublicId()
        {
            List<PeriodModel> oPeriodModel = new List<PeriodModel>();

            oPeriodModel = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetPeriodByPlanPublicId("DA5C572E", true);

            Assert.IsNull(oPeriodModel);
        }

        [TestMethod]
        public void ThirdKnoledgeSearch()
        {
            int TotalRows = 0;

            List<TDQueryModel> oReturn = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.ThirdKnowledgeSearch
                ("DA5C572E", null,"gmail.com", null, null, 0, 1, null, null, out TotalRows);

            Assert.AreEqual(true, oReturn != null);
        }

        [TestMethod]
        public void ThirdKnowledgeDetailByPublicId()
        {
            int TotalRows = 0;
            List<TDQueryModel> oReturn = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.ThirdKnowledgeSearchByPublicId("A837B1AE",0,0,out TotalRows);

            Assert.AreEqual(true, oReturn != null);
        }
              

        [TestMethod]
        public void GetQueriesInProgress()
        {
            List<TDQueryModel> oReturn = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetQueriesInProgress();
            Assert.AreEqual(true, oReturn != null);
        }

        [TestMethod]
        public void GetUserByCompanyPublicId ()
        {
            List<string> oReturn = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetUsersBycompanyPublicId("DA5C572E");
            Assert.AreEqual(true, oReturn != null);
        }
    }
}
