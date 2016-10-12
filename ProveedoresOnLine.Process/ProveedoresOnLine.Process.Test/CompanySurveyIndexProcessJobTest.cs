using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProveedoresOnLine.Process.Test
{
    /// <summary>
    /// Summary description for CompanySurveyIndexProcessJobTest
    /// </summary>
    [TestClass]
    public class CompanySurveyIndexProcessJobTest
    {
        [TestMethod]
        public void CompanySurveyIndexProcessJob_Execute()
        {
            ProveedoresOnLine.Process.Implement.CompanySurveyIndexProcessJob SBJob = new Implement.CompanySurveyIndexProcessJob();
            SBJob.Execute(null);
        }
    }
}
