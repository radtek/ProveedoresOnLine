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

        public int NotificationUpsert(int? NotificationId, string Image, string Label, string Url, string User, int State, bool Enable)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationId", NotificationId));
            lstParams.Add(DataInstance.CreateTypedParameter("vImage", Image));
            lstParams.Add(DataInstance.CreateTypedParameter("vLabel", Label));
            lstParams.Add(DataInstance.CreateTypedParameter("vUrl", Url));
            lstParams.Add(DataInstance.CreateTypedParameter("vUser", User));
            lstParams.Add(DataInstance.CreateTypedParameter("vState", State));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "N_Notification_Upsert",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            return Convert.ToInt32(response.ScalarResult);
        }

        public int NotificationInfoUpsert(int? NotificationInfoId, int NotificationId, int NotificationInfoType, string Value, string LargeValue, bool Enable)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationInfoId", NotificationInfoId));
            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationId", NotificationId));
            lstParams.Add(DataInstance.CreateTypedParameter("vNotificationInfoType", NotificationInfoType));
            lstParams.Add(DataInstance.CreateTypedParameter("vValue", Value));
            lstParams.Add(DataInstance.CreateTypedParameter("vLargeValue", LargeValue));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", Enable == true ? 1 : 0));

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
                         Image = n.Field<string>("Image"),
                         Label = n.Field<string>("Label"),
                         Url = n.Field<string>("Url"),
                         User = n.Field<string>("User"),
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