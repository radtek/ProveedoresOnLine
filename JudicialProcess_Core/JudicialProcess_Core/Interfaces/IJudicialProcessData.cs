using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudicialProcess_Core.Interfaces
{
    internal interface IJudicialProcessData
    {
        bool IsAuthorized(string Token);

        void SaveTransaction(string Token, string Message, string Query, int ServiceType, bool IsSuccess);
    }
}
