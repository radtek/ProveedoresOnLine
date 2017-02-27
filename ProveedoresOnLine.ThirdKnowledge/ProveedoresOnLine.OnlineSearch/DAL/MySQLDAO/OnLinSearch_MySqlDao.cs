using ProveedoresOnLine.OnlineSearch.Interfaces;
using ProveedoresOnLine.OnlineSearch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.DAL.MySQLDAO
{
    internal class OnLinSearch_MySqlDao : IOnLineSearchData
    {
        private ADO.Interfaces.IADO DataInstance;

        public OnLinSearch_MySqlDao()
        {
            DataInstance = new ADO.MYSQL.MySqlImplement(ProveedoresOnLine.OnlineSearch.Models.Constants.C_POL_ThirdKnowledgeConnectionName);
        }

        public TreeModel GetAnswerByTreeidAndQuestion(int TreeType, string Question)
        {          
            List<System.Data.IDbDataParameter> lstParams = new List<IDbDataParameter>();

            lstParams.Add(DataInstance.CreateTypedParameter("vTreeTypeId", TreeType));
            lstParams.Add(DataInstance.CreateTypedParameter("vQuestion", Question));

            ADO.Models.ADOModelResponse response = DataInstance.ExecuteQuery(new ADO.Models.ADOModelRequest()
            {
                CommandExecutionType = ADO.Models.enumCommandExecutionType.DataTable,
                CommandText = "U_GetAnswerByTreeIdAndQuestion",
                CommandType = CommandType.StoredProcedure,
                Parameters = lstParams,
            });

            TreeModel oReturn = null;

            if (response.DataTableResult != null &&
                response.DataTableResult.Rows.Count > 0)
            {
                oReturn = new TreeModel()
                {
                    TreeId = response.DataTableResult.Rows[0].Field<int>("TreeId"),
                    TreeName = response.DataTableResult.Rows[0].Field<string>("TreeName"),
                    TreeType = new CatalogModel()
                    {
                        ItemId = response.DataTableResult.Rows[0].Field<int>("TreeTypeId"),
                        ItemName = response.DataTableResult.Rows[0].Field<string>("TreeTypeName"),
                    },
                    LastModify = response.DataTableResult.Rows[0].Field<DateTime>("LastModify"),
                    CreateDate = response.DataTableResult.Rows[0].Field<DateTime>("CreateDate"),
                    Enable = response.DataTableResult.Rows[0].Field<UInt64>("Enable") == 1 ? true : false,

                    TreeItem = (from tinf in response.DataTableResult.AsEnumerable()
                                group tinf by new
                                {
                                    ParentItemId = tinf.Field<int>("ParentItemId"),
                                    ParentItemName = tinf.Field<string>("ParentItemName"),
                                    ChildItemId = tinf.Field<int>("ChildItemId"),
                                    ChildItemName = tinf.Field<string>("ChildItemName"),
                                }
                                    into gtinf
                                    select new TreeItemModel()
                                    {
                                        ParentItem = new CatalogModel()
                                        {
                                            ItemId = gtinf.Key.ParentItemId,
                                            ItemName = gtinf.Key.ParentItemName,
                                        },
                                        ChildItem = new CatalogModel()
                                        {
                                            ItemId = gtinf.Key.ChildItemId,
                                            ItemName = gtinf.Key.ChildItemName,
                                        }
                                    }
                                ).ToList()
                };
            }
            return oReturn;
        }
    }
}
