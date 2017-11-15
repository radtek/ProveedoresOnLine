using MessageModule.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModule.Client.DAL.Controller
{
    internal class ClientDataController : MessageModule.Client.Interfaces.IClientData
    {
        #region singleton instance

        private static MessageModule.Client.Interfaces.IClientData oInstance;
        internal static MessageModule.Client.Interfaces.IClientData Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new ClientDataController();
                return oInstance;
            }
        }

        private MessageModule.Client.Interfaces.IClientData DataFactory;

        #endregion

        #region Constructor

        public ClientDataController()
        {
            ClientDataFactory factory = new ClientDataFactory();
            DataFactory = factory.GetClientInstance();
        }

        #endregion

        #region Message Queue
        public int MessageQueueCreate(string Agent, DateTime ProgramTime, string User)
        {
            return DataFactory.MessageQueueCreate(Agent, ProgramTime, User);
        }

        public void MessageQueueInfoCreate(int MessageQueueId, string Parameter, string Value)
        {
            DataFactory.MessageQueueInfoCreate(MessageQueueId, Parameter, Value);
        } 
        #endregion

        #region Notifications

        public int NotificationUpsert(int? NotificationId, string Image, string Label, string Url, string User, int State, bool Enable)
        {
            return DataFactory.NotificationUpsert(NotificationId, Image, Label, Url, User, State, Enable);
        }

        public int NotificationInfoUpsert(int? NotificationInfoId, int NotificationId, int NotificationInfoType, string Value, string LargeValue, bool Enable)
        {
            return DataFactory.NotificationInfoUpsert(NotificationInfoId, NotificationId, NotificationInfoType, Value, LargeValue, Enable);
        }

        public List<NotificationModel> NotificationGetByUser(string User, bool Enable)
        {
            return DataFactory.NotificationGetByUser(User, Enable);
        }

        #endregion
    }
}
