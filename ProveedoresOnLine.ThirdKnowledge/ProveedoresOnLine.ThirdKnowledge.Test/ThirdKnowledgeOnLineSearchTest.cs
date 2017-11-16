using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;
using System.Collections.Generic;

namespace ProveedoresOnLine.ThirdKnowledge.Test
{
    [TestClass]
    public class ThirdKnowledgeOnLineSearchTest
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
        public async Task JudicialProcess()
        {
            await Controller.ThirdKnowledgeModule.JudicialProcessSearch(3, "","71984381");
            //return "";
        }

        [TestMethod]
        public void GetAnswerByTreeidAndQuestion()
        {
           OnlineSearch.Controller.SearchController.GetAnswerByTreeidAndQuestion(0,"");
        }

        [TestMethod]
        public async Task RegSearch()
        {

            List<Tuple<string, List<string>, List<string>>> resultado = null;
            resultado = await Controller.ThirdKnowledgeModule.RegisterSearch(1,"", "71984381");
            
        }

        [TestMethod]
        public async Task validateProcess()
        {
            string cedula = "72156155";
            List<Tuple<string, List<string>, List<string>>> resultado1, resultado2 = null;
            resultado1 = await Controller.ThirdKnowledgeModule.RegisterSearch(1, "", cedula);

            if (resultado1.Count > 0)
            {
                foreach (Tuple<string, List<string>, List<string>> item in resultado1)
                {
                    if (!string.IsNullOrEmpty(item.Item1))
                    {
                        resultado2 = await Controller.ThirdKnowledgeModule.PPSearch(1, item.Item1.Replace("\"","").Replace("\"", ""), cedula);
                    }
                }
            }
        }

    }
}
