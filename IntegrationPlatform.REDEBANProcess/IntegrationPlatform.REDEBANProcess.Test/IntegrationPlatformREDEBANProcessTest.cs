using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationPlatform.REDEBANProcess.Test
{
    [TestClass]
    public class IntegrationPlatformREDEBANProcessTest
    {
        [TestMethod]
        public void GetAllProviders()
        {
            var oReturn = REDEBANProcess.IntegrationPlatformREDEBANProcess.GetAllProviders();

            Assert.AreEqual(true, oReturn.Count > 0);
        }

        [TestMethod]
        public void GetProviderInfo()
        {
            var oReturn = REDEBANProcess.IntegrationPlatformREDEBANProcess.GetProviderInfo("26D388E3", "9B15FEF0");

            Assert.AreEqual(true, oReturn != null);
        }
    }
}
