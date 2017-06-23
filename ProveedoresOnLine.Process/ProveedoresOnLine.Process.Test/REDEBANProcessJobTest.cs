using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Process.Test
{
    [TestClass]
    public class REDEBANProcessJobTest
    {
        [TestMethod]
        public void RedebanProcess()
        {
            ProveedoresOnLine.Process.Implement.REDEBANProcess SBJob = new Implement.REDEBANProcess();
            SBJob.Execute(null);
        }
    }
}
