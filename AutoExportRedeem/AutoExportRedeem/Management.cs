using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoExportRedeem
{
    class Management
    {
        private DatabaseModule databaseModule;
        private DatabaseModule databaseModuleLog;
        private ReportModule reportModule;
        private DirectoryModule directoryModule;
        private FileNameUtil fileNameUtil;
        private SettingBean settingBean;

        private int lineNumber = 10001;

        private Dictionary<string, DataTable> dicMapByStoreCode;

        private static Management instance;

        private Management() { }

        public static Management Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Management();
                }
                return instance;
            }
        }

        public void Start()
        {
            if (Initialize())
            {
                Console.WriteLine("PROGRAM WILL START PROCESS AUTOMATICALLY.");
                Thread.Sleep(3000);
                Console.WriteLine("STARTED : " + DateTime.Now); 
                WriteLog(System_Type.Process, @"== START PROGRAM ==");

                List<string> allStoreCode = GetAllStoreCode();

                DateTime currentDateTime = DateTime.Now.AddMonths(-1);
                foreach (string item in allStoreCode)
                {
                    DataTable data = dicMapByStoreCode[item];

                    string fileReportPath = fileNameUtil.GetfilereportPath(settingBean.localPath, item, currentDateTime);
                    string fileReportName = fileNameUtil.GetfilereportName(settingBean.fileFormatName, item, currentDateTime);
                    string saveFileName = fileReportPath + fileReportName + ".pdf";
                    directoryModule.CreateDirectory(fileReportPath);

                    WriteLog(System_Type.Export_Data, "== EXPORTING DATA STORE CODE " + item + " ==");

                    bool isExported = reportModule.GoExport(data, data.Rows[0][0].ToString(), saveFileName);

                    if (isExported)
                    {
                        WriteLog(System_Type.Export_Data, "== EXPORTED DATA STORE CODE " + item + " ==");
                    }
                    else
                    {
                        directoryModule.DeleteDirectory(fileReportPath);
                        WriteLog(System_Type.Export_Data, "== ERROR EXPORTING DATA STORE CODE " + item + " ==");
                    }
                    data.Dispose();
                }

                WriteLog(System_Type.Transfer_Data, "== START TRANSFER DATA ==");
                if (directoryModule.MoveAllExportFile())
                {
                    WriteLog(System_Type.Transfer_Data, "== END TRANSFER DATA ==");
                }
                else
                {
                    WriteLog(System_Type.Transfer_Data, "== ERROR TRANSFER DATA ==");
                    Console.WriteLine("END PROCESS. PRESS ANY KEY TO EXIT.");
                    EndProcess(1);
                }

                WriteLog(System_Type.Process, @"== END PROGRAM ==");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");
                WriteLog(System_Type.Process, @"==================================");

                Console.WriteLine("END PROCESS. PROGRAM WILL CLOSE AUTOMATICALLY.");

                EndProcess(0);
            }
            else
            {
                EndProcess(1);
            }
        }

        private bool Initialize()
        {
            return InitialIni() && InitialDatabase() && InitialFileNameUtil()
                        && InitialReport() && InitialDirectory();
        }

        private bool InitialIni()
        {
            settingBean = new SettingBean();
            String configFilePath = AppDomain.CurrentDomain.BaseDirectory + @"setting.ini";
            IniFile ini = new IniFile(configFilePath);

            try
            {
                settingBean.dataSource = ini.IniReadValue("Database", "DataSource");
                settingBean.databaseName = ini.IniReadValue("Database", "DatabaseName");
                settingBean.username = ini.IniReadValue("Database", "Username");
                settingBean.password = ini.IniReadValue("Database", "Password");
                settingBean.databaseTimeout = Int32.Parse(ini.IniReadValue("Database", "DatabaseTimeout"));

                settingBean.dataSourceLog = ini.IniReadValue("Database_Log", "DataSource");
                settingBean.databaseNameLog = ini.IniReadValue("Database_Log", "DatabaseName");
                settingBean.usernameLog = ini.IniReadValue("Database_Log", "Username");
                settingBean.passwordLog = ini.IniReadValue("Database_Log", "Password");
                settingBean.databaseTimeoutLog = Int32.Parse(ini.IniReadValue("Database_Log", "DatabaseTimeout"));
                
                settingBean.fileFormatName = ini.IniReadValue("Format", "FileName");
                settingBean.folderStructure = ini.IniReadValue("Format", "FolderStructure");

                settingBean.localFolder = @"C:\e_synergy_fc_aer_temp\";
                settingBean.localPath = settingBean.localFolder + settingBean.folderStructure;

                settingBean.networkPath = ini.IniReadValue("Network", "NetworkPath");
                settingBean.networkUsername = ini.IniReadValue("Network", "NetworkUsername");
                settingBean.networkPassword = ini.IniReadValue("Network", "NetworkPassword");

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("PROBLEM OCCURED > setting.ini ERROR");
                return false;
            }
        }

        private bool InitialDatabase()
        {
            try
            {
                databaseModule = new DatabaseModule(settingBean.dataSource, settingBean.databaseName,
                    settingBean.username, settingBean.password, settingBean.databaseTimeout);
            }
            catch (Exception ex)
            {
                Console.WriteLine("PROBLEM OCCURED > CANT CONNECT TO DATABASE");
                return false;
            }

            try
            {
                databaseModuleLog = new DatabaseModule(settingBean.dataSourceLog, settingBean.databaseNameLog,
                    settingBean.usernameLog, settingBean.passwordLog, settingBean.databaseTimeoutLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine("PROBLEM OCCURED > CANT CONNECT TO DATABASE_LOG");
                return false;
            }

            try
            {
                databaseModuleLog.ExecuteStoreProcedure("SP_CLEAROLDLOG");
            }
            catch (Exception ex)
            {
                WriteLog(System_Type.Process, @"== CANT FIND OR EXECUTE SP_CLEAROLDLOG ==");
                return false;
            }

            try
            {
                DateTime targetDateTime = DateTime.Now.AddMonths(-2);
                string temp = targetDateTime.Year.ToString() + targetDateTime.Month.ToString().PadLeft(2,'0');
                databaseModule.ExecuteStoreProcedure("spu_RedeemFC",temp);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                WriteLog(System_Type.Process, @"== CANT FIND OR EXECUTE spu_RedeemFC ==");
                return false;
            }

        }

        private bool InitialReport()
        {
            try
            {
                reportModule = new ReportModule();
                return true;
            }
            catch (Exception ex)
            {
                WriteLog(System_Type.Process, @"== PROBLEM OCCURED, CRYSTAL REPORT ERROR ==");
                return false;
            }

        }

        private bool InitialDirectory()
        {
            try
            {
                directoryModule = new DirectoryModule(settingBean.localFolder,
                    settingBean.networkPath, settingBean.networkUsername, settingBean.networkPassword);
                if (!directoryModule.isExistNetworkPath())
                {
                    int returnCode = directoryModule.TryCreateMapDrive();
                    if (returnCode == 1)
                    {
                        WriteLog(System_Type.Process, @"== NETWORK DIRECTORY NOT FOUND ==");
                        return false;
                    }
                    else if (returnCode == 2)
                    {
                        WriteLog(System_Type.Process, @"== ND NOT FOUND OR CANT CREATE MAP ==");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    //int errorCode = directoryModule.TryCreateMapDrive();
                    //if (errorCode != 0)
                    //{
                        //Console.WriteLine(errorCode + "<< ErrorCode");
                        //directoryModule.SetTargetDriveExtra();
                        //WriteLog(System_Type.Process, @"== CANT CREATE MAP DRIVE NETWORK ==");
                        //return false;
                    //}
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);   
                WriteLog(System_Type.Process, @"== CANT CREATE DIRECTORY ==");
                return false;
            }
        }

        private bool InitialFileNameUtil()
        {
            fileNameUtil = new FileNameUtil();
            return true;
        }

        private List<string> GetAllStoreCode()
        {
            try
            {
                WriteLog(System_Type.Retrieve_Data, @"== RETRIEVING DATA ==");

                DataTable allRecord = databaseModule.ExecuteQuery(@"select * from RedeemFCByDetail");
                //DataTable allRecord = databaseModule.ExecuteQuery(@"select top 5000 * from RedeemFCByDetail");

                List<string> distinctStoreCodeList = (from rec in allRecord.AsEnumerable()
                                                      select rec.Field<string>(0)).Distinct().ToList();
                List<string> storeCodeList = new List<string>();
                foreach (string item in distinctStoreCodeList)
                {
                    storeCodeList.Add(item.Substring(0, 4));
                }
                storeCodeList.Sort();
                WriteLog(System_Type.Retrieve_Data, @"== RETRIEVED DATA ==");

                SetupDicMapByStoreCode(storeCodeList, allRecord);

                return storeCodeList;
            }
            catch (Exception ex)
            {
                WriteLog(System_Type.Retrieve_Data, @"== ERROR RETRIEVING DATA ==");
                EndProcess(1);
                return null;
            }
        }

        private void SetupDicMapByStoreCode(List<string> allStoreCode, DataTable allRecord)
        {
            dicMapByStoreCode = new Dictionary<string, DataTable>();
            DataTable tempDT = allRecord.Clone();
            DataView tempDV;
            DataTable tempDT2;
            foreach (string item in allStoreCode)
            {
                try
                {
                    WriteLog(System_Type.Adjust_Data, "== ADJUSTING DATA STORE CODE " + item + " ==");
                    tempDT.Clear();
                    DataRow dr;
                    for (int i = allRecord.Rows.Count - 1; i >= 0; i--)
                    {
                        dr = allRecord.Rows[i];
                        if (dr[0].ToString().Contains(item))
                        {
                            tempDT.ImportRow(dr);
                            allRecord.Rows.Remove(dr);
                        }
                    }
                    tempDV = tempDT.DefaultView;
                    tempDV.Sort = "ProdName asc";
                    tempDT2 = tempDV.ToTable();
                    dicMapByStoreCode.Add(item, tempDT2);

                    WriteLog(System_Type.Adjust_Data, "== ADJUSTED DATA STORE CODE " + item + " ==");

                }
                catch (Exception ex)
                {
                    WriteLog(System_Type.Adjust_Data, "== ERROR ADJUSTING DATA STORE CODE " + item + " ==");
                    continue;
                }

            }

            allRecord.Dispose();
        }

        private void EndProcess(int exitCode)
        {
            Thread.Sleep(3000);
            if (directoryModule != null)
            {
                directoryModule.DeleteTempFolder();
            }

            if (exitCode != 0)
            {
                WriteLog(System_Type.Process, @"== END PROGRAM WITH ERROR ==");
            }
            Thread.Sleep(3000);
            Environment.Exit(exitCode);
        }

        public void WriteLog(System_Type type, string desc)
        {
            if (databaseModuleLog != null)
            {
                databaseModuleLog.WriteLog(type, desc, "" + lineNumber++);
            }
            Console.WriteLine(desc);
        }
    }
}