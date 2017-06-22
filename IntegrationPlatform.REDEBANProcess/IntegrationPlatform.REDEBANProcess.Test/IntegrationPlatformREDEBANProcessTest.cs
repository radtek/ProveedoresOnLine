using System;
using OfficeOpenXml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace IntegrationPlatform.REDEBANProcess.Test
{
    [TestClass]
    public class IntegrationPlatformREDEBANProcessTest
    {
        [TestMethod]
        public void StartProcess()
        {
            REDEBANProcess.IntegrationPlatformREDEBANProcess.StartProcess();
        }

        [TestMethod]
        public void GetAllProviders()
        {
            var oReturn = REDEBANProcess.IntegrationPlatformREDEBANProcess.GetAllProviders();

            Assert.AreEqual(true, oReturn.Count > 0);
        }

        [TestMethod]
        public void GetProviderInfo()
        {
            var oReturn = REDEBANProcess.IntegrationPlatformREDEBANProcess.GetProviderInfo("26D388E3", "9B15FEF0");

            Assert.AreEqual(true, oReturn != null);
        }
        [TestMethod]
        public void CreateExcelWorkBook()
        {
            var sheetName = "Hoja1";
            var p = new ExcelPackage();

            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet



            Assert.AreEqual(true, ws != null);
        }
        [TestMethod]
        public void CreateExcelFile()
        {
            var p = new ExcelPackage();
            Byte[] bin = p.GetAsByteArray();
            string file = Guid.NewGuid().ToString() + ".xlsx";
            File.WriteAllBytes(file, bin);

            //These lines will open it in Excel
            ProcessStartInfo pi = new ProcessStartInfo(file);
            Process.Start(pi);
        }
    }
}
