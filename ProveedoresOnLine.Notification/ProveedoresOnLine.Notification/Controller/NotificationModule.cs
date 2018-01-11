using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProveedoresOnLine.Notification.Models.Enumerations;

namespace ProveedoresOnLine.Notification.Controller
{
    public static class NotificationModule
    {
        //public static void

        /// <summary>
        /// Functio to mange all notifications
        /// </summary>
        /// <param name="CompanyPublicId">Customer Public Id </param>
        public static void StartProcess()
        {
            string CurrentProviderPublicId = "";
            try
            {
                LogFile("Process Start :::::: " + DateTime.Now);
                List<ProveedoresOnLine.Company.Models.Company.CompanyNotificationModel> oNotificatiosToManage = new List<Company.Models.Company.CompanyNotificationModel>();
                oNotificatiosToManage = ProveedoresOnLine.Company.Controller.Company.NotificationConfigGetAll();

                
                //Divide by rules Type
                if (oNotificatiosToManage != null && oNotificatiosToManage.Count > 0)
                {
                    LogFile("Process Notifications Count :::::: " + oNotificatiosToManage.Count);
                    //Get Notification Types
                    oNotificatiosToManage.All(n =>
                    {
                        CurrentProviderPublicId = n.CompanyPublicId;
                        if (n.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationType).Select(x => x.Value).FirstOrDefault() == ((int)enumNotificationType.Document).ToString())
                        {
                            //Call DocumentPRovider Function
                            ProviderCore(n.CompanyPublicId, n);
                        }
                        else if (n.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationType).Select(x => x.Value).FirstOrDefault() == ((int)enumNotificationType.RestrictiveList_POL).ToString())
                        {
                            //Call DocumentPRovider Function
                        }
                        else if (n.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationType).Select(x => x.Value).FirstOrDefault() == ((int)enumNotificationType.RestictiveList_TK).ToString())
                        {
                            //Call DocumentPRovider Function
                        }
                        else if (n.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationType).Select(x => x.Value).FirstOrDefault() == ((int)enumNotificationType.ProviderStatus).ToString())
                        {
                            //Call DocumentPRovider Function
                        }
                        else if (n.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationType).Select(x => x.Value).FirstOrDefault() == ((int)enumNotificationType.ValidityProvider).ToString())
                        {
                            //Call DocumentPRovider Function
                        }
                        LogFile("Process Notifications Process End :::::: " + DateTime.Now);
                        return true;
                    });
                }
                else
                {
                    LogFile("No Notifications to Send :::::: " + DateTime.Now);
                }

            }
            catch (Exception ex )
            {
                LogFile("Process Notifications Error!!! :::::: " + ex.Message + ":::CompanyPublicId:::" + CurrentProviderPublicId);
                throw;
            }
        }

        public static bool ProviderCore(string CompanyPublicId, ProveedoresOnLine.Company.Models.Company.CompanyNotificationModel NotificationConfigModel)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType< Notification.Core.NotificationProvider_Core>().As<Interfaces.INotificationCore>();

            var container = builder.Build();
            return container.Resolve<Interfaces.INotificationCore>().ManageNotification(CompanyPublicId, NotificationConfigModel);
        }

        #region Log File

        public static void LogFile(string LogMessage)
        {
            try
            {
                //get file Log
                string LogFile = AppDomain.CurrentDomain.BaseDirectory.Trim().TrimEnd(new char[] { '\\' }) + "\\" +
                    System.Configuration.ConfigurationManager.AppSettings
                    [ProveedoresOnLine.Notification.Models.Constants.C_AppSettings_LogFile].Trim().TrimEnd(new char[] { '\\' });

                if (!System.IO.Directory.Exists(LogFile))
                    System.IO.Directory.CreateDirectory(LogFile);

                LogFile += "\\" + "Log_NotificationProcess_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(LogFile))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "::" + LogMessage);
                    sw.Close();
                }
            }
            catch { }
        }
        #endregion
    }
}
