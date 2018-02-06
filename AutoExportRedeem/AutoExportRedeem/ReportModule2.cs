using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoExportRedeem
{
    class ReportModule2
    {
        private ReportDocument report;
        private ExportOptions CrExportOptions;
        private DiskFileDestinationOptions CrDiskFileDestinationOptions;

        private string reportPath;

        private string currentMonthName;
        private int currentYear;

        private string saveFileName;

        public ReportModule2()
        {
            Init();

            report = new ReportDocument();

            report.Load(reportPath, OpenReportMethod.OpenReportByTempCopy);

            CrExportOptions = report.ExportOptions;
            CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
            CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;

            CrDiskFileDestinationOptions = new DiskFileDestinationOptions();

            CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
        }

        public void Init()
        {
            this.reportPath = AppDomain.CurrentDomain.BaseDirectory + @"\Report\Template2.rpt";
            DateTime cDateTime = DateTime.Now;
            this.currentYear = cDateTime.Year;
            int currentMonth = cDateTime.Month;
            if (currentMonth == 1)
            {
                this.currentYear = this.currentYear - 1;
                currentMonth = 12;
            }
            else
            {
                currentMonth = currentMonth - 1;
            }


            SetMonthName(currentMonth);
        }

        private void SetMonthName(int currentMonth)
        {
            currentMonthName = "";
            switch (currentMonth)
            {
                case 1: currentMonthName = "January"; break;
                case 2: currentMonthName = "February"; break;
                case 3: currentMonthName = "March"; break;
                case 4: currentMonthName = "April"; break;
                case 5: currentMonthName = "May"; break;
                case 6: currentMonthName = "June"; break;
                case 7: currentMonthName = "July"; break;
                case 8: currentMonthName = "August"; break;
                case 9: currentMonthName = "September"; break;
                case 10: currentMonthName = "October"; break;
                case 11: currentMonthName = "November"; break;
                case 12: currentMonthName = "December"; break;
                default: break;
            }
        }

        public bool GoExport(DataTable reportData, string storeName, string saveFileName)
        {
            try
            {

                this.saveFileName = saveFileName;
                report.SetDataSource(reportData);
                report.SetParameterValue("MonthName", currentMonthName);
                report.SetParameterValue("Year", currentYear + "");
                report.SetParameterValue("StoreName", storeName);
                CrDiskFileDestinationOptions.DiskFileName = this.saveFileName;

                report.Export();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
