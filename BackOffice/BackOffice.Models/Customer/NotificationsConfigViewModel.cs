using ProveedoresOnLine.Company.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Customer
{
    public class NotificationsConfigViewModel
    {
        public NotificationsConfigViewModel()
        {

        }

        public NotificationsConfigViewModel(CompanyNotificationModel oCompanyNotification)
        {
            NotificationConfigId = oCompanyNotification.NotificationConfigId.ToString();
            NotificationTitle = oCompanyNotification.NotificationName;

            MessageType = oCompanyNotification.CompanyNotificationInfo.
                Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.MessageType).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            MessageTypeId = oCompanyNotification.CompanyNotificationInfo.
                Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.MessageType).
                Select(y => y.CompanyNotificationInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            NotificationType = oCompanyNotification.CompanyNotificationInfo.
                Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.NotificationType).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            NotificationTypeId = oCompanyNotification.CompanyNotificationInfo.
                Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.NotificationType).
                Select(y => y.CompanyNotificationInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            DocumentType = oCompanyNotification.CompanyNotificationInfo.
              Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.DocumentType).
              Select(y => y.Value).
              DefaultIfEmpty(string.Empty).
              FirstOrDefault();
            DocumentTypeId = oCompanyNotification.CompanyNotificationInfo.
                Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.DocumentType).
                Select(y => y.CompanyNotificationInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            Document = oCompanyNotification.CompanyNotificationInfo.
                Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.Document).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            DocumentId = oCompanyNotification.CompanyNotificationInfo.
              Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.Document).
              Select(y => y.CompanyNotificationInfoId.ToString()).
              DefaultIfEmpty(string.Empty).
              FirstOrDefault();
            NotificationCritery = oCompanyNotification.CompanyNotificationInfo.
                Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.NotificationCritery).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            NotificationCriteryId = oCompanyNotification.CompanyNotificationInfo.
                Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.NotificationCritery).
                Select(y => y.CompanyNotificationInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            MessageBody = oCompanyNotification.CompanyNotificationInfo.
               Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.BodyMessage).
               Select(y => y.LargeValue).
               DefaultIfEmpty(string.Empty).
               FirstOrDefault();
            MessageBodyId = oCompanyNotification.CompanyNotificationInfo.
               Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.BodyMessage).
               Select(y => y.CompanyNotificationInfoId.ToString()).
               DefaultIfEmpty(string.Empty).
               FirstOrDefault();
            Responsable = oCompanyNotification.CompanyNotificationInfo.
               Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.Responsable).
               Select(y => y.LargeValue).
               DefaultIfEmpty(string.Empty).
               FirstOrDefault();
            Enable = oCompanyNotification.Enable;
            ResponsableId = oCompanyNotification.CompanyNotificationInfo.
               Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.Responsable).
               Select(y => y.CompanyNotificationInfoId.ToString()).
               DefaultIfEmpty(string.Empty).
               FirstOrDefault();
            Enable = oCompanyNotification.Enable;
            RuleType = oCompanyNotification.CompanyNotificationInfo.
               Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.RuleType).
               Select(y => y.Value).
               DefaultIfEmpty(string.Empty).
               FirstOrDefault();
            RuleTypeId = oCompanyNotification.CompanyNotificationInfo.
              Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.RuleType).
              Select(y => y.CompanyNotificationInfoId.ToString()).
              DefaultIfEmpty(string.Empty).
              FirstOrDefault();

            NotificationValue = oCompanyNotification.CompanyNotificationInfo.
               Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.NotificationValue).
               Select(y => y.Value).
               DefaultIfEmpty(string.Empty).
               FirstOrDefault();
            NotificationValueId = oCompanyNotification.CompanyNotificationInfo.
               Where(y => y.ConfigItemType.ItemId == (int)BackOffice.Models.General.enumNotificationInfoType.NotificationValue).
               Select(y => y.CompanyNotificationInfoId.ToString()).
               DefaultIfEmpty(string.Empty).
               FirstOrDefault();
        }

        public string NotificationConfigId { get; set; }
        public string NotificationTitle { get; set; }
        public string MessageTypeId { get; set; }
        public string MessageType { get; set; }
        public string  NotificationType { get; set; }
        public string NotificationTypeId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTypeId { get; set; }
        public string Document { get; set; }
        public string DocumentId { get; set; }
        public string NotificationCritery { get; set; }
        public string NotificationCriteryId { get; set; }
        public string MessageBody { get; set; }
        public string MessageBodyId { get; set; }
        public string Responsable { get; set; }
        public string ResponsableId { get; set; }

        public bool Enable { get; set; }

        public string RuleType { get; set; }
        public string RuleTypeId { get; set; }
        public string NotificationValue { get; set; }
        public string NotificationValueId { get; set; }
    }
}
