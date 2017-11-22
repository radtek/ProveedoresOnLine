﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModule.Client.Models
{
    public class NotificationModel
    {
        public int NotificationId { get; set; }

        public int NotificationType { get; set; }

        public string Label { get; set; }

        public string CompanyPublicId { get; set; }       
        
        public string User { get; set; }

        public int State { get; set; }

        public bool Enable { get; set; }

        public DateTime LastModify { get; set; }

        public DateTime CreateDate { get; set; }

        public List<NotificationInfoModel> ListNotificationInfo { get; set; }

    }
}
