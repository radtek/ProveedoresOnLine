using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Register_Core.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task Search()
        {
            await Controller.Register_Core.Search(1, "71984381", "2276D600");
        }
    }
}
