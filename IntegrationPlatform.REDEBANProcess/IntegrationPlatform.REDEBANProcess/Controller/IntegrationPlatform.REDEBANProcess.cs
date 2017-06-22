using IntegrationPlatform.REDEBANProcess.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

                    #endregion

                    providers.All(x =>
                    {
                        ColIndex = 1;
                       
                        var prvInfo = GetProviderInfo("26D388E3", x.CompanyPublicId);
                        MinRowIndex = rowIndex;
                        
                        maxIndex += GetMaxIndex(prvInfo);
                        

                        if (prvInfo != null)
                        {

                            p.Workbook.Properties.Author = "ProveedoresOnLine S.A.S";
                            p.Workbook.Properties.Title = "Informe Gerencial de Proveedores REDEBAN";



                            ws.Cells[rowIndex, 1].Value = prvInfo.Provider.CompanyName;
                            ws.Cells[rowIndex, 2].Value = prvInfo.Provider.IdentificationNumber;
                            ws.Cells[rowIndex, 3].Value = prvInfo.Provider.Status;
                            ws.Cells[rowIndex, 4].Value = prvInfo.Provider.Representant;
                            ws.Cells[rowIndex, 5].Value = prvInfo.Provider.Telephone;
                            ws.Cells[rowIndex, 6].Value = prvInfo.Provider.City;

                            #region LegalInfo - ChaimberOfCommerce
                            if (prvInfo.LegalInfo_ChaimberOfCommerce.Count > 0 && prvInfo.LegalInfo_ChaimberOfCommerce != null)
                            {
                                var legalRowIndex = rowIndex;
                                prvInfo.LegalInfo_ChaimberOfCommerce.All(y =>
                                {
                                    
                                    ws.Cells[legalRowIndex, 7].Value = y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 602006).Select(z => z.Value).First();
                                    legalRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 7].Value = "N/A";
                            }
                            #endregion
                            #region LegalInfo - RUT
                            if (prvInfo.LegalInfo_RUT.Count > 0 && prvInfo.LegalInfo_RUT != null)
                            {
                                var legalRowIndex = rowIndex;
                                prvInfo.LegalInfo_RUT.All(y =>
                                {
                                    
                                    ws.Cells[legalRowIndex, 8].Value = y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 603012).Select(z => z.Value).First();
                                    legalRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 8].Value = "N/A";
                            }


                            #endregion
                            #region FinancialInfo - Financial Stats
                            if (prvInfo.FinancialInfo_FinStats.Count > 0 && prvInfo.FinancialInfo_FinStats != null)
                            {
                                var FinRowIndex = rowIndex;
                                prvInfo.FinancialInfo_FinStats.All(y =>
                                {
                                    
                                    ws.Cells[FinRowIndex, 9].Value = y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 502002).Select(z => z.Value).First();
                                    FinRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 9].Value = "N/A";
                            }
                            #endregion
                            #region FinancialInfo - Bank Certification

                            if (prvInfo.FinancialInfo_BankCert.Count > 0 && prvInfo.FinancialInfo_BankCert != null)
                            {
                                var FinRowIndex = rowIndex;
                                prvInfo.FinancialInfo_BankCert.All(y =>
                                {

                                    ws.Cells[FinRowIndex, 10].Value = y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 505010).Select(z => z.Value).First();
                                    FinRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 10].Value = "N/A";
                            }

                            #endregion

                            #region CommercialInfo - Experience Certification
                            if (prvInfo.Commercial_CertExp.Count > 0 && prvInfo.Commercial_CertExp != null)
                            {
                                var CommRowIndex = rowIndex;
                                prvInfo.Commercial_CertExp.All(y =>
                                {

                                    ws.Cells[CommRowIndex, 11].Value = y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 302011).Select(z => z.Value).First();
                                    CommRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 11].Value = "N/A";
                            }


                            #endregion

                            #region CertificationInfo - Certifications
                            if (prvInfo.HSEQ_Cert.Count > 0 && prvInfo.HSEQ_Cert != null)
                            {
                                var CertRowIndex = rowIndex;
                                prvInfo.HSEQ_Cert.All(y =>
                                {

                                    ws.Cells[CertRowIndex, 12].Value = y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 702006).Select(z => z.Value).First();
                                    CertRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 12].Value = "N/A";
                            }

                            #endregion
                            #region CertificationInfo - Healt, Enviroment, Security
                            if (prvInfo.HSEQ_Health.Count > 0 && prvInfo.HSEQ_Health != null)
                            {
                                var CertRowIndex = rowIndex;
                                prvInfo.HSEQ_Health.All(y =>
                                {

                                    ws.Cells[CertRowIndex, 13].Value = y.ItemInfo.Where(z => z.ItemInfoType.ItemId == 703003).Select(z => z.Value).First();
                                    CertRowIndex++;
                                    return true;

                                });
                            }
                            else
                            {
                                ws.Cells[rowIndex, 13].Value = "N/A";
                            }


                            #endregion

                            proCount++;

                            if (proCount >= providers.Count)
                            {
                                //Generate A File with Random name
                                Byte[] bin = p.GetAsByteArray();
                                string file = Guid.NewGuid().ToString() + ".xlsx";
                                File.WriteAllBytes(file, bin);

                                //These lines will open it in Excel
                                ProcessStartInfo pi = new ProcessStartInfo(file);
                                Process.Start(pi);
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
            var sheetName = "Hoja1";
            var p = new ExcelPackage();

            Excel_pkg.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = Excel_pkg.Workbook.Worksheets[1];
            ws.Name = sheetName;
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

        public static List<CompanyModel> GetAllProviders()
        {
            return DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.GetAllProviders();
        }

        public static REDEBANInfoModel GetProviderInfo(string CustomerPublicId, string ProviderPublicId)
        {
            return DAL.Controller.IntegrationPlatformREDEBANDataController.Instance.GetProviderInfo(CustomerPublicId, ProviderPublicId);
        }
    }
}
