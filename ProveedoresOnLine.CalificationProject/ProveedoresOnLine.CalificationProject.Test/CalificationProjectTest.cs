﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProveedoresOnLine.CalificationProject.Models.CalificationProject;
using System.Collections.Generic;

namespace ProveedoresOnLine.CalificationProject.Test
{
    [TestClass]
    public class CalificationProjectTest
    {
        [TestMethod]
        public void CalificationProjectConfigGetByCompanyId()
        {
            List<CalificationProjectConfigModel> oReturn = new List<CalificationProjectConfigModel>();
            oReturn = ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigGetByCompanyId
                ("1B40C887",
                true);

            Assert.AreEqual(true, oReturn.Count > 0);
        }

        [TestMethod]
        public void CalificationProjectConfigItemUpsert()
        {
            ConfigItemModel oReturn = new ConfigItemModel() { 
                CalificationProjectConfigItemId = 0,
                CalificationProjectConfigId = 1,
                CalificationProjectConfigItemName = null,
                CalificationProjectConfigItemType = new Company.Models.Util.CatalogModel()
                {
                    ItemId = 2003001, //Módulo Legal
                },
                Enable = true,
            };

            oReturn = ProveedoresOnLine.CalificationProject.Controller.CalificationProject.CalificationProjectConfigItemUpsert
                (oReturn);

            Assert.AreEqual(true, oReturn.CalificationProjectConfigItemId > 0);
        }
    }
}
