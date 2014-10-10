﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionManager.Models.Auth
{
    public class UserInfo
    {
        public int UserInfoId { get; set; }
        public enumUserInfoType? InfoType { get; set; }
        public string Value { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
