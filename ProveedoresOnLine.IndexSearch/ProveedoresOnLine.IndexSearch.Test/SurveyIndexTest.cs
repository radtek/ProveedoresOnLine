using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProveedoresOnLine.IndexSearch.Test
{
    /// <summary>
    /// Summary description for SurveyIndexTest
    /// </summary>
    [TestClass]
    public class SurveyIndexTest
    {
        [TestMethod]
        public void StartProcess()
        {
            ProveedoresOnLine.SurveyIndexSearch.SurveyIndexSearchProcess.StartProcess();
        }
    }
}
