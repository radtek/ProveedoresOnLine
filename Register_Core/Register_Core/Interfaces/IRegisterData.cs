using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Register_Core.Interfaces
{
    interface IRegisterData
    {
        bool IsAuthorized(string Token);

        void SaveTransaction(string Token, string Message, string Query, int ServiceType, bool IsSuccess);
    }
}
