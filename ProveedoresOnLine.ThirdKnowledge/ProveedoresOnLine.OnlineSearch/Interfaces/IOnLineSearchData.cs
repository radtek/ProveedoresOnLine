using ProveedoresOnLine.OnlineSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Interfaces
{
    internal interface IOnLineSearchData 
    {
        TreeModel GetAnswerByTreeidAndQuestion(int TreeType, string Question);
    }
}
