using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MessageModule.Client.Models;

namespace MessageModule.Client.DAL.MySQLDAO
{
    internal class Client_MySqlDao : MessageModule.Client.Interfaces.IClientData
    {
        private ADO.Interfaces.IADO DataInstance;

        public Client_MySqlDao()
        {
            DataInstance = new ADO.MYSQL.MySqlImplement(MessageModule.Client.Models.Constants.C_MS_ClientConnection);
        }

        public int MessageQueueCreate(string Agent, DateTime ProgramTime, string User)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<System.Data.IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vAgent", Agent));
            lstParams.Add(DataInstance.CreateTypedParameter("vProgramTime", ProgramTime));
            lstParams.Add(DataInstance.CreateTypedParameter("vUser", User));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "B_MessageQueue_Create",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });

            return Convert.ToInt32(response.ScalarResult);
        }

        public void MessageQueueInfoCreate(int MessageQueueId, string Parameter, string Value)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<System.Data.IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vMessageQueueId", MessageQueueId));
            lstParams.Add(DataInstance.CreateTypedParameter("vParameter", Parameter));
            lstParams.Add(DataInstance.CreateTypedParameter("vValue", Value));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "B_MessageQueueInfo_Create",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });
        }

        #region Notifications

        public int NotificationUpsert(NotificationModel NotificationUpsert)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationId", NotificationUpsert.NotificationId));
            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationType", NotificationUpsert.NotificationType));
            lstParams.Add(DataInstance.CreateTypedParameter("vLabel", NotificationUpsert.Label));
            lstParams.Add(DataInstance.CreateTypedParameter("vCompanyPublicId", NotificationUpsert.CompanyPublicId));
            lstParams.Add(DataInstance.CreateTypedParameter("vUser", NotificationUpsert.User));
            lstParams.Add(DataInstance.CreateTypedParameter("vState", NotificationUpsert.State));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", NotificationUpsert.Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "N_Notification_Upsert",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            return Convert.ToInt32(response.ScalarResult);
        }

        public int NotificationInfoUpsert(NotificationInfoModel NotificationInfoUpsert)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationInfoId", NotificationInfoUpsert.NotificationInfoId));
            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationId", NotificationInfoUpsert.NotificationId));
            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationInfoType", NotificationInfoUpsert.NotificationInfoType));
            lstParams.Add(DataInstance.CreateTypedParameter("vValue", NotificationInfoUpsert.Value));
            lstParams.Add(DataInstance.CreateTypedParameter("vLargeValue", NotificationInfoUpsert.LargeValue));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", NotificationInfoUpsert.Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "N_NotificationInfo_Upsert",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            return Convert.ToInt32(response.ScalarResult);
        }

        public List<NotificationModel> NotificationGetByUser(string User, bool Enable)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vUser", User));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataSet,
                CommandText = "N_Notification_GetByUser",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            List<NotificationModel> oReturn = null;

            if (response.DataSetResult != null &&
                response.DataSetResult.Tables.Count > 1)
            {

                if (response.DataSetResult.Tables[0] != null && response.DataSetResult.Tables[0].Rows.Count > 0)
                {
                    oReturn =
                    (from n in response.DataSetResult.Tables[0].AsEnumerable()
                     where !n.IsNull("NotificationId")
                     select new NotificationModel()
                     {
                         NotificationId = n.Field<int>("NotificationId"),
                         NotificationType = n.Field<int>("NotificationType"),
                         Label = n.Field<string>("Label"),
                         CompanyPublicId = n.Field<string>("CompanyPublicId"),
                         User = n.Field<string>("User"),
                         State = n.Field<int>("State"),
                         Enable = n.Field<UInt64>("Enable") == 1 ? true : false,
                         LastModify = n.Field<DateTime>("LastModify"),
                         CreateDate = n.Field<DateTime>("CreateDate"),

                         ListNotificationInfo = (from ni in response.DataSetResult.Tables[1].AsEnumerable()
                                                 where !ni.IsNull("NotificationInfoId") &&
                                                        ni.Field<int>("NotificationId") == n.Field<int>("NotificationId")
                                                 select new NotificationInfoModel()
                                                 {
                                                     NotificationInfoId = ni.Field<int>("NotificationInfoId"),
                                                     NotificationId = ni.Field<int>("NotificationId"),
                                                     NotificationInfoType = ni.Field<int>("NotificationInfoType"),
                                                     Value = ni.Field<string>("Value"),
                                                     LargeValue = ni.Field<string>("LargeValue"),
                                                     Enable = ni.Field<UInt64>("Enable") == 1 ? true : false,
                                                     LastModify = ni.Field<DateTime>("LastModify"),
                                                     CreateDate = ni.Field<DateTime>("CreateDate"),
                                                 }).ToList(),
                     }).ToList();
                }
            }

            return oReturn;
        }

        #endregion
    }
}