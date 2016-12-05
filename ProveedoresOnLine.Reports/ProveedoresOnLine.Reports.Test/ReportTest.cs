using Microsoft.Reporting.WebForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading.Tasks;
using ProveedoresOnLine.Company.Models.Util;
using ProveedoresOnLine.SurveyModule.Models;

namespace ProveedoresOnLine.Reports.Test
{
    [TestClass]
    public class ReportTest
    {
        #region Survey Reports
        [TestMethod]
        public void SV_Report_SurveyDetail()
        {
            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("currentCompanyName", "Alpina"));
            parameters.Add(new ReportParameter("currentCompanyTipoDni", "Nit"));
            parameters.Add(new ReportParameter("currentCompanyDni", "1235466879645"));
            parameters.Add(new ReportParameter("currentCompanyLogo", "http://www.industriaalimenticia.com/ext/resources/images/news/alpinalogo3.jpg"));
            parameters.Add(new ReportParameter("providerName", "NombreProveedor"));
            parameters.Add(new ReportParameter("providerTipoDni", "Nit"));
            parameters.Add(new ReportParameter("providerDni", "7894596126"));
            parameters.Add(new ReportParameter("remarks", "Remarks"));
            parameters.Add(new ReportParameter("actionPlan", "Action Plan"));
            parameters.Add(new ReportParameter("dateStart", "12/12/2015"));
            parameters.Add(new ReportParameter("dateEnd", "13/12/2015"));
            parameters.Add(new ReportParameter("average", "50"));
            parameters.Add(new ReportParameter("reportDate", "01/01/2016"));
            parameters.Add(new ReportParameter("responsible", "Alexander"));
            parameters.Add(new ReportParameter("author", "Autor Chino"));
            Tuple<byte[], string, string> report = ProveedoresOnLine.Reports.Controller.ReportModule.CP_SurveyReportDetail((int)ProveedoresOnLine.Reports.Models.Enumerations.enumReportType.RP_SurveyReport, "PDF", parameters, "");
            parameters = null;
        }

        [TestMethod]
        public void SV_Report_AllSurveyByCustomer()
        {
            List<SurveyModule.Models.SurveyModel> oSurveyParents = SurveyModule.Controller.SurveyModule.SurveyGetAllByCustomer("1EA5A78A");
            List<Tuple<SurveyModel, List<GenericItemModel>, List<GenericItemModel>, List<GenericItemModel>>> oReturn = new List<Tuple<SurveyModel, List<GenericItemModel>, List<GenericItemModel>, List<GenericItemModel>>>();
            
            oSurveyParents.All(x =>
            {
            List<Tuple<SurveyModel, List<GenericItemModel>, List<GenericItemModel>, List<GenericItemModel>>> oChildReturn = new List<Tuple<SurveyModel, List<GenericItemModel>, List<GenericItemModel>, List<GenericItemModel>>>();
                List<SurveyModule.Models.SurveyModel> ChildSurvey = new List<SurveyModule.Models.SurveyModel>();
                ChildSurvey.AddRange(SurveyModule.Controller.SurveyModule.ReportAllSurvey(Convert.ToString(x.SurveyPublicId), "1EA5A78A"));
                    //TODO: Build obj tuple with parent and sons

                List<GenericItemModel> Areas = new List<GenericItemModel>();
                List<GenericItemModel> Questions = new List<GenericItemModel>();
                List<GenericItemModel> Answers = new List<GenericItemModel>();

                ChildSurvey.All(m =>
                {
                    Areas.Add(m.RelatedSurveyConfig.RelatedSurveyConfigItem.Where(y => y.ItemType.ItemId == 1202001 && y.ParentItem == null).Select(z => z).FirstOrDefault());

                    Areas.All(ar =>
                    {
                        Questions.AddRange(m.RelatedSurveyConfig.RelatedSurveyConfigItem.Where(e => e.ItemType.ItemId == 1202002 && e.ParentItem != null && e.ParentItem.ItemId == ar.ItemId).Select(e => e).ToList());
                        return true;
                    });
                    Questions.All(q =>
                    {
                        Answers.Add(m.RelatedSurveyItem.Where(e => e != null && e.RelatedSurveyConfigItem != null && e.RelatedSurveyConfigItem.ItemId == q.ItemId).Select(e => new GenericItemModel()
                        {
                            ItemId = e.ItemId,
                            ItemInfo = e.ItemInfo,
                            CreateDate = x.CreateDate,
                            ItemName = m.RelatedSurveyConfig.RelatedSurveyConfigItem.
                                                                                Where(inf => inf.ItemId == int.Parse(e.ItemInfo.Where(subinf => subinf.ItemInfoType.ItemId == 1205003).
                                                                                       Select(subinf => subinf.Value).FirstOrDefault())).Select(inf => inf.ItemName).FirstOrDefault(),
                            ParentItem = new GenericItemModel()
                            {
                                ItemId = e.RelatedSurveyConfigItem.ItemId
                            }
                        }).FirstOrDefault());
                        return true;
                    });
                    return true;
                });
                                
                oChildReturn.Add(new Tuple<SurveyModel, List<GenericItemModel>, List<GenericItemModel>, List<GenericItemModel>>(x, Areas, Questions, Answers));
                oReturn.AddRange(oChildReturn);
                return true;
            });
            
            Assert.IsTrue(oSurveyParents != null);
        }

        #endregion

        #region GerencialReport

        [TestMethod]
        public void C_GerencialReport()
        {
            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("CustomerName", "Representante de prueba"));

            DataTable data = new DataTable();
            DataTable data2 = new DataTable();
            DataTable data3 = new DataTable();
            DataTable data4 = new DataTable();

            Tuple<byte[], string, string> report =
                ProveedoresOnLine.Reports.Controller.ReportModule.CP_GerencialReport("PDF",
                                                                                     data,
                                                                                     data2,
                                                                                     data3,
                                                                                     data4,
                                                                                     parameters,
                                                                                     "C:\\Publicar Software\\ProveedoresOnLine\\ProveedoresOnLine.Reports\\ProveedoresOnLine.Reports.Test\\Reports\\C_Report_GerencialInfo.rdlc");
            parameters = null;
        }

        #endregion

        #region SelectionProcessReport
        [TestMethod]
        public void PJ_Report_SelectionProcess()
        {
            List<ReportParameter> parameters = new List<ReportParameter>();
            //current User
            parameters.Add(new ReportParameter("reportGeneratedBy", "generado@mail.com"));
            //CurrentCompany
            parameters.Add(new ReportParameter("currentCompanyName", "Publicar SAS"));
            parameters.Add(new ReportParameter("currentCompanyTypeId", "NIT"));
            parameters.Add(new ReportParameter("currentCompanyId", "123456879"));
            parameters.Add(new ReportParameter("currentCompanyLogo", "http://proveedoresonline.s3-website-us-east-1.amazonaws.com/BackOffice/CompanyFile/DA5C572E/CompanyFile_DA5C572E_20150311090116.png"));
            //Header
            parameters.Add(new ReportParameter("PJ_Name", "Nombre del proyecto"));
            parameters.Add(new ReportParameter("PJ_Type", "tipo del proyecto"));
            parameters.Add(new ReportParameter("PJ_Date", "18/08/2015"));
            parameters.Add(new ReportParameter("PJ_Price", "2.000.000" + "COP"));
            parameters.Add(new ReportParameter("PJ_MinExperience", "1"));
            parameters.Add(new ReportParameter("PJ_InternalCodeProcess", "2"));
            parameters.Add(new ReportParameter("PJ_YearsExperince", "3"));
            parameters.Add(new ReportParameter("PJ_ActivityName", "Insumos de papeleria"));
            parameters.Add(new ReportParameter("PJ_AdjudicateNote", "Nota de adjudicación que jue adjudicada por Alex-JP"));
            parameters.Add(new ReportParameter("PJ_ResponsibleName", "responsable@mail.com"));
            //areas
            DataTable dtProvidersProject = new DataTable();
            dtProvidersProject.Columns.Add("providerName");
            dtProvidersProject.Columns.Add("TypeId");
            dtProvidersProject.Columns.Add("providerId");
            dtProvidersProject.Columns.Add("hsq");
            dtProvidersProject.Columns.Add("tecnica");
            dtProvidersProject.Columns.Add("financiera");
            dtProvidersProject.Columns.Add("legal");
            dtProvidersProject.Columns.Add("estado");
            DataRow rowProvider = dtProvidersProject.NewRow();
            //add provider info
            rowProvider["providerName"] = "El Tiempo";
            rowProvider["TypeId"] = "Provedor:";
            rowProvider["providerId"] = "459698654";
            rowProvider["hsq"] = "5 % no pasa Aprobado";
            rowProvider["tecnica"] = "pasa Aprobado";
            rowProvider["financiera"] = "3 % no pasa Aprobado";
            rowProvider["legal"] = "pasa Aprobado";
            rowProvider["estado"] = "Ajudicado";

            dtProvidersProject.Rows.Add(rowProvider);

            Tuple<byte[], string, string> report = ProveedoresOnLine.Reports.Controller.ReportModule.PJ_SelectionProcessReport(dtProvidersProject, parameters, "PDF", @"C:\PublicarPO\ProveedoresOnLine.Reports\ProveedoresOnLine.Reports\Reports\");
        }
        #endregion

        #region CustomerProviderReport

        [TestMethod]
        public void CustomerProviderReport()
        {
            List<ProveedoresOnLine.Reports.Models.Reports.CustomerProviderReportModel> oReturn = new List<Models.Reports.CustomerProviderReportModel>();

            oReturn = ProveedoresOnLine.Reports.Controller.ReportModule.R_ProviderCustomerReport("205974DF");

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        }

        #endregion
    }
}
