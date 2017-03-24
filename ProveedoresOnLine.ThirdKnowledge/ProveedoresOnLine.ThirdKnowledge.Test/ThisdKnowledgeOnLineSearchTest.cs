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
        public async Task ProcSearch()
        {
            await Controller.ThirdKnowledgeModule.OnLnieSearch(1, "71984381");
            //return "";
        }

        [TestMethod]
        public async Task PanamaPSearch()
        {
            await Controller.ThirdKnowledgeModule.PPSearch(1, "OSCAR EDUARDO PALADINES MEJIA", "");
            //return "";
        }

        [TestMethod]
        public void GetAnswerByTreeidAndQuestion()
        {
           OnlineSearch.Controller.SearchController.GetAnswerByTreeidAndQuestion(0,"");
        }
    }
}
