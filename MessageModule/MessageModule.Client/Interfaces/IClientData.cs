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

        int NotificationUpsert(int? NotificationId, string Image, string Label, string Url, string User, int State, bool Enable);

        int NotificationInfoUpsert(int? NotificationInfoId, int NotificationId, int NotificationInfoType, string Value, string LargeValue, bool Enable);

        List<NotificationModel> NotificationGetByUser(string User, bool Enable);

        #endregion
    }
}
