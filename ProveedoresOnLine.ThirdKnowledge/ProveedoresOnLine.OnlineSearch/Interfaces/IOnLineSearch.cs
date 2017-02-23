using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Interfaces
{
     public interface IOnLineSearch 
    {
        void Search(int IdentificationType, string Name, string IdentificationNumber);
    }
}
