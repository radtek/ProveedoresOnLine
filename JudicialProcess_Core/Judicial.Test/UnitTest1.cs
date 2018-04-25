using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Judicial.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task Search()
        {
            await JudicialProcess_Core.Controller.JudicialProcess_Core.Search(3, "", "71984381", "");
        }
    }
}
