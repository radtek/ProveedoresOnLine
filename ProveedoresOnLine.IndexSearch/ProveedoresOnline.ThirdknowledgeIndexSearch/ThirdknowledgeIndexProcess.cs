using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnline.ThirdknowledgeIndexSearch
{
    public class ThirdknowledgeIndexProcess
    {
        public static void StartProcess()
        {
            ProveedoresOnLine.IndexSearch.Controller.IndexSearch.GetThirdKnowlegdeIndex();
        }
    }
}
