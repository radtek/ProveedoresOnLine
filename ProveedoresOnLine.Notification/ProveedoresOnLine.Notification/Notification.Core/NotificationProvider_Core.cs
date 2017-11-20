using ProveedoresOnLine.Notification.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProveedoresOnLine.Notification.Models;
using static ProveedoresOnLine.Notification.Models.Enumerations;
using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.Notification.Controller;
using System.Globalization;

namespace ProveedoresOnLine.Notification.Notification.Core
{
    internal class NotificationProvider_Core : INotificationCore
    {
        public bool ManageNotification(string CompanyPublicId, ProveedoresOnLine.Company.Models.Company.CompanyNotificationModel NotificationConfigModel)
        {

            //Get All Providers by this Customer (CompanyPublicId)
            List<CompanyModel> oProviders = CompanyProvider.Controller.CompanyProvider.GetAllProvidersByCustomerPublicId(CompanyPublicId);
            CultureInfo culture = CultureInfo.CreateSpecificCulture("es-ES");
            if (NotificationConfigModel.CompanyNotificationInfo != null)
            {
                if (NotificationConfigModel.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationType).Select(x => x.Value).FirstOrDefault() == ((int)enumNotificationType.Document).ToString())
                {
                    //Wich Document To Valid
                    int DocumentType = NotificationConfigModel.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.DocumentType).Select(x => int.Parse(x.Value)).FirstOrDefault();
                    string DocumentName = NotificationConfigModel.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.Document).Select(x => x.Value).FirstOrDefault();
                    //Vigency, Priority, Status
                    int Critery = NotificationConfigModel.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationCritery).Select(x => int.Parse(x.Value)).FirstOrDefault();
                    //<=, <,> ....
                    int RuleType = NotificationConfigModel.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.RuleType).Select(x => int.Parse(x.Value)).FirstOrDefault();
                    //Days, Less 30, ...
                    int RuleValue = NotificationConfigModel.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationValue).Select(x => int.Parse(x.Value)).FirstOrDefault();
                    // Indicatator if is necessary to build a Message Object
                    bool BuildMsgObject = false;

                    #region General Info
                    if (DocumentType == (int)enumDocumentType.GeneralInfo)
                    {
                        // In Chaimber Of Commerce case
                        if (DocumentName.ToLower() == "cámara de comercio")
                        {
                            oProviders.All(p =>
                            {
                                BuildMsgObject = false;
                                //Get LegalInfo
                                List<Company.Models.Util.GenericItemModel> oLegalInfo = CompanyProvider.Controller.CompanyProvider.LegalGetBasicInfo(p.CompanyPublicId, (int)enumGeneralInfoType.ChaimberOfComerceInfo, true);

                                DateTime ChaimberOfComerceDateToValidate = new DateTime();
                                if (oLegalInfo != null)
                                    ChaimberOfComerceDateToValidate = oLegalInfo.FirstOrDefault().ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.ChaimberOfComerceDateToValidate).Select(x => DateTime.Parse(x.Value, culture)).FirstOrDefault();

                                #region Validate the vigency just for the Documents of Provider
                                //Valid the Rules
                                switch (RuleType)
                                {
                                    #region Evalue Rules
                                    case ((int)enumRuleType.LessThan):
                                        if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                        {
                                            ChaimberOfComerceDateToValidate = ChaimberOfComerceDateToValidate.AddMonths(-1);
                                            if (ChaimberOfComerceDateToValidate.Date == DateTime.Now.Date)
                                                BuildMsgObject = true;
                                        }
                                        if (RuleValue == (int)enumVigencyType.SixtyDays)
                                        {
                                            ChaimberOfComerceDateToValidate = ChaimberOfComerceDateToValidate.AddMonths(-2);
                                            if (ChaimberOfComerceDateToValidate.Date == DateTime.Now.Date)
                                                BuildMsgObject = true;
                                        }
                                        if (RuleValue == (int)enumVigencyType.NinetyDays)
                                        {
                                            ChaimberOfComerceDateToValidate = ChaimberOfComerceDateToValidate.AddMonths(-3);
                                            if (ChaimberOfComerceDateToValidate.Date == DateTime.Now.Date)
                                                BuildMsgObject = true;
                                        }
                                        if (RuleValue == (int)enumVigencyType.OneYear)
                                        {
                                            ChaimberOfComerceDateToValidate = ChaimberOfComerceDateToValidate.AddYears(-1);
                                            if (ChaimberOfComerceDateToValidate.Date == DateTime.Now.Date)
                                                BuildMsgObject = true;
                                        }
                                        //Send notification
                                        if (BuildMsgObject)
                                            this.SendNotification(NotificationConfigModel, p, DocumentType);
                                        break;
                                    case ((int)enumRuleType.LessOrEqualThan):
                                        if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                        {
                                            ChaimberOfComerceDateToValidate = ChaimberOfComerceDateToValidate.AddDays(-31);
                                            if (ChaimberOfComerceDateToValidate.Date == DateTime.Now.Date)
                                                BuildMsgObject = true;
                                        }
                                        if (RuleValue == (int)enumVigencyType.SixtyDays)
                                        {
                                            ChaimberOfComerceDateToValidate = ChaimberOfComerceDateToValidate.AddDays(-61);
                                            if (ChaimberOfComerceDateToValidate.Date == DateTime.Now.Date)
                                                BuildMsgObject = true;
                                        }
                                        if (RuleValue == (int)enumVigencyType.NinetyDays)
                                        {
                                            ChaimberOfComerceDateToValidate = ChaimberOfComerceDateToValidate.AddDays(-91);
                                            if (ChaimberOfComerceDateToValidate.Date == DateTime.Now.Date)
                                                BuildMsgObject = true;
                                        }
                                        if (RuleValue == (int)enumVigencyType.OneYear)
                                        {
                                            ChaimberOfComerceDateToValidate = ChaimberOfComerceDateToValidate.AddYears(-1);
                                            if (ChaimberOfComerceDateToValidate.Date == DateTime.Now.Date)
                                                BuildMsgObject = true;
                                        }

                                        //Send notification
                                        if (BuildMsgObject)
                                            this.SendNotification(NotificationConfigModel, p, DocumentType);

                                        break;
                                        #endregion

                                }
                                #endregion

                                return true;
                            });
                        }
                    }
                    #endregion
                    #region HSEQ Info
                    if (DocumentType == (int)enumDocumentType.HSEQ)
                    {
                        oProviders.All(p =>
                        {
                            BuildMsgObject = false;
                            //Get LegalInfo
                            List<Company.Models.Util.GenericItemModel> oHSEQInfo = CompanyProvider.Controller.CompanyProvider.CertficationGetBasicInfo(p.CompanyPublicId, (int)enumGeneralInfoType.HSEQCertifications, true);

                            DateTime HSQQDateToValidate = new DateTime();
                            if (oHSEQInfo != null)
                                HSQQDateToValidate = oHSEQInfo.FirstOrDefault().ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.HSEQVigency).Select(x => DateTime.Parse(x.Value, culture)).FirstOrDefault();

                            #region Validate the vigency just for the Documents of Provider
                                //Valid the Rules
                            switch (RuleType)
                            {
                                #region Evalue Rules
                                case ((int)enumRuleType.LessThan):
                                    if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                    {
                                        HSQQDateToValidate = HSQQDateToValidate.AddDays(-30);
                                        if (HSQQDateToValidate.Date == DateTime.Now.Date)
                                            BuildMsgObject = true;
                                    }
                                    if (RuleValue == (int)enumVigencyType.SixtyDays)
                                    {
                                        HSQQDateToValidate = HSQQDateToValidate.AddDays(-60);
                                        if (HSQQDateToValidate.Date == DateTime.Now.Date)
                                            BuildMsgObject = true;
                                    }
                                    if (RuleValue == (int)enumVigencyType.NinetyDays)
                                    {
                                        HSQQDateToValidate = HSQQDateToValidate.AddDays(-90);
                                        if (HSQQDateToValidate.Date == DateTime.Now.Date)
                                            BuildMsgObject = true;
                                    }
                                    if (RuleValue == (int)enumVigencyType.OneYear)
                                    {
                                        HSQQDateToValidate = HSQQDateToValidate.AddYears(-1);
                                        if (HSQQDateToValidate.Date == DateTime.Now.Date)
                                            BuildMsgObject = true;
                                    }
                                    //Send notification
                                    if (BuildMsgObject)
                                        this.SendNotification(NotificationConfigModel, p, DocumentType);
                                    break;
                                case ((int)enumRuleType.LessOrEqualThan):
                                    if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                    {
                                        HSQQDateToValidate = HSQQDateToValidate.AddDays(-31);
                                        if (HSQQDateToValidate.Date == DateTime.Now.Date)
                                            BuildMsgObject = true;
                                    }
                                    if (RuleValue == (int)enumVigencyType.SixtyDays)
                                    {
                                        HSQQDateToValidate = HSQQDateToValidate.AddDays(-61);
                                        if (HSQQDateToValidate.Date == DateTime.Now.Date)
                                            BuildMsgObject = true;
                                    }
                                    if (RuleValue == (int)enumVigencyType.NinetyDays)
                                    {
                                        HSQQDateToValidate = HSQQDateToValidate.AddDays(-91);
                                        if (HSQQDateToValidate.Date == DateTime.Now.Date)
                                            BuildMsgObject = true;
                                    }
                                    if (RuleValue == (int)enumVigencyType.OneYear)
                                    {
                                        HSQQDateToValidate = HSQQDateToValidate.AddYears(-1);
                                        if (HSQQDateToValidate.Date == DateTime.Now.Date)
                                            BuildMsgObject = true;
                                    }
                                    //Send notification
                                    if (BuildMsgObject)
                                        this.SendNotification(NotificationConfigModel, p, DocumentType);
                                    break;
                                    #endregion
                            }
                            #endregion

                            return true;
                        });
                    }
                    #endregion
                    #region Additional Info
                    if (DocumentType == (int)enumDocumentType.AdditionalInfo)
                    {
                        oProviders.All(p =>
                        {
                            BuildMsgObject = false;
                            //Get LegalInfo
                            List<Company.Models.Util.GenericItemModel> oDocumentInfo = CompanyProvider.Controller.CompanyProvider.AditionalDocumentGetByType(p.CompanyPublicId, (int)enumGeneralInfoType.AdditionalDocument, true);

                            DateTime oDateToValid = new DateTime();
                            if (oDocumentInfo != null)
                            {
                                Company.Models.Util.GenericItemModel oDocumentToValid =
                                 oDocumentInfo.Where(y => y.ItemName.Trim() == DocumentName.Trim()).Select(y => y).FirstOrDefault();
                                if (oDocumentToValid != null)
                                {
                                    oDateToValid = oDocumentToValid.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.AdditionalVigencyDate).Select(x => Convert.ToDateTime(x.Value)).FirstOrDefault();
                                    //oDocumentInfo = oDocumentInfo.FirstOrDefault().ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.HSEQVigency).Select(x => Convert.ToDateTime(x.Value)).FirstOrDefault();
                                    if (oDateToValid != new DateTime())
                                    {
                                        #region Validate the vigency just for the Documents of Provider
                                        //Valid the Rules
                                        switch (RuleType)
                                        {
                                            #region Evalue Rules
                                            case ((int)enumRuleType.LessThan):
                                                if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                                {
                                                    oDateToValid = oDateToValid.AddDays(-30);
                                                    if (oDateToValid.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.SixtyDays)
                                                {
                                                    oDateToValid = oDateToValid.AddDays(-60);
                                                    if (oDateToValid.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.NinetyDays)
                                                {
                                                    oDateToValid = oDateToValid.AddDays(-90);
                                                    if (oDateToValid.Date == DateTime.Now)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.OneYear)
                                                {
                                                    oDateToValid = oDateToValid.AddYears(-1);
                                                    if (oDateToValid.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                //Send notification
                                                if (BuildMsgObject)
                                                    this.SendNotification(NotificationConfigModel, p, DocumentType);
                                                break;
                                            case ((int)enumRuleType.LessOrEqualThan):
                                                if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                                {
                                                    oDateToValid = oDateToValid.AddDays(-31);
                                                    if (oDateToValid.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.SixtyDays)
                                                {
                                                    oDateToValid = oDateToValid.AddDays(-61);
                                                    if (oDateToValid.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.NinetyDays)
                                                {
                                                    oDateToValid = oDateToValid.AddDays(-91);
                                                    if (oDateToValid.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.OneYear)
                                                {
                                                    oDateToValid = oDateToValid.AddYears(-1);
                                                    if (oDateToValid.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                //Send notification
                                                if (BuildMsgObject)
                                                    this.SendNotification(NotificationConfigModel, p, DocumentType);
                                                break;
                                                #endregion
                                        }
                                        #endregion
                                    }

                                }
                            }
                            return true;
                        });
                    }
                    #endregion
                }

            }
            return true;
        }

        private void SendNotification(ProveedoresOnLine.Company.Models.Company.CompanyNotificationModel NotificationConfigModel, CompanyModel oCompany, int DocumentType)
        {

            List<string> oResponsables = new List<string>();
            oResponsables.AddRange(NotificationConfigModel.CompanyNotificationInfo.Where(x => x.ConfigItemType.ItemId == 2008008).Select(x => x.LargeValue).FirstOrDefault().Split(';'));
            if (oResponsables.Count > 0)
            {
                CompanyModel oProviderInfo = Company.Controller.Company.CompanyGetBasicInfo(oCompany.CompanyPublicId);
                oResponsables.All(x =>
                {
                    MessageModule.Client.Models.ClientMessageModel oMessage = new MessageModule.Client.Models.ClientMessageModel()
                    {
                        Agent = "POL_NotificationMessage_Mail",
                        User = "Proveedores OnLine Notifications",
                        ProgramTime = DateTime.Now,
                        MessageQueueInfo = new System.Collections.Generic.List<Tuple<string, string>>()
                        {
                            new Tuple<string,string>("To",x),
                            new Tuple<string,string>("ProviderName",oCompany.CompanyName),
                            new Tuple<string,string>("Nit",!string.IsNullOrEmpty(oProviderInfo.IdentificationNumber) ? oProviderInfo.IdentificationNumber : "N/D"),
                            new Tuple<string,string>("Logo",!string.IsNullOrEmpty(oProviderInfo.CompanyInfo.Where(y => y.ItemInfoType.ItemId == 203005).Select(y => y.Value).FirstOrDefault()) ? oProviderInfo.CompanyInfo.Where(y => y.ItemInfoType.ItemId == 203005).Select(y => y.Value).FirstOrDefault() : ""),
                            new Tuple<string,string>("MessageBody",NotificationConfigModel.CompanyNotificationInfo.Where(m => m.ConfigItemType.ItemId == 2008007).Select(m => m.LargeValue).FirstOrDefault()),
                            new Tuple<string,string>("Subject","Notificación Proveedor " + oCompany.CompanyName),
                        },
                    };
                    //ToDo
                    //Create the Notification model to send
                    MessageModule.Client.Models.NotificationModel oNotification = new MessageModule.Client.Models.NotificationModel()
                    {
                        Image = DocumentType == (int)enumDocumentType.AdditionalInfo ?
                                ProveedoresOnLine.Notification.Models.InternalSettings.Instance[Models.Constants.C_Settings_NotificationIconAdditionalInfo].Value.Trim() + oCompany.CompanyPublicId :
                                DocumentType == (int)enumDocumentType.HSEQ ?
                                ProveedoresOnLine.Notification.Models.InternalSettings.Instance[Models.Constants.C_Settings_NotificationIconHSEQ].Value.Trim() + oCompany.CompanyPublicId
                                : DocumentType == (int)enumDocumentType.GeneralInfo ?
                                ProveedoresOnLine.Notification.Models.InternalSettings.Instance[Models.Constants.C_Settings_NotificationIconGeneralInfo].Value.Trim() + oCompany.CompanyPublicId
                                : "N/A",
                        Label = NotificationConfigModel.NotificationName,
                        Url = DocumentType == (int)enumDocumentType.AdditionalInfo ?
                                ProveedoresOnLine.Notification.Models.InternalSettings.Instance[Models.Constants.C_Settings_NotificationAdditionalInfo_MK].Value.Trim() + oCompany.CompanyPublicId :
                                DocumentType == (int)enumDocumentType.HSEQ ?
                                ProveedoresOnLine.Notification.Models.InternalSettings.Instance[Models.Constants.C_Settings_NotificationHSEQ_MK].Value.Trim() + oCompany.CompanyPublicId
                                : DocumentType == (int)enumDocumentType.GeneralInfo ?
                                ProveedoresOnLine.Notification.Models.InternalSettings.Instance[Models.Constants.C_Settings_NotificationGeneralInfo_Mk].Value.Trim() + oCompany.CompanyPublicId 
                                : "N/A",
                        User = x,
                        State = 2013002,
                        Enable = true,
                    };

                    int idNotification = MessageModule.Client.Controller.ClientController.NotificationUpsert(oNotification);
                    NotificationModule.LogFile("Notification created to !!! :::::: " + x + ":::IdNotification::" + idNotification + "::::::" + DateTime.Now);
                    int idMessage = MessageModule.Client.Controller.ClientController.CreateMessage(oMessage);
                    NotificationModule.LogFile("Message Sent to !!! :::::: " + x + ":::IdMessageQueue::" + idMessage + "::::::" + DateTime.Now);
                    return true;
                });
            }
        }
    }
}
