using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess.Models
{
    public class RedebanLogModel
    {
        public int RedebanProcessLogId { get; set; }

        public int ProviderId { get; set; }

        public string CompanyPublicId { get; set; }

        public string ProcessName { get; set; }

        public string FileName { get; set; }

        public bool SendStatus { get; set; }

        public bool IsSucces { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastModify { get; set; }

        public bool Enable { get; set; }
    }
}
