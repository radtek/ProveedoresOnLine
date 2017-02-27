using ProveedoresOnLine.OnlineSearch.DAL.Controller;
using ProveedoresOnLine.OnlineSearch.Interfaces;
using ProveedoresOnLine.OnlineSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Controller
{
    public class SearchController
    {
        public static ProveedoresOnLine.OnlineSearch.Interfaces.IOnLineSearch objSearch = null;
                
        public  SearchController(ProveedoresOnLine.OnlineSearch.Interfaces.IOnLineSearch SearchImplement)
        {
            objSearch = SearchImplement;
        }

        public void Search(int IdentificationType, string Name, string IdentificationNumber)
        {
            objSearch.SearchProc(0, "", "");
        }

        public static TreeModel GetAnswerByTreeidAndQuestion(int TreeType, string Question)
        {
            return OnLineSearchDataController.Instance.GetAnswerByTreeidAndQuestion(TreeType, Question);
        }
    }
}
