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

        public int NotificationUpsert(NotificationModel NotificationUpsert)
        {
            return DataFactory.NotificationUpsert(NotificationUpsert);
        }

        public int NotificationInfoUpsert(NotificationInfoModel NotificationInfoUpsert)
        {
            return DataFactory.NotificationInfoUpsert(NotificationInfoUpsert);
        }

        public List<NotificationModel> NotificationGetByUser(string User, bool Enable)
        {
            return DataFactory.NotificationGetByUser(User, Enable);
        }

        #endregion
    }
}
