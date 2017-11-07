using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Notification.Models
{
    public static class Enumerations
    {
        public enum enumNotificationInfoType
        {
            MessageType = 2008001,
            NotificationType = 2008002,
            Document = 2008003,
            NotificationCritery = 2008004,
            RuleType = 2008005,
            NotificationValue = 2008006,
            BodyMessage = 2008007,
            Responsable = 2008008,
            DocumentType = 2008009,
        }

        public enum enumNotificationType
        {
            Document = 2006001,
            RestrictiveList_POL = 2006002,
            RestictiveList_TK = 2006003,
            ProviderStatus = 2006004,
            ValidityProvider = 2006005
        }

        public enum enumDocumentType
        {
            GeneralInfo =2009001,
            AdditionalInfo = 2009002,
            HSEQ = 2009003,
        }

        public enum enumDocument
        {
            ChaimberOfCommerce = 2012001,            
        }

        public enum enumGeneralInfoType
        {
            ChaimberOfComerceInfo = 601001,
            ChaimberOfComerceDateToValidate = 602007,
            HSEQCertifications = 701001,
            HSEQVigency = 702004,
            AdditionalDocument = 1701001,
            AdditionalVigencyDate = 1702006,

        }

        public enum enumRuleType
        {
            Positive = 2001001,
            Negativo = 2001002,
            LessThan = 2001003,
            MoreThan = 2001004,
            EqualThan = 2001005,
            LessOrEqualThan = 2001006,
            MoreOrEqualThan = 2001007,
            Between = 2001008,
            PassOrNot = 2001009
        }

        public enum enumVigencyType
        {
            ThirtyDays = 2010001,
            SixtyDays = 2010002,
            NinetyDays = 2010003,
            OneYear = 2010004,
        }

        public enum enumNotificationCritery
        {
            Vigency = 2007001,
            Priority = 2007002,
            Status = 2007003,            
        }
    }
}
