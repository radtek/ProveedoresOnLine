using MessageModule.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModule.Client.Interfaces
{
    internal interface IClientData
    {
        int MessageQueueCreate(string Agent, DateTime ProgramTime, string User);

        void MessageQueueInfoCreate(int MessageQueueId, string Parameter, string Value);

        #region Notifications

        int NotificationUpsert(NotificationModel NotificationUpsert);

        int NotificationInfoUpsert(NotificationInfoModel NotificationInfoUpsert);

        List<NotificationModel> NotificationGetByUser(string User, bool Enable);

        #endregion
    }
}
