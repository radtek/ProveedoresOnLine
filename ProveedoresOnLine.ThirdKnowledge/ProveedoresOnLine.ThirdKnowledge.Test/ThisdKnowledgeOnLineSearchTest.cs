using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;

namespace ProveedoresOnLine.ThirdKnowledge.Test
{
    [TestClass]
    public class ThisdKnowledgeOnLineSearchTest
    {
        [TestMethod]
        public async Task Search()
        {
            await ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.OnLnieSearch(1,"");
            //return "";
        }

        [TestMethod]
        public void GetAnswerByTreeidAndQuestion()
        {
            ProveedoresOnLine.OnlineSearch.Controller.SearchController.GetAnswerByTreeidAndQuestion(0,"");
        }
    }
}
