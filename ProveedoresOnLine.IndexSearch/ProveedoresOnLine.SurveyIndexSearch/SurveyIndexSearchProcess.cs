using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.SurveyIndexSearch
{
    public class SurveyIndexSearchProcess
    {
        public static void StartProcess()
        {
            ProveedoresOnLine.IndexSearch.Controller.IndexSearch.SurveyIndexationFunction();
        }
    }
}
