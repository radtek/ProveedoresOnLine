using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProveedoresOnLine.Reports.Models.Reports;
using BackOffice.Models.Report;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.Reports.Models.Util;

namespace BackOffice.Web.ControllersApi
{
    public class ReportApiController : ApiController
    {
        [HttpPost]
        [HttpGet]
        public List<ConfigReportViewModel> CC_Report()
        {
            List<ConfigReportViewModel> oReturn = new List<ConfigReportViewModel>();

            var ViewReport = ProveedoresOnLine.Reports.Controller.ReportModule.CC_Report_GetReportPublicId(null);

            ViewReport.All(x =>
            {
                oReturn.Add(new ConfigReportViewModel()
                {
                    CreateDate = x.CreateDate,
                    Enable = x.Enable,
                    LastModify = x.LastModify,
                    ReportId = x.ReportId,
                    ReportName = x.ReportName,
                    ReportPublic = x.ReportPublic,
                    ReportTypeId = x.ReportType.ItemId.ToString(),
                    User = x.User,
                });
                return true;
            });


            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public List<ConfigReportViewModel> CC_Report_Upsert
        (string CC_Report_Upsert,
            string ReportId)

        {
            List<ConfigReportViewModel> oReturn = new List<ConfigReportViewModel>();

            if (CC_Report_Upsert == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]))
            {
                ConfigReportViewModel oDaTaToUpsert = (ConfigReportViewModel)
                (new System.Web.Script.Serialization.JavaScriptSerializer()).
                Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                            typeof(ConfigReportViewModel));


                ConfigReportModel oCreated = new ConfigReportModel()
                {

                    ReportId = ReportId == "null" ? null : ReportId,
                    ReportName = oDaTaToUpsert.ReportName,
                    ReportType = new GenericItemModel()
                    {
                        ItemId = Convert.ToInt32(oDaTaToUpsert.ReportTypeId)
                    },
                    User = BackOffice.Models.General.SessionModel.CurrentLoginUser.Email.ToString(),
                    Enable = oDaTaToUpsert.Enable
                };

                var asdf = ProveedoresOnLine.Reports.Controller.ReportModule.CC_Report_UpSert(oCreated);

                //var rep = ProveedoresOnLine.Reports.Controller.ReportModule.CC_Report_GetReportPublicId(null);

                var ViewReport = ProveedoresOnLine.Reports.Controller.ReportModule.CC_Report_GetReportPublicId(null);

                ViewReport.Where(y => y.ReportId == asdf).FirstOrDefault(x =>
                 {
                     oReturn.Add(new ConfigReportViewModel()
                     {
                         CreateDate = x.CreateDate,
                         Enable = x.Enable,
                         LastModify = x.LastModify,
                         ReportId = x.ReportId,
                         ReportName = x.ReportName,
                         ReportPublic = x.ReportPublic,
                         ReportTypeId = x.ReportType.ItemId.ToString(),
                         User = x.User,
                     });
                     return true;
                 });


            }
            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public List<ConfigReportInfoViewModel> CC_ReportInfo(
        string ReportId)
        {
            List<ConfigReportInfoViewModel> oReturn = new List<ConfigReportInfoViewModel>();

            var ViewReport = ProveedoresOnLine.Reports.Controller.ReportModule.CC_Report_GetReportPublicId(null);

            string vtype = null;
            string vtypeId = null;
            string vField = null;
            string vFieldId = null;
            string vEnable = null;
            string vEnableId = null;
            string vParent = null;



            ViewReport.Where(x => x.ReportId == ReportId).FirstOrDefault(x =>
            {
                x.configReportInfo.All(y =>
                {
                    vtype = y.Parent == null ? y.ReportInfoType.ItemInfoId == (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoType ? y.Value : vtype : vtype;
                    vtypeId = y.Parent == null ? y.ReportInfoType.ItemInfoId == (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoType ? y.ReportInfoId : vtypeId : vtypeId;

                    vField = y.Parent != null ? y.ReportInfoType.ItemInfoId == (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoField ? y.Value : vField : vField;
                    vFieldId = y.Parent != null ? y.ReportInfoType.ItemInfoId == (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoField ? y.ReportInfoId : vFieldId : vFieldId;


                    vEnable = y.Parent != null ? y.ReportInfoType.ItemInfoId == (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoEnable ? y.Value : vEnable : vEnable;
                    vEnableId = y.Parent != null ? y.ReportInfoType.ItemInfoId == (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoEnable ? y.ReportInfoId : vEnableId : vEnableId;

                    vParent = y.Parent != null ? y.Parent.ToString() : vParent;

                    if (vField != null && vEnable != null && vtype != null)
                    {
                        oReturn.Add(new ConfigReportInfoViewModel()
                        {
                            Parent = vParent,
                            ReportId = y.ReportId,
                            ReportInfoId = vtypeId,
                            ReportInfoType = vtype,
                            ReportInfoFieldId = vFieldId,
                            Field = vField,
                            ReportInfoEnableId = vEnableId,
                            Enable = vEnable,


                        });
                        vtype = null;
                        vtypeId = null;
                        vField = null;
                        vFieldId = null;
                        vEnable = null;
                        vEnableId = null;
                        vParent = null;
                    }
                    return true;
                });
                return true;
            });


            return oReturn;
        }

        [HttpPost]
        [HttpGet]
        public List<CatalogModel> CC_ReportCatalog(
            int InfoTypeId
            )
        {

            List<CatalogModel> oReturn = new List<CatalogModel>();

            var Result = ProveedoresOnLine.CompanyProvider.Controller.CompanyProvider.CatalogGetProviderOptions();



            if (Result.Count > 0)
            {
                oReturn = Result.Where(x => x.CatalogId == Convert.ToInt32(InfoTypeId)).ToList();

            }

            return oReturn;
        }


        [HttpPost]
        [HttpGet]
        public List<ConfigReportInfoViewModel> CC_ReportInfo_Upsert
     (string CC_ReportInfo_Upsert, string parent)

        {
            List<ConfigReportInfoViewModel> oReturn = new List<ConfigReportInfoViewModel>();

            if (CC_ReportInfo_Upsert == "true" &&
                !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["DataToUpsert"]))
            {
                ConfigReportInfoViewModel oDaTaToUpsert = (ConfigReportInfoViewModel)
                (new System.Web.Script.Serialization.JavaScriptSerializer()).
                Deserialize(System.Web.HttpContext.Current.Request["DataToUpsert"],
                            typeof(ConfigReportInfoViewModel));

                string vparent = "";

                if (!string.IsNullOrEmpty(oDaTaToUpsert.ReportInfoType))
                {
                    
                    ConfigReportInfoModel TypeInformationCreated = new ConfigReportInfoModel()
                    {

                        ReportInfoId = oDaTaToUpsert.ReportInfoId == "null" ? null : oDaTaToUpsert.ReportInfoId,
                        ReportId = Convert.ToInt32(oDaTaToUpsert.ReportId),
                        ReportInfoType = new GenericReportItemInfoModel()
                        {
                            ItemInfoId = (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoType
                        },
                        Enable = Convert.ToBoolean(oDaTaToUpsert.Enable),
                        Parent = null,
                        Value = oDaTaToUpsert.ReportInfoType
                    };

                    vparent = ProveedoresOnLine.Reports.Controller.ReportModule.CC_ReportInfo_UpSert(TypeInformationCreated);

                    ConfigReportInfoModel FieldCreated = new ConfigReportInfoModel()
                    {

                        ReportInfoId = oDaTaToUpsert.ReportInfoFieldId == "null" ? null : oDaTaToUpsert.ReportInfoFieldId,
                        ReportId = Convert.ToInt32(oDaTaToUpsert.ReportId),
                        ReportInfoType = new GenericReportItemInfoModel()
                        {
                            ItemInfoId = (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoField
                        },
                        Enable = Convert.ToBoolean(oDaTaToUpsert.Enable),
                        Parent = Convert.ToInt32(vparent),

                        Value = oDaTaToUpsert.Field

                    };

                    var Fieldresult = ProveedoresOnLine.Reports.Controller.ReportModule.CC_ReportInfo_UpSert(FieldCreated);

                    ConfigReportInfoModel EnableCreated = new ConfigReportInfoModel()
                    {

                        ReportInfoId = oDaTaToUpsert.ReportInfoEnableId == "null" ? null : oDaTaToUpsert.ReportInfoEnableId,
                        ReportId = Convert.ToInt32(oDaTaToUpsert.ReportId),
                        ReportInfoType = new GenericReportItemInfoModel()
                        {
                            ItemInfoId = (int)ProveedoresOnLine.Reports.Models.Enumerations.enumDynamicReporInfotType.RP_InfoEnable
                        },
                        Enable = Convert.ToBoolean(oDaTaToUpsert.Enable),
                        Parent = Convert.ToInt32(vparent),
                        Value = oDaTaToUpsert.Enable
                    };

                    var Enableresult = ProveedoresOnLine.Reports.Controller.ReportModule.CC_ReportInfo_UpSert(EnableCreated);
                }

                var ViewReport = this.CC_ReportInfo(oDaTaToUpsert.ReportId.ToString());

                ViewReport.Where(y => y.Parent == vparent).FirstOrDefault(x =>
                {
                    oReturn.Add(x);
                    return true;
                });

            }
            
            return oReturn;

        }


    }

}

