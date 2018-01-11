using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using ProveedoresOnLine.IndexSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.IndexSearch.Test
{
    [TestClass]
    public class ThirdknowledgeIndexTest
    {
        [TestMethod]
        public void IndexThirdKnowledge()
        {
            ProveedoresOnline.ThirdknowledgeIndexSearch.ThirdknowledgeIndexProcess.StartProcess();           
        }
        
        [TestMethod]
        public void QueryModelIndeAll()
        {
            Controller.IndexSearch.QueryModelIndeAll();
        }

        [TestMethod]
        public void QueryModelIndexSearch()
        {
             Controller.IndexSearch.QueryModelIndexSearch();
        }

    }
}
