using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProveedoresOnLine.ThirdKnowledge.Test
{
    [TestClass]
    public class ThisdKnowledgeOnLineSearchTest
    {
        [TestMethod]
        public void Search()
        {
            ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.OnLnieSearch();
        }
    }
}
