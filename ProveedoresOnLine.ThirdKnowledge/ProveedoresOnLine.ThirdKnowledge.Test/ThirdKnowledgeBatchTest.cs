using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProveedoresOnLine.ThirdKnowledge.Test
{
    [TestClass]
    public class ThirdKnowledgeBatchTest
    {
        [TestMethod]
        public async Task  StartProcess()
        {
            await ProveedoresOnLine.ThirdKnowledgeBatch.ThirdKnowledgeFTPProcess.StartProcess();
        }
    }
}
