﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess.Models
{
    public class Constants
    {
        #region InternalSettings

        public const string C_SettingsModuleName = "IntegrationPlatformREDEBANProcess";

        #endregion

        public const string C_AppSettings_LogFile = "LogFile";
      
        public const string C_REDEBAN_ProviderPublicId = "REDEBAN_ProviderPublicId";

        public const string C_Settings_File_TempDirectory = "REDEBAN_File_TempDirectory";

        public const string C_Settings_File_ExcelDirectory = "REDEBAN_File_ExcelDirectory";

        public const string C_Settings_REDEBAN_Mail = "REDEBAN_Mail";

        #region Notifications

        public const string N_RedebanReportMessage = "N_RedebanReportMessage";

        #endregion
    }
}
