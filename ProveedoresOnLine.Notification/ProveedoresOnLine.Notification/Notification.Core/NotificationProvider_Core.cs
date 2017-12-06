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
            string ProviderPublicId = "";
            if (NotificationConfigModel.CompanyNotificationInfo != null)
            {
                try
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
                                    ProviderPublicId = p.CompanyPublicId;
                                    NotificationModule.LogFile("Process Notifications:::Rule Section:: Cámara de Comercio" + ":::CompanyPublicId:::" + p.CompanyPublicId);
                                    BuildMsgObject = false;
                                    //Get LegalInfo
                                    List<Company.Models.Util.GenericItemModel> oLegalInfo = CompanyProvider.Controller.CompanyProvider.LegalGetBasicInfo(p.CompanyPublicId, (int)enumGeneralInfoType.ChaimberOfComerceInfo, true);

                                    var ChaimberOfComerceDateToValidate = "";
                                    if (oLegalInfo != null)
                                        ChaimberOfComerceDateToValidate = oLegalInfo.FirstOrDefault().ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.ChaimberOfComerceDateToValidate).Select(x => x.Value).FirstOrDefault();

                                    #region Validate the vigency just for the Documents of Provider
                                    //Valid the Rules
                                    if (!string.IsNullOrEmpty(ChaimberOfComerceDateToValidate))
                                    {
                                        DateTime dateToValidate = DateTime.Parse(ChaimberOfComerceDateToValidate, culture);
                                        switch (RuleType)
                                        {
                                            #region Evalue Rules
                                            case ((int)enumRuleType.LessThan):
                                                if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                                {
                                                    dateToValidate = dateToValidate.AddMonths(-1);
                                                    if (dateToValidate.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.SixtyDays)
                                                {
                                                    dateToValidate = dateToValidate.AddMonths(-2);
                                                    if (dateToValidate.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.NinetyDays)
                                                {
                                                    dateToValidate = dateToValidate.AddMonths(-3);
                                                    if (dateToValidate.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.OneYear)
                                                {
                                                    dateToValidate = dateToValidate.AddYears(-1);
                                                    if (dateToValidate.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                //Send notification
                                                if (BuildMsgObject)
                                                    this.SendNotification(NotificationConfigModel, p, DocumentType);
                                                break;
                                            case ((int)enumRuleType.LessOrEqualThan):
                                                if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                                {
                                                    dateToValidate = dateToValidate.AddMonths(-1);
                                                    if (dateToValidate.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.SixtyDays)
                                                {
                                                    dateToValidate = dateToValidate.AddMonths(-2);
                                                    if (dateToValidate.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.NinetyDays)
                                                {
                                                    dateToValidate = dateToValidate.AddMonths(-3);
                                                    if (dateToValidate.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }
                                                if (RuleValue == (int)enumVigencyType.OneYear)
                                                {
                                                    dateToValidate = dateToValidate.AddYears(-1);
                                                    if (dateToValidate.Date == DateTime.Now.Date)
                                                        BuildMsgObject = true;
                                                }

                                                //Send notification
                                                if (BuildMsgObject)
                                                    this.SendNotification(NotificationConfigModel, p, DocumentType);

                                                break;
                                                #endregion
                                        }
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
                                ProviderPublicId = p.CompanyPublicId;
                                NotificationModule.LogFile("Process Notifications:::Rule Section:: HSEQ" + ":::CompanyPublicId:::" + p.CompanyPublicId);
                                BuildMsgObject = false;
                                //Get LegalInfo
                                List<Company.Models.Util.GenericItemModel> oHSEQInfo = CompanyProvider.Controller.CompanyProvider.CertficationGetBasicInfo(p.CompanyPublicId, (int)enumGeneralInfoType.HSEQCertifications, true);

                                var HSQQDateToValidate = "";
                                if (oHSEQInfo != null)
                                    HSQQDateToValidate = oHSEQInfo.FirstOrDefault().ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.HSEQVigency).Select(x => x.Value).FirstOrDefault();

                                #region Validate the vigency just for the Documents of Provider
                                //Valid the Rules
                                if (!string.IsNullOrEmpty(HSQQDateToValidate))
                                {
                                    DateTime dateToValidate = DateTime.Parse(HSQQDateToValidate, culture);
                                    switch (RuleType)
                                    {
                                        #region Evalue Rules
                                        case ((int)enumRuleType.LessThan):
                                            if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                            {
                                                dateToValidate = dateToValidate.AddMonths(-1);
                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            if (RuleValue == (int)enumVigencyType.SixtyDays)
                                            {
                                                dateToValidate = dateToValidate.AddMonths(-2);
                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            if (RuleValue == (int)enumVigencyType.NinetyDays)
                                            {
                                                dateToValidate = dateToValidate.AddMonths(-3);
                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            if (RuleValue == (int)enumVigencyType.OneYear)
                                            {
                                                dateToValidate = dateToValidate.AddYears(-1);
                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            //Send notification
                                            if (BuildMsgObject)
                                                this.SendNotification(NotificationConfigModel, p, DocumentType);
                                            break;
                                        case ((int)enumRuleType.LessOrEqualThan):
                                            if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                            {
                                                dateToValidate = dateToValidate.AddMonths(-1);
                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            if (RuleValue == (int)enumVigencyType.SixtyDays)
                                            {
                                                dateToValidate = dateToValidate.AddMonths(-2);
                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            if (RuleValue == (int)enumVigencyType.NinetyDays)
                                            {
                                                dateToValidate = dateToValidate.AddMonths(-3);
                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            if (RuleValue == (int)enumVigencyType.OneYear)
                                            {
                                                dateToValidate = dateToValidate.AddYears(-1);
                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            //Send notification
                                            if (BuildMsgObject)
                                                this.SendNotification(NotificationConfigModel, p, DocumentType);
                                            break;
                                        case ((int)enumRuleType.EqualThan):
                                            if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                            {

                                                if (dateToValidate.Date == DateTime.Now.Date)
                                                    BuildMsgObject = true;
                                            }
                                            //Send notification
                                            if (BuildMsgObject)
                                                this.SendNotification(NotificationConfigModel, p, DocumentType);
                                            break;
                                            #endregion
                                    }
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
                                ProviderPublicId = p.CompanyPublicId;
                                NotificationModule.LogFile("Process Notifications:::Rule Section:: AdditionalInfo" + ":::CompanyPublicId:::" + p.CompanyPublicId);
                                BuildMsgObject = false;
                                //Get LegalInfo
                                List<Company.Models.Util.GenericItemModel> oDocumentInfo = CompanyProvider.Controller.CompanyProvider.AditionalDocumentGetByType(p.CompanyPublicId, (int)enumGeneralInfoType.AdditionalDocument, true);

                                var personalizedDocument = "";
                                if (oDocumentInfo != null)
                                {
                                    Company.Models.Util.GenericItemModel oDocumentToValid =
                                     oDocumentInfo.Where(y => y.ItemName.Trim() == DocumentName.Trim()).Select(y => y).FirstOrDefault();
                                    if (oDocumentToValid != null)
                                    {
                                        personalizedDocument = oDocumentToValid.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.AdditionalVigencyDate).Select(x => x.Value).FirstOrDefault();

                                        if (!string.IsNullOrEmpty(personalizedDocument))
                                        {
                                            DateTime oDateToValid = DateTime.Parse(personalizedDocument, culture);
                                            #region Validate the vigency just for the Documents of Provider
                                            //Valid the Rules
                                            switch (RuleType)
                                            {
                                                #region Evalue Rules
                                                case ((int)enumRuleType.LessThan):
                                                    if (RuleValue == (int)enumVigencyType.ThirtyDays)
                                                    {
                                                        oDateToValid = oDateToValid.AddMonths(-1);
                                                        if (oDateToValid.Date == DateTime.Now.Date)
                                                            BuildMsgObject = true;
                                                    }
                                                    if (RuleValue == (int)enumVigencyType.SixtyDays)
                                                    {
                                                        oDateToValid = oDateToValid.AddMonths(-2);
                                                        if (oDateToValid.Date == DateTime.Now.Date)
                                                            BuildMsgObject = true;
                                                    }
                                                    if (RuleValue == (int)enumVigencyType.NinetyDays)
                                                    {
                                                        oDateToValid = oDateToValid.AddMonths(-3);
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
                                                        oDateToValid = oDateToValid.AddMonths(-1);
                                                        if (oDateToValid.Date == DateTime.Now.Date)
                                                            BuildMsgObject = true;
                                                    }
                                                    if (RuleValue == (int)enumVigencyType.SixtyDays)
                                                    {
                                                        oDateToValid = oDateToValid.AddMonths(-2);
                                                        if (oDateToValid.Date == DateTime.Now.Date)
                                                            BuildMsgObject = true;
                                                    }
                                                    if (RuleValue == (int)enumVigencyType.NinetyDays)
                                                    {
                                                        oDateToValid = oDateToValid.AddMonths(-3);
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
                                NotificationModule.LogFile("Process Notifification done ::: send Notification?" + "::" + BuildMsgObject + "Date:::" + DateTime.Now );
                                return true;
                            });
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    NotificationModule.LogFile("Process Notifications Error!!! :::::: " + ex.Message + ":::CompanyPublicId:::" + ProviderPublicId);                    
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
                    //Create the Notification model to send
                    MessageModule.Client.Models.NotificationModel oNotification = new MessageModule.Client.Models.NotificationModel()
                    {
                        NotificationType = DocumentType,
                        CompanyPublicId = oCompany.CompanyPublicId,
                        Label = NotificationConfigModel.NotificationName,
                        User = x,
                        State = 2013002, // Sin leer
                        Enable = true,
                    };

                    int idNotification = MessageModule.Client.Controller.ClientController.NotificationUpsert(oNotification);
                    NotificationModule.LogFile("Notification created to !!! :::::: " + x + ":::IdNotification::" + idNotification + "::::::" + DateTime.Now);

                    if (idNotification != 0)
                    {
                        //Create the NotificationInfo model to send
                        MessageModule.Client.Models.NotificationInfoModel oNotificationInfo = new MessageModule.Client.Models.NotificationInfoModel()
                        {
                            NotificationId = idNotification,
                            NotificationInfoType = 2008007, // Item body
                            Value = null,
                            LargeValue = NotificationConfigModel.CompanyNotificationInfo.Where(m => m.ConfigItemType.ItemId == 2008007).Select(m => m.LargeValue).FirstOrDefault(),
                            Enable = true,
                        };

                        int idNotificationInfo = MessageModule.Client.Controller.ClientController.NotificationInfoUpsert(oNotificationInfo);
                        NotificationModule.LogFile("NotificationInfo created to !!! :::::: " + x + ":::IdNotificationInfo::" + idNotificationInfo + "::::::" + DateTime.Now);
                    }
                    

                    int idMessage = MessageModule.Client.Controller.ClientController.CreateMessage(oMessage);
                    NotificationModule.LogFile("Message Sent to !!! :::::: " + x + ":::IdMessageQueue::" + idMessage + "::::::" + DateTime.Now);
                    return true;
                });
            }
        }
    }
}
