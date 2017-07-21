using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Register_Core.Models.Enumerations;

namespace Register_Core.Models
{
    public class TransactionModel
    {
        public string Token { get; set; }

        public string Message { get; set; }

        public string Query { get; set; }

        public enumServiceType ServiceType { get; set; }

        public bool IsSuccess { get; set; }
    }
}
