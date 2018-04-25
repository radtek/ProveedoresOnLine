using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Register_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task Search()
        {
            await Register_Core.Controller.Register_Core.Search(1, "1031169089", "2276D600");
        }
    }
}
