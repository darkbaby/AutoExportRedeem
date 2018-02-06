using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoExportRedeem
{
    class FileNameUtil
    {
        private string[] datePrefix = { "DDMMYYYY", "MMDDYYYY", "YYYYMMDD", "YYYYMM", "MMYYYY" };
        private string[] dateFormat = { "ddMMyyyy", "MMddyyyy", "yyyyMMdd", "yyyyMM", "MMyyyy" };
        private string[] storePrefix = { "STORECODE" };

        private bool FindStringDateFormat(string s)
        {
            bool chk = false;
            chk = datePrefix.Contains(s);
            return chk;
        }

        private bool FindStringStoreCodeFormat(string s)
        {
            bool chk = false;
            chk = storePrefix.Contains(s);
            return chk;
        }

        private bool FindStringReportName(string s)
        {
            bool chk = false;
            chk = !FindStringDateFormat(s);
            chk = !FindStringStoreCodeFormat(s);
            return chk;
        }

        public string GetfilereportName(string fileformat, string storeCode, DateTime dtfrom)
        {
            string rptFileName = "";
            string filedateformat = "";
            int idxDatePrefix = 0, idxFileNamePrefix = 0, idxStoreCodePrefix = 0;
            string filename = "";
            string dtfmt = "MMyyyy";
            string[] reportFormat = fileformat.Split('_');
            filedateformat = reportFormat[Array.FindIndex(reportFormat, 0, FindStringDateFormat)];
            filename = reportFormat[Array.FindIndex(reportFormat, 0, FindStringReportName)];
            dtfmt = dateFormat[Array.IndexOf(datePrefix, filedateformat)];

            idxDatePrefix = Array.FindIndex(reportFormat, 0, FindStringDateFormat);
            idxFileNamePrefix = Array.FindIndex(reportFormat, 0, FindStringReportName);
            idxStoreCodePrefix = Array.FindIndex(reportFormat, 0, FindStringStoreCodeFormat);
            if (idxFileNamePrefix >= 0)
            {
                reportFormat[idxFileNamePrefix] = filename;
            }
            if (idxDatePrefix >= 0)
            {
                reportFormat[idxDatePrefix] = dtfrom.ToString(dtfmt);
            }
            if (idxStoreCodePrefix >= 0)
            {
                reportFormat[idxStoreCodePrefix] = storeCode;
            }

            foreach (string s in reportFormat)
            {
                if (rptFileName == "")
                {
                    rptFileName += s;
                }
                else
                {
                    rptFileName += "_" + s;
                }
            }
            return rptFileName;
        }

        public string GetfilereportPath(string fileformat, string storeCode, DateTime dtfrom)
        {
            string rptFileName = "";
            string filedateformat = "";
            int idxDatePrefix = 0, /*idxFileNamePrefix = 0,*/ idxStoreCodePrefix = 0;
            string filename = "";
            string dtfmt = "MMyyyy";
            string[] reportFormat = fileformat.Split('\\');
            filedateformat = reportFormat[Array.FindIndex(reportFormat, 0, FindStringDateFormat)];
            filename = reportFormat[Array.FindIndex(reportFormat, 0, FindStringReportName)];
            dtfmt = dateFormat[Array.IndexOf(datePrefix, filedateformat)];

            idxDatePrefix = Array.FindIndex(reportFormat, 0, FindStringDateFormat);
            //idxFileNamePrefix = Array.FindIndex(reportFormat, 0, FindStringReportName);
            idxStoreCodePrefix = Array.FindIndex(reportFormat, 0, FindStringStoreCodeFormat);

            //reportFormat[idxFileNamePrefix] = filename;
            if (idxDatePrefix >= 0)
            {
                reportFormat[idxDatePrefix] = dtfrom.ToString(dtfmt);
            }
            if (idxStoreCodePrefix >= 0)
            {
                reportFormat[idxStoreCodePrefix] = storeCode;
            }
            foreach (string s in reportFormat)
            {
                if (rptFileName == "")
                {
                    rptFileName += @s;
                }
                else
                {
                    rptFileName += @"\" + @s;
                }
            }
            return rptFileName;
        }

    }
}
