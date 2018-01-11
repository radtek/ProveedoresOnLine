using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.CompanyIndexSearch
{
    public class CompanyIndexSearchProcess
    {
        public static void CompanyIndexationFunction()
        {
            ProveedoresOnLine.IndexSearch.Controller.IndexSearch.CompanyIndexationFunction();
        }
        public static void CompanyCustomerIndexationFunction()
        {
            ProveedoresOnLine.IndexSearch.Controller.IndexSearch.CompanyCustomerIndexationFunction();
        }
        public static void CustomerProviderIdexationFunction()
        {
            ProveedoresOnLine.IndexSearch.Controller.IndexSearch.CustomerProviderIdexationFunction();
        }

        public static void CalificationIdexationFunction()
        {
            ProveedoresOnLine.IndexSearch.Controller.IndexSearch.CalificationIdexationFunction();
        }

        public static void CustomFiltersIdexationFunction()
        {
            ProveedoresOnLine.IndexSearch.Controller.IndexSearch.CustomFiltersIdexationFunction();
        }

    }
}