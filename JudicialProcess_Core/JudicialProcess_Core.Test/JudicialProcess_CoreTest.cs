using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace JudicialProcess_Core.Test
{
    [TestClass]
    public class JudicialProcess_CoreTest
    {
        [TestMethod]
        public async Task Search()
        {
            await Controller.JudicialProcess_Core.Search(3,"", "71984381", "");
        }
    }
}
