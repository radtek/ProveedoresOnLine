using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
