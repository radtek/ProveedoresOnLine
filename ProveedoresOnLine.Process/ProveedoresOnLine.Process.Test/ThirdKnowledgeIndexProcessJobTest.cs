using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProveedoresOnLine.Process.Test
{
    [TestClass]
    public class ThirdKnowledgeIndexProcessJobTest
    {
        [TestMethod]
        public void ThirdKnowledgeIndexProcessJob_Execute()
        {
            ProveedoresOnLine.Process.Implement.ThirdknowledgeIndexProcessJob SBJOb = new Implement.ThirdknowledgeIndexProcessJob();
            SBJOb.Execute(null);
        }
    }
}
