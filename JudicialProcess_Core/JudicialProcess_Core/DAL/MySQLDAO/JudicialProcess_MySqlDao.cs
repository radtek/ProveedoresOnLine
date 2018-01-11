using JudicialProcess_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudicialProcess_Core.DAL.MySQLDAO
{
    internal class JudicialProcess_MySqlDao : IJudicialProcessData
    {
        private ADO.Interfaces.IADO DataInstance;

        public JudicialProcess_MySqlDao()
        {
            DataInstance = new ADO.MYSQL.MySqlImplement(Models.Constans.C_POL_IntegrationPlatformConnectionName);
        }

        public bool IsAuthorized(string Token)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<System.Data.IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vToken", Token));            

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "Grl_AthorizationValidate",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });
            if (response.ScalarResult != null)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        public void SaveTransaction(string Token, string Message, string Query, int ServiceType, bool IsSuccess)
        {
            List<System.Data.IDbDataParameter> lstParams = new List<System.Data.IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vToken", Token));
            lstParams.Add(DataInstance.CreateTypedParameter("vMessage", Message));
            lstParams.Add(DataInstance.CreateTypedParameter("vQuery", Query));
            lstParams.Add(DataInstance.CreateTypedParameter("vServiceType", ServiceType));
            lstParams.Add(DataInstance.CreateTypedParameter("vEnable", 1));
            lstParams.Add(DataInstance.CreateTypedParameter("vIsSuccess", IsSuccess == true ? 1 : 0));            

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.Scalar,
                CommandText = "Grl_Service_Transaction_Insert",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = lstParams
            });          
        }
    }
}
