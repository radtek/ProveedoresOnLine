using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModule.Client.Models
{
    public class NotificationInfoModel
    {
        public int NotificationInfoId { get; set; }

        public int NotificationId { get; set; }

        public int NotificationInfoType { get; set; }

        public string Value { get; set; }

        public string LargeValue { get; set; }

        public bool Enable { get; set; }

        public DateTime LastModify { get; set; }

        public DateTime CreateDate { get; set; }

    }
}