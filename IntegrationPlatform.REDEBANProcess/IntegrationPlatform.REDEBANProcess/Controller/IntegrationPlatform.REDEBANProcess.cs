using IntegrationPlatform.REDEBANProcess.Models;
using MessageModule.Client.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess
{
    public class IntegrationPlatformREDEBANProcess
    {
        public static void StartProcess()
        {
            var providers = GetAllProviders();
            var InfoToExcel = new List<REDEBANInfoModel>();
            var oRowExcel = new REDEBANInfoModel();
            var oFileExcel = new StringBuilder();
            var PrvInfo_DS = new DataSet();

            var rowIndex = 2;
            var maxIndex = 0;
            var MinRowIndex = rowIndex;
            var ColIndex = 0;
            var proCount = 0;
            if (providers != null)
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    var ws = CreateExcelFile(p, "Proveedores");

                    #region Headers
                    
                    ws.Cells[1, 1].Value = "Razón Social";
                    ws.Cells[1, 2].Value = "No Identificación";
                    ws.Cells[1, 3].Value = "Estado";
                    ws.Cells[1, 4].Value = "Representante";
                    ws.Cells[1, 5].Value = "Teléfono";
                    ws.Cells[1, 6].Value = "Ciudad";
                    ws.Cells[1, 7].Value = "Cámara de Comercio";
                    ws.Cells[1, 8].Value = "RUT";
                    ws.Cells[1, 9].Value = "Estados Financieros";
                    ws.Cells[1, 10].Value = "Certificación Bancaria";
                    ws.Cells[1, 11].Value = "Certificación de Experiencia";
                    ws.Cells[1, 12].Value = "Certificación de Calidad";
                    ws.Cells[1, 13].Value = "Salud, Medio Ambiente y Seguridad";
                    ws.Cells[1, 14].Value = "Sistema de Riesgos Laborales";
                    ws.Cells[1, 15].Value = "Certificado de Accidentalidad";
                    
                    //Freeze HeadLine
                    ws.View.FreezePanes(2,1);
                    
                    //Some Style for the header line
                    for (int i = 1; i < 16; i++)
                    {
                        var HeaderLine = ws.Cells[1, i];
                        HeaderLine.Style.Font.Bold = true;
                        HeaderLine.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    

                    #endregion

                    providers.All(x =>
                    {
                        ColIndex = 1;
                        //TODO: setting
                        var prvInfo = GetProviderInfo("26D388E3", x.CompanyPublicId);
                        MinRowIndex = rowIndex;

                        maxIndex += GetMaxIndex(prvInfo);


                        if (prvInfo != null)
                        {
                            //Properties for Excel 
                            p.Workbook.Properties.Author = "ProveedoresOnLine S.A.S";
                            p.Workbook.Properties.Title = "Informe Gerencial de Proveedores REDEBAN";


                            //Provider Info
                            ws.Cells[rowIndex, 1].Value = prvInfo.Provider.CompanyName;
                            ws.Cells[rowIndex, 2].Value = prvInfo.Provider.IdentificationNumber;
                            ws.Cells[rowIndex, 3].Value = prvInfo.Provider.Status;
                            ws.Cells[rowIndex, 4].Value = prvInfo.Provider.Representant;
                            ws.Cells[rowIndex, 5].Value = prvInfo.Provider.Telephone;
                            ws.Cells[rowIndex, 6].Value = prvInfo.Provider.City;

                            //Fill Backgorund Color Main Line
                            for (int i = 1; i < 16; i++)
                            {
                                var MainCell = ws.Cells[rowIndex, i];
                                MainCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                MainCell.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                                
                            }

                            //TODO: setting
                            #region LegalInfo - ChaimberOfCommerce
                            if (prvInfo.LegalInfo_ChaimberOfCommerce.Count > 0 && prvInfo.LegalInfo_ChaimberOfCommerce != null)
                            {
                                var legalRowIndex = rowIndex;
                                prvInfo.LegalInfo_ChaimberOfCommerce.All(y =>
                                {
                                    var Cell = ws.Cells[legalRowIndex, 7];
                                    Cell.Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 602006).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                    legalRowIndex++;
                                    return true;
                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 7].Value = "N/A";
                            }
                            #endregion
                            //TODO: setting
                            #region LegalInfo - RUT
                            if (prvInfo.LegalInfo_RUT.Count > 0 && prvInfo.LegalInfo_RUT != null)
                            {
                                var legalRowIndex = rowIndex;
                                prvInfo.LegalInfo_RUT.All(y =>
                                {

                                    ws.Cells[legalRowIndex, 8].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 603012).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                    legalRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 8].Value = "N/A";
                            }


                            #endregion
                            //TODO: setting
                            #region FinancialInfo - Financial Stats
                            if (prvInfo.FinancialInfo_FinStats.Count > 0 && prvInfo.FinancialInfo_FinStats != null)
                            {
                                var FinRowIndex = rowIndex;
                                prvInfo.FinancialInfo_FinStats.All(y =>
                                {

                                    ws.Cells[FinRowIndex, 9].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 502002).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                    FinRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 9].Value = "N/A";
                            }
                            #endregion
                            //TODO: setting
                            #region FinancialInfo - Bank Certification

                            if (prvInfo.FinancialInfo_BankCert.Count > 0 && prvInfo.FinancialInfo_BankCert != null)
                            {
                                var FinRowIndex = rowIndex;
                                prvInfo.FinancialInfo_BankCert.All(y =>
                                {
                                    ws.Cells[FinRowIndex, 10].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 505010).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";
                                    FinRowIndex++;
                                    return true;
                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 10].Value = "N/A";
                            }

                            #endregion
                            //TODO: setting
                            #region CommercialInfo - Experience Certification
                            if (prvInfo.Commercial_CertExp.Count > 0 && prvInfo.Commercial_CertExp != null)
                            {
                                var CommRowIndex = rowIndex;
                                prvInfo.Commercial_CertExp.All(y =>
                                {
                                    ws.Cells[CommRowIndex, 11].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 302011).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";
                                    CommRowIndex++;
                                    return true;
                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 11].Value = "N/A";
                            }


                            #endregion
                            //TODO: setting
                            #region CertificationInfo - Certifications
                            if (prvInfo.HSEQ_Cert.Count > 0 && prvInfo.HSEQ_Cert != null)
                            {
                                var CertRowIndex = rowIndex;
                                prvInfo.HSEQ_Cert.All(y =>
                                {
                                    ws.Cells[CertRowIndex, 12].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 702006).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";
                                    CertRowIndex++;
                                    return true;
                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 12].Value = "N/A";
                            }

                            #endregion
                            //TODO: setting
                            #region CertificationInfo - Health, Enviroment, Security
                            if (prvInfo.HSEQ_Health.Count > 0 && prvInfo.HSEQ_Health != null)
                            {
                                var CertRowIndex = rowIndex;
                                prvInfo.HSEQ_Health.All(y =>
                                {
                                    ws.Cells[CertRowIndex, 13].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 703002 ||
                                    z.ItemInfoType.ItemId == 703003 || z.ItemInfoType.ItemId == 703004 || z.ItemInfoType.ItemId == 703005
                                    || z.ItemInfoType.ItemId == 703005 || z.ItemInfoType.ItemId == 703006).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                    CertRowIndex++;
                                    return true;
                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 13].Value = "N/A";
                            }


                            #endregion
                            //TODO: setting
                            #region CertificationInfo - System Risk Work
                            if (prvInfo.HSEQ_Riskes.Count > 0 && prvInfo.HSEQ_Riskes != null)
                            {
                                var CertRowIndex = rowIndex;
                                prvInfo.HSEQ_Riskes.All(y =>
                                {
                                    ws.Cells[CertRowIndex, 14].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 703002 ||
                                    z.ItemInfoType.ItemId == 703003 || z.ItemInfoType.ItemId == 703004 || z.ItemInfoType.ItemId == 703005
                                    || z.ItemInfoType.ItemId == 703005 || z.ItemInfoType.ItemId == 703006 || z.ItemInfoType.ItemId == 703007
                                    || z.ItemInfoType.ItemId == 703008 || z.ItemInfoType.ItemId == 703009 || z.ItemInfoType.ItemId == 703010).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                    CertRowIndex++;
                                    return true;
                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 14].Value = "N/A";
                            }
                            #endregion
                            //TODO: setting
                            #region CertificationInfo - Accidental Certification
                            if (prvInfo.HSEQ_Acc.Count > 0 && prvInfo.HSEQ_Acc != null)
                            {
                                var CertRowIndex = rowIndex;
                                prvInfo.HSEQ_Acc.All(y =>
                                {
                                    ws.Cells[CertRowIndex, 15].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 705007).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";
                                    CertRowIndex++;
                                    return true;
                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 15].Value = "N/A";
                            }
                            #endregion

                            proCount++;

                            if (proCount >= providers.Count)
                            {
                                string strFolder = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                    [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_File_TempDirectory].Value;

                                if (!System.IO.Directory.Exists(strFolder))
                                    System.IO.Directory.CreateDirectory(strFolder);

                                //Generate the report
                                Byte[] bin = p.GetAsByteArray();
                                string file = "REDEBAN_ProviderInfo_" + DateTime.Now.ToString("yyyy_MM_dd_hhmmss")  + ".xlsx";
                                File.WriteAllBytes(strFolder + file, bin);

                                #region UploadFileToS3

                                //Send to S3
                                string oFileCompleteName = strFolder + file;
                                //UpLoad file to s3                
                                string strRemoteFile = ProveedoresOnLine.FileManager.FileController.LoadFile(oFileCompleteName, "/REDEBAN_Process");

                                //remove temporal file
                                if (System.IO.File.Exists(oFileCompleteName))
                                    System.IO.File.Delete(oFileCompleteName);

                                #endregion

                                #region Notification

                                #endregion


                                RedebanProcessLogUpsert(0, "Provider Files Report", strRemoteFile, false, true, true);
                                //TODO: setting
                                //Send Message
                                MessageModule.Client.Models.NotificationModel oDataMessage = new MessageModule.Client.Models.NotificationModel();
                                oDataMessage.CompanyPublicId = "26D388E3";
                                oDataMessage.User = "diego.jaramillo@proveedoresonline.co";
                                oDataMessage.CompanyLogo = "http://proveedoresonline.s3-website-us-east-1.amazonaws.com/BackOffice/CompanyFile/26D388E3/CompanyFile_26D388E3_20160609142707.png";
                                oDataMessage.CompanyName = "REDEBAN";
                                oDataMessage.IdentificationType = "NIT";
                                oDataMessage.IdentificationNumber = "123";

                                #region Notification

                                oDataMessage.Label = Models.InternalSettings.Instance
                                        [Models.Constants.N_RedebanReportMessage].Value;
                                oDataMessage.Url = strRemoteFile;
                                oDataMessage.NotificationType = (int)IntegrationPlatform.REDEBANProcess.Models.Enumerations.enumNotificationType.RedebanNotification;
                                oDataMessage.Enable = true;

                                #endregion

                                IntegrationPlatform.REDEBANProcess.IntegrationPlatformREDEBANProcess.SendMessage(oDataMessage);

                                //These lines will open it in Excel, for test purposes
                                //ProcessStartInfo pi = new ProcessStartInfo(file);
                                //Process.Start(pi);
                            }
                            ColIndex++;

                            rowIndex += GetMaxIndex(prvInfo);
                        }
                        return true;
                    });
                }

            }
        }

        private static ExcelWorksheet CreateExcelFile(ExcelPackage Excel_pkg, string SheetName)
        {
            var p = new ExcelPackage();
            Excel_pkg.Workbook.Worksheets.Add(SheetName);
            ExcelWorksheet ws = Excel_pkg.Workbook.Worksheets[1];
            ws.Name = SheetName;
            ws.Cells.Style.Font.Size = 11;
            ws.Cells.Style.Font.Name = "Calibri";
            return ws;
        }

        private static int GetMaxIndex(REDEBANInfoModel model)
        {
            List<int> i = new List<int>();

            i.Add(model.LegalInfo_ChaimberOfCommerce.Count);
            i.Add(model.LegalInfo_RUT.Count);
            i.Add(model.FinancialInfo_FinStats.Count);
            i.Add(model.FinancialInfo_BankCert.Count);
            i.Add(model.Commercial_CertExp.Count);
            i.Add(model.HSEQ_Cert.Count);
            i.Add(model.HSEQ_Health.Count);
            i.Add(model.HSEQ_Riskes.Count);
            i.Add(model.HSEQ_Acc.Count);

            return i.Max();
        }

        private static void SendMessage(NotificationModel oDataMessage)
        {
            try
            {
                #region Email

                //Create message object
                MessageModule.Client.Models.ClientMessageModel oMessageToSend = new MessageModule.Client.Models.ClientMessageModel()
                {
                    Agent = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance[Constants.C_Settings_REDEBAN_Mail].Value,
                    User = oDataMessage.User,
                    ProgramTime = DateTime.Now,
                    MessageQueueInfo = new List<Tuple<string, string>>(),
                };

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>("To", "diego.jaramillo@proveedoresonline.co"));
                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>("InfoFileUrl", oDataMessage.Url));
                
                //get customer info
                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                    ("CustomerLogo", oDataMessage.CompanyLogo));

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                    ("CustomerName", oDataMessage.CompanyName));

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                    ("CustomerIdentificationTypeName", oDataMessage.IdentificationType));

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>
                    ("CustomerIdentificationNumber", oDataMessage.IdentificationNumber));

                MessageModule.Client.Controller.ClientController.CreateMessage(oMessageToSend);

                #endregion

                #region Notification

                oDataMessage.NotificationId = MessageModule.Client.Controller.ClientController.NotificationUpsert(oDataMessage);

                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<CompanyModel> GetAllProviders()
        {
            return DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.GetAllProviders();
        }

        public static REDEBANInfoModel GetProviderInfo(string CustomerPublicId, string ProviderPublicId)
        {
            return DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.GetProviderInfo(CustomerPublicId, ProviderPublicId);
        }

        public static int RedebanProcessLogUpsert(int RedebanProcessLogId, string ProceesName, string FileName, bool SendStatus, bool IsSucces, bool Enable)
        {
            return DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.RedebanProcessLogUpsert(RedebanProcessLogId, ProceesName, FileName, SendStatus, IsSucces, Enable);
        }


    }
}
