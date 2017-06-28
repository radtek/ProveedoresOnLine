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
using System.Configuration;

namespace IntegrationPlatform.REDEBANProcess
{
    public class IntegrationPlatformREDEBANProcess
    {
        public static void StartProcess()
        {
            try
            {
                LogFile("REDEBAN Integration Process Init");
                var providers = GetAllProviders();
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
                        ws.View.FreezePanes(2, 1);

                        //Some Style for the header line
                        for (int i = 1; i < 16; i++)
                        {
                            var HeaderLine = ws.Cells[1, i];
                            HeaderLine.Style.Font.Bold = true;
                            HeaderLine.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }


                        #endregion

                        LogFile("Provider to generate: " + providers.Count);
                        providers.All(x =>
                        {
                            ColIndex = 1;
                            LogFile("Current Provider: " + x.CompanyPublicId);
                            var prvInfo = GetProviderInfo(IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_REDEBAN_ProviderPublicId].Value, x.CompanyPublicId);
                            MinRowIndex = rowIndex;

                            maxIndex += GetMaxIndex(prvInfo);


                            if (prvInfo != null)
                            {
                                //Properties for Excel 
                                p.Workbook.Properties.Author = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_Author].Value;
                                p.Workbook.Properties.Title = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_Title].Value;


                                //Provider Info
                                if (prvInfo.ProviderFullInfo != null)
                                {
                                    ws.Cells[rowIndex, 1].Value = prvInfo.ProviderFullInfo.CompanyName;
                                    ws.Cells[rowIndex, 2].Value = prvInfo.ProviderFullInfo.IdentificationNumber;
                                    ws.Cells[rowIndex, 3].Value = prvInfo.ProviderFullInfo.Status;
                                    ws.Cells[rowIndex, 4].Value = prvInfo.ProviderFullInfo.Representant;
                                    ws.Cells[rowIndex, 5].Value = prvInfo.ProviderFullInfo.Telephone;
                                    ws.Cells[rowIndex, 6].Value = prvInfo.ProviderFullInfo.City;
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 1].Value = prvInfo.ProviderBasicInfo.CompanyName;
                                    ws.Cells[rowIndex, 2].Value = prvInfo.ProviderBasicInfo.IdentificationNumber;
                                    ws.Cells[rowIndex, 3].Value = prvInfo.ProviderBasicInfo.Status;
                                    ws.Cells[rowIndex, 4].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                    ws.Cells[rowIndex, 5].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                    ws.Cells[rowIndex, 6].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }


                                //Fill Backgorund Color Main Line
                                for (int i = 1; i < 16; i++)
                                {
                                    var MainCell = ws.Cells[rowIndex, i];
                                    MainCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    MainCell.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                                }

                                #region LegalInfo - ChaimberOfCommerce
                                if (prvInfo.LegalInfo_ChaimberOfCommerce.Count > 0 && prvInfo.LegalInfo_ChaimberOfCommerce != null)
                                {
                                    var legalRowIndex = rowIndex;
                                    prvInfo.LegalInfo_ChaimberOfCommerce.All(y =>
                                    {
                                        var Cell = ws.Cells[legalRowIndex, 7];
                                        Cell.Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == (int)Models.Enumerations.enumLegalInfoType.ChaimberOfCommerceFile).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                        legalRowIndex++;
                                        return true;
                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 7].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }
                                #endregion

                                #region LegalInfo - RUT
                                if (prvInfo.LegalInfo_RUT.Count > 0 && prvInfo.LegalInfo_RUT != null)
                                {
                                    var legalRowIndex = rowIndex;
                                    prvInfo.LegalInfo_RUT.All(y =>
                                    {

                                        ws.Cells[legalRowIndex, 8].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == (int)Models.Enumerations.enumLegalInfoType.RUTFile).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                        legalRowIndex++;
                                        return true;

                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 8].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }


                                #endregion

                                #region FinancialInfo - Financial Stats
                                if (prvInfo.FinancialInfo_FinStats.Count > 0 && prvInfo.FinancialInfo_FinStats != null)
                                {
                                    var FinRowIndex = rowIndex;
                                    prvInfo.FinancialInfo_FinStats.All(y =>
                                    {

                                        ws.Cells[FinRowIndex, 9].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == (int)Models.Enumerations.enumFinancialInfoType.FinancialStatsFile).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                        FinRowIndex++;
                                        return true;

                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 9].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }
                                #endregion

                                #region FinancialInfo - Bank Certification

                                if (prvInfo.FinancialInfo_BankCert.Count > 0 && prvInfo.FinancialInfo_BankCert != null)
                                {
                                    var FinRowIndex = rowIndex;
                                    prvInfo.FinancialInfo_BankCert.All(y =>
                                    {
                                        ws.Cells[FinRowIndex, 10].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == (int)Models.Enumerations.enumFinancialInfoType.BankCertificationFile).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";
                                        FinRowIndex++;
                                        return true;
                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 10].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }

                                #endregion

                                #region CommercialInfo - Experience Certification
                                if (prvInfo.Commercial_CertExp.Count > 0 && prvInfo.Commercial_CertExp != null)
                                {
                                    var CommRowIndex = rowIndex;
                                    prvInfo.Commercial_CertExp.All(y =>
                                    {
                                        ws.Cells[CommRowIndex, 11].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCommercialInfoType.ExpereienceCertificationFile).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";
                                        CommRowIndex++;
                                        return true;
                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 11].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }


                                #endregion

                                #region CertificationInfo - Certifications
                                if (prvInfo.HSEQ_Cert.Count > 0 && prvInfo.HSEQ_Cert != null)
                                {
                                    var CertRowIndex = rowIndex;
                                    prvInfo.HSEQ_Cert.All(y =>
                                    {
                                        ws.Cells[CertRowIndex, 12].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.C_CertificationFile).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";
                                        CertRowIndex++;
                                        return true;
                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 12].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }

                                #endregion

                                #region CertificationInfo - Health, Enviroment, Security
                                if (prvInfo.HSEQ_Health.Count > 0 && prvInfo.HSEQ_Health != null)
                                {
                                    var CertRowIndex = rowIndex;
                                    prvInfo.HSEQ_Health.All(y =>
                                    {
                                        ws.Cells[CertRowIndex, 13].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId ==
                                        (int)Models.Enumerations.enumCertificationInfoType.CH_PoliticsSecurity ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_PoliticsNoAlcohol ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_ProgramOccupationalHealth ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_RuleIndustrialSecurity ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_MatrixRiskControl ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_CorporateSocialResponsability ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_ProgramEnterpriseSecurity ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_PoliticsRecruiment ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_CertificationsForm ||
                                        z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CH_PoliticIntegral
                                        ).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                        CertRowIndex++;
                                        return true;
                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 13].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }


                                #endregion

                                #region CertificationInfo - System Risk Work
                                if (prvInfo.HSEQ_Riskes.Count > 0 && prvInfo.HSEQ_Riskes != null)
                                {
                                    var CertRowIndex = rowIndex;
                                    prvInfo.HSEQ_Riskes.All(y =>
                                    {
                                        ws.Cells[CertRowIndex, 14].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CR_CertificateAffiliateARL).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";

                                        CertRowIndex++;
                                        return true;
                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 14].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }
                                #endregion

                                #region CertificationInfo - Accidental Certification
                                if (prvInfo.HSEQ_Acc.Count > 0 && prvInfo.HSEQ_Acc != null)
                                {
                                    var CertRowIndex = rowIndex;
                                    prvInfo.HSEQ_Acc.All(y =>
                                    {
                                        ws.Cells[CertRowIndex, 15].Formula = "HYPERLINK(\"" + y.ItemInfo.Where(z => z.ItemInfoType.ItemId == (int)Models.Enumerations.enumCertificationInfoType.CA_CertificateAccidentARL).Select(z => z.Value).FirstOrDefault() + "\",\"" + "Ver Archivo" + "\")";
                                        CertRowIndex++;
                                        return true;
                                    });
                                }
                                else
                                {
                                    ws.Cells[rowIndex, 15].Value = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_ExcelFile_NoAnswer].Value;
                                }
                                #endregion

                                proCount++;

                                if (proCount >= providers.Count)
                                {
                                    LogFile("REDEBAN Integration Process File Generation");
                                    string strFolder = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_Settings_File_TempDirectory].Value;

                                    if (!System.IO.Directory.Exists(strFolder))
                                        System.IO.Directory.CreateDirectory(strFolder);

                                    //Generate the report
                                    Byte[] bin = p.GetAsByteArray();
                                    string file = "REDEBAN_ProviderInfo_" + DateTime.Now.ToString("yyyy_MM_dd_hhmmss") + ".xlsx";
                                    File.WriteAllBytes(strFolder + file, bin);

                                    #region UploadFileToS3

                                    //Send to S3
                                    string oFileCompleteName = strFolder + file;
                                    //UpLoad file to s3                
                                    string strRemoteFile = ProveedoresOnLine.FileManager.FileController.LoadFile(oFileCompleteName, "/REDEBAN_Process");

                                    //remove temporal file
                                    if (System.IO.File.Exists(oFileCompleteName))
                                        System.IO.File.Delete(oFileCompleteName);

                                    LogFile("REDEBAN Integration Process File Upload");
                                    #endregion
                                    

                                    #region Message

                                    //Send Message
                                    MessageModule.Client.Models.NotificationModel oDataMessage = new MessageModule.Client.Models.NotificationModel();
                                    oDataMessage.CompanyPublicId = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.C_REDEBAN_ProviderPublicId].Value;
                                    oDataMessage.User = "REDEBAN Process";
                                    oDataMessage.CompanyLogo = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.N_RedebanCompanyLogo].Value;
                                    oDataMessage.CompanyName = IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance
                                        [IntegrationPlatform.REDEBANProcess.Models.Constants.N_RedebanCompanyName].Value;
                                    oDataMessage.IdentificationType = "NIT";
                                    oDataMessage.IdentificationNumber = "830070527";

                                    #endregion

                                    #region Notification

                                    oDataMessage.Label = Models.InternalSettings.Instance
                                            [Models.Constants.N_RedebanReportMessage].Value;
                                    oDataMessage.Url = strRemoteFile;
                                    oDataMessage.NotificationType = (int)IntegrationPlatform.REDEBANProcess.Models.Enumerations.enumNotificationType.RedebanNotification;
                                    oDataMessage.Enable = true;

                                    #endregion



                                    //These lines will open it in Excel, for test purposes
                                    //ProcessStartInfo pi = new ProcessStartInfo(file);
                                    //Process.Start(pi);
                                    var oRedebanLogModel = new RedebanLogModel()
                                    {
                                        RedebanProcessLogId = 0,
                                        ProcessName = "Redeban Provider Files Report Process",
                                        FileName = strRemoteFile,
                                        SendStatus = false,
                                        IsSucces = true,
                                        Enable = true

                                    };
                                    RedebanProcessLogUpsert(oRedebanLogModel);

                                    IntegrationPlatform.REDEBANProcess.IntegrationPlatformREDEBANProcess.SendMessage(oDataMessage);

                                    LogFile("REDEBAN Integration Process File Queue Message");
                                    
                                }
                                ColIndex++;

                                rowIndex += GetMaxIndex(prvInfo);
                            }
                            LogFile("REDEBAN Integration Process Has Succesfully ended");
                            return true;
                        });
                    }
                }                
            }
            catch (Exception ex)
            {
                var oRedebanLogModel = new RedebanLogModel()
                {
                    RedebanProcessLogId = 0,
                    ProcessName = "Redeban Provider Files Report Process",
                    FileName = string.Empty,
                    SendStatus = false,
                    IsSucces = false,
                    Enable = true

                };
                RedebanProcessLogUpsert(oRedebanLogModel);
                LogFile("Something happened: " + ex.Message +"StackTrace: " + ex.StackTrace + "InnerException: "+ ex.InnerException);
                throw ex;
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

                oMessageToSend.MessageQueueInfo.Add(new Tuple<string, string>("To", IntegrationPlatform.REDEBANProcess.Models.InternalSettings.Instance[Constants.N_Redeban_Mail_To].Value));
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
                RedebanLogModel oLogModel = IntegrationPlatformREDEBANProcess.GetLogBySendStatus(false);

                oLogModel.SendStatus = true;

                RedebanProcessLogUpsert(oLogModel);
            }
            catch (Exception ex)
            {
                RedebanLogModel oLogModel = IntegrationPlatformREDEBANProcess.GetLogBySendStatus(false);

                oLogModel.SendStatus = false;

                RedebanProcessLogUpsert(oLogModel);
                throw ex;
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

        public static int RedebanProcessLogUpsert(RedebanLogModel oModel)
        {
            try
            {
                if (oModel != null)
                {
                    oModel.RedebanProcessLogId = DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.RedebanProcessLogUpsert(oModel.RedebanProcessLogId, oModel.ProcessName, oModel.FileName, oModel.SendStatus, oModel.IsSucces, oModel.Enable);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return oModel.RedebanProcessLogId;
        }

        public static RedebanLogModel GetLogBySendStatus(bool SendStatus)
        {
            return DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.GetLogBySendStatus(SendStatus);
        }

        #region Log File

        private static void LogFile(string LogMessage)
        {
            try
            {
                //get file Log
                string LogFile = AppDomain.CurrentDomain.BaseDirectory.Trim().TrimEnd(new char[] { '\\' }) + "\\" +
                    System.Configuration.ConfigurationManager.AppSettings
                    [IntegrationPlatform.REDEBANProcess.Models.Constants.C_AppSettings_LogFile].Trim().TrimEnd(new char[] { '\\' });

                if (!System.IO.Directory.Exists(LogFile))
                    System.IO.Directory.CreateDirectory(LogFile);

                LogFile += "\\" + "Log_REDEBANProcess_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(LogFile))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "::" + LogMessage);
                    sw.Close();
                }
            }
            catch { }
        }
        #endregion

    }
}
