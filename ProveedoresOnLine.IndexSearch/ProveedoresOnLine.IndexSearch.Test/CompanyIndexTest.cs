﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProveedoresOnLine.IndexSearch.Test
{
    /// <summary>
    /// Summary description for CompanyIndex
    /// </summary>
    [TestClass]
    public class CompanyIndexTest
    {
        [TestMethod]
        public void CompanyIndexationFunction()
        {
            ProveedoresOnLine.CompanyIndexSearch.CompanyIndexSearchProcess.CompanyIndexationFunction();
        }

        [TestMethod]
        public void CompanyCustomerIndexationFunction()
        {
            ProveedoresOnLine.CompanyIndexSearch.CompanyIndexSearchProcess.CompanyCustomerIndexationFunction();
        }
    }
}
