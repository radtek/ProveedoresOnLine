using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.General
{
    public static class SessionModel
    {
        public static List<MessageModule.Client.Models.NotificationModel> ListNotifications { get; set; }
        public static SessionManager.Models.Auth.User CurrentLoginUser { get { return SessionManager.SessionController.Auth_UserLogin; } }

        public static bool UserIsLoggedIn { get { return (CurrentLoginUser != null); } }

        #region Notifications

        public static void ChangeStatusNotification(MessageModule.Client.Models.NotificationModel mNotification)
        {
            mNotification.State = 2013001; // Leido
            MessageModule.Client.Controller.ClientController.NotificationUpsert(mNotification);

            ListNotifications = MessageModule.Client.Controller.ClientController.NotificationGetByUser(CurrentLoginUser.Email, true);
        }

        public static bool NewNotifications
        {
            get
            {
                ListNotifications = MessageModule.Client.Controller.ClientController.NotificationGetByUser(CurrentLoginUser.Email, true);

                bool oNewNotifications = false;

                oNewNotifications = ListNotifications != null ? true : false;

                return oNewNotifications;
            }
        }

        #endregion
        public static bool UserIsAutorized
        {
            get
            {
                return CurrentLoginUser.
                            RelatedApplicationRole.
                            Any(x => x.Application == SessionManager.Models.Auth.enumApplication.Backoffice);
            }
        }

        public static SessionManager.Models.POLMarketPlace.MarketPlaceUser CurrentCompanyLoginUser
        {
            get
            {
                return SessionManager.SessionController.POLMarketPlace_MarketPlaceUserLogin;
            }
            set
            {
                SessionManager.SessionController.POLMarketPlace_MarketPlaceUserLogin = value;
            }
        }

        public static SessionManager.Models.POLMarketPlace.Session_CompanyModel CurrentCompany { get { return CurrentCompanyLoginUser.RelatedCompany.Where(x => x.CurrentSessionCompany == true).FirstOrDefault(); } }

        public static enumCompanyType CurrentCompanyType { get { return CurrentCompany == null ? enumCompanyType.Provider : (enumCompanyType)CurrentCompany.CompanyType.ItemId; } }
    }
}
