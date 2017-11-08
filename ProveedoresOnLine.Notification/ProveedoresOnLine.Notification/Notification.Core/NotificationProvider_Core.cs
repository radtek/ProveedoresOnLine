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

namespace ProveedoresOnLine.Notification.Notification.Core
{
    internal class NotificationProvider_Core : INotificationCore
    {
        public bool ManageNotification(string CompanyPublicId, List<ProveedoresOnLine.Company.Models.Company.CompanyNotificationInfoModel> NotificationConfigInfoModel)
        {

            //Get All Providers by this Customer (CompanyPublicId)
            List<CompanyModel> oProviders = CompanyProvider.Controller.CompanyProvider.GetAllProvidersByCustomerPublicId(CompanyPublicId);

            if (NotificationConfigInfoModel != null)
            {
                if (NotificationConfigInfoModel.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationType).Select(x => x.Value).FirstOrDefault() == ((int)enumNotificationType.Document).ToString())
                {
                    //Wich Document To Valid
                    int DocumentType = NotificationConfigInfoModel.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.DocumentType).Select(x => int.Parse(x.Value)).FirstOrDefault();
                    string DocumentName = NotificationConfigInfoModel.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.Document).Select(x => x.Value).FirstOrDefault();
                    //Vigency, Priority, Status
                    int Critery = NotificationConfigInfoModel.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationCritery).Select(x => int.Parse(x.Value)).FirstOrDefault();
                    //<=, <,> ....
                    int RuleType = NotificationConfigInfoModel.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.RuleType).Select(x => int.Parse(x.Value)).FirstOrDefault();
                    //Days, Less 30, ...
                    int RuleValue = NotificationConfigInfoModel.Where(x => x.ConfigItemType.ItemId == (int)enumNotificationInfoType.NotificationValue).Select(x => int.Parse(x.Value)).FirstOrDefault();
                    // Indicatator if is necessary to build a Message Object
                    bool BuildMsgObject = false;

                    #region General Info
                    if (DocumentType == (int)enumDocumentType.GeneralInfo)
                    {
                        // In Chaimber Of Commerce case
                        if (DocumentName == "Cámara de Comercio")
                        {
                            oProviders.All(p =>
                            {
                                //Get LegalInfo
                                List<Company.Models.Util.GenericItemModel> oLegalInfo = CompanyProvider.Controller.CompanyProvider.LegalGetBasicInfo(p.CompanyPublicId, (int)enumGeneralInfoType.ChaimberOfComerceInfo, true);

                                DateTime ChaimberOfComerceDateToValidate = new DateTime();
                                if (oLegalInfo != null)
                                    ChaimberOfComerceDateToValidate = oLegalInfo.FirstOrDefault().ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.ChaimberOfComerceDateToValidate).Select(x => Convert.ToDateTime(x.Value)).FirstOrDefault();

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
                                            this.SendNotification(NotificationConfigInfoModel, p);
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
                                            this.SendNotification(NotificationConfigInfoModel, p);
                                                                                
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
                            //Get LegalInfo
                            List<Company.Models.Util.GenericItemModel> oHSEQInfo = CompanyProvider.Controller.CompanyProvider.CertficationGetBasicInfo(p.CompanyPublicId, (int)enumGeneralInfoType.HSEQCertifications, true);

                            DateTime HSQQDateToValidate = new DateTime();
                            if (oHSEQInfo != null)
                                HSQQDateToValidate = oHSEQInfo.FirstOrDefault().ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.HSEQVigency).Select(x => Convert.ToDateTime(x.Value)).FirstOrDefault();

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
                                        this.SendNotification(NotificationConfigInfoModel, p);
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
                                        this.SendNotification(NotificationConfigInfoModel, p);
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
                            //Get LegalInfo
                            List<Company.Models.Util.GenericItemModel> oDocumentInfo = CompanyProvider.Controller.CompanyProvider.AditionalDocumentGetByType(p.CompanyPublicId, (int)enumGeneralInfoType.AdditionalDocument, true);

                            DateTime oDateToValid = new DateTime();
                            if (oDocumentInfo != null)
                            {
                                Company.Models.Util.GenericItemModel oDocumentToValid =
                                 oDocumentInfo.Where(y => y.ItemName.Trim() == DocumentName.Trim()).Select(y => y).FirstOrDefault();
                                if (oDocumentToValid != null)
                                {
                                    oDateToValid = oDocumentToValid.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)enumGeneralInfoType.AdditionalVigencyDate).Select(x => DateTime.Parse(x.Value)).FirstOrDefault();
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
                                                    this.SendNotification(NotificationConfigInfoModel, p);
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
                                                    this.SendNotification(NotificationConfigInfoModel, p);
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

        private void SendNotification(List<ProveedoresOnLine.Company.Models.Company.CompanyNotificationInfoModel> NotificationConfigInfoModel, CompanyModel oCompany)
        {
            try
            {
                List<string> oResponsables = new List<string>();
                oResponsables.AddRange(NotificationConfigInfoModel.Where(x => x.ConfigItemType.ItemId == 2008008).Select(x => x.LargeValue).FirstOrDefault().Split(';'));
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
                            new Tuple<string,string>("Nit",oCompany.IdentificationNumber),
                            new Tuple<string,string>("Logo",oProviderInfo.CompanyInfo.Where(y => y.ItemInfoType.ItemId == 203005).Select(y => y.Value).FirstOrDefault()),
                            new Tuple<string,string>("MessageBody",NotificationConfigInfoModel.Where(m => m.ConfigItemType.ItemId == 2008007).Select(m => m.LargeValue).FirstOrDefault()),
                            new Tuple<string,string>("Subject","Notificación Proveedor " + oCompany.CompanyName),
                        },

                        };
                        MessageModule.Client.Controller.ClientController.CreateMessage(oMessage);
                        return true;
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
